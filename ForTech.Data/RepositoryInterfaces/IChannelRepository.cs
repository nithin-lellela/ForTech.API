using ForTech.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Data.RepositoryInterfaces
{
    public interface IChannelRepository
    {
        Task<Channel> CreateNewChannel(Channel Channel);
        Task<List<Channel>> GetAllChannels();
        Task<Channel> GetChannel(Guid Id);
        Task<bool> IsChannelExists(string Name);
        Task<UserFavouriteChannels> AddUserFavouriteChannel(UserFavouriteChannels Channel);
        Task<List<UserFavouriteChannels>> GetAllUserFavouriteChannels(string Id);
        Task<bool> RemoveUserFavouriteChannel(string Id, Guid ChannelId);
        Task<bool> UpdateChannel(Channel Channel);
        Task<List<Channel>> GetTopChannels();
    }
}
