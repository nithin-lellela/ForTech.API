using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Core
{
    public class ForumUpvote
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
