using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccSettlementScheduleDService : GenericQueryService<AccSettlementScheduleD, AccSettlementScheduleDDto, AccSettlementScheduleDVw>, IAccSettlementScheduleDService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;

        public AccSettlementScheduleDService(IQueryRepository<AccSettlementScheduleD> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;

            this.session = session;
        }

        public async Task<IResult<AccSettlementScheduleDDto>> Add(AccSettlementScheduleDDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccSettlementScheduleDDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");

            try
            {

                var item = _mapper.Map<AccSettlementScheduleD>(entity);
                var newEntity = await accRepositoryManager.AccSettlementScheduleDRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccSettlementScheduleDDto>(newEntity);


                return await Result<AccSettlementScheduleDDto>.SuccessAsync(entityMap, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<AccSettlementScheduleDDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccSettlementScheduleDRepository.GetById(Id);
            if (item == null) return Result<AccSettlementScheduleDDto>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccSettlementScheduleDRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccSettlementScheduleDDto>.SuccessAsync(_mapper.Map<AccSettlementScheduleDDto>(item), " record removed");
            }
            catch (Exception exp)
            {
                return await Result<AccSettlementScheduleDDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccSettlementScheduleDRepository.GetById(Id);
            if (item == null) return Result<AccSettlementScheduleDDto>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccSettlementScheduleDRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccSettlementScheduleDDto>.SuccessAsync(_mapper.Map<AccSettlementScheduleDDto>(item), " record removed");
            }
            catch (Exception exp)
            {
                return await Result<AccSettlementScheduleDDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

    
      
        public async Task<IResult<AccSettlementScheduleDEditDto>> Update(AccSettlementScheduleDEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccSettlementScheduleDEditDto>.FailAsync($"Error in {this.GetType()} : the passed entity IS NULL.");

            var item = await accRepositoryManager.AccSettlementScheduleDRepository.GetById(entity.Id);

            if (item == null) return await Result<AccSettlementScheduleDEditDto>.FailAsync($"--- there is no Data with this id: {entity.Id}---");

            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            _mapper.Map(entity, item);

            accRepositoryManager.AccSettlementScheduleDRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccSettlementScheduleDEditDto>.SuccessAsync(_mapper.Map<AccSettlementScheduleDEditDto>(item), "Item updated successfully");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccSettlementScheduleDEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

    }
}