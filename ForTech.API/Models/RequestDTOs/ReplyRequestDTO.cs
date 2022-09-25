using System;
using System.ComponentModel.DataAnnotations;

namespace ForTech.API.Models.RequestDTOs
{
    public class ReplyRequestDTO
    {
        [Required]
        public Guid ForumId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
