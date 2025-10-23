using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccAccountsSubHelpeVwConfig : IEntityTypeConfiguration<AccAccountsSubHelpeVw>
    {
        public void Configure(EntityTypeBuilder<AccAccountsSubHelpeVw> entity)
        {
            entity.ToView("ACC_Accounts_Sub_helpe_VW");
        }
    }
}
