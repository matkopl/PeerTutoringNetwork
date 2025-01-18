using BL.Managers;
using BL.Models;
using BL.Services;
using Microsoft.AspNetCore.Mvc;
using PeerTutoringNetwork.DTOs;

namespace PeerTutoringNetwork.Controllers
{
    public class SessionController : Controller
    {
        private readonly AdminPanelService _adminService;

        public SessionController(PeerTutoringNetworkContext context)
        {
            _adminService = new AdminPanelService(context);
        }

        [HttpPost("Login")]
        public IActionResult Login(UserLoginDto loginDto)
        {
            if (_adminService.Authenticate(loginDto.Username, loginDto.Password, out var authenticatedUser))
            {
                var token = SessionManager.Instance.GetSession(authenticatedUser.UserId);
                return Ok(new { Message = "Login successful", Token = token });
            }

            return Unauthorized("Invalid username or password");
        }

        [HttpPost("Logout")]
        public IActionResult Logout(int userId)
        {
            SessionManager.Instance.RemoveSession(userId);
            return Ok("User logged out successfully");
        }

        [HttpGet("ActiveUsers")]
        public IActionResult GetActiveUsers()
        {
            var activeUsers = SessionManager.Instance.GetActiveUsers();
            return Ok(activeUsers);
        }
    }
}
