using System.ComponentModel.DataAnnotations;

namespace ForTech.API.Models.RequestDTOs
{
    public class UserRegisterRequestDTO
    {
        [Required]
        public string Name { get; set; }
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
        [Required]
        public string ProfileImageUrl { get; set; }
    }
}
