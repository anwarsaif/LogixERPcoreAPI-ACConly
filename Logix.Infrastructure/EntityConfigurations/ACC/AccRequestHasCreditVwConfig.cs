using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccRequestHasCreditVwConfig : IEntityTypeConfiguration<AccRequestHasCreditVw>
    {
        public void Configure(EntityTypeBuilder<AccRequestHasCreditVw> entity)
        {
            entity.ToView("AccRequestHasCreditVw");
        }
    }

}
