using BL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.DTO;
using PeerTutoringNetwork.DTOs;
using PeerTutoringNetwork.Security;

namespace PeerTutoringNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly PeerTutoringNetworkContext _context;
        public AdminController(IConfiguration configuration, PeerTutoringNetworkContext context)
        {
            _configuration = configuration;
            _context = context;
        }


        [HttpDelete("DeleteUser/{id}")]

        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = _context.Users.Find(id);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                _context.Users.Remove(user);
                _context.SaveChanges();

                return Ok("User deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetUserStatistics")]

        public IActionResult GetUserStatistics()
        {
            try
            {
                var totalUsers = _context.Users.Count();
                return Ok(new { totalUsers });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpPost("[action]")]
        public ActionResult AddUser(UserRegisterDto userDto)
        {
            try
            {
                if (_context.Users.Any(x => x.Username == userDto.Username || x.Email == userDto.Email))
                    return BadRequest(new { message = "User with the same username or email already exists" });

                var salt = PasswordHashProvider.GetSalt();
                var hash = PasswordHashProvider.GetHash(userDto.Password, salt);

                var user = new User
                {
                    Username = userDto.Username,
                    Email = userDto.Email,
                    PwdHash = Convert.FromBase64String(hash),
                    PwdSalt = Convert.FromBase64String(salt),
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Phone = userDto.Phone,
                    RoleId = userDto.RoleId
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                // Vraćamo JSON s porukom i korisnikom
                return Ok(new { message = "User added successfully", user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("[action]")]
        public ActionResult EditUser(UserUpdateDto userDto)
        {
            try
            {
                // Pronađi postojećeg korisnika prema ID-u
                var user = _context.Users.FirstOrDefault(x => x.UserId == userDto.UserId);
                if (user == null)
                    return NotFound("User not found");

                // Ažuriraj podatke korisnika
                user.Username = userDto.Username;
                user.Email = userDto.Email;
                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.RoleId = userDto.RoleId;

                // Lozinka (opcionalno, samo ako je poslano)
                if (!string.IsNullOrEmpty(userDto.Password))
                {
                    var salt = PasswordHashProvider.GetSalt();
                    var hash = PasswordHashProvider.GetHash(userDto.Password, salt);
                    user.PwdHash = Convert.FromBase64String(hash);
                    user.PwdSalt = Convert.FromBase64String(salt);
                }

                // Spremi promjene
                _context.SaveChanges();

                return Ok("User updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetRoleStatistics")]
        public ActionResult GetRoleStatistics()
        {
            try
            {
                var roleStatistics = _context.Users
                    .GroupBy(u => u.RoleId)
                    .Select(group => new
                    {
                        RoleId = group.Key,
                        Count = group.Count()
                    })
                    .ToList();

                return Ok(roleStatistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
