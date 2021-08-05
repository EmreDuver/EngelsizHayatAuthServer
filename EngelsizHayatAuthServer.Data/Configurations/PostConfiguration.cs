using EngelsizHayatAuthServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace EngelsizHayatAuthServer.Data.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Text).IsRequired().HasMaxLength(280);
            builder.Property(x => x.CreateTime).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Like).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.Active).IsRequired().HasDefaultValue(true);

            builder.HasMany(x => x.Comments).WithOne(x => x.Post).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.NestedComments).WithOne(x => x.Post).HasForeignKey(x => x.PostId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
