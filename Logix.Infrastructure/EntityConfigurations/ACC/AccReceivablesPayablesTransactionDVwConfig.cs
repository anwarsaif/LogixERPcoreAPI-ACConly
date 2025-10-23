using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccReceivablesPayablesTransactionDVwConfig : IEntityTypeConfiguration<AccReceivablesPayablesTransactionDVw>
    {
        public void Configure(EntityTypeBuilder<AccReceivablesPayablesTransactionDVw> entity)
        {
            entity.ToView("Acc_Receivables_Payables_Transaction_D_VW");

        }
    }
}
