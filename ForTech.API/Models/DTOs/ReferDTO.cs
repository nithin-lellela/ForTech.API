using System;

namespace ForTech.API.Models.DTOs
{
    public class ReferDTO
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public Guid ChannelId { get; set; }
        public string ForumUserName { get; set; }
        public string ForumUserId { get; set; }
        public string ChannelName { get; set; }
        public string SenderUserId { get; set; }
        public string SenderUserName { get; set; }
        public string ReceiverUserId { get; set; }
        public string ReceiverUserName { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsReferOpened { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}
