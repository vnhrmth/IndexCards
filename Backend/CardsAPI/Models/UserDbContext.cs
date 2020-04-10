using System;
using Microsoft.EntityFrameworkCore;

namespace CardsAPI.Models
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbTopic> Topics { get; set; }
        public DbSet<DbCard> Cards { get; set; }
    }
}
