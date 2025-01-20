using BL.lnterfaces;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Class
{// 5. Strategy pattern -- ovo je Strategy pattern jer se koristi za izvršavanje različitih akcija u zavisnosti od korisnika
    public class StudentActionStrategy : IUserActionStrategy
    {
        private readonly PeerTutoringNetworkContext _context;

        public StudentActionStrategy(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        public void ExecuteAction(int userId)
        {
            Console.WriteLine($"Student Action: Creating a new appointment for User ID: {userId}");

            var appointment = new Appointment
            { 
                MentorId = 2, 
                SubjectId = 1, 
                AppointmentDate = DateTime.Now.AddDays(1)
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            Console.WriteLine("New appointment created successfully.");
        }
    }
}
