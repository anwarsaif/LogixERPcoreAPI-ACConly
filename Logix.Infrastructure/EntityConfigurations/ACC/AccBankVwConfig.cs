using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccBankVwConfig : IEntityTypeConfiguration<AccBankVw>
    {
        public void Configure(EntityTypeBuilder<AccBankVw> entity)
        {
            entity.ToView("Acc_Bank_Vw");
        }
    }
}
