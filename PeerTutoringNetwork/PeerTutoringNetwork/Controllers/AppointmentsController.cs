using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BL.Models;
using BL.Services;
using PeerTutoringNetwork.Viewmodels;

namespace PeerTutoringNetwork.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly PeerTutoringNetworkContext _context;
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(PeerTutoringNetworkContext context, IAppointmentService appointmentService)
        {
            _context = context;
            _appointmentService = appointmentService;
        }

        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetAppointments();
            var appointmentVms = appointments.Select(a => new AppointmentVM
            {
                AppointmentId = a.AppointmentId,
                MentorId = a.MentorId,
                SubjectId = a.SubjectId,
                MentorUsername = a.Mentor.Username,
                SubjectName = a.Subject.SubjectName,
                AppointmentDate = a.AppointmentDate
            });

            return View(appointmentVms);
        }

        public async Task<IActionResult> Create()
        {
            // TODO select all mentors and all subjects
            ViewData["MentorId"] = new SelectList(_context.Users, "UserId", "Username");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "SubjectId", "SubjectName");
            return View();
        }

        public async Task<IActionResult> CreateAction(AppointmentVM appointmentVM)
        {
            var appointment = new Appointment
            {
                MentorId = appointmentVM.MentorId,
                SubjectId = appointmentVM.SubjectId,
                AppointmentDate = appointmentVM.AppointmentDate
            };

            await _appointmentService.CreateAppointment(appointment);
            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/EditAction/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest("id is required");
            // TODO select all mentors and all subjects
            try
            {
                var appointment = await  _appointmentService.GetAppointment(id.Value);
                var appointmentVM = new AppointmentVM
                {
                    AppointmentId = appointment.AppointmentId,
                    MentorId = appointment.MentorId,
                    SubjectId = appointment.SubjectId,
                    AppointmentDate = appointment.AppointmentDate
                };
                ViewData["MentorId"] = new SelectList(_context.Users, "UserId", "Username", appointmentVM.MentorId);
                ViewData["SubjectId"] =
                    new SelectList(_context.Subjects, "SubjectId", "SubjectName", appointmentVM.SubjectId);

                return View(appointmentVM);
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }

        // POST: Appointments/EditAction/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAction(AppointmentVM appointmentVM)
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
                    await _appointmentService.UpdateAppointment(appointment.AppointmentId, appointment);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                return RedirectToAction(nameof(Index));
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAction(int id)
        {
            try
            {
                await _appointmentService.DeleteAppointment(id);
            }
            catch (Exception)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest("Id is required");
            try
            {
                var appointment = await _appointmentService.GetAppointment(id.Value);
                var appointmentVM = new AppointmentVM
                {
                    AppointmentId = appointment.AppointmentId,
                    MentorUsername = appointment.Mentor.Username,
                    SubjectName = appointment.Subject.SubjectName,
                    AppointmentDate = appointment.AppointmentDate
                };
                return View(appointmentVM);
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }

        public IActionResult Calendar()
        {
            //TODO need to figure out what is the current user
            return View(new List<AppointmentVM>()
            {
                new AppointmentVM()
                {
                    MentorUsername = "Mentor Mock",
                    SubjectName = "Subject Mock",
                    AppointmentDate = DateTime.Now + TimeSpan.FromDays(1)
                },
                new AppointmentVM()
                {
                    MentorUsername = "Mentor Mock",
                    SubjectName = "Subject Mock",
                    AppointmentDate = DateTime.Now + TimeSpan.FromDays(4)
                },
                new AppointmentVM()
                {
                    MentorUsername = "Mentor Mock",
                    SubjectName = "Subject Mock",
                    AppointmentDate = DateTime.Now + TimeSpan.FromDays(-5)
                }
            });
        }
    }
}