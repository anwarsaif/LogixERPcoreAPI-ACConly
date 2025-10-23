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
    public class AccCostCentersLevelService : GenericQueryService<AccCostCentersLevel, AccCostCentersLevelDto, AccCostCentersLevel>, IAccCostCentersLevelService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public AccCostCentersLevelService(IQueryRepository<AccCostCentersLevel> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session, ILocalizationService localization) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;

            this.session = session;
            this.localization = localization;
        }

        public Task<IResult<AccCostCentersLevelDto>> Add(AccCostCentersLevelDto entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<IEnumerable<AccCostCentersLevel>>> GetAllVW(Expression<Func<AccCostCentersLevel, bool>> expression, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<AccCostCentersLevel>> GetOneVW(Expression<Func<AccCostCentersLevel, bool>> expression, CancellationToken cancellationToken = default)
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

        public Task<IResult<AccCostCentersLevelEditDto>> Update(AccCostCentersLevelEditDto entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult> Updatedigit(long level, int digit, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccCostCentersLevelRepository.GetById(level);
            if (item == null) return Result<AccCostCentersLevelDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));
            item.NoOfDigit = digit;
            accRepositoryManager.AccCostCentersLevelRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccCostCentersLevelDto>.SuccessAsync(_mapper.Map<AccCostCentersLevelDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccCostCentersLevelDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

    }
}
