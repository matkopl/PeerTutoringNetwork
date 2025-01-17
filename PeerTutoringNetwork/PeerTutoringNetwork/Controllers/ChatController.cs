using BL.Models;
using BL.Services;
using PeerTutoringNetwork.Viewmodels;

namespace PeerTutoringNetwork.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ChatController : Controller
{
    private readonly IChatService _chatService;
    private readonly PeerTutoringNetworkContext _context;

    public ChatController(IChatService chatService, PeerTutoringNetworkContext context)
    {
        _chatService = chatService;
        _context = context;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
    {
        await _chatService.SendMessage(message.User, message.Message);
        return Ok();
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("FetchAllUsersForChat")]
    public IActionResult FetchAllUsersForChat()
    {
        var users = _context.Users
            .Select(user => new UserVM
            {
                UserId = user.UserId.ToString(),
                UserName = user.Username
            })
            .ToList();
        return Ok(users);
    }
}

public class ChatMessage
{
    public string User { get; set; }
    public string Message { get; set; }
}