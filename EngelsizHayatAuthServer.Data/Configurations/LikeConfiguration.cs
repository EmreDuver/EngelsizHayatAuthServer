using EngelsizHayatAuthServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngelsizHayatAuthServer.Data.Configurations
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasKey(x => new {x.Id, x.UserId, x.Type});
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.UserId).ValueGeneratedNever();
            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.Type).ValueGeneratedNever();
            builder.Property(x => x.Type).IsRequired();
        }
    }
}