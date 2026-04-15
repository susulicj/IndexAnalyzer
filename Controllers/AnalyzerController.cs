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
    public async Task<IActionResult> GetExecutionPlan([FromBody] string query)
    {
        var plan = await _service.GetExecutionPlan(query);
        return Ok(plan);
    }

    [HttpPost("execution-plan-2")]
    public async Task<IActionResult> GetPlan([FromBody] string query)
    {
        var xml = await _service.GetExecutionPlan(query);

        var result = new ExecutionPlanResult
        {
            PlanXml = xml,
            Warnings = _service.AnalyzeExecutionPlan(xml),
            Score = _service.GetExecutionPlanScore(xml)
        };

        return Ok(result);
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