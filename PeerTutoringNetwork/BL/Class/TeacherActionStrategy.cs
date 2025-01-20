using BL.lnterfaces;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Class
{// 5. Strategy pattern -- ovo je Strategy pattern jer se koristi za izvršavanje različitih akcija u zavisnosti od korisnika
    public class TeacherActionStrategy : IUserActionStrategy
    {
        private readonly PeerTutoringNetworkContext _context;

        public TeacherActionStrategy(PeerTutoringNetworkContext context)
        {
            _context = context;
        }

        public void ExecuteAction(int userId)
        {
            Console.WriteLine($"Teacher Action: Fetching appointments and reviews for User ID: {userId}");

            var appointments = _context.Appointments.Where(a => a.MentorId == userId).ToList();
            var reviews = _context.Reviews.Where(r => r.UserId == userId).ToList();

            Console.WriteLine("Appointments:");
            foreach (var appointment in appointments)
            {
                Console.WriteLine($"Appointment ID: {appointment.AppointmentId}, Subject: {appointment.SubjectId}");
            }

            Console.WriteLine("Reviews:");
            foreach (var review in reviews)
            {
                Console.WriteLine($"Review ID: {review.ReviewId}, Rating: {review.Rating}");
            }
        }
    }
}
