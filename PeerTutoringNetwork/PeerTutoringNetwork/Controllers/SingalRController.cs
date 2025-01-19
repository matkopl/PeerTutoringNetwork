using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using BL.Hubs;

namespace PeerTutoringNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignalRController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet("start")]
        public IActionResult Start()
        {
            // This endpoint can be used to initialize the SignalR hub
            return Ok("SignalR Hub is running");
        }
    }
}