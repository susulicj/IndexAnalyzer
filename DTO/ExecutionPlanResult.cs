namespace DTO{
    public class ExecutionPlanResult
        {
            public string PlanXml { get; set; }
            public List<string> Warnings { get; set; }
            public int Score { get; set; }
        }
}