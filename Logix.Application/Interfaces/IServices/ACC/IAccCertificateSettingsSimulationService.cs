using Logix.Application.DTOs.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccCertificateSettingsSimulationService : IGenericQueryService<AccCertificateSettingsSimulationDto>, IGenericWriteService<AccCertificateSettingsSimulationDto, AccCertificateSettingsSimulationEditDto>
    {
    }
}
