using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccCertificateSettingsSimulationService : GenericQueryService<AccCertificateSettingsSimulation, AccCertificateSettingsSimulationDto>, IAccCertificateSettingsSimulationService
    {
        public AccCertificateSettingsSimulationService(IQueryRepository<AccCertificateSettingsSimulation> queryRepository,
            IMapper mapper) : base(queryRepository, mapper)
        {

        }

        public Task<IResult<AccCertificateSettingsSimulationDto>> Add(AccCertificateSettingsSimulationDto entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<AccCertificateSettingsSimulationEditDto>> Update(AccCertificateSettingsSimulationEditDto entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
