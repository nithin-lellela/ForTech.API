using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Core
{
    public class Forum
    {
        public Guid Id { get; set; }
        public Guid ChannelId { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public DateTime DataCreated { get; set; }
        public DateTime DateModified { get; set; }
        public int ForumUpvotes { get; set; }
        public int ForumReplies { get; set; }

    }
}
