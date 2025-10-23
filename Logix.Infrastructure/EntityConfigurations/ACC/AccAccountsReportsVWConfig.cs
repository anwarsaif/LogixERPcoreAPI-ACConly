using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccAccountsReportsVWConfig : IEntityTypeConfiguration<AccAccountsReportsVw>
    {
        public void Configure(EntityTypeBuilder<AccAccountsReportsVw> entity)
        {
            entity.ToView("ACC_Accounts_Reports_VW");
        }
    }

}
