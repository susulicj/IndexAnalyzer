namespace DTO{
    public class QueryRequest
    {
        public string Query { get; set; }
        public string TableName { get; set; }
        public bool UseIndex { get; set; }
        public string? IndexName { get; set; }
    }
}
