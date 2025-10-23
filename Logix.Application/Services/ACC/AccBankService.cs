using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccBankService : GenericQueryService<AccBank, AccBankDto, AccBankVw>, IAccBankService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public AccBankService(IQueryRepository<AccBank> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session, ILocalizationService localization) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;

            this.session = session;
            this.localization = localization;
        }

        public async Task<IResult<AccBankDto>> Add(AccBankDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccBankDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {

                bool Succeeded = false;
                entity.FacilityId = session.FacilityId;
                if (entity.RBMainAccountType == 1)
                {
                    var Acc = await accRepositoryManager.AccAccountRepository.GetOne(x => x.AccAccountCode == entity.AccountCode && x.FacilityId == session.FacilityId);

                    if (Acc.AccAccountId != 0 || Acc.AccAccountParentId != 0)
                    {
                        long AccAccountId = Acc.AccAccountId;
                        entity.AccAccountId = AccAccountId;
                        entity.AccAccountParentID = Acc.AccAccountParentId;
                        Succeeded = true;
                    }
                    else
                    {
                        return await Result<AccBankDto>.WarningAsync($"{localization.GetAccResource("AccountInformationError")}");

                    }


                }

                var item = _mapper.Map<AccBank>(entity);

                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                item.IsDeleted = false;
                item.CreatedBy = session.UserId;
                item.CreatedOn = DateTime.Now;
                var newEntity = await accRepositoryManager.AccBankRepository.AddAndReturn(item);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                if (entity.RBMainAccountType == 2)
                {


                    var Acc = await accRepositoryManager.AccAccountRepository.GetOne(x => x.AccAccountCode == entity.AccountParentCode && x.FacilityId == session.FacilityId);


                    long AccgroupID = Acc.AccGroupId ?? 0;
                    entity.AccAccountParentID = Acc.AccAccountParentId;

                    var obj = new AccAccountDto();
                    obj.AccGroupId = AccgroupID;

                    obj.DeptID = 0;
                    obj.AccAccountParentId = entity.AccAccountParentID;
                    var AccAccountcode = await accRepositoryManager.AccAccountRepository.GetAccountCode(session.FacilityId, obj.AccAccountParentId ?? 0);
                    obj.AccAccountCode = AccAccountcode;
                    obj.FacilityId = entity.FacilityId;
                    obj.CcId = 0;
                    obj.AccountCloseTypeId = 0;
                    var itemAcc = _mapper.Map<AccAccount>(obj);
                    itemAcc.AccAccountId = 0;
                    itemAcc.AccountLevel = Acc.AccountLevel;
                    var newEntityAcc = await accRepositoryManager.AccAccountRepository.AddAndReturn(itemAcc);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    if (newEntityAcc.AccAccountId == 0)
                    {



                        return await Result<AccBankDto>.FailAsync(localization.GetResource1("AddError"));

                    }

                    var itemupdate = await accRepositoryManager.AccBankRepository.GetById(newEntity.BankId);
                    if (itemupdate == null) return Result<AccBankDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));
                    itemupdate.AccAccountId = newEntityAcc.AccAccountId;
                    itemupdate.AccAccountParentID = entity.AccAccountParentID;
                    accRepositoryManager.AccBankRepository.Update(item);
                    Succeeded = true;
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                var entityMap = _mapper.Map<AccBankDto>(newEntity);
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);
                if (Succeeded == true)
                {
                    return await Result<AccBankDto>.SuccessAsync(entityMap, localization.GetResource1("CreateSuccess"));

                }
                else
                {
                    return await Result<AccBankDto>.SuccessAsync(entityMap, localization.GetResource1("AddError"));

                }
            }
            catch (Exception exc)
            {

                return await Result<AccBankDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccBankRepository.GetById(Id);
            if (item == null) return Result<AccBankDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccBankRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccBankDto>.SuccessAsync(_mapper.Map<AccBankDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccBankDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccBankRepository.GetById(Id);
            if (item == null) return Result<AccBankDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccBankRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccBankDto>.SuccessAsync(_mapper.Map<AccBankDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccBankDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccBankEditDto>> Update(AccBankEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccBankEditDto>.FailAsync(localization.GetMessagesResource("UpdateNullEntity"));

            var item = await accRepositoryManager.AccBankRepository.GetById(entity.BankId);

            if (item == null) return await Result<AccBankEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
            var AccAccountId = await accRepositoryManager.AccAccountRepository.GetAccountIdByCode(entity.AccountCode, session.FacilityId);
            if (AccAccountId == 0)
            {
                return await Result<AccBankEditDto>.WarningAsync(localization.GetAccResource("AccAccountNotfind"));

            }

            entity.AccAccountId = AccAccountId;
            item.IsDeleted = false;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            _mapper.Map(entity, item);

            accRepositoryManager.AccBankRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccBankEditDto>.SuccessAsync(_mapper.Map<AccBankEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccBankEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> UpdateUsersPermission(long BankID, string UsersPermission, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccBankRepository.GetById(BankID);
            if (item == null) return Result<AccBankDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));

            item.UsersPermission = UsersPermission;
            accRepositoryManager.AccBankRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccBankDto>.SuccessAsync(_mapper.Map<AccBankDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccBankDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

    }
}
