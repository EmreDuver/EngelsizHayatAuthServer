using EngelsizHayatAuthServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngelsizHayatAuthServer.Data.Configurations
{
    public class NestedCommentConfiguration : IEntityTypeConfiguration<NestedComment>
    {
        public void Configure(EntityTypeBuilder<NestedComment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Text).IsRequired().HasMaxLength(280);
            builder.Property(x => x.CreateTime).IsRequired();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Like).IsRequired().HasDefaultValue(0);
            builder.Property(x => x.PostId).IsRequired();
            builder.Property(x => x.CommentId).IsRequired();


        }
    }
}