using Microsoft.AspNetCore.Identity;
using System;

namespace ForTech.Core
{
    public class User: IdentityUser
    {
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public string Role { get; set; }
        public string Tier { get; set; }
        public string City { get; set; }
        public int Score { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Skills { get; set; }
        public string Phone{ get; set; }
        public int Experience { get; set; }
    }
}
