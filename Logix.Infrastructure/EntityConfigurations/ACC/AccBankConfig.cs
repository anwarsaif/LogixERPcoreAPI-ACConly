using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccBankConfig:IEntityTypeConfiguration<AccBank>
    {
        public void Configure(EntityTypeBuilder<AccBank> entity)
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");


            entity.Property(b => b.CreatedBy)
            .HasColumnName("Insert_User_ID")
            .HasColumnType("int")
            .IsRequired(false);

            entity.Property(b => b.CreatedOn)
            .HasColumnName("Insert_Date")
            .HasColumnType("datetime");
            //.IsRequired(false);


            entity.Property(b => b.ModifiedBy)
.HasColumnName("Update_User_ID")
.HasColumnType("int")
.IsRequired(false);

            entity.Property(b => b.ModifiedOn)
            .HasColumnName("Update_Date")
            .HasColumnType("datetime")
            .IsRequired(false);


            entity.Property(b => b.IsDeleted)
.HasColumnName("FlagDelete")
.HasColumnType("bit")
.IsRequired(false);
        }
    }



}
