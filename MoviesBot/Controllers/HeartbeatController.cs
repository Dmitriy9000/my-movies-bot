using Microsoft.AspNetCore.Mvc;

namespace MoviesBot.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HeartbeatController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Heartbeat");
        }
    }
}
