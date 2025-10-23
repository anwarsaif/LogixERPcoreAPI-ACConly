using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccGroupService : GenericQueryService<AccGroup, AccGroupDto, AccGroup>, IAccGroupService
    {
        private readonly IAccRepositoryManager accRepositoryManager;

        private readonly IMapper _mapper;
        private readonly ICurrentData currentData;
        private readonly ILocalizationService localization;

        public AccGroupService(IQueryRepository<AccGroup> queryRepository, IAccRepositoryManager AccRepositoryManager, IMapper mapper, ICurrentData currentData, ILocalizationService localization) : base(queryRepository, mapper)
        {
            this.accRepositoryManager = AccRepositoryManager;
            this._mapper = mapper;

            this.currentData = currentData;
            this.localization = localization;
        }
        public async Task<IResult<AccGroupDto>> Add(AccGroupDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                return await Result<AccGroupDto>.FailAsync(localization.GetMessagesResource("AddNullEntity"));

            try
            {
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

                // تحويل البيانات إلى الكيان المناسب
                var accGroup = _mapper.Map<AccGroup>(entity);
                var savedGroup = await accRepositoryManager.AccGroupRepository.AddAndReturn(accGroup);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                // إنشاء الحساب المرتبط بالمجموعة
                var accountDto = new AccAccountDto
                {
                    AccAccountCode = entity.AccGroupCode,
                    AccAccountName = entity.AccGroupName,
                    AccAccountName2 = entity.AccGroupName2,
                    AccAccountParentId = 0,
                    AccGroupId = savedGroup.AccGroupId,
                    CcId = 0,
                    IsSub = true,
                    AccountCloseTypeId = 0,
                    DeptID = 0,
                    FacilityId = currentData.FacilityId,
                    SystemId = 2,
                };

                var accountEntity = _mapper.Map<AccAccount>(accountDto);
                var savedAccount = await accRepositoryManager.AccAccountRepository.AddAndReturn(accountEntity);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                // التحديث إذا لم يكن للحساب أب
                if (savedAccount.AccAccountParentId == 0)
                {
                    var accountToUpdate = await accRepositoryManager.AccAccountRepository.GetById(savedAccount.AccAccountId);

                    accountToUpdate.AccAccountParentId = accountToUpdate.AccAccountId;
                    var level = await accRepositoryManager.AccAccountRepository.GetAccountLevel(accountToUpdate.AccAccountParentId.Value, currentData.FacilityId);

                    accountToUpdate.AccountLevel = (int?)(level ?? 0) + 1;
                    accRepositoryManager.AccAccountRepository.Update(accountToUpdate);
                }

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                var resultDto = _mapper.Map<AccGroupDto>(savedGroup);
                return await Result<AccGroupDto>.SuccessAsync(resultDto, localization.GetMessagesResource("success"));
            }
            catch (Exception ex)
            {
                await accRepositoryManager.UnitOfWork.RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
                var errorMsg = $"Exception in {GetType().Name}: {ex.Message}";
                return await Result<AccGroupDto>.FailAsync(errorMsg);
            }
        }

        public async Task<IResult<AccGroupEditDto>> Update(AccGroupEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccGroupEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");

            var item = await accRepositoryManager.AccGroupRepository.GetById(entity.AccGroupId);

            if (item == null) return await Result<AccGroupEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;

            _mapper.Map(entity, item);

            accRepositoryManager.AccGroupRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccGroupEditDto>.SuccessAsync(_mapper.Map<AccGroupEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccGroupEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var chid = await accRepositoryManager.AccAccountRepository.GetAll(x => x.AccGroupId == Id);
            if (chid != null && chid.Any())
            {
                return Result<AccGroupDto>.Fail($"{localization.GetMessagesResource("NoDeleteAccGroup")}");
            }


            var item = await accRepositoryManager.AccGroupRepository.GetById(Id);
            if (item == null) return Result<AccGroupDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            accRepositoryManager.AccGroupRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccGroupDto>.SuccessAsync(_mapper.Map<AccGroupDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccGroupDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var chid = await accRepositoryManager.AccAccountRepository.GetAll(x => x.AccGroupId == Id);
            if (chid != null && chid.Any())
            {
                return Result<AccGroupDto>.Fail($"{localization.GetMessagesResource("NoDeleteAccGroup")}");
            }
            var item = await accRepositoryManager.AccGroupRepository.GetById(Id);
            if (item == null) return Result<AccGroupDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            accRepositoryManager.AccGroupRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccGroupDto>.SuccessAsync(_mapper.Map<AccGroupDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccGroupDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
    }

}
