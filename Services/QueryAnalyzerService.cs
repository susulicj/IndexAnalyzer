using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

namespace Services
{
    public class QueryAnalyzerService
    {
        private readonly string _connectionString;

        public QueryAnalyzerService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("FakultetCS");
        }


        public async Task<object> AnalyzeQuery(string query)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var cmd = new SqlCommand(query, conn);
            var reader = await cmd.ExecuteReaderAsync();

            stopwatch.Stop();

            return new
            {
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                RowsReturned = reader.HasRows
            };
        }


        public async Task<string> GetExecutionPlan(string query)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

           
            using (var cmdOn = new SqlCommand("SET SHOWPLAN_XML ON", conn))
                await cmdOn.ExecuteNonQueryAsync();

            string xml = null;

        
            using (var cmd = new SqlCommand(query, conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    xml = reader.GetValue(0)?.ToString();
                }
            } 

            using (var cmdOff = new SqlCommand("SET SHOWPLAN_XML OFF", conn))
                await cmdOff.ExecuteNonQueryAsync();

            return xml;
        }

        public List<string> AnalyzeExecutionPlan(string xml)
        {
            var result = new List<string>();

            if (xml.Contains("TableScan"))
                result.Add("Table Scan - nema indeksa");

            if (xml.Contains("ClusteredIndexScan"))
                result.Add(" Clustered Index Scan - skupo");

            if (xml.Contains("KeyLookup"))
                result.Add(" Key Lookup - nedovoljno pokriven index");

            if (xml.Contains("HashMatch"))
                result.Add(" Hash Join - teška operacija");

            if (xml.Contains("IndexSeek"))
                result.Add("Index Seek - optimalno");

            return result;
        }

        public int GetExecutionPlanScore(string xml)
        {
            int score = 100;

            if (xml.Contains("TableScan")) score -= 40;
            if (xml.Contains("ClusteredIndexScan")) score -= 30;
            if (xml.Contains("KeyLookup")) score -= 15;
            if (xml.Contains("HashMatch")) score -= 10;

            if (xml.Contains("IndexSeek")) score += 10;

            return Math.Clamp(score, 0, 100);
        }

        public async Task<List<object>> GetQueryStoreData()
        {
            var result = new List<object>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

           var cmd = new SqlCommand(@"
                SELECT TOP 10 
                    qsqt.query_sql_text,
                    rs.avg_duration,
                    rs.last_execution_time
                FROM sys.query_store_query_text qsqt
                JOIN sys.query_store_query qsq 
                    ON qsqt.query_text_id = qsq.query_text_id
                JOIN sys.query_store_plan qsp 
                    ON qsq.query_id = qsp.query_id
                JOIN sys.query_store_runtime_stats rs 
                    ON qsp.plan_id = rs.plan_id
                ORDER BY rs.avg_duration DESC ", conn);

            var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new
                {
                    Query = reader.GetString(0),
                    AvgDuration = reader.GetDouble(1),
                    LastExecution = reader.GetDateTimeOffset(2)
                });
            }

            return result;
        }

        public async Task<object> AnalyzeQueryWithMetrics(string query, string tableName, bool useIndex, string indexName = null)
        {
            using var conn = new SqlConnection(_connectionString);

            string messages = "";
            conn.FireInfoMessageEventOnUserErrors = true;

            conn.InfoMessage += (sender, e) =>
            {
                messages += e.Message + "\n";
            };

            await conn.OpenAsync();

            var modifiedQuery = ApplyIndexHint(
                query,
                tableName,
                useIndex ? indexName : null
            );

            var cmdText = $@"
            SET STATISTICS IO ON;
            SET STATISTICS TIME ON;

            {modifiedQuery}

            SET STATISTICS IO OFF;
            SET STATISTICS TIME OFF;
            ";

            var stopwatch = Stopwatch.StartNew();

            using var cmd = new SqlCommand(cmdText, conn);
            await cmd.ExecuteNonQueryAsync();

            stopwatch.Stop();


            return new
            {
                UseIndex = useIndex,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds,
                RawMessages = messages
            };
        }
        private object ParseMetrics(string messages)
        {
            int logicalReads = 0;
            int physicalReads = 0;
            int cpuTime = 0;

            var lines = messages.Split('\n');

            foreach (var line in lines)
            {
                if (line.Contains("logical reads"))
                {
                    var parts = line.Split(',');

                    foreach (var p in parts)
                    {
                        if (p.Contains("logical reads"))
                        {
                            var val = p.Split('=')[1].Trim();
                            logicalReads = int.Parse(val);
                        }

                        if (p.Contains("physical reads"))
                        {
                            var val = p.Split('=')[1].Trim();
                            physicalReads = int.Parse(val);
                        }
                    }
                }

                if (line.Contains("CPU time"))
                {
                    var parts = line.Split(',');

                    var cpuPart = parts[0].Split('=')[1]
                        .Replace("ms", "")
                        .Trim();

                    cpuTime = int.Parse(cpuPart);
                }
            }

            return new
            {
                LogicalReads = logicalReads,
                PhysicalReads = physicalReads,
                CpuTimeMs = cpuTime
            };
        }

       public string ApplyIndexHint(string query, string tableName, string indexName = null)
        {
            var safeTable = $"[{tableName}]";

            var hint = indexName == null
                ? "WITH (INDEX(0))"
                : $"WITH (INDEX({indexName}))";
            return query.Replace(
                $"FROM {tableName}",
                $"FROM {safeTable} {hint}"
            );
        }
    }
}
