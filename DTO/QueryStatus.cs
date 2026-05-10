public class QueryMetricsDto
{
    public int LogicalReads { get; set; }

    public int PhysicalReads { get; set; }

    public int CpuTimeMs { get; set; }

    public int ElapsedTimeMs { get; set; }

    public int ScanCount { get; set; }

    public int ReadAheadReads { get; set; }
}