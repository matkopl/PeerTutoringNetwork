using BL.Models;
using Microsoft.EntityFrameworkCore;

namespace PeerTutoringNetwork.DesignPatterns
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T value);
        Task UpdateAsync(T value);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }

    public class SubjectRepository : IRepository<Subject>
    {
        private readonly PeerTutoringNetworkContext _context;

        public SubjectRepository(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _context.Subjects.Include(s => s.CreatedByUser).ToListAsync();
        }

        public async Task<Subject> GetByIdAsync(int id)
        {
            return await _context.Subjects.Include(s => s.CreatedByUser)
                                          .FirstOrDefaultAsync(s => s.SubjectId == id);
        }

        public async Task AddAsync(Subject entity)
        {
            await _context.Subjects.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Subject entity)
        {
            _context.Subjects.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Subjects.AnyAsync(e => e.SubjectId == id);
        }
    }

    public class ReservationRepository : IRepository<AppointmentReservation>
    {
        private readonly PeerTutoringNetworkContext _context;

        public ReservationRepository(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AppointmentReservation>> GetAllAsync()
        {
            return await _context.AppointmentReservations
                .Include(r => r.Appointment)
                .ThenInclude(a => a.Mentor)
                .Include(r => r.Appointment.Subject)
                .Include(r => r.Student)
                .ToListAsync();
        }

        public async Task<AppointmentReservation> GetByIdAsync(int id)
        {
            return await _context.AppointmentReservations
                .Include(r => r.Appointment)
                .ThenInclude(a => a.Mentor)
                .Include(r => r.Appointment.Subject)
                .Include(r => r.Student)
                .FirstOrDefaultAsync(r => r.ReservationId == id);
        }

        public async Task AddAsync(AppointmentReservation value)
        {
            await _context.AppointmentReservations.AddAsync(value);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AppointmentReservation value)
        {
            _context.AppointmentReservations.Update(value);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var reservation = await _context.AppointmentReservations.FindAsync(id);
            if (reservation != null)
            {
                _context.AppointmentReservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.AppointmentReservations.AnyAsync(r => r.ReservationId == id);
        }
    }
}
