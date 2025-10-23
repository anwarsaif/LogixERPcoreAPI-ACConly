using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccJournalSignatureVwConfig : IEntityTypeConfiguration<AccJournalSignatureVw>
    {
        public void Configure(EntityTypeBuilder<AccJournalSignatureVw> entity)
        {
            entity.ToView("ACC_Journal_Signature_VW");
        }
    }
}
