﻿using BL.Models;
using Microsoft.AspNetCore.Authorization;
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
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PeerTutoringNetworkContext _context;

        public UserController(IConfiguration configuration, PeerTutoringNetworkContext context)
        {
            _configuration = configuration;
            _context = context;
        }


        [HttpPost("[action]")]
        public ActionResult<UserRegisterDto> Register(UserRegisterDto registerDto)
        {
            try
            {
                // Check if there is such a username in the database already
                var trimmedUsername = registerDto.Username.Trim();
                if (_context.Users.Any(x => x.Username.Equals(trimmedUsername)))
                    return BadRequest($"Username {trimmedUsername} already exists");

                // Hashiranje i generiranje salt-a pri registraciji
                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(registerDto.Password, b64salt);

                Console.WriteLine($"Salt (Base64): {b64salt}");
                Console.WriteLine($"Hash (Base64): {b64hash}");


                var user = new User
                {
                    UserId = registerDto.Id,
                    Username = registerDto.Username,
                    PwdHash = Convert.FromBase64String(b64hash),
                    PwdSalt = Convert.FromBase64String(b64salt),
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    Phone = registerDto.Phone,
                    RoleId = registerDto.RoleId
                };


                _context.Add(user);
                _context.SaveChanges();


                registerDto.Id = user.UserId;

                return Ok(registerDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult Login(UserLoginDto loginDto)
        {
            try
            {
                var existingUser = _context.Users.FirstOrDefault(x => x.Username == loginDto.Username);
                if (existingUser == null)
                    return Unauthorized("Username does not exist");

                var saltBase64 = Convert.ToBase64String(existingUser.PwdSalt);
                var computedHashBase64 = PasswordHashProvider.GetHash(loginDto.Password, saltBase64);
                var storedHashBase64 = Convert.ToBase64String(existingUser.PwdHash);

                if (computedHashBase64 != storedHashBase64)
                    return Unauthorized("Incorrect password");

                var secureKey = _configuration["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(
                    secureKey,
                    60,
                    existingUser.Username,
                    existingUser.UserId.ToString(),
                    existingUser.RoleId.ToString()
                );

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public ActionResult GetProfile(int userId)
        {
            try
            {
                var user = _context.Users
                    .Select(u => new UserProfileDto
                    {
                        UserId = u.UserId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Phone = u.Phone,
                        Username = u.Username,
                        RoleId = u.RoleId
                    })
                    .FirstOrDefault(u => u.UserId == userId);

                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("[action]")]
        public ActionResult UpdateProfile(UserProfileUpdateDto updateDto)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == updateDto.UserId);

                if (user == null)
                    return NotFound("User not found");

                // Ažuriranje podataka
                user.FirstName = updateDto.FirstName ?? user.FirstName;
                user.LastName = updateDto.LastName ?? user.LastName;
                user.Email = updateDto.Email ?? user.Email;
                user.Phone = updateDto.Phone ?? user.Phone;

                _context.SaveChanges();

                return Ok("Profile updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("[action]")]
        public ActionResult ClearOptionalData(int userId)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

                if (user == null)
                    return NotFound("User not found");

                // Brisanje neobaveznih podataka
                user.Phone = null;
                user.Email = null;

                _context.SaveChanges();

                return Ok("Optional data cleared successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                // Dohvaćanje svih korisnika iz baze
                var users = _context.Users
                    .Select(user => new
                    {
                        Id = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        Role = user.Role.RoleName
                    })
                    .ToList();

                return Ok(users); // Vraća korisnike kao JSON
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                // Provjera postoji li korisnik s istim korisničkim imenom ili emailom
                if (_context.Users.Any(x => x.Username == userDto.Username || x.Email == userDto.Email))
                    return BadRequest("User with the same username or email already exists");

                // Hashiranje lozinke i generiranje salt-a
                var salt = PasswordHashProvider.GetSalt();
                var hash = PasswordHashProvider.GetHash(userDto.Password, salt);

                // Kreiraj novog korisnika
                var user = new User
                {
                    Username = userDto.Username,
                    Email = userDto.Email,
                    PwdHash = Convert.FromBase64String(hash),
                    PwdSalt = Convert.FromBase64String(salt),
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    RoleId = userDto.RoleId
                };

                // Dodaj korisnika u bazu
                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok("User added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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

    }
}

