using EngelsizHayatAuthServer.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EngelsizHayatAuthServer.Data
{
    public class AppDbContext : IdentityDbContext<UserApp , IdentityRole , string>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            
        }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<Gender> Genders{ get; set; }
        public DbSet<AppRole> AppRoles{ get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<NestedComment> NestedComments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<SignalRConnectionString> SignalRConnectionStrings { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(builder);
        }
    }
}
