using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Core
{
    public class UserFavouriteChannels
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public Guid ChannelId { get; set; }
        public string ChannelName { get; set; }
    }
}
