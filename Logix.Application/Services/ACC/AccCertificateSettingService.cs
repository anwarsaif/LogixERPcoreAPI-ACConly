using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccCertificateSettingService : GenericQueryService<AccCertificateSetting, AccCertificateSettingDto>, IAccCertificateSettingService
    {
        public AccCertificateSettingService(IQueryRepository<AccCertificateSetting> queryRepository,
            IMapper mapper) : base(queryRepository, mapper)
        {

        }

        public Task<IResult<AccCertificateSettingDto>> Add(AccCertificateSettingDto entity, CancellationToken cancellationToken = default)
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

        public Task<IResult<AccCertificateSettingEditDto>> Update(AccCertificateSettingEditDto entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
