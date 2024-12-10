using System.ComponentModel.DataAnnotations;

namespace PeerTutoringNetwork.DTOs
{
    public class UserRegisterDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "First name should be between 2 and 100 characters long")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name should be between 2 and 100 characters long")]
        public string LastName { get; set; }

        [StringLength(500, ErrorMessage = "Bio should not exceed 500 characters")]
        public string Bio { get; set; }

        [Phone(ErrorMessage = "Provide a correct phone number")]
        [StringLength(20, ErrorMessage = "Phone number should not exceed 20 characters")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Role ID is required")]
        public int RoleId { get; set; }
    }
}
