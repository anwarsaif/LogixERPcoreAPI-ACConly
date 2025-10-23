using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccSettlementInstallmentsVwConfig : IEntityTypeConfiguration<AccSettlementInstallmentsVw>
    {
        public void Configure(EntityTypeBuilder<AccSettlementInstallmentsVw> entity)
        {
            entity.ToView("Acc_Settlement_Installments_VW");
        }
    }
    

      
}
