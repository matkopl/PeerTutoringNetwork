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
    public class ReservationController : ControllerBase
    {
        private readonly PeerTutoringNetworkContext _context;

        public ReservationController(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        // GET: api/Reservation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservations()
        {
            try
            {
                var reservations = await _context.AppointmentReservations
                    .Include(r => r.Appointment) // Load appointment details
                    .Include(r => r.Student)    // Load student details
                    .Select(r => new ReservationDTO
                    {
                        ReservationId = r.ReservationId,
                        AppointmentId = r.AppointmentId,
                        StudentId = r.StudentId,
                        StudentName = $"{r.Student.Profiles.FirstOrDefault().FirstName} {r.Student.Profiles.FirstOrDefault().LastName}",
                        AppointmentDate = r.Appointment.AppointmentDate,
                        StartTime = r.Appointment.StartTime,
                        EndTime = r.Appointment.EndTime
                    })
                    .ToListAsync();

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/Reservation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservationById(int id)
        {
            var reservation = await _context.AppointmentReservations
                .Include(r => r.Appointment)
                .Include(r => r.Student)
                .Where(r => r.ReservationId == id)
                .Select(r => new ReservationDTO
                {
                    ReservationId = r.ReservationId,
                    AppointmentId = r.AppointmentId,
                    StudentId = r.StudentId,
                    StudentName = $"{r.Student.Profiles.FirstOrDefault().FirstName} {r.Student.Profiles.FirstOrDefault().LastName}",
                    AppointmentDate = r.Appointment.AppointmentDate,
                    StartTime = r.Appointment.StartTime,
                    EndTime = r.Appointment.EndTime
                })
                .FirstOrDefaultAsync();

            if (reservation == null)
            {
                return NotFound($"Reservation with ID {id} not found.");
            }

            return Ok(reservation);
        }

        // POST api/Reservation
        [HttpPost]
        public async Task<ActionResult<AppointmentReservation>> CreateReservation([FromBody] AppointmentReservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Check if appointment has available slots
                var appointment = await _context.Appointments
                    .Include(a => a.AppointmentReservations)
                    .FirstOrDefaultAsync(a => a.AppointmentId == reservation.AppointmentId);

                if (appointment == null)
                {
                    return NotFound("Appointment not found.");
                }

                _context.AppointmentReservations.Add(reservation);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetReservationById), new { id = reservation.ReservationId }, reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/Reservation/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] AppointmentReservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return BadRequest("ID in URL and request body do not match.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingReservation = await _context.AppointmentReservations.FindAsync(id);
            if (existingReservation == null)
            {
                return NotFound($"Reservation with ID {id} not found.");
            }

            existingReservation.AppointmentId = reservation.AppointmentId;
            existingReservation.StudentId = reservation.StudentId;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE api/Reservation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.AppointmentReservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound($"Reservation with ID {id} not found.");
            }

            try
            {
                _context.AppointmentReservations.Remove(reservation);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
