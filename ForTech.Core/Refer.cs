using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Core
{
    public class Refer
    {
        public Guid Id { get; set; }
        public Guid ForumId { get; set; }
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsReferOpened { get; set; }
    }
}
