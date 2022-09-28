using System.ComponentModel.DataAnnotations;

namespace ForTech.API.Models.RequestDTOs
{
    public class UserRegisterRequestDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string City { get; set; }
        public string Skills { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string ProfileImageUrl { get; set; }
    }
}
