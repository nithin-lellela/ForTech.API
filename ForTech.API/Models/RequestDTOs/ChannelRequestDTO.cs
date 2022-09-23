using System.ComponentModel.DataAnnotations;

namespace ForTech.API.Models.RequestDTOs
{
    public class ChannelRequestDTO
    {
        [Required]
        public string Name { get; set; }
    }
}
