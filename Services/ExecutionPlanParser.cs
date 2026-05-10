using System.Xml.Linq;


namespace Services
{
    public class ExecutionPlanParser
    {
        public ExecutionPlanResult Parse(string xml)
        {
            var result = new ExecutionPlanResult();
            result.RawXml = xml;

            if (string.IsNullOrWhiteSpace(xml))
                return result;

            var doc = XDocument.Parse(xml);

            XNamespace ns = "http://schemas.microsoft.com/sqlserver/2004/07/showplan";

            var relOps = doc.Descendants(ns + "RelOp");

            foreach (var relOp in relOps)
            {
                var physicalOp = relOp.Attribute("PhysicalOp")?.Value;
                var LogicalOp = relOp.Attribute("LogicalOp")?.Value;
                if (!string.IsNullOrEmpty(LogicalOp))
                {
                    result.LogicalOpList.Add(LogicalOp);
                }
                else 
                {
                    result.LogicalOpList.Add("No logical operator");
                }

                if (!string.IsNullOrEmpty(physicalOp))
                {
                    result.PhysicalOpList.Add(physicalOp);

                    switch (physicalOp)
                    {
                        case "Clustered Index Scan":
                            result.Warnings.Add("Clustered Index Scan detected - full table scan");
                            break;

                        case "Table Scan":
                            result.HasTableScan = true;
                            result.Warnings.Add("Table Scan detected");
                            break;

                        case "Index Seek":
                            result.HasIndexSeek = true;
                            break;

                        case "Hash Match":
                            result.JoinType = "Hash Match";
                            break;

                        case "Nested Loops":
                            result.JoinType = "Nested Loops";
                            break;
                    }
                }
                var costAttr = relOp.Attribute("EstimatedTotalSubtreeCost");

                if (costAttr != null &&
                    double.TryParse(costAttr.Value, out double cost))
                {
                    result.TotalCost += cost;
                }

                var rowsAttr = relOp.Attribute("EstimateRows");

                if (rowsAttr != null &&
                    double.TryParse(rowsAttr.Value, out double rows))
                {
                    result.EstimatedRows += rows;
                }
            }

            var objects = doc.Descendants(ns + "Object");

            foreach (var obj in objects)
            {
                var index = obj.Attribute("Index")?.Value;

                if (!string.IsNullOrEmpty(index))
                {
                    if (!result.Indexes.Contains(index))
                    {
                        result.Indexes.Add(index);
                    }
                }
            }

            var keyLookup = doc
                .Descendants(ns + "IndexScan")
                .Any(x => x.Attribute("Lookup")?.Value == "1");
          
            if (keyLookup)
            {
                result.HasKeyLookup = true;
                result.Warnings.Add("Key Lookup detected");
            }

            return result;
        }
    }
}