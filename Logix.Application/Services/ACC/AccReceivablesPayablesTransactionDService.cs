using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Helpers;
using Logix.Application.Helpers.Acc;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccReceivablesPayablesTransactionDService : GenericQueryService<AccReceivablesPayablesTransactionD, AccReceivablesPayablesTransactionDDto, AccReceivablesPayablesTransactionDVw>, IAccReceivablesPayablesTransactionDService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly IHrRepositoryManager hrRepositoryManager;
       private readonly IGetAccountIDByCodeHelper getAccountIDByCodeHelper;
       private readonly IGetRefranceByCodeHelper getRefranceByCodeHelper;
        private readonly ISysConfigurationAppHelper sysConfigurationAppHelper;
        private readonly IWorkflowHelper workflowHelper;
        private readonly IMainRepositoryManager mainRepositoryManager;
        private readonly ILocalizationService localization;

        public AccReceivablesPayablesTransactionDService(IQueryRepository<AccReceivablesPayablesTransactionD> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session,IHrRepositoryManager hrRepositoryManager,IGetAccountIDByCodeHelper getAccountIDByCodeHelper, IGetRefranceByCodeHelper getRefranceByCodeHelper,ISysConfigurationAppHelper SysConfigurationAppHelper, IWorkflowHelper workflowHelper,IMainRepositoryManager mainRepositoryManager, ILocalizationService localization) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;
            this.session = session;
            this.hrRepositoryManager = hrRepositoryManager;
            this.getAccountIDByCodeHelper = getAccountIDByCodeHelper;
            this.getRefranceByCodeHelper = getRefranceByCodeHelper;
            this.sysConfigurationAppHelper = SysConfigurationAppHelper;
            this.workflowHelper = workflowHelper;
            this.mainRepositoryManager = mainRepositoryManager;
            this.localization = localization;
        }

        public Task<IResult<AccReceivablesPayablesTransactionDDto>> Add(AccReceivablesPayablesTransactionDDto entity, CancellationToken cancellationToken = default)
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

        public Task<IResult<AccReceivablesPayablesTransactionDEditDto>> Update(AccReceivablesPayablesTransactionDEditDto entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
