using BL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("[action]")]
        public ActionResult GetToken()
        {
            try
            {
                // The same secure key must be used here to create JWT,
                // as the one that is used by middleware to verify JWT
                var secureKey = _configuration["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 10);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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
                var b64salt = PasswordHashProvider.GetSalt(); // Ovo je već u Base64 string formatu
                var b64hash = PasswordHashProvider.GetHash(registerDto.Password, b64salt); // Također vraća Base64 hash

                Console.WriteLine($"Salt (Base64): {b64salt}");
                Console.WriteLine($"Hash (Base64): {b64hash}");

                // Pohranjujemo u bazi kao byte[], zato ih konvertujemo:
                var user = new User
                {
                    UserId = registerDto.Id,
                    Username = registerDto.Username,
                    PwdHash = Convert.FromBase64String(b64hash), // Konvertiramo hash u byte[]
                    PwdSalt = Convert.FromBase64String(b64salt), // Konvertiramo salt u byte[]
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    Phone = registerDto.Phone,
                    RoleId = registerDto.RoleId
                };

                // Dodavanje korisnika u bazu
                _context.Add(user);
                _context.SaveChanges();

                // Update DTO Id to return it to the client
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
                var genericLoginFail = "Incorrect username or password";

                // Trim korisničkog imena za sigurnost
                var trimmedUsername = loginDto.Username.Trim();

                // Dohvati korisnika prema obrezanom korisničkom imenu
                var existingUser = _context.Users.FirstOrDefault(x => x.Username == trimmedUsername);
                if (existingUser == null)
                    return Unauthorized("Username does not exist");

                // Pretvori salt iz baze (byte[] u Base64 string)
                var saltBase64 = Convert.ToBase64String(existingUser.PwdSalt);

                // Generiraj hash unesenog passworda koristeći salt iz baze
                var computedHashBase64 = PasswordHashProvider.GetHash(loginDto.Password, saltBase64);

                // Dohvati spremljeni hash iz baze u Base64 formatu
                var storedHashBase64 = Convert.ToBase64String(existingUser.PwdHash);

                // Usporedi generirani hash sa spremljenim hashom
                if (computedHashBase64 != storedHashBase64)
                    return Unauthorized("Incorrect password");

                // Kreiraj JWT token
                var secureKey = _configuration["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 60, trimmedUsername);

                // Vrati token
                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }



    }
}

