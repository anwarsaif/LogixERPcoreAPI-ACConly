using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccCostCenteHelpVwConfig : IEntityTypeConfiguration<AccCostCenteHelpVw>
    {
        public void Configure(EntityTypeBuilder<AccCostCenteHelpVw> entity)
        {
            entity.ToView("ACC_CostCente_Help_VW");
        }
    }
}
