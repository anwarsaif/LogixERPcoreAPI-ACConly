using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccReferenceTypeConfig : IEntityTypeConfiguration<AccReferenceType>
    {
        public void Configure(EntityTypeBuilder<AccReferenceType> entity)
        {
            
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(getdate())");


            entity.Property(e => e.FlagDelete).HasDefaultValueSql("((0))");

       
        }
    }

}
