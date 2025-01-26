using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("ping")]
    public class PingController : ControllerBase
    {

        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

    }
}
