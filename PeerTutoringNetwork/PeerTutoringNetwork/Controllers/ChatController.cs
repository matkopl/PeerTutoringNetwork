using System.Security.Claims;
using BL.Models;
using BL.Services;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.Viewmodels;

namespace PeerTutoringNetwork.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class ChatController : Controller
{
    private readonly IChatService _chatService;
    private readonly IUserService _userService;
    private readonly PeerTutoringNetworkContext _context;

    public ChatController(IChatService chatService, PeerTutoringNetworkContext context, IUserService userService)
    {
        _chatService = chatService;
        _context = context;
        _userService = userService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] ChatMessage message)
    {
        await _chatService.SendMessage(message.User, message.Message);
        return Ok();
    }

    [HttpGet("index")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("Chat")]
    [HttpGet("Chat/{selectedUserId}")]
    public async Task<IActionResult> Chat(int selectedUserId)
    {
        var user = await _userService.GetUserById(selectedUserId);
        if (user == null)
        {
            return NotFound();
        }

        var userVM = new UserVM
        {
            UserId = user.UserId.ToString(),
            UserName = user.Username,
            Role = user.RoleId
        };

        return View(userVM);
    }

    [HttpGet("FetchAllUsersForChat")]
    public IActionResult FetchAllUsersForChat()
    {
        var users = _context.Users
            .Select(user => new UserVM
            {
                UserId = user.UserId.ToString(),
                UserName = user.Username,
                Role = user.RoleId
            })
            .ToList();
        return Ok(users);
    }


    [HttpGet("GetRoleById/{id}")]
    public IActionResult GetRoleById(int id)
    {
        var role = _context.Roles
            .Where(r => r.RoleId == id)
            .Select(r => new
            {
                r.RoleId,
                r.RoleName
            })
            .FirstOrDefault();

        if (role == null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserVMById(int userId)
    {
        var user = await _userService.GetUserById(userId);
        if (user == null)
        {
            return NotFound();
        }

        var userVM = new UserVM
        {
            UserId = user.UserId.ToString(),
            UserName = user.Username,
            Role = user.RoleId
        };

        return Json(userVM);
    }
}

public class ChatMessage
{
    public string User { get; set; }
    public string Message { get; set; }
}