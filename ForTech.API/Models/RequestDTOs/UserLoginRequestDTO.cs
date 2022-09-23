using System.ComponentModel.DataAnnotations;

namespace ForTech.API.Models.RequestDTOs
{
    public class UserLoginRequestDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
