using BL.lnterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Class
{
    public class StudentUser : IUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Role => 1;
    }
}
