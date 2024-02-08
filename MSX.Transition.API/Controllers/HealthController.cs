using Microsoft.AspNetCore.Mvc;

namespace MSX.Transition.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class HealthController : ControllerBase
    {
        public HealthController()
        {
        }

        [HttpGet("ping")]
        public async Task<IActionResult> PingAsync()
        {
            await Task.Delay(0);
            var currDateTime = DateTimeOffset.UtcNow;
            return Ok(currDateTime);
        }
    }
}
