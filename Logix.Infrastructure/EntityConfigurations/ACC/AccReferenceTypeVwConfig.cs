using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccReferenceTypeVwConfig : IEntityTypeConfiguration<AccReferenceTypeVw>
    {
        public void Configure(EntityTypeBuilder<AccReferenceTypeVw> entity)
        {
            entity.ToView("ACC_Reference_Type_VW");
        }
    }

}
