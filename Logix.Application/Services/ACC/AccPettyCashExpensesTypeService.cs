using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Helpers.Acc;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccPettyCashExpensesTypeService : GenericQueryService<AccPettyCashExpensesType, AccPettyCashExpensesTypeDto, AccPettyCashExpensesTypeVw>, IAccPettyCashExpensesTypeService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData currentData;
        private readonly ILocalizationService localization;
        private readonly IGetAccountIDByCodeHelper getAccountIDByCodeHelper;

        public AccPettyCashExpensesTypeService(IQueryRepository<AccPettyCashExpensesType> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData currentData, ILocalizationService localization, IGetAccountIDByCodeHelper getAccountIDByCodeHelper) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;

            this.currentData = currentData;
            this.localization = localization;
            this.getAccountIDByCodeHelper = getAccountIDByCodeHelper;
        }

        public async Task<IResult<AccPettyCashExpensesTypeDto>> Add(AccPettyCashExpensesTypeDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccPettyCashExpensesTypeDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {
                //-------------------------------- '--جاب  رقم حساب 

                long AccAccountID = 0;



                if (entity.LinkAccounting == true)
                {
                    AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(1, entity.AccAccountCode);

                    if (AccAccountID == 0)
                    {
                        return await Result<AccPettyCashExpensesTypeDto>.WarningAsync(localization.GetAccResource("AccAccountNotfind"));

                    }



                }


                entity.AccAccountId = AccAccountID;
                entity.FacilityId = (int)currentData.FacilityId;
                var item = _mapper.Map<AccPettyCashExpensesType>(entity);
                var newEntity = await accRepositoryManager.AccPettyCashExpensesTypeRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccPettyCashExpensesTypeDto>(newEntity);


                return await Result<AccPettyCashExpensesTypeDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {

                return await Result<AccPettyCashExpensesTypeDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccPettyCashExpensesTypeRepository.GetById(Id);
            if (item == null) return Result<AccPettyCashExpensesTypeDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            accRepositoryManager.AccPettyCashExpensesTypeRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccPettyCashExpensesTypeDto>.SuccessAsync(_mapper.Map<AccPettyCashExpensesTypeDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccPettyCashExpensesTypeDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccPettyCashExpensesTypeRepository.GetById(Id);
            if (item == null) return Result<AccPettyCashExpensesTypeDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            accRepositoryManager.AccPettyCashExpensesTypeRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccPettyCashExpensesTypeDto>.SuccessAsync(_mapper.Map<AccPettyCashExpensesTypeDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccPettyCashExpensesTypeDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccPettyCashExpensesTypeEditDto>> Update(AccPettyCashExpensesTypeEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccPettyCashExpensesTypeEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");

            long AccAccountID = 0;



            if (entity.LinkAccounting == true)
            {
                AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(1, entity.AccAccountCode);

                if (AccAccountID == 0)
                {
                    return await Result<AccPettyCashExpensesTypeEditDto>.WarningAsync(localization.GetAccResource("AccAccountNotfind"));

                }



            }


            entity.AccAccountId = AccAccountID;
            entity.FacilityId = (int)currentData.FacilityId;

            var item = await accRepositoryManager.AccPettyCashExpensesTypeRepository.GetById(entity.Id);

            if (item == null) return await Result<AccPettyCashExpensesTypeEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
            item.IsDeleted = false;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            _mapper.Map(entity, item);

            accRepositoryManager.AccPettyCashExpensesTypeRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccPettyCashExpensesTypeEditDto>.SuccessAsync(_mapper.Map<AccPettyCashExpensesTypeEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccPettyCashExpensesTypeEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

    }
}
