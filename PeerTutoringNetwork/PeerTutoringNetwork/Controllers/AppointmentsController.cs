using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BL.Models;
using PeerTutoringNetwork.Viewmodels;

namespace PeerTutoringNetwork.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly PeerTutoringNetworkContext _context;

        public AppointmentsController(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Mentor)
                .Include(a => a.Subject)
                .Select(a => new AppointmentVM
                {
                    AppointmentId = a.AppointmentId,
                    MentorId = a.MentorId,
                    SubjectId = a.SubjectId,
                    MentorUsername = a.Mentor.Username,
                    SubjectName = a.Subject.SubjectName,
                    AppointmentDate = a.AppointmentDate
                })
                .ToListAsync();

            return View(appointments);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["MentorId"] = new SelectList(_context.Users, "UserId", "Username");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName");
            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MentorId,SubjectId,AppointmentDate")] AppointmentVM appointmentVM)
        {
            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    MentorId = appointmentVM.MentorId,
                    SubjectId = appointmentVM.SubjectId,
                    AppointmentDate = appointmentVM.AppointmentDate
                };

                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MentorId"] = new SelectList(_context.Users, "UserId", "Username", appointmentVM.MentorId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName", appointmentVM.SubjectId);
            return View(appointmentVM);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            var appointmentVM = new AppointmentVM
            {
                AppointmentId = appointment.AppointmentId,
                MentorId = appointment.MentorId,
                SubjectId = appointment.SubjectId,
                AppointmentDate = appointment.AppointmentDate
            };

            ViewData["MentorId"] = new SelectList(_context.Users, "UserId", "Username", appointmentVM.MentorId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName", appointmentVM.SubjectId);
            return View(appointmentVM);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,MentorId,SubjectId,AppointmentDate")] AppointmentVM appointmentVM)
        {
            if (id != appointmentVM.AppointmentId) return NotFound();

            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    AppointmentId = appointmentVM.AppointmentId,
                    MentorId = appointmentVM.MentorId,
                    SubjectId = appointmentVM.SubjectId,
                    AppointmentDate = appointmentVM.AppointmentDate
                };

                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["MentorId"] = new SelectList(_context.Users, "UserId", "Username", appointmentVM.MentorId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName", appointmentVM.SubjectId);
            return View(appointmentVM);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Mentor)
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null) return NotFound();

            var appointmentVM = new AppointmentVM
            {
                AppointmentId = appointment.AppointmentId,
                MentorUsername = appointment.Mentor.Username,
                SubjectName = appointment.Subject.SubjectName,
                AppointmentDate = appointment.AppointmentDate
            };

            return View(appointmentVM);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Mentor)
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null) return NotFound();

            var appointmentVM = new AppointmentVM
            {
                AppointmentId = appointment.AppointmentId,
                MentorUsername = appointment.Mentor.Username,
                SubjectName = appointment.Subject.SubjectName,
                AppointmentDate = appointment.AppointmentDate
            };

            return View(appointmentVM);
        }

    }
}
