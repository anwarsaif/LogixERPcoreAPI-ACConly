using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccSettlementInstallmentService : GenericQueryService<AccSettlementInstallment, AccSettlementInstallmentDto, AccSettlementInstallmentsVw>, IAccSettlementInstallmentService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;

        public AccSettlementInstallmentService(IQueryRepository<AccSettlementInstallment> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;

            this.session = session;
        }

        public async Task<IResult<AccSettlementInstallmentDto>> Add(AccSettlementInstallmentDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccSettlementInstallmentDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");

            try
            {

                var item = _mapper.Map<AccSettlementInstallment>(entity);
                var newEntity = await accRepositoryManager.AccSettlementInstallmentRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccSettlementInstallmentDto>(newEntity);


                return await Result<AccSettlementInstallmentDto>.SuccessAsync(entityMap, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<AccSettlementInstallmentDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccSettlementInstallmentRepository.GetById(Id);
            if (item == null) return Result<AccSettlementInstallmentDto>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccSettlementInstallmentRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccSettlementInstallmentDto>.SuccessAsync(_mapper.Map<AccSettlementInstallmentDto>(item), " record removed");
            }
            catch (Exception exp)
            {
                return await Result<AccSettlementInstallmentDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccSettlementInstallmentRepository.GetById(Id);
            if (item == null) return Result<AccSettlementInstallmentDto>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccSettlementInstallmentRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccSettlementInstallmentDto>.SuccessAsync(_mapper.Map<AccSettlementInstallmentDto>(item), " record removed");
            }
            catch (Exception exp)
            {
                return await Result<AccSettlementInstallmentDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }



        public async Task<IResult<AccSettlementInstallmentEditDto>> Update(AccSettlementInstallmentEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccSettlementInstallmentEditDto>.FailAsync($"Error in {this.GetType()} : the passed entity IS NULL.");

            var item = await accRepositoryManager.AccSettlementInstallmentRepository.GetById(entity.Id);

            if (item == null) return await Result<AccSettlementInstallmentEditDto>.FailAsync($"--- there is no Data with this id: {entity.Id}---");

            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            _mapper.Map(entity, item);

            accRepositoryManager.AccSettlementInstallmentRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccSettlementInstallmentEditDto>.SuccessAsync(_mapper.Map<AccSettlementInstallmentEditDto>(item), "Item updated successfully");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccSettlementInstallmentEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

    }
}