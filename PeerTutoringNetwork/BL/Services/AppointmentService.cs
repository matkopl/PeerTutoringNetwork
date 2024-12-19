using BL.Models;
using Microsoft.EntityFrameworkCore;

namespace BL.Services;

public interface IAppointmentService
{
    Task CreateAppointment(Appointment NewAppointment);
    Task UpdateAppointment(int id, Appointment UpdatedAppointment);
    Task DeleteAppointment(int id);
    Task<Appointment> GetAppointment(int id);
    Task<List<Appointment>> GetAppointments();
}

public class AppointmentService : IAppointmentService
{
    private readonly PeerTutoringNetworkContext _context;

    public AppointmentService(PeerTutoringNetworkContext context)
    {
        _context = context;
    }

    public async Task CreateAppointment(Appointment newAppointment)
    {
        await _context.Appointments.AddAsync(newAppointment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAppointment(int id, Appointment UpdatedAppointment)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(ap => ap.AppointmentId == id);
        if (appointment == null)
            throw new Exception("Appointment not found");
       
        appointment.MentorId = UpdatedAppointment.MentorId;
        appointment.SubjectId = UpdatedAppointment.SubjectId;
        appointment.AppointmentDate = UpdatedAppointment.AppointmentDate;
  
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments.FirstOrDefaultAsync(ap => ap.AppointmentId == id);
        if (appointment == null)
            throw new Exception("Appointment not found");

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task<Appointment> GetAppointment(int id)
    {
        var appointment = await _context.Appointments
            .Include(a => a.Mentor)
            .Include(a => a.Subject)
            .FirstOrDefaultAsync(m => m.AppointmentId == id);
        if (appointment == null)
        {
            throw new Exception("Appointment not found");
        }

        return appointment;
    }

    public async Task<List<Appointment>> GetAppointments()
    {
        var appointments = await _context.Appointments
            .Include(a => a.Mentor)
            .Include(a => a.Subject)
            .ToListAsync();
        return appointments;
    }
}