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
    public class AccJournalDetailesVwConfig : IEntityTypeConfiguration<AccJournalDetailesVw>
    {
        public void Configure(EntityTypeBuilder<AccJournalDetailesVw> entity)
        {
            entity.ToView("ACC_Journal_Detailes_VW");

            entity.Property(e => e.ChequDateHijri).IsFixedLength();

            entity.Property(e => e.JDateHijri).IsFixedLength();

            entity.Property(e => e.MJDateGregorian).IsFixedLength();
        }
    }
    
}