using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class AnalyzerController : ControllerBase
{
    private readonly QueryAnalyzerService _service;

    public AnalyzerController(QueryAnalyzerService service)
    {
        _service = service;
    }
   
    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] string query)
    {
        var result = await _service.AnalyzeQuery(query);
        return Ok(result);
    }

    [HttpGet("querystore")]
    public async Task<IActionResult> GetQueryStore()
    {
        var data = await _service.GetQueryStoreData();
        return Ok(data);
    }

    [HttpPost("execution-plan")]
    public async Task<IActionResult> GetExecutionPlan([FromBody] QueryRequestPlan query)
    {
        var plan = await _service.GetExecutionPlan(query.Query);
        return Ok(plan);
    }

    [HttpPost("metrics")]
    public async Task<IActionResult> GetMetrics([FromBody] QueryRequest request)
    {
        var result = await _service.AnalyzeQueryWithMetrics(
            request.Query,
            request.TableName,
            request.UseIndex,
            request.IndexName
        );

        return Ok(result);
    }
}