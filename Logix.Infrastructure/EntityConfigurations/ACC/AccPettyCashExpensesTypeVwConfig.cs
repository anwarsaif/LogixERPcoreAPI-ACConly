using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccPettyCashExpensesTypeVwConfig : IEntityTypeConfiguration<AccPettyCashExpensesTypeVw>
    {
        public void Configure(EntityTypeBuilder<AccPettyCashExpensesTypeVw> entity)
        {
            entity.ToView("ACC_Petty_Cash_Expenses_Type_VW");

        }
    }
}
