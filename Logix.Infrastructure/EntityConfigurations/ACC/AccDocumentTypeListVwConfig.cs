using Logix.Domain.ACC;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logix.Infrastructure.EntityConfigurations.ACC
{
    public class AccDocumentTypeListVwConfig : IEntityTypeConfiguration<AccDocumentTypeListVw>
    {
        public void Configure(EntityTypeBuilder<AccDocumentTypeListVw> entity)
        {
            entity.ToView("ACC_Document_Type_List_VW");
        }
    }  

}
