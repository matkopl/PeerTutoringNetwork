using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PeerTutoringNetwork.Viewmodels
{
    public class AppointmentVM
    {
        public int AppointmentId { get; set; }

        [Required(ErrorMessage = "Mentor selection is required.")]
        public int MentorId { get; set; }

        [Required(ErrorMessage = "Subject selection is required.")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        public DateTime AppointmentDate { get; set; }

        [ValidateNever] 
        public string MentorUsername { get; set; }

        [ValidateNever] 
        public string SubjectName { get; set; }
    }
}
