using System;
using System.Linq;
using System.Threading.Tasks;
using BL.Models;
using Microsoft.EntityFrameworkCore;

// TODO  Interface Segregation Principle 
public interface IAppointmentService
{
    Task<List<Appointment>> GetAppointments();
    Task<bool> DeleteAppointment(int id);
}
public class AppointmentService : IAppointmentService
{
    private readonly PeerTutoringNetworkContext _context;

    public AppointmentService(PeerTutoringNetworkContext context)
    {
        _context = context;
    }

    public async Task<List<Appointment>> GetAppointments()
    {
        var appointments = await _context.Appointments
             .Include(a => a.AppointmentReservations)
             .Include(a => a.Mentor)
             .Include(a => a.Subject)
             .ToListAsync();
        return appointments;
    }

    public async Task<bool> DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return false;
        }

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return true;
    }
}