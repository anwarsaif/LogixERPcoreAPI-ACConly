using Logix.Application.DTOs.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccCertificateSettingService : IGenericQueryService<AccCertificateSettingDto>, IGenericWriteService<AccCertificateSettingDto, AccCertificateSettingEditDto>
    {
    }
}
