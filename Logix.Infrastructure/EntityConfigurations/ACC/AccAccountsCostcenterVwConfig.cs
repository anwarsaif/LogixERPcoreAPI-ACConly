using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccAccountsCostcenterVwConfig : IEntityTypeConfiguration<AccAccountsCostcenterVw>
    {
        public void Configure(EntityTypeBuilder<AccAccountsCostcenterVw> entity)
        {
            entity.ToView("Acc_Accounts_Costcenter_VW");
        }
    }
}
