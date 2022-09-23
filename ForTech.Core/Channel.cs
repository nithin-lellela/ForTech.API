using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForTech.Core
{
    public class Channel
    {
        public Guid Id { get; set; }
        public string ChannelName { get; set; }
        public int NoOfInteractions { get; set; }
    }
}
