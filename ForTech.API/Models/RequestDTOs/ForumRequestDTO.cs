using System;
using System.ComponentModel.DataAnnotations;

namespace ForTech.API.Models.RequestDTOs
{
    public class ForumRequestDTO
    {
        public Guid Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public Guid ChannelId { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
