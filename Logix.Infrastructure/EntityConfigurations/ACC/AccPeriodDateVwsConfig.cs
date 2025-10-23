using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccPeriodDateVwsConfig : IEntityTypeConfiguration<AccPeriodDateVws>
    {
        public void Configure(EntityTypeBuilder<AccPeriodDateVws> entity)
        {
            entity.ToView("ACC_Period_Date_VW", "dbo");

            entity.Property(e => e.PeriodId).ValueGeneratedOnAdd();
        }
    }
}
