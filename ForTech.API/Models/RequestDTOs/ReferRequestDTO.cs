using System;

namespace ForTech.API.Models.RequestDTOs
{
    public class ReferRequestDTO
    {
        public Guid ForumId { get; set; }
        public Guid ChannelId { get; set; }
        public string ForumOwnerName { get; set; }
        public string ForumUserId { get; set; }
        public string ChannelName { get; set; }
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
    }
}
