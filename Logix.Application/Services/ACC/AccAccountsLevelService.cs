using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using System.Linq.Expressions;

namespace Logix.Application.Services.ACC
{
    public class AccAccountsLevelService : GenericQueryService<AccAccountsLevel, AccAccountsLevelDto, AccAccountsLevel>, IAccAccountsLevelService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public AccAccountsLevelService(IQueryRepository<AccAccountsLevel> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session, ILocalizationService localization) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;

            this.session = session;
            this.localization = localization;
        }

        public Task<IResult<AccAccountsLevelDto>> Add(AccAccountsLevelDto entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<IEnumerable<AccAccountsLevel>>> GetAllVW(Expression<Func<AccAccountsLevel, bool>> expression, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<AccAccountsLevel>> GetOneVW(Expression<Func<AccAccountsLevel, bool>> expression, CancellationToken cancellationToken = default)
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

        public Task<IResult<AccAccountsLevelEditDto>> Update(AccAccountsLevelEditDto entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult> Updatedigit(long level, int digit, string Color, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccAccountsLevelRepository.GetById(level);
            if (item == null) return Result<AccAccountsLevelDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));
            item.NoOfDigit = digit;
            item.Color = Color;
            accRepositoryManager.AccAccountsLevelRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccAccountsLevelDto>.SuccessAsync(_mapper.Map<AccAccountsLevelDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccAccountsLevelDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

    }
}
