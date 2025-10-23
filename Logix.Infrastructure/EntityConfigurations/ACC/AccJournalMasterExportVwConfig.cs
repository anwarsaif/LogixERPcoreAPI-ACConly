using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccJournalMasterExportVwConfig : IEntityTypeConfiguration<AccJournalMasterExportVw>
    {
        public void Configure(EntityTypeBuilder<AccJournalMasterExportVw> entity)
        {
            entity.ToView("ACC_Journal_Master_Export_VW");
        }
    }
}
