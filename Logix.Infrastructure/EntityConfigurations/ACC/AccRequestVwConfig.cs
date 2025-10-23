using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccRequestVwConfig : IEntityTypeConfiguration<AccRequestVw>
    {
        public void Configure(EntityTypeBuilder<AccRequestVw> entity)
        {
            entity.ToView("Acc_Request_VW");
        }
    }
    
}