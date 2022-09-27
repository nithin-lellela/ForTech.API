using System;

namespace ForTech.API.Models.DTOs
{
    public class ForumDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid ChannelId { get; set; }
        public string UserName { get; set; }
        public string ChannelName { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int ForumUpvotes { get; set; }
        public int ForumReplies { get; set; }
        public bool IsLiked { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}
