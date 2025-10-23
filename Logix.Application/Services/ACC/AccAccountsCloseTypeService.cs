using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;

using Logix.Domain.Main;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccAccountsCloseTypeService : GenericQueryService<AccAccountsCloseType, AccAccountsCloseType, AccAccountsCloseType>, IAccAccountsCloseTypeService
    {
        private readonly IAccRepositoryManager accRepositoryManager;

        private readonly IMapper _mapper;
        private readonly ICurrentData session;

        public AccAccountsCloseTypeService(IQueryRepository<AccAccountsCloseType> queryRepository, IAccRepositoryManager AccRepositoryManager, IMapper mapper, ICurrentData session) : base(queryRepository, mapper)
        {
            this.accRepositoryManager = AccRepositoryManager;
            this._mapper = mapper;

            this.session = session;
        }
    }
   
}
