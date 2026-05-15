using Microsoft.AspNetCore.Mvc;

namespace ArchieHealthTracker.Controllers;

[ApiController]
public class PingController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Get()
    {
        return Ok();
    }
}