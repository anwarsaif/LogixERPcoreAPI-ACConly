using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccFacilitiesVwConfig : IEntityTypeConfiguration<AccFacilitiesVw>
    {
        public void Configure(EntityTypeBuilder<AccFacilitiesVw> entity)
        {
            entity.ToView("ACC_Facilities_VW");
        }
    }
}
