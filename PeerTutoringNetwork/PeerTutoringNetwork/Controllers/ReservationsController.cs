using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.Viewmodels;
using BL.Models;
using PeerTutoringNetwork.DesignPatterns;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PeerTutoringNetwork.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly PeerTutoringNetworkContext _context;
        private readonly IFactory<AppointmentReservation, ReservationVM> _reservationFactory;
        private readonly IRepository<AppointmentReservation> _reservationRepository;
        private readonly ISubject _reservationNotifier;

        public ReservationsController(
            PeerTutoringNetworkContext context,
            IFactory<AppointmentReservation, ReservationVM> reservationFactory,
            IRepository<AppointmentReservation> reservationRepository,
            ISubject reservationNotifier)
        {
            _context = context;
            _reservationFactory = reservationFactory;
            _reservationRepository = reservationRepository;
            _reservationNotifier = reservationNotifier;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var reservations = await _reservationRepository.GetAllAsync();
            var reservationsVM = reservations.Select(r => _reservationFactory.CreateVM(r)).ToList();
            return View(reservationsVM);
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null) return NotFound();

            var reservationVM = _reservationFactory.CreateVM(reservation);
            return View(reservationVM);
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId");
            ViewData["StudentId"] = new SelectList(_context.Users, "UserId", "Username");
            return View();
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,StudentId,ReservationTime")] ReservationVM reservationVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid reservation data.");
            }

            var reservation = _reservationFactory.CreateModel(reservationVM);

            // Check if the appointment already has a reservation
            var isReserved = await _context.AppointmentReservations.AnyAsync(r => r.AppointmentId == reservationVM.AppointmentId);
            if (isReserved)
            {
                return Conflict("This appointment is already reserved.");
            }

            await _reservationRepository.AddAsync(reservation);

            // Notify observers
            _reservationNotifier.NotifyObservers($"Reservation created for Appointment ID: {reservation.AppointmentId}, Student ID: {reservation.StudentId}");

            return Json(new { success = true, message = "Appointment reserved successfully!" });
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _reservationRepository.GetByIdAsync(id.Value);
            if (reservation == null) return NotFound();

            var reservationVM = _reservationFactory.CreateVM(reservation);
            ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", reservationVM.AppointmentId);
            ViewData["StudentId"] = new SelectList(_context.Users, "UserId", "Username", reservationVM.StudentId);
            return View(reservationVM);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,AppointmentId,StudentId,ReservationTime")] ReservationVM reservationVM)
        {
            if (id != reservationVM.ReservationId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", reservationVM.AppointmentId);
                ViewData["StudentId"] = new SelectList(_context.Users, "UserId", "Username", reservationVM.StudentId);
                return View(reservationVM);
            }

            var reservation = _reservationFactory.CreateModel(reservationVM);
            await _reservationRepository.UpdateAsync(reservation);

            return RedirectToAction(nameof(Index));
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null) return NotFound();

            var reservationVM = _reservationFactory.CreateVM(reservation);
            return View(reservationVM);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null) return NotFound();

            await _reservationRepository.DeleteAsync(id);

            // Notify observers
            _reservationNotifier.NotifyObservers($"Reservation deleted for Appointment ID: {reservation.AppointmentId}, Student ID: {reservation.StudentId}");

            return RedirectToAction(nameof(Index));
        }
    }
}
