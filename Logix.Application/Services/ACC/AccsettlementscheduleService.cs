using AutoMapper;
using Castle.MicroKernel.Registration;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Helpers;
using Logix.Application.Helpers.Acc;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Org.BouncyCastle.Asn1.Ocsp;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Logix.Application.Services.ACC
{
    public class AccsettlementscheduleService : GenericQueryService<AccSettlementSchedule, AccSettlementScheduleDto, AccSettlementSchedule>, IAccsettlementscheduleService
    {
        private readonly IAccRepositoryManager accRepositoryManager;

        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IGetRefranceByCodeHelper getRefranceByCodeHelper;
        private readonly IGetAccountIDByCodeHelper getAccountIDByCodeHelper;

        public AccsettlementscheduleService(IQueryRepository<AccSettlementSchedule> queryRepository, IAccRepositoryManager AccRepositoryManager, IMapper mapper, ICurrentData session, ILocalizationService localization, IGetRefranceByCodeHelper getRefranceByCodeHelper, IGetAccountIDByCodeHelper getAccountIDByCodeHelper) : base(queryRepository, mapper)
        {
            this.accRepositoryManager = AccRepositoryManager;
            this._mapper = mapper;

            this.session = session;
            this.localization = localization;
            this.getRefranceByCodeHelper = getRefranceByCodeHelper;
            this.getAccountIDByCodeHelper = getAccountIDByCodeHelper;
        }
        public async Task<IResult<AccSettlementScheduleDto>> Add(AccSettlementScheduleDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccSettlementScheduleDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {
                //==============================المستر
                #region----------------------------  المستر
                //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين 
                decimal sumCredit = entity.DetailsList.Sum(x => x.Credit) ?? 0 * (entity.ExchangeRate ?? 0);
                decimal sumDebit = entity.DetailsList.Sum(x => x.Debit) ?? 0 * (entity.ExchangeRate ?? 0);
                if (sumCredit != sumDebit)
                {
                    return await Result<AccSettlementScheduleDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }

                entity.FacilityId = session.FacilityId;
                entity.InstallmentValue =0;
                entity.StatusId = 1;
                //------------------------------توليد الكود
                var settlementSchedules = await accRepositoryManager.AccsettlementscheduleRepository.GetAll(j => j.IsDeleted == false && j.FacilityId == session.FacilityId);
                var maxAppCode = settlementSchedules.Max(j => (long?)j.Code) ?? 0;

                entity.Code = maxAppCode + 1;
                var item = _mapper.Map<AccSettlementSchedule>(entity);
                item.CreatedBy = session.UserId;
                item.CreatedOn = DateTime.Now;
                item.IsDeleted = false;
                item.FinYear = session.FinYear;
                item.ModifiedBy = 0;
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

                var newEntity = await accRepositoryManager.AccsettlementscheduleRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                #endregion
                //============== التفاصيل
                #region ============== التفاصيل
                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<AccSettlementScheduleDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");
                foreach (var child in entity.DetailsList)

                {
                    //-------------------------------- '--جاب  رقم حساب 

                    long AccAccountID = 0;

                    if (!string.IsNullOrEmpty(child.AccAccountCode))
                    {
                        AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                        if (AccAccountID == 0)
                        {
                            return await Result<AccSettlementScheduleDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                        }
                    }

                    //-------------------------------- '--جاب  رقم مرجع 

                    long ReferenceNo = 0;
                    ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                    if (ReferenceNo == 0)
                    {
                        return await Result<AccSettlementScheduleDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

                    }
                    long CCId = 0;
                    if (!string.IsNullOrEmpty(child.CostCenterCode))
                    {
                        CCId = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(child.CostCenterCode, session.FacilityId);

                    }
                    long CCId2 = 0;
                    if (!string.IsNullOrEmpty(child.CostCenterCode2))
                    {
                        CCId2 = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(child.CostCenterCode2, session.FacilityId);

                    }
                    long CCId3 = 0;
                    if (!string.IsNullOrEmpty(child.CostCenterCode3))
                    {
                        CCId3 = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(child.CostCenterCode3, session.FacilityId);

                    }
                    long CCId4 = 0;
                    if (!string.IsNullOrEmpty(child.CostCenterCode4))
                    {
                        CCId4 = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(child.CostCenterCode4, session.FacilityId);

                    }
                    long CCId5 = 0;
                    if (!string.IsNullOrEmpty(child.CostCenterCode4))
                    {
                        CCId5 = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(child.CostCenterCode5, session.FacilityId);

                    }
                    
                        child.SsId = newEntity.Id;
                        child.AccAccountId = AccAccountID;
                        child.ReferenceNo = ReferenceNo;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        child.BranchId = entity.BranchId;
                        child.ActivityId = child.ActivityId ?? 0;
                        child.AssestId = child.AssestId ?? 0;
                        child.EmpId = child.EmpId ?? 0;
                        var itemD = _mapper.Map<AccSettlementScheduleD>(child);
                        itemD.CreatedBy = session.UserId;
                        itemD.CreatedOn = DateTime.Now;
                        itemD.IsDeleted = false;
                        var newEntityD = await accRepositoryManager.AccSettlementScheduleDRepository.AddAndReturn(itemD);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                    
          

                        
                }
                #endregion

                //============== الدفعات
                #region ============== الدفعات
                if (entity.InstallmentsList == null || entity.InstallmentsList.Count == 0) return await Result<AccSettlementScheduleDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");
                foreach (var Install in entity.InstallmentsList)
                {
                    Install.SsId = newEntity.Id;
                    var itemInstall = _mapper.Map<AccSettlementInstallment>(Install);
                    itemInstall.CreatedBy = session.UserId;
                    itemInstall.CreatedOn = DateTime.Now;
                    itemInstall.IsDeleted = false;
                    var newEntityD = await accRepositoryManager.AccSettlementInstallmentRepository.AddAndReturn(itemInstall);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                #endregion
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);
                var entityMap = _mapper.Map<AccSettlementScheduleDto>(newEntity);


                return await Result<AccSettlementScheduleDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {

                return await Result<AccSettlementScheduleDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult<AccSettlementScheduleEditDto>> Update(AccSettlementScheduleEditDto entity, CancellationToken cancellationToken = default)
        {
            try
            {
                if (entity == null) return await Result<AccSettlementScheduleEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");
                //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين 
                decimal sumCredit = entity.DetailsList.Where(d => d.IsDeleted == false).Sum(x => x.Credit) ?? 0 * (entity.ExchangeRate ?? 0);
                decimal sumDebit = entity.DetailsList.Where(d => d.IsDeleted == false).Sum(x => x.Debit) ?? 0 * (entity.ExchangeRate ?? 0);
                if (sumCredit != sumDebit)
                {
                    return await Result<AccSettlementScheduleEditDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }

                var item = await accRepositoryManager.AccsettlementscheduleRepository.GetById(entity.Id);

            if (item == null) return await Result<AccSettlementScheduleEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;

            _mapper.Map(entity, item);
            await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
            accRepositoryManager.AccsettlementscheduleRepository.Update(item);
            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                //============== التفاصيل
                //#region ============== التفاصيل
                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<AccSettlementScheduleEditDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");
                foreach (var child in entity.DetailsList)

                {
                    //-------------------------------- '--جاب  رقم حساب 

                    long AccAccountID = 0;

                    if (!string.IsNullOrEmpty(child.AccAccountCode))
                    {
                        AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                        if (AccAccountID == 0)
                        {
                            return await Result<AccSettlementScheduleEditDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                        }
                    }

                    //-------------------------------- '--جاب  رقم مرجع 

                    long ReferenceNo = 0;
                    ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                    if (ReferenceNo == 0)
                    {
                        return await Result<AccSettlementScheduleEditDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

                    }
                    long CCId = 0;
                    if (!string.IsNullOrEmpty(child.CostCenterCode))
                    {
                        CCId = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(child.CostCenterCode, session.FacilityId);

                    }
                    long CCId2 = 0;
                    if (!string.IsNullOrEmpty(child.CostCenterCode2))
                    {
                        CCId2 = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(child.CostCenterCode2, session.FacilityId);

                    }
                    long CCId3 = 0;
                    if (!string.IsNullOrEmpty(child.CostCenterCode3))
                    {
                        CCId3 = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(child.CostCenterCode3, session.FacilityId);

                    }
                    long CCId4 = 0;
                    if (!string.IsNullOrEmpty(child.CostCenterCode4))
                    {
                        CCId4 = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(child.CostCenterCode4, session.FacilityId);

                    }
                    long CCId5 = 0;
                    if (!string.IsNullOrEmpty(child.CostCenterCode4))
                    {
                        CCId5 = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(child.CostCenterCode5, session.FacilityId);

                    }
                    if (child.Id == 0 && child.IsDeleted == false)
                    {
                        child.SsId = entity.Id;
                        child.AccAccountId = AccAccountID;
                        child.ReferenceNo = ReferenceNo;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        child.BranchId = entity.BranchId;
                        child.ActivityId = child.ActivityId ?? 0;
                        child.AssestId = child.AssestId ?? 0;
                        child.EmpId = child.EmpId ?? 0;
                        var itemD = _mapper.Map<AccSettlementScheduleD>(child);
                        itemD.CreatedBy = session.UserId;
                        itemD.CreatedOn = DateTime.Now;
                        itemD.IsDeleted = false;
                        var newEntityD = await accRepositoryManager.AccSettlementScheduleDRepository.AddAndReturn(itemD);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                    }
                    else
                    {
                        var itemD = await accRepositoryManager.AccSettlementScheduleDRepository.GetById(child.Id);

                        if (itemD == null) return await Result<AccSettlementScheduleEditDto>.FailAsync($"--- there is no Data with this id: {entity.Id}---");
                        child.AccAccountId = AccAccountID;
                        child.ReferenceNo = ReferenceNo;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        child.BranchId = entity.BranchId;
                        itemD.ModifiedOn = DateTime.Now;
                        itemD.ModifiedBy = (int)session.UserId;
                        _mapper.Map(child, itemD);

                        accRepositoryManager.AccSettlementScheduleDRepository.Update(itemD);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    }
                }
                //#endregion
                //============== الدفعات
                #region ============== الدفعات
                if (entity.InstallmentsList == null || entity.InstallmentsList.Count == 0) return await Result<AccSettlementScheduleEditDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");
                foreach (var Install in entity.InstallmentsList)
                {
                    if (Install.Id == 0 && Install.IsDeleted == false)
                    {
                        Install.SsId = entity.Id;
                        var itemInstall = _mapper.Map<AccSettlementInstallment>(Install);
                        itemInstall.CreatedBy = session.UserId;
                        itemInstall.CreatedOn = DateTime.Now;
                        itemInstall.IsDeleted = false;
                        var newEntityD = await accRepositoryManager.AccSettlementInstallmentRepository.AddAndReturn(itemInstall);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    }
                    else
                    {
                        var itemInstall = await accRepositoryManager.AccSettlementInstallmentRepository.GetById(Install.Id);
                        if (itemInstall == null) return await Result<AccSettlementScheduleEditDto>.FailAsync($"--- there is no Data with this id: {entity.Id}---");
                        itemInstall.ModifiedOn = DateTime.Now;
                        itemInstall.ModifiedBy = (int)session.UserId;
                        _mapper.Map(Install, itemInstall);
                        accRepositoryManager.AccSettlementInstallmentRepository.Update(itemInstall);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    }


                }
                #endregion

                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                return await Result<AccSettlementScheduleEditDto>.SuccessAsync(_mapper.Map<AccSettlementScheduleEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccSettlementScheduleEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {

            try
            {
                var item = await accRepositoryManager.AccsettlementscheduleRepository.GetById(Id);
                if (item == null) return Result<AccSettlementScheduleDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
                item.IsDeleted = true;
                item.ModifiedOn = DateTime.Now;
                item.ModifiedBy = (int)session.UserId;
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                accRepositoryManager.AccsettlementscheduleRepository.Update(item);
                var epdRes = await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);


                var itemsD = await accRepositoryManager.AccSettlementScheduleDRepository.GetAll(x => x.SsId == item.Id);
                if (itemsD == null) return Result<AccSettlementScheduleDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");

                foreach (var items in itemsD)
                {
                    items.IsDeleted = true;
                    items.ModifiedOn = DateTime.UtcNow;
                    items.ModifiedBy = (int)session.UserId;
                    accRepositoryManager.AccSettlementScheduleDRepository.Update(items);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                //===================الدفعات

                var itemsInstall = await accRepositoryManager.AccSettlementInstallmentRepository.GetAll(x => x.SsId == item.Id);
                if (itemsInstall == null) return Result<AccSettlementScheduleDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");

                foreach (var itemsIns in itemsInstall)
                {
                    itemsIns.IsDeleted = true;
                    itemsIns.ModifiedOn = DateTime.UtcNow;
                    itemsIns.ModifiedBy = (int)session.UserId;
                    accRepositoryManager.AccSettlementInstallmentRepository.Update(itemsIns);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                return await Result<AccSettlementScheduleDto>.SuccessAsync(_mapper.Map<AccSettlementScheduleDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccSettlementScheduleDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
    

            try
            {
        var item = await accRepositoryManager.AccsettlementscheduleRepository.GetById(Id);
            if (item == null) return Result<AccSettlementScheduleDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                accRepositoryManager.AccsettlementscheduleRepository.Update(item);
                var epdRes = await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);


                var itemsD = await accRepositoryManager.AccSettlementScheduleDRepository.GetAll(x => x.SsId == item.Id);
                if (itemsD == null) return Result<AccSettlementScheduleDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");

                foreach (var items in itemsD)
                {
                    items.IsDeleted = true;
                    items.ModifiedOn = DateTime.UtcNow;
                    items.ModifiedBy = (int)session.UserId;
                    accRepositoryManager.AccSettlementScheduleDRepository.Update(items);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                //===================الدفعات

                var itemsInstall= await accRepositoryManager.AccSettlementInstallmentRepository.GetAll(x => x.SsId == item.Id);
                if (itemsInstall == null) return Result<AccSettlementScheduleDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");

                foreach (var itemsIns in itemsInstall)
                {
                    itemsIns.IsDeleted = true;
                    itemsIns.ModifiedOn = DateTime.UtcNow;
                    itemsIns.ModifiedBy = (int)session.UserId;
                    accRepositoryManager.AccSettlementInstallmentRepository.Update(itemsIns);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                return await Result<AccSettlementScheduleDto>.SuccessAsync(_mapper.Map<AccSettlementScheduleDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccSettlementScheduleDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult<AccJournalSchedulDto>> CreateJournal(AccJournalSchedulDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                return await Result<AccJournalSchedulDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {
                int? statusId = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(session.FacilityId);
                long xCount = 0;

                var selectedIdsArr = entity.SelectedId?.Split(',');
                if (selectedIdsArr == null || selectedIdsArr.Length == 0)
                    return await Result<AccJournalSchedulDto>.FailAsync(localization.GetMessagesResource("NoSelectedIds"));

                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

                foreach (var idStr in selectedIdsArr)
                {
                    if (!long.TryParse(idStr, out long id)) continue;

                    var item = await accRepositoryManager.AccSettlementInstallmentRepository.GetByIdVW(id);
                    if (item == null) continue;

                    var periodId = await accRepositoryManager.AccPeriodsRepository.GetPreiodIDByDate(item.InstallmentDate, session.FacilityId);

                    var accJournalMaster = new AccJournalMasterDto
                    {
                        FacilityId = item.FacilityId,
                        PeriodId = periodId,
                        CcId = item.BranchId,
                        JBian = item.DescriptionM,
                        DocTypeId = 33,
                        StatusId = statusId,
                        FinYear = session.FinYear,
                        ReferenceNo = item.Id,
                        JDateGregorian = item.InstallmentDate,
                        JDateHijri = item.InstallmentDate,
                        AmountWrite = "",
                        JDescription = item.DescriptionM,
                        ReferenceCode = item.InstallmentNo.ToString(),
                        CurrencyId = item.CurrencyId,
                        ExchangeRate = item.ExchangeRate,
                        ChequNo = "",
                        ChequDateHijri = "",
                        BankId = 0,
                        PaymentTypeId = 0,
                        Amount = 0
                    };

                    var addJournalMasterResult = await accRepositoryManager.AccJournalMasterRepository.AddACCJournalMaster(accJournalMaster, cancellationToken);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    if (!addJournalMasterResult.Succeeded)
                    {
                        await accRepositoryManager.UnitOfWork.RollbackTransactionAsync(cancellationToken);
                        return await Result<AccJournalSchedulDto>.FailAsync(addJournalMasterResult.Status.message);
                    }

                   
                    long jId = addJournalMasterResult.Data.JId;
                    var detailsList = await accRepositoryManager.AccSettlementScheduleDRepository.GetAll(d => d.SsId == item.SsId);

                    foreach (var detail in detailsList)
                    {
                        var accJournalDetailDebit = new AccJournalDetaileDto
                        {
                            JId = jId,
                            CcId = detail.CcId,
                            JDateGregorian = item.InstallmentDate,
                            AccAccountId = detail.AccAccountId,
                            Credit = detail.Credit,
                            Debit = detail.Debit,
                            Description = detail.Description,
                            ReferenceTypeId = detail.ReferenceTypeId,
                            ReferenceNo = detail.ReferenceNo,
                            CurrencyId = detail.CurrencyId,
                            ExchangeRate = detail.ExchangeRate,
                            Cc2Id = detail.Cc2Id,
                            Cc3Id = detail.Cc3Id,
                            Cc4Id = detail.Cc4Id,
                            Cc5Id = detail.Cc5Id,
                            ReferenceCode = detail.ReferenceCode,
                            BranchId = detail.BranchId,
                            ActivityId = detail.ActivityId,
                            AssestId = detail.AssestId,
                            EmpId = detail.EmpId
                        };

                        var addJournalDetailResult = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(accJournalDetailDebit, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                        if (!addJournalDetailResult.Succeeded)
                        {
                            await accRepositoryManager.UnitOfWork.RollbackTransactionAsync(cancellationToken);
                            return await Result<AccJournalSchedulDto>.FailAsync(addJournalDetailResult.Status.message);
                        }
                    }

                    xCount++;
                }

                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                if (xCount == 0)
                    return await Result<AccJournalSchedulDto>.FailAsync(localization.GetMessagesResource("NoRecordsProcessed"));

                return await Result<AccJournalSchedulDto>.SuccessAsync(entity, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {
                await accRepositoryManager.UnitOfWork.RollbackTransactionAsync(cancellationToken);
                return await Result<AccJournalSchedulDto>.FailAsync($"EXP in {this.GetType()}, Message: {exc.Message}");
            }
        }


    }
}
