using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Core
{
    public class ReplyUpvote
    {
        public Guid Id { get; set; }
        public Guid ForumReplyId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
