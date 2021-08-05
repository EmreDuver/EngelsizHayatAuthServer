using EngelsizHayatAuthServer.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EngelsizHayatAuthServer.Data.Configurations
{
    public class SignalRConnectionStringConfiguration: IEntityTypeConfiguration<SignalRConnectionString>
    {
        public void Configure(EntityTypeBuilder<SignalRConnectionString> builder)
        {
            builder.HasKey(x => x.UserId);
            builder.Property(x => x.UserId).ValueGeneratedNever();
            builder.Property(x => x.ConnectionId).IsRequired();
        }
    }
}