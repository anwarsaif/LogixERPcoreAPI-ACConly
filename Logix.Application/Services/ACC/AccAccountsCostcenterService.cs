using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccAccountsCostcenterService : GenericQueryService<AccAccountsCostcenter, AccAccountsCostcenterDto, AccAccountsCostcenter>, IAccAccountsCostcenterService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public AccAccountsCostcenterService(IQueryRepository<AccAccountsCostcenter> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session, ILocalizationService localization) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;

            this.session = session;
            this.localization = localization;
        }

        public async Task<IResult<AccAccountsCostcenterDto>> Add(AccAccountsCostcenterDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccAccountsCostcenterDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");

            try
            {
                if (entity.IsRequired== true && entity.IsEditable == false && string.IsNullOrEmpty(entity.CostCenterCode))
                {
                    return await Result<AccAccountsCostcenterDto>.FailAsync(localization.GetMessagesResource("MsgCcIdDefault"));
                }
                long CcIdDefault = 0;
                if (!string.IsNullOrEmpty(entity.CostCenterCode))
                {
                    CcIdDefault = await accRepositoryManager.AccCostCenterRepository.GetOne(s=>s.CcId,x=>x.IsDeleted == false && x.CostCenterCode==entity.CostCenterCode); 
               
                }
                entity.CcIdDefault = CcIdDefault;
                var item = _mapper.Map<AccAccountsCostcenter>(entity);
                var newEntity = await accRepositoryManager.AccAccountsCostcenterRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccAccountsCostcenterDto>(newEntity);


                return await Result<AccAccountsCostcenterDto>.SuccessAsync(entityMap, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<AccAccountsCostcenterDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccAccountsCostcenterRepository.GetById(Id);
            if (item == null) return Result<AccAccountsCostcenterDto>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccAccountsCostcenterRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccAccountsCostcenterDto>.SuccessAsync(_mapper.Map<AccAccountsCostcenterDto>(item), " record removed");
            }
            catch (Exception exp)
            {
                return await Result<AccAccountsCostcenterDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccAccountsCostcenterRepository.GetById(Id);
            if (item == null) return Result<AccAccountsCostcenterDto>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccAccountsCostcenterRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccAccountsCostcenterDto>.SuccessAsync(_mapper.Map<AccAccountsCostcenterDto>(item), " record removed");
            }
            catch (Exception exp)
            {
                return await Result<AccAccountsCostcenterDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccAccountsCostcenterEditDto>> Update(AccAccountsCostcenterEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccAccountsCostcenterEditDto>.FailAsync($"Error in {this.GetType()} : the passed entity IS NULL.");

            var item = await accRepositoryManager.AccAccountsCostcenterRepository.GetById(entity.Id);

            if (item == null) return await Result<AccAccountsCostcenterEditDto>.FailAsync($"--- there is no Data with this id: {entity.Id}---");
            item.IsDeleted = false;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            _mapper.Map(entity, item);

            accRepositoryManager.AccAccountsCostcenterRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccAccountsCostcenterEditDto>.SuccessAsync(_mapper.Map<AccAccountsCostcenterEditDto>(item), "Item updated successfully");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccAccountsCostcenterEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

    }
}
