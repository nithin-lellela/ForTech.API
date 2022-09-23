using System;
using System.ComponentModel.DataAnnotations;

namespace ForTech.API.Models.RequestDTOs
{
    public class UserFavChannelReqDTO
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public Guid ChannelId { get; set; }
        [Required]
        public string ChannelName { get; set; }
    }
}
