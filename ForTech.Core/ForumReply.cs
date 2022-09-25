using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Core
{
    public class ForumReply
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int ForumReplyUpvotes { get; set; }
    }
}
