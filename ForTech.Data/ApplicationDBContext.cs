using ForTech.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace ForTech.Data
{
    public class ApplicationDBContext : IdentityDbContext<User, IdentityRole, string>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Channel> Channel { get; set; }
        public DbSet<UserFavouriteChannels> FavouriteChannels { get; set; }
        public DbSet<Forum> Forum { get; set; }
        public DbSet<ForumUpvote> ForumUpvotes { get; set; }
        public DbSet<ForumReply> ForumReply { get; set; }
        public DbSet<ReplyUpvote> ReplyUpvotes { get; set; }
        public DbSet<Refer> Refers { get; set; }
    }
}
