using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccCertificateSettingConfig : IEntityTypeConfiguration<AccCertificateSetting>
    {
        public void Configure(EntityTypeBuilder<AccCertificateSetting> entity)
        {
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ExpiredDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.StartedDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");
        }
    }
}
