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
    public class GenderConfiguration : IEntityTypeConfiguration<Gender>
    {
        public void Configure(EntityTypeBuilder<Gender> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();

            //builder.HasOne<UserApp>(x => x.UserApp).WithOne(x => x.Gender).HasForeignKey<UserApp>(x => x.GenderId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
