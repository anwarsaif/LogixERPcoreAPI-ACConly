using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccRequestConfig : IEntityTypeConfiguration<AccRequest>
    {
        public void Configure(EntityTypeBuilder<AccRequest> entity)
        {

            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");
        }
    }
    
         public class AccPettyCashDConfig : IEntityTypeConfiguration<AccPettyCashD>
    {
        public void Configure(EntityTypeBuilder<AccPettyCashD> entity)
        {

            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");

            entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))");
        }
    }
}
