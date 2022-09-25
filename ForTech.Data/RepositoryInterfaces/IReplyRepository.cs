using ForTech.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Data.RepositoryInterfaces
{
    public interface IReplyRepository
    {
        Task<ForumReply> AddForumReply(ForumReply ForumReply);
        Task<List<ForumReply>> GetAllRepliesByForum(Guid ForumId);
        Task<ForumReply> GetForumReply(Guid ReplyId);
        Task<bool> DeleteForum(Guid ReplyId);
        Task<ReplyUpvote> AddReplyUpvote(ReplyUpvote ReplyUpvote);
        Task<bool> DeleteReplyUpvote(Guid forumReplyId, string userId);
        Task<ForumReply> UpdateForumReplyUpvotes(Guid Id, bool isVoted);
        Task<int> GetReplyUpvotes(Guid Id);
        Task<bool> IsReplyLiked(Guid Id, string userId);  
    }
}
