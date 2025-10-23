using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccRequestEmployeeVwConfig : IEntityTypeConfiguration<AccRequestEmployeeVw>
    {
        public void Configure(EntityTypeBuilder<AccRequestEmployeeVw> entity)
        {
            entity.ToView("Acc_Request_Employee_VW");
        }
    }

}