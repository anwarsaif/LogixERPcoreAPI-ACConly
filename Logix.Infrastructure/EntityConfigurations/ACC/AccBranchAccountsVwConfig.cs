using Logix.Application.Services.ACC;
using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccBranchAccountsVwConfig : IEntityTypeConfiguration<AccBranchAccountsVw>
    {
        public void Configure(EntityTypeBuilder<AccBranchAccountsVw> entity)
        {
            entity.ToView("Acc_Branch_Accounts_VW");
        }
    }
}
