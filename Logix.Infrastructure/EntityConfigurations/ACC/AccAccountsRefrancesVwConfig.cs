using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccAccountsRefrancesVwConfig : IEntityTypeConfiguration<AccAccountsRefrancesVw>
    {
        public void Configure(EntityTypeBuilder<AccAccountsRefrancesVw> entity)
        {
            entity.ToView("Acc_Accounts_Refrances_VW");
        }
    }

}
