using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IndexController : ControllerBase
{
    private readonly IConfiguration _config;

    public IndexController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("enable")]
    public async Task<IActionResult> EnableIndex(string indexName, string table)
    {
        var query = $"ALTER INDEX [{indexName}] ON [{table}] REBUILD";

        using var conn = new SqlConnection(_config.GetConnectionString("FakultetCS"));
        await conn.OpenAsync();

        var cmd = new SqlCommand(query, conn);
        await cmd.ExecuteNonQueryAsync();

        return Ok("Index enabled");
    }

    [HttpPost("disable")]
    public async Task<IActionResult> DisableIndex(string indexName, string table)
    {
        var query = $"ALTER INDEX [{indexName}] ON [{table}] DISABLE";

        using var conn = new SqlConnection(_config.GetConnectionString("FakultetCS"));
        await conn.OpenAsync();

        var cmd = new SqlCommand(query, conn);
        await cmd.ExecuteNonQueryAsync();

        return Ok("Index disabled");
    }
}