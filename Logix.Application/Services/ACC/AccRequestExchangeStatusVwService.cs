using AutoMapper;
using Logix.Application.Common;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccRequestExchangeStatusVwService : GenericQueryService<AccRequestExchangeStatusVw, AccRequestExchangeStatusVw, AccRequestExchangeStatusVw>, IAccRequestExchangeStatusVwService
    {
        private readonly IAccRepositoryManager accRepositoryManager;

        private readonly IMapper _mapper;
        private readonly ICurrentData session;

        public AccRequestExchangeStatusVwService(IQueryRepository<AccRequestExchangeStatusVw> queryRepository, IAccRepositoryManager AccRepositoryManager, IMapper mapper, ICurrentData session) : base(queryRepository, mapper)
        {
            this.accRepositoryManager = AccRepositoryManager;
            this._mapper = mapper;

            this.session = session;
        }
    }

}
