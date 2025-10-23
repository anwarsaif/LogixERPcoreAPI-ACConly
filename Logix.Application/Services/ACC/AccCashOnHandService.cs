using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccCashOnHandService : GenericQueryService<AccCashOnHand, AccCashOnHandDto, AccCashOnHandVw>, IAccCashOnHandService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public AccCashOnHandService(IQueryRepository<AccCashOnHand> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session, ILocalizationService localization) : base(queryRepository, mapper)
        {
            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;
            this.session = session;
            this.localization = localization;
        }
        public async Task<IResult<AccCashOnHandDto>> Add(AccCashOnHandDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccCashOnHandDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {

                bool Succeeded = false;

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
                        return await Result<AccCashOnHandDto>.WarningAsync($"{localization.GetAccResource("AccountInformationError")}");

                    }

                }

                var item = _mapper.Map<AccCashOnHand>(entity);

                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                item.IsDeleted = false;
                item.CreatedBy = session.UserId;
                item.CreatedOn = DateTime.UtcNow;
                var newEntity = await accRepositoryManager.AccCashOnHandRepository.AddAndReturn(item);
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
                    obj.FacilityId = session.FacilityId;
                    var AccAccountcode = await accRepositoryManager.AccAccountRepository.GetAccountCode(session.FacilityId, obj.AccAccountParentId ?? 0);
                    obj.AccAccountCode = AccAccountcode;
                    var itemAcc = _mapper.Map<AccAccount>(obj);
                    itemAcc.AccAccountId = 0;
                    var newEntityAcc = await accRepositoryManager.AccAccountRepository.AddAndReturn(itemAcc);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    if (newEntityAcc.AccAccountId == 0)
                    {

                        await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                        var entity3Map = _mapper.Map<AccCashOnHandDto>(newEntity);

                        return await Result<AccCashOnHandDto>.SuccessAsync(entity3Map, localization.GetMessagesResource("success"));

                    }

                    var itemupdate = await accRepositoryManager.AccCashOnHandRepository.GetById(newEntity.Id);
                    if (itemupdate == null) return Result<AccCashOnHandDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));
                    itemupdate.AccAccountId = newEntityAcc.AccAccountId;
                    itemupdate.AccAccountParentID = entity.AccAccountParentID;
                    accRepositoryManager.AccCashOnHandRepository.Update(item);
                    Succeeded = true;
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                var entityMap = _mapper.Map<AccCashOnHandDto>(newEntity);



                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);
                if (Succeeded = true)
                {
                    return await Result<AccCashOnHandDto>.SuccessAsync(entityMap, localization.GetResource1("CreateSuccess"));

                }
                else
                {
                    return await Result<AccCashOnHandDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));

                }
            }
            catch (Exception exc)
            {

                return await Result<AccCashOnHandDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }



        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccCashOnHandRepository.GetById(Id);
            if (item == null) return Result<AccCashOnHandDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccCashOnHandRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccCashOnHandDto>.SuccessAsync(_mapper.Map<AccCashOnHandDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccCashOnHandDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccCashOnHandRepository.GetById(Id);
            if (item == null) return Result<AccCashOnHandDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccCashOnHandRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccCashOnHandDto>.SuccessAsync(_mapper.Map<AccCashOnHandDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccCashOnHandDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccCashOnHandEditDto>> Update(AccCashOnHandEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccCashOnHandEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");

            var item = await accRepositoryManager.AccCashOnHandRepository.GetById(entity.Id);

            if (item == null) return await Result<AccCashOnHandEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
            _mapper.Map(entity, item);
            item.IsDeleted = false;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;

            var AccAccountId = await accRepositoryManager.AccAccountRepository.GetAccountIdByCode(entity.AccAccountCode, session.FacilityId);
            if (AccAccountId == 0)
            {
                return await Result<AccCashOnHandEditDto>.WarningAsync(localization.GetAccResource("AccAccountNotfind"));

            }
            item.AccAccountId = AccAccountId;

            accRepositoryManager.AccCashOnHandRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccCashOnHandEditDto>.SuccessAsync(_mapper.Map<AccCashOnHandEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccCashOnHandEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult> UpdateUsersPermission(long ID, string UsersPermission, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccCashOnHandRepository.GetById(ID);
            if (item == null) return Result<AccCashOnHandDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));

            item.UsersPermission = UsersPermission;
            accRepositoryManager.AccCashOnHandRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccCashOnHandDto>.SuccessAsync(_mapper.Map<AccCashOnHandDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccCashOnHandDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }



    }
}
