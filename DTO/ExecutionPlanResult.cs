namespace DTO{
    public class ExecutionPlanResult
        {
            public string RawXml { get; set; } = "";

            public List<string> LogicalOpList { get; set; } = new();
            public List<string> PhysicalOpList { get; set; } = new();

            public List<string> Indexes { get; set; } = new();

            public List<string> Warnings { get; set; } = new();

            public double TotalCost { get; set; }

            public double EstimatedRows { get; set; }

            public bool HasIndexSeek { get; set; }

            public bool HasTableScan { get; set; }

            public bool HasKeyLookup { get; set; }

            public string JoinType { get; set; } = "";
        }
}