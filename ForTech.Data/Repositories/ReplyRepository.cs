using ForTech.Core;
using ForTech.Data.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Data.Repositories
{
    public class ReplyRepository : IReplyRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public ReplyRepository(ApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<ForumReply> AddForumReply(ForumReply ForumReply)
        {
            await _dbContext.ForumReply.AddAsync(ForumReply);
            await _dbContext.SaveChangesAsync();
            return ForumReply;
        }

        public async Task<ReplyUpvote> AddReplyUpvote(ReplyUpvote ReplyUpvote)
        {
            var replyUpvote = await _dbContext.ReplyUpvotes.AddAsync(ReplyUpvote);
            await _dbContext.SaveChangesAsync();
            return replyUpvote.Entity;
        }

        public async Task<bool> DeleteForum(Guid ReplyId)
        {
            var forumReply = await GetForumReply(ReplyId);
            if(forumReply == null)
            {
                return false;
            }
            _dbContext.ForumReply.Remove(forumReply);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReplyUpvote(Guid forumReplyId, string userId)
        {
            var upvote = await _dbContext.ReplyUpvotes.Where(x => x.ForumReplyId == forumReplyId).Where(x => x.UserId == userId).ToListAsync();
            if(upvote.Count == 0)
            {
                return false;
            }
            _dbContext.ReplyUpvotes.Remove(upvote[0]);
            await _dbContext.SaveChangesAsync();
            return true;
         }

        public async Task<List<ForumReply>> GetAllRepliesByForum(Guid ForumId)
        {
            return await _dbContext.ForumReply.AsNoTracking().Where(x => x.ForumId == ForumId).ToListAsync();
        }

        public async Task<ForumReply> GetForumReply(Guid ReplyId)
        {
            return await _dbContext.ForumReply.AsNoTracking().FirstOrDefaultAsync(x => x.Id == ReplyId);
        }

        public async Task<int> GetReplyUpvotes(Guid Id)
        {
            var forumReply = await GetForumReply(Id);
            if(forumReply != null)
            {
                return forumReply.ForumReplyUpvotes;
            }
            return 0;
        }

        public async Task<bool> IsReplyLiked(Guid Id, string userId)
        {
            var reply = await _dbContext.ReplyUpvotes.Where(x => x.ForumReplyId == Id).Where(x => x.UserId == userId).ToListAsync();
            if(reply.Count == 1)
            {
                return true;
            }
            return false;
        }

        public async Task<ForumReply> UpdateForumReplyUpvotes(Guid Id, bool isVoted)
        {
            var reply = await GetForumReply(Id);
            var updatedForumReply = new ForumReply()
            {
                Id = reply.Id,
                UserId = reply.UserId,
                ForumId = reply.ForumId,
                DateCreated = reply.DateCreated,
                UserName = reply.UserName,
                Description = reply.Description,
                ForumReplyUpvotes = isVoted ? reply.ForumReplyUpvotes + 1 : reply.ForumReplyUpvotes - 1
            };
            _dbContext.ForumReply.Update(updatedForumReply);
            await _dbContext.SaveChangesAsync();
            return updatedForumReply;
        }
    }
}
