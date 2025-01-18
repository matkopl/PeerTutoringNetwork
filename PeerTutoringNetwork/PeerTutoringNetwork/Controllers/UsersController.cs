using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BL.Models;
using PeerTutoringNetwork.DTOs;
using PeerTutoringNetwork.Security;
using BL.Factories;

namespace PeerTutoringNetwork.Controllers
{ // 2. Factory pattern -- ovo je Factory pattern jer se koristi za instanciranje objekata u metodi AddUser
    public class UsersController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly PeerTutoringNetworkContext _context;

        public UsersController(IConfiguration configuration, PeerTutoringNetworkContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] UserRegisterDto userDto)
        {
            try
            {
                // Koristi Factory Method za stvaranje korisnika
                var user = UserFactory.CreateUser(userDto.RoleId);

                user.UserId = userDto.Id;
                user.Username = userDto.Username;
                user.Email = userDto.Email;

               
                _context.Users.Add(new User
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    RoleId = userDto.RoleId switch
                    {
                        3 => 3, // Admin
                        2 => 2, // Teacher
                        1 => 1 ,// Student
                        _ => 0
                    }
                });

                _context.SaveChanges();

                return Ok("User added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var peerTutoringNetworkContext = _context.Users.Include(u => u.Role);
            return View(await peerTutoringNetworkContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,PwdHash,PwdSalt,FirstName,LastName,Email,Phone,RoleId")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,PwdHash,PwdSalt,FirstName,LastName,Email,Phone,RoleId")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
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
                // Jasne poruke za neuspješne pokušaje
                var genericLoginFail = "Incorrect username or password";
                var usernameFail = "Username does not exist";
                var passwordFail = "Incorrect password";

                // Dohvati korisnika iz baze prema korisničkom imenu
                var existingUser = _context.Users.FirstOrDefault(x => x.Username == loginDto.Username);
                if (existingUser == null)
                    return Unauthorized(usernameFail);

                // Pretvori `PwdSalt` iz baze (byte[]) natrag u Base64 string
                var saltBase64 = Convert.ToBase64String(existingUser.PwdSalt);
                Console.WriteLine($"[Login] Retrieved Salt (Base64) from DB: {saltBase64}");

                // Generiraj hash unesenog passworda koristeći isti salt
                var computedHashBase64 = PasswordHashProvider.GetHash(loginDto.Password, saltBase64);
                Console.WriteLine($"[Login] Computed Hash (Base64) for input password: {computedHashBase64}");

                // Pretvori spremljeni hash iz baze (byte[]) u Base64 string
                var storedHashBase64 = Convert.ToBase64String(existingUser.PwdHash);
                Console.WriteLine($"[Login] Retrieved Hash (Base64) from DB: {storedHashBase64}");


                // Usporedi hash-eve
                if (computedHashBase64 != storedHashBase64)
                    return Unauthorized(passwordFail);

                // Kreiraj i vrati JWT token
                var secureKey = _configuration["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 60, loginDto.Username);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
