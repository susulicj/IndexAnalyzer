namespace Models{
public class PerformanceLog
    {
        public int Id { get; set; }

        public string QueryName { get; set; }

        public bool HasIndex { get; set; }

        public long ExecutionTimeMs { get; set; }

        public int LogicalReads { get; set; }

        public DateTime ExecutedAt { get; set; }
    }
}