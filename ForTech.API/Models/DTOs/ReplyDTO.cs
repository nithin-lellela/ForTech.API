using System;

namespace ForTech.API.Models.DTOs
{
    public class ReplyDTO
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int ForumReplyUpvotes { get; set; }
        public bool IsLiked { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}
