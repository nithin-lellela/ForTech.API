using System;

namespace ForTech.API.Models.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        public string Role { get; set; }
        public string Tier { get; set; }
        public string City { get; set; }
        public int Score { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Token { get; set; }
        public int Experience { get; set; }
        public string Skills { get; set; }
        public string PhoneNumber { get; set; }
    }
}
