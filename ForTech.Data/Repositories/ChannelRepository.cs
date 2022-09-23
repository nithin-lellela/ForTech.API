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
    public class ChannelRepository : IChannelRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public ChannelRepository(ApplicationDBContext DbContext)
        {
            _dbContext = DbContext;
        }

        public async Task<UserFavouriteChannels> AddUserFavouriteChannel(UserFavouriteChannels Channel)
        {
            if(await IsUserFavouriteExists(Channel.UserId, Channel.ChannelId) == null)
            {
                _dbContext.FavouriteChannels.Add(Channel);
                await _dbContext.SaveChangesAsync();
                return Channel;
            }
            return null;
        }

        public async Task<Channel> CreateNewChannel(Channel Channel)
        {
            _dbContext.Channel.Add(Channel);
            await _dbContext.SaveChangesAsync();
            return Channel;
        }

        public async Task<List<Channel>> GetAllChannels()
        {
            return await _dbContext.Channel.ToListAsync();
        }

        public async Task<List<UserFavouriteChannels>> GetAllUserFavouriteChannels(string Id)
        {
            return await _dbContext.FavouriteChannels.Where(x => x.UserId == Id).ToListAsync();
        }

        public async Task<Channel> GetChannel(Guid Id)
        {
            return await _dbContext.Channel.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<bool> IsChannelExists(string Name)
        {
            var channel = await _dbContext.Channel.FirstOrDefaultAsync(x => x.ChannelName == Name);
            if (channel != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveUserFavouriteChannel(string Id, Guid ChannelId)
        {
            var isExists = await IsUserFavouriteExists(Id, ChannelId);
            if (isExists != null)
            {
                _dbContext.FavouriteChannels.Remove(isExists);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<UserFavouriteChannels> IsUserFavouriteExists(string Id, Guid ChannelId)
        {
            var userChannels = await GetAllUserFavouriteChannels(Id);
            var channel = userChannels.FirstOrDefault(x => x.ChannelId == ChannelId);
            return channel;
        }
    }
}
