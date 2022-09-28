using ForTech.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Data.RepositoryInterfaces
{
    public interface IForumRepository
    {
        Task<Forum> CreateForum(Forum Forum);
        Task<List<Forum>> GetAllForums();
        Task<Forum> GetForum(Guid Id);
        Task<List<Forum>> GetAllForumsByChannelId(Guid ChannelId);
        Task<List<Forum>> GetAllForumsByUserId(string UserId);
        Task<List<Forum>> GetTopForumsByChannelId(Guid ChannelId);
        Task<List<Forum>> GetUnansweredByChannelId(Guid channelId);
        Task<int> GetChannelForumsInteractions(Guid channelId);
        Task<Forum> UpdateForum(Forum Forum);
        Task<bool> DeleteForum(Guid Id);
        Task<Forum> UpdateUpvotes(Guid Id, bool isVoted);
        Task<Forum> UpdateReplies(Guid Id, bool reply);
        Task<ForumUpvote> AddUpvote(ForumUpvote ForumUpvote);
        Task<bool> DeleteUpvote(string Id, Guid PostId);
        Task<int> GetForumUpvotes(Guid Id);
        Task<bool> IsForumLiked(string UserId, Guid Id);


    }
}
