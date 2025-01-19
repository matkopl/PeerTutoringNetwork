﻿using BL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.DTO;
using PeerTutoringNetwork.DTOs;
using PeerTutoringNetwork.Security;
using PeerTutoringNetwork.Viewmodels;

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

        [HttpGet("[action]/{userId}")]
        public ActionResult GetUserById(int userId)
        {
            try
            {
                // Dohvati korisnika prema userId
                var user = _context.Users
                    .Where(u => u.UserId == userId)
                    .Select(u => new UserProfileDto
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Phone = u.Phone,
                        RoleId = u.RoleId
                    })
                    .FirstOrDefault();

                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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

        /*  [HttpGet("GetUserStatistics")]

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

          }*/

        [HttpGet("GetUserStatistics")]
        public IActionResult GetUserStatistics()
        {
            var userCount = _context.Users.Count();
            var adminCount = _context.Users.Count(u => u.RoleId == 3); // Admin RoleId = 3
            var teacherCount = _context.Users.Count(u => u.RoleId == 2); // Teacher RoleId = 2
            var studentCount = _context.Users.Count(u => u.RoleId == 1); // Student RoleId = 1

            return Ok(new
            {
                userCount,
                adminCount,
                teacherCount,
                studentCount
            });
        }

        [HttpGet("GetSubjectAverageRatings")]
        public IActionResult GetSubjectAverageRatings()
        {
            // Dohvaćanje svih subjekata, uključujući one koji nemaju recenzije
            var subjectsWithRatings = _context.Subjects
                .Select(subject => new
                {
                    // Ako predmet ima ocjene, dohvati prosječnu ocjenu, inače prikaži poruku
                    SubjectName = subject.SubjectName,
                    AverageRating = _context.Reviews
                        .Where(r => r.SubjectId == subject.SubjectId)
                        .Average(r => (double?)r.Rating ?? 0)
                })
            .ToList();


            return Ok(subjectsWithRatings);
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

                var response = new
                {
                    message = "User added successfully",
                    user = new
                    {
                        user.UserId,
                        user.Username,
                        user.Email,
                        user.FirstName,
                        user.LastName,
                        user.Phone,
                        user.RoleId
                    }
                };

                return Ok(response);
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

        [HttpGet("GetAllSubjects")]
        public ActionResult<IEnumerable<object>> GetAllSubjects()
        {
            var subjects = _context.Subjects
                .Select(s => new
                {
                    SubjectId = s.SubjectId,
                    Name = s.SubjectName, // Provjerite da li postoji ovo polje u bazi
                    Description = s.Description
                })
                .ToList();

            return Ok(subjects);
        }

        [HttpPost("AddSubject")]
        public IActionResult AddSubject([FromBody] SubjectDto subjectDto)
        {
            if (subjectDto == null || string.IsNullOrEmpty(subjectDto.Name))
            {
                return BadRequest("Invalid subject data.");
            }

            // Stvori novi Subject koristeći podatke iz DTO-a
            var subject = new Subject
            {
                SubjectName = subjectDto.Name,
                Description = subjectDto.Description,
                CreatedByUserId = subjectDto.UserId // Postavljamo UserId iz DTO-a
            };

            _context.Subjects.Add(subject);
            _context.SaveChanges();

            return Ok(new { message = "Subject added successfully" });
        }


        [HttpDelete("DeleteSubject/{subjectId}")]
        public ActionResult DeleteSubject(int subjectId)
        {
            var subject = _context.Subjects.FirstOrDefault(s => s.SubjectId == subjectId);
            if (subject == null)
                return NotFound("Subject not found.");

            _context.Subjects.Remove(subject);
            _context.SaveChanges();
            return Ok(new { message = "Subject deleted successfully." });
        }

    }
}
