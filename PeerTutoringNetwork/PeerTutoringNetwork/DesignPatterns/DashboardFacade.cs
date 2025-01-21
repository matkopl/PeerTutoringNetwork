using BL.Models;
using Microsoft.EntityFrameworkCore;
using PeerTutoringNetwork.Viewmodels;

namespace PeerTutoringNetwork.DesignPatterns
{
    public class DashboardFacade
    {
        private readonly PeerTutoringNetworkContext _context;

        public DashboardFacade(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        public async Task<StudentDashboardVM> GetStudentDashboardData(int userId)
        {
            var reservations = await _context.AppointmentReservations
                .Include(r => r.Appointment)
                .ThenInclude(a => a.Mentor)
                .Include(r => r.Appointment.Subject)
                .Where(r => r.StudentId == userId)
                .Select(r => new ReservationVM
                {
                    ReservationId = r.ReservationId,
                    AppointmentId = r.AppointmentId,
                    MentorUsername = r.Appointment.Mentor.Username,
                    SubjectName = r.Appointment.Subject.SubjectName,
                    ReservationTime = r.ReservationTime ?? DateTime.Now
                })
                .ToListAsync();

            var availableAppointments = await _context.Appointments
                .Include(a => a.Mentor)
                .Include(a => a.Subject)
                .Where(a => !_context.AppointmentReservations.Any(r => r.AppointmentId == a.AppointmentId))
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

            return new StudentDashboardVM
            {
                Reservations = reservations,
                AvailableAppointments = availableAppointments
            };
        }

        public MentorDashboardVM GetMentorDashboardData()
        {
            return new MentorDashboardVM
            {
                TotalAppointments = _context.Appointments.Count(),
                TotalSubjects = _context.Subjects.Count(),
                RecentAppointments = _context.Appointments
                    .OrderByDescending(a => a.AppointmentDate)
                    .Take(5)
                    .ToList(),
                RecentSubjects = _context.Subjects
                    .OrderByDescending(s => s.SubjectId)
                    .Take(5)
                    .ToList()
            };
        }
    }
}
