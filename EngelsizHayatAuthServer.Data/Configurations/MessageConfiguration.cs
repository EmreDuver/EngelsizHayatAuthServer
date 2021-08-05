using System;
using EngelsizHayatAuthServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngelsizHayatAuthServer.Data.Configurations
{
    public class MessageConfiguration: IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.SenderId).IsRequired();
            builder.Property(x => x.ReceiverId).IsRequired();
            builder.Property(x => x.Text).IsRequired().HasMaxLength(500);
            builder.Property(x => x.Status).HasDefaultValue(0);
            builder.Property(x => x.SendDateUnix).IsRequired();
            builder.Property(x => x.ReadDateUnix).HasDefaultValue(0);
        }
    }
}