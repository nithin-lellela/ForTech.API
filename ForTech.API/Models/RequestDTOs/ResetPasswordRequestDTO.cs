using System.ComponentModel.DataAnnotations;

namespace ForTech.API.Models.RequestDTOs
{
    public class ResetPasswordRequestDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        [Required]
        [DataType(dataType: DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(dataType: DataType.Password)]
        public string ConfirmNewPassword { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
