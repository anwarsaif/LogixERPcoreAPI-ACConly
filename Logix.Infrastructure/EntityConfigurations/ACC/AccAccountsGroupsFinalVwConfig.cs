using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccAccountsGroupsFinalVwConfig : IEntityTypeConfiguration<AccAccountsGroupsFinalVw>
    {
        public void Configure(EntityTypeBuilder<AccAccountsGroupsFinalVw> entity)
        {
            entity.ToView("ACC_Accounts_Groups_Final_VW");
        }
    }
}
