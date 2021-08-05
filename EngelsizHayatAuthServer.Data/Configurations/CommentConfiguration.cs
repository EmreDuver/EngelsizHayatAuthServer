using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngelsizHayatAuthServer.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Text).IsRequired().HasMaxLength(280);
            builder.Property(x => x.CreateTime).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Like).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.PostId).IsRequired();

            builder.HasMany(x => x.NestedComments).WithOne(x => x.Comment).HasForeignKey(x => x.CommentId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
