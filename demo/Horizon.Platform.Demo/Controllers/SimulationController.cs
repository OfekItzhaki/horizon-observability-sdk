using Microsoft.AspNetCore.Mvc;
using Horizon.Platform.Sdk;

namespace Horizon.Platform.Demo.Controllers;

[ApiController]
[Route("[controller]")]
public class SimulationController : ControllerBase
{
    private readonly ILogger<SimulationController> _logger;
    private static readonly Random _random = new();

    public SimulationController(ILogger<SimulationController> logger)
    {
        _logger = logger;
    }

    [HttpGet("success")]
    public async Task<IActionResult> GetSuccess()
    {
        _logger.LogInformation("Processing a successful request...");
        
        // Simulate work
        await Task.Delay(_random.Next(50, 200));
        
        HorizonMetrics.RecordCheckout(1, "Store-A");
        
        return Ok(new { Message = "Success!", TraceId = System.Diagnostics.Activity.Current?.TraceId.ToString() });
    }

    [HttpGet("slow")]
    public async Task<IActionResult> GetSlow()
    {
        _logger.LogInformation("Processing a slow request...");
        
        // Simulate latency
        await Task.Delay(_random.Next(500, 2000));
        
        return Ok(new { Message = "Sorry for the delay.", TraceId = System.Diagnostics.Activity.Current?.TraceId.ToString() });
    }

    [HttpGet("error")]
    public IActionResult GetError()
    {
        _logger.LogError("Something went wrong!");
        throw new InvalidOperationException("This is a simulated crash!");
    }
}
