using Microsoft.AspNetCore.Mvc;
using PeerTutoringNetwork.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.DTOs;

namespace PeerTutoringNetwork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly PeerTutoringNetworkContext _context;

        public ProfileController(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        // GET: api/Profile
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfileDTO>>> GetProfiles()
        {
            try
            {
                var profiles = await _context.Profiles
                    .Include(p => p.User) // Ako su korisnički podaci povezani s profilom
                    .Select(p => new ProfileDTO
                    {
                        ProfileId = p.ProfileId,
                        FullName = $"{p.FirstName} {p.LastName}",
                        Email = p.User.Username, // Pretpostavka da 'User' ima email/username polje
                        PhoneNumber = p.PhoneNumber
                    })
                    .ToListAsync();

                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // GET api/Profile/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Profile>> GetProfileById(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound($"Profile with ID {id} not found.");
            }
            return Ok(profile);
        }

        // POST api/Profile
        [HttpPost]
        public async Task<ActionResult<Profile>> CreateProfile([FromBody] Profile profile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetProfileById), new { id = profile.ProfileId }, profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/Profile/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] Profile profile)
        {
            if (id != profile.ProfileId)
            {
                return BadRequest("ID in URL and request body do not match.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProfile = await _context.Profiles.FindAsync(id);
            if (existingProfile == null)
            {
                return NotFound($"Profile with ID {id} not found.");
            }

            // Update existing profile fields
            existingProfile.FirstName = profile.FirstName;
            existingProfile.LastName = profile.LastName;
            existingProfile.PhoneNumber = profile.PhoneNumber;
            existingProfile.Bio = profile.Bio;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent(); // 204 No Content
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/Profile/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound($"Profile with ID {id} not found.");
            }

            try
            {
                _context.Profiles.Remove(profile);
                await _context.SaveChangesAsync();
                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
