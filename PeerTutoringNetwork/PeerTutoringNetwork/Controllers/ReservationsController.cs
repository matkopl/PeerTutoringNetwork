using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BL.Models;

namespace PeerTutoringNetwork.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly PeerTutoringNetworkContext _context;

        public ReservationsController(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var peerTutoringNetworkContext = _context.AppointmentReservations.Include(a => a.Appointment).Include(a => a.Student);
            return View(await peerTutoringNetworkContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentReservation = await _context.AppointmentReservations
                .Include(a => a.Appointment)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (appointmentReservation == null)
            {
                return NotFound();
            }

            return View(appointmentReservation);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId");
            ViewData["StudentId"] = new SelectList(_context.Users, "UserId", "Username");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,AppointmentId,StudentId,ReservationTime")] AppointmentReservation appointmentReservation)
        {


            _context.Add(appointmentReservation);
            await _context.SaveChangesAsync();

            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", appointmentReservation.AppointmentId);
            ViewData["StudentId"] = new SelectList(_context.Users, "UserId", "Username", appointmentReservation.StudentId);
            return View(appointmentReservation);
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentReservation = await _context.AppointmentReservations.FindAsync(id);
            if (appointmentReservation == null)
            {
                return NotFound();
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", appointmentReservation.AppointmentId);
            ViewData["StudentId"] = new SelectList(_context.Users, "UserId", "Username", appointmentReservation.StudentId);
            return View(appointmentReservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,AppointmentId,StudentId,ReservationTime")] AppointmentReservation appointmentReservation)
        {
            if (id != appointmentReservation.ReservationId)
            {
                return NotFound();
            }

            ModelState.Remove("Student");
            ModelState.Remove("Appointment");


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointmentReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentReservationExists(appointmentReservation.ReservationId))
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
            foreach (var entry in ModelState)
            {
                Console.WriteLine($"Key: {entry.Key}, AttemptedValue: {entry.Value.AttemptedValue}");
                foreach (var error in entry.Value.Errors)
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", appointmentReservation.AppointmentId);
            ViewData["StudentId"] = new SelectList(_context.Users, "UserId", "Username", appointmentReservation.StudentId);
            return View(appointmentReservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointmentReservation = await _context.AppointmentReservations
                .Include(a => a.Appointment)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (appointmentReservation == null)
            {
                return NotFound();
            }

            return View(appointmentReservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointmentReservation = await _context.AppointmentReservations.FindAsync(id);
            if (appointmentReservation != null)
            {
                _context.AppointmentReservations.Remove(appointmentReservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentReservationExists(int id)
        {
            return _context.AppointmentReservations.Any(e => e.ReservationId == id);
        }
    }
}