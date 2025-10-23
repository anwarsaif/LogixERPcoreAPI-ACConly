using Logix.Domain.ACC;
using Logix.Domain.Main;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccAccountConfig : IEntityTypeConfiguration<AccAccount>
    {
        public void Configure(EntityTypeBuilder<AccAccount> entity)
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






            entity.HasIndex(e => new { e.AccAccountCode, e.IsDeleted, e.FacilityId }, "Acc_Code")
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");


            entity.Property(e => e.CcId).HasComment("مركز التكلفة الافتراضي");

            entity.Property(e => e.CurrencyId).HasDefaultValueSql("((1))");

            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");

            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

            entity.Property(e => e.IsHelpAccount).HasDefaultValueSql("((0))");
        }
    }

}
