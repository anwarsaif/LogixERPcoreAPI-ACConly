using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccJournalDetaileConfig : IEntityTypeConfiguration<AccJournalDetaile>
    {
        public void Configure(EntityTypeBuilder<AccJournalDetaile> entity)
        {

            entity.Property(e => e.Auto).HasDefaultValueSql("((0))");

            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.Property(e => e.ReferenceNo).HasComment("رقم المرجع في نظام التقسيط");

            entity.Property(b => b.CreatedBy)
            .HasColumnName("Insert_User_ID")
            .HasColumnType("int")
            .IsRequired(false);

            entity.Property(b => b.CreatedOn)
            .HasColumnName("Insert_Date")
            .HasColumnType("datetime")
            .IsRequired(false);


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