using System;
using EngelsizHayatAuthServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngelsizHayatAuthServer.Data.Configurations
{
    public class UserAppConfiguration : IEntityTypeConfiguration<UserApp>
    {
        public void Configure(EntityTypeBuilder<UserApp> builder)
        {
            builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.LastName).HasMaxLength(100).IsRequired(false);
            builder.Property(x => x.BirthDay).HasDefaultValue(new DateTime(1900,01,01));
            builder.Property(x => x.GenderId).HasDefaultValue(4);
            builder.Property(x => x.Biography).HasMaxLength(200);
            builder.Property(x => x.PhotoPath).HasDefaultValue("default_user_photo.jpg");

            builder.HasOne<Location>(x => x.Location).WithOne(x => x.UserApp).HasForeignKey<Location>(x => x.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Posts).WithOne(x => x.UserApp).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.Comments).WithOne(x => x.UserApp).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.NestedComments).WithOne(x => x.UserApp).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
           
        }
    }
}