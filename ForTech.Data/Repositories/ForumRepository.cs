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
    public class ForumRepository : IForumRepository
    { 
    
        private readonly ApplicationDBContext _dbContext;
        public ForumRepository(ApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public async Task<ForumUpvote> AddUpvote(ForumUpvote ForumUpvote)
        {
            var upvote = await _dbContext.ForumUpvotes.AddAsync(ForumUpvote);
            await _dbContext.SaveChangesAsync();
            return upvote.Entity;
        }

        public async Task<Forum> CreateForum(Forum Forum)
        {
            var forum = await _dbContext.Forum.AddAsync(Forum);
            await _dbContext.SaveChangesAsync();
            return forum.Entity;
        }

        public async Task<bool> DeleteForum(Guid Id)
        {
            var forum = await GetForum(Id);
            if(forum != null)
            {
                _dbContext.Forum.Remove(forum);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteUpvote(string Id, Guid PostId)
        {
            var upvote = await _dbContext.ForumUpvotes.Where(x => x.ForumId == PostId).Where(x => x.UserId == Id).ToListAsync();
            if(upvote.Count == 0)
            {
                return false;
            }
            _dbContext.ForumUpvotes.Remove(upvote[0]);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Forum>> GetAllForums()
        {
            return await _dbContext.Forum.AsNoTracking().ToListAsync();
        }

        public async Task<List<Forum>> GetAllForumsByChannelId(Guid ChannelId)
        {
            return await _dbContext.Forum.Where(x => x.ChannelId == ChannelId).ToListAsync();
        }

        public async Task<List<Forum>> GetAllForumsByUserId(string UserId)
        {
            return await _dbContext.Forum.Where(x => x.UserId == UserId).ToListAsync();
        }

        public async Task<Forum> GetForum(Guid Id)
        {
            return await _dbContext.Forum.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<int> GetForumUpvotes(Guid Id)
        {
            var forum = await GetForum(Id);
            if(forum != null)
            {
                return forum.ForumUpvotes;
            }
            return 0;
        }

        public async Task<List<Forum>> GetTopForumsByChannelId(Guid ChannelId)
        {
            var forums = await GetAllForumsByChannelId(ChannelId);
            forums = forums.OrderByDescending(x => x.ForumUpvotes).OrderByDescending(x => x.ForumReplies).ToList();
            return forums;
        }

        public async Task<bool> IsForumLiked(string UserId, Guid Id)
        {
            var forumUpvote = await _dbContext.ForumUpvotes.Where(x => x.ForumId == Id).Where(x => x.UserId == UserId).ToListAsync();
            if(forumUpvote.Count == 1)
            {
                return true;
            }
            return false;
        }

        public async Task<Forum> UpdateForum(Forum Forum)
        {
            _dbContext.Forum.Update(Forum);
            await _dbContext.SaveChangesAsync();
            return Forum;
        }

        public async Task<Forum> UpdateReplies(Guid Id, bool reply)
        {
            var forum = await GetForum(Id);
            var updatedForum = new Forum()
            {
                Id = forum.Id,
                UserId = forum.UserId,
                ChannelId = forum.ChannelId,
                Description = forum.Description,
                DataCreated = forum.DataCreated,
                DateModified = DateTime.Now,
                ForumUpvotes = forum.ForumUpvotes,
                ForumReplies = reply ? forum.ForumReplies + 1 : forum.ForumReplies - 1,
            };
            _dbContext.Forum.Update(updatedForum);
            await _dbContext.SaveChangesAsync();
            return updatedForum;
        }

        public async Task<Forum> UpdateUpvotes(Guid Id, bool isVoted)
        {
            var forum = await GetForum(Id);
            var updatedForum = new Forum()
            {
                Id = forum.Id,
                UserId = forum.UserId,
                ChannelId = forum.ChannelId,
                Description = forum.Description,
                DataCreated = forum.DataCreated,
                DateModified = DateTime.Now,
                ForumUpvotes = isVoted ? forum.ForumUpvotes + 1 : forum.ForumUpvotes - 1,
                ForumReplies = forum.ForumReplies,
            };
            _dbContext.Forum.Update(updatedForum);
            await _dbContext.SaveChangesAsync();
            return updatedForum;
        }


    }
}
