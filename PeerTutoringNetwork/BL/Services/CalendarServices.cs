using System;
using System.Linq;
using System.Threading.Tasks;
using BL.Models;
using Microsoft.EntityFrameworkCore;

public class AppointmentService
{
    private readonly PeerTutoringNetworkContext _context;

    public AppointmentService(PeerTutoringNetworkContext context)
    {
        _context = context;
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