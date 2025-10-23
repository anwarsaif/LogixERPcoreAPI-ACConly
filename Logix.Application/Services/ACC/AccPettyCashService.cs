using AutoMapper;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Vml.Office;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.GB;
using Logix.Application.Helpers;
using Logix.Application.Helpers.Acc;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.HR;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Logix.Application.Services.ACC
{
    public class AccPettyCashService : GenericQueryService<AccPettyCash, AccPettyCashDto, AccPettyCashVw>, IAccPettyCashService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly IHrRepositoryManager hrRepositoryManager;
        private readonly IGetAccountIDByCodeHelper getAccountIDByCodeHelper;
        private readonly IGetRefranceByCodeHelper getRefranceByCodeHelper;
        private readonly ISysConfigurationAppHelper sysConfigurationAppHelper;
        private readonly IWorkflowHelper workflowHelper;
        private readonly IMainRepositoryManager mainRepositoryManager;
        private readonly ILocalizationService localization;

        public AccPettyCashService(IQueryRepository<AccPettyCash> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session, IHrRepositoryManager hrRepositoryManager, IGetAccountIDByCodeHelper getAccountIDByCodeHelper, IGetRefranceByCodeHelper getRefranceByCodeHelper, ISysConfigurationAppHelper SysConfigurationAppHelper, IWorkflowHelper workflowHelper, IMainRepositoryManager mainRepositoryManager, ILocalizationService localization) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;

            this.session = session;
            this.hrRepositoryManager = hrRepositoryManager;
            this.getAccountIDByCodeHelper = getAccountIDByCodeHelper;
            this.getRefranceByCodeHelper = getRefranceByCodeHelper;
            this.sysConfigurationAppHelper = SysConfigurationAppHelper;
            this.workflowHelper = workflowHelper;
            this.mainRepositoryManager = mainRepositoryManager;
            this.localization = localization;
        }

        public async Task<IResult<AccPettyCashDto>> Add(AccPettyCashDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccPettyCashDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {
                //-------------------------------- '--جاب  رقم حساب 

                long AccAccountID = 0;

                if (!string.IsNullOrEmpty(entity.AccAccountCode))
                {
                    AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);

                    if (AccAccountID == 0)
                    {
                        return await Result<AccPettyCashDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                    }
                }
                //-------------------------------- '--جاب  رقم مرجع 

                long ReferenceNo = 0;
                ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode ?? "0");

                if (ReferenceNo == 0)
                {
                    return await Result<AccPettyCashDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

                }
                //------------------------------توليد الكود
                var AccPetty = await accRepositoryManager.AccPettyCashRepository.GetAll(j => j.IsDeleted == false && j.FacilityId == session.FacilityId);
                var maxAppCode = AccPetty.Max(j => (long?)j.Code) ?? 0;

                entity.Code = maxAppCode + 1;
                ///  'ارسال الى سير العمل
                long AppID = await workflowHelper.Send(session.EmpId, 811, entity.AppTypeId ?? 0);
                entity.AppId = AppID;
                entity.ReferenceNo = ReferenceNo;
                entity.AccAccountId = AccAccountID;
                entity.StatusId = entity.AppId == 0? 2:1;
                entity.FacilityId = session.FacilityId;
                entity.EmpId = session.EmpId;
                  entity.JId??=0;
                entity.CashOrBankAccountId = 0;
                    var item = _mapper.Map<AccPettyCash>(entity);
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

                var newEntity = await accRepositoryManager.AccPettyCashRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                //============== التفاصيل
                #region ============== التفاصيل
                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<AccPettyCashDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");
                foreach (var child in entity.DetailsList)

                {
                   

                    
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

                    child.PettyCashId = newEntity.Id;
                    child.CcId = CCId;
                    child.Cc2Id = CCId2;
                    child.Cc3Id = CCId3;
                    child.Cc4Id = CCId4;
                    child.Cc5Id = CCId5;
                    child.BranchId = entity.BranchId;
                    child.ActivityId = child.ActivityId ?? 0;
                    child.AssestId = child.AssestId ?? 0;
                    child.TempId = child.TempId ?? 0;
                    child.EmpId = child.EmpId ?? 0;
                    var itemD = _mapper.Map<AccPettyCashD>(child);
                    itemD.CreatedBy = session.UserId;
                    itemD.CreatedOn = DateTime.Now;
                    itemD.IsDeleted = false;
                    var newEntityD = await accRepositoryManager.AccPettyCashDRepository.AddAndReturn(itemD);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);




                }
                #endregion
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, newEntity.Id, 36);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                var entityMap = _mapper.Map<AccPettyCashDto>(newEntity);


                return await Result<AccPettyCashDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {

                return await Result<AccPettyCashDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

       

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {

            try
            {
                var item = await accRepositoryManager.AccPettyCashRepository.GetById(Id);
                if (item == null) return Result<AccPettyCashDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
                item.IsDeleted = true;
                item.ModifiedOn = DateTime.UtcNow;
                item.ModifiedBy = (int)session.UserId;
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                accRepositoryManager.AccPettyCashRepository.Update(item);
                var epdRes = await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);


                var itemsD = await accRepositoryManager.AccPettyCashDRepository.GetAll(x => x.PettyCashId == item.Id);
                if (itemsD == null) return Result<AccPettyCashDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");

                foreach (var items in itemsD)
                {
                    items.IsDeleted = true;
                    items.ModifiedOn = DateTime.UtcNow;
                    items.ModifiedBy = (int)session.UserId;
                    accRepositoryManager.AccPettyCashDRepository.Update(items);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }

                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                return await Result<AccPettyCashDto>.SuccessAsync(_mapper.Map<AccPettyCashDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccPettyCashDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await accRepositoryManager.AccPettyCashRepository.GetById(Id);
            if (item == null) return Result<AccPettyCashDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
            accRepositoryManager.AccPettyCashRepository.Update(item);
            var epdRes = await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

          
                var itemsD = await accRepositoryManager.AccPettyCashDRepository.GetAll(x => x.PettyCashId == item.Id);
                if (itemsD == null) return Result<AccPettyCashDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");

                foreach (var items in itemsD)
                {
                    items.IsDeleted = true;
                    items.ModifiedOn = DateTime.UtcNow;
                    items.ModifiedBy = (int)session.UserId;
                    accRepositoryManager.AccPettyCashDRepository.Update(items);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }

                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                return await Result<AccPettyCashDto>.SuccessAsync(_mapper.Map<AccPettyCashDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccPettyCashDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }


        public async Task<IResult<AccPettyCashEditDto>> Update(AccPettyCashEditDto entity, CancellationToken cancellationToken = default)
        {
            try
            {
                if (entity == null) return await Result<AccPettyCashEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");
            //-------------------------------- '--جاب  رقم حساب 

            long AccAccountID = 0;

            if (!string.IsNullOrEmpty(entity.AccAccountCode))
            {
                AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);

                if (AccAccountID == 0)
                {
                    return await Result<AccPettyCashEditDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));


                }
            }
            //-------------------------------- '--جاب  رقم مرجع 

            long ReferenceNo = 0;
            ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode ?? "0");

            if (ReferenceNo == 0)
            {
                return await Result<AccPettyCashEditDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

            }

            var item = await accRepositoryManager.AccPettyCashRepository.GetById(entity.Id);
            if (item == null) return await Result<AccPettyCashEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
                entity.ReferenceNo = ReferenceNo;
                entity.AccAccountId = AccAccountID;
                entity.EmpId = session.EmpId;
                item.ModifiedOn = DateTime.Now;

            item.ModifiedBy = (int)session.UserId;
          
                _mapper.Map(entity, item);
            await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

            accRepositoryManager.AccPettyCashRepository.Update(item);
            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                #region ============== التفاصيل
                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<AccPettyCashEditDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");
                foreach (var child in entity.DetailsList)

                {



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
                    child.PettyCashId = entity.Id;
                    child.CcId = CCId;
                    child.Cc2Id = CCId2;
                    child.Cc3Id = CCId3;
                    child.Cc4Id = CCId4;
                    child.Cc5Id = CCId5;
                    child.BranchId = entity.BranchId;
                    child.ActivityId = child.ActivityId ?? 0;
                    child.AssestId = child.AssestId ?? 0;
                    child.EmpId = child.EmpId ?? 0;
                    var itemD = _mapper.Map<AccPettyCashD>(child);
                    itemD.CreatedBy = session.UserId;
                    itemD.CreatedOn = DateTime.Now;
                    itemD.IsDeleted = false;
                    var newEntityD = await accRepositoryManager.AccPettyCashDRepository.AddAndReturn(itemD);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                    }
                    else
                    {
                        var itemD = await accRepositoryManager.AccPettyCashDRepository.GetById(child.Id);

                        if (itemD == null) return await Result<AccPettyCashEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
                      
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        itemD.ModifiedOn = DateTime.UtcNow;
                        itemD.ModifiedBy = (int)session.UserId;
                        _mapper.Map(child, itemD);

                        accRepositoryManager.AccPettyCashDRepository.Update(itemD);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                    }



                }
                #endregion
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, item.Id, 36);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }

                return await Result<AccPettyCashEditDto>.SuccessAsync(_mapper.Map<AccPettyCashEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccPettyCashEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult<AccPettyCashDto>> CreateJournal(AccPettyCashDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccPettyCashDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {
                int? Status_Id = 0;
                //--------------------اضافة قيد موقت
                if (entity.StatusId != 5)
                {
                    Status_Id = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(session.FacilityId);

                }
                else
                {
                    Status_Id = entity.StatusId;
                }
                //==========خاصية تجميع الحسابات المتشابهة ومركز التكلفة و النوع في قيد تسويه العهدة
                string PropertyValue = await sysConfigurationAppHelper.GetValue(177, session.FacilityId);

                long AccAccountID = 0;

                if (!string.IsNullOrEmpty(entity.AccAccountCode))
                {
                    AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);

                    if (AccAccountID == 0)
                    {
                        return await Result<AccPettyCashDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                    }
                }
                //-------------------------------- '--جاب  رقم مرجع 

                long ReferenceNo = 0;
                ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode ?? "0");

                if (ReferenceNo == 0)
                {
                    return await Result<AccPettyCashDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

                }
                if (await accRepositoryManager.AccJournalMasterRepository.GetJournalMasterStatuse(entity.Id, 34)==2)
                {
                    return await Result<AccPettyCashDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

                }
                AccJournalMasterDto accJournalMaster = new()
                {

                    DocTypeId = 34,
                    StatusId = Status_Id,
                    FacilityId = session.FacilityId,
                    FinYear = session.FinYear,
                    ReferenceNo = entity.Id,
                    JDateGregorian = entity.TDate,
                    JDateHijri = entity.TDate,
                    Amount = entity.Amount,
                    AmountWrite = "",
                    JDescription = entity.Description,
                    PaymentTypeId = 2,
                    PeriodId = entity.PeriodId,
                    ReferenceCode = entity.Code.ToString(),
                    JBian = entity.Description,
                    BankId = entity.BankId ?? 0,
                    CcId = entity.BranchId,
                    CurrencyId = 1,
                    ExchangeRate = 1,
                    ChequNo = "",
                    ChequDateHijri = "",
                };
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var addJournalMaster = await accRepositoryManager.AccJournalMasterRepository.AddACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

            
                if (!addJournalMaster.Succeeded)
                    return await Result<AccPettyCashDto>.FailAsync(addJournalMaster.Status.message);

                entity.JId = addJournalMaster.Data.JId;
                long jId = addJournalMaster.Data.JId;
                decimal Credit = 0;
                var PettyCashDetailslist=new List<AccPettyCashDVw>();
                if (PropertyValue == "1")
                {
                    var CashDetails = await accRepositoryManager.AccPettyCashDRepository.GetPettyCashDetails2(entity.Id);
                    PettyCashDetailslist.AddRange(CashDetails);
                }
                else
                {
                    var CashDetails = await accRepositoryManager.AccPettyCashDRepository.GetAllVW(x=>x.PettyCashId==entity.Id);
                    PettyCashDetailslist.AddRange(CashDetails);
                }
                decimal ExchangeRate = 0;
                int DefCurrency = await mainRepositoryManager.SysCurrencyRepository.GetDefaultCurrency();
                //-------------------------------------- تفاصيل القيد  الحساب
               
                #region "Acc Journal Detail Credit"


                foreach ( var item in PettyCashDetailslist)
                {
                    
                    if (item.AccAccountId == 0)
                    {
                        return await Result<AccPettyCashDto>.FailAsync($"{localization.GetMessagesResource("ExpensetypeNot")}");

                    }
                  
                 
                    //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 

                    int AccountCurreny = await accRepositoryManager.AccAccountRepository.GetCuureny(item.AccAccountId ?? 0, session.FacilityId);
                    //---------------------------- الدالة تاتي في العملة الرئسية للنظام  
                    ExchangeRate = await mainRepositoryManager.SysExchangeRateRepository.GetExchangeRateValue((long)AccountCurreny);

                    AccJournalDetaileDto accJournalDetailCredit = new()
                    {
                        JId = jId,
                        AccAccountId = item.AccAccountId,
                        Credit = 0,
                        Debit = item.Amount,
                        Description =item.Description,
                        JDateGregorian = entity.TDate,
                        ReferenceTypeId =1,
                        CcId=item.CcId,
                        ReferenceNo = 0,
                        CurrencyId = AccountCurreny,
                        ExchangeRate = ExchangeRate,
                        ReferenceCode = entity.ReferenceCode,
                    };
                    var addJournalDetaiCredit = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(accJournalDetailCredit, cancellationToken);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    if (!addJournalDetaiCredit.Succeeded)
                        return await Result<AccPettyCashDto>.FailAsync(addJournalDetaiCredit.Status.message);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                  
                    // 'تسجيل الضريبة
                    if (item.VatAmount>0)
                    {
                        long PurchasesVATAccount = await accRepositoryManager.AccFacilityRepository.GetPurchasesVATAccount(session.FacilityId);
                        AccountCurreny = await accRepositoryManager.AccAccountRepository.GetCuureny(PurchasesVATAccount, session.FacilityId);
                        AccJournalDetaileDto accJournalDetailVatAmount = new()
                        {
                            JId = jId,
                            AccAccountId = PurchasesVATAccount,
                            Credit = 0,
                            Debit = item.VatAmount,
                            Description = item.Description,
                            JDateGregorian = entity.TDate,
                            ReferenceTypeId = 1,
                            CcId = 0,
                            ReferenceNo = 0,
                            CurrencyId = AccountCurreny,
                            ExchangeRate = ExchangeRate,
                        };
                        var addJournalDetaiVatAmount = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(accJournalDetailVatAmount, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                        if (!addJournalDetaiVatAmount.Succeeded)
                            return await Result<AccPettyCashDto>.FailAsync(addJournalDetaiVatAmount.Status.message);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    }

                    Credit += item.Total??0;
                }

              
                #endregion

                //-------------------------------------- تفاصيل القيد  الحساب   
                #region "Acc Journal Detail Credit"


                //---------------------------- الدالة تاتي في العملة الرئسية للنظام  
                ExchangeRate = await mainRepositoryManager.SysExchangeRateRepository.GetExchangeRateValue((long)DefCurrency);


             
                AccJournalDetaileDto accJournalDetail1Debit = new()
                {
                    JId = jId,
                    AccAccountId = AccAccountID,
                    Credit = Credit,
                    Debit = 0,
                    Description = entity.Description,
                    JDateGregorian = entity.TDate,
                    ReferenceTypeId = entity.ReferenceTypeId,
                    ReferenceNo = ReferenceNo,
                    CurrencyId = DefCurrency,
                    ExchangeRate = ExchangeRate,
                    ReferenceCode = entity.ReferenceCode,
                };

                var addJournalDetailDebit = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(accJournalDetail1Debit, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                if (!addJournalDetailDebit.Succeeded)
                    return await Result<AccPettyCashDto>.FailAsync(addJournalDetailDebit.Status.message);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                #endregion "Acc Journal Detail Debit"

                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);
                return await Result<AccPettyCashDto>.SuccessAsync(entity, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {
                return await Result<AccPettyCashDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }



        }
        public async Task<IResult<AccPettyCashDto>> CreateJournal2(AccPettyCashDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccPettyCashDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {
                int? Status_Id = 0;
                //--------------------اضافة قيد موقت
                if (entity.StatusId != 5)
                {
                    Status_Id = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(session.FacilityId);

                }
                else
                {
                    Status_Id = entity.StatusId;
                }
                //==========خاصية تجميع الحسابات المتشابهة ومركز التكلفة و النوع في قيد تسويه العهدة
                string PropertyValue = await sysConfigurationAppHelper.GetValue(177, session.FacilityId);

                long AccAccountID = 0;

                if (!string.IsNullOrEmpty(entity.AccAccountCode))
                {
                    AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);

                    if (AccAccountID == 0)
                    {
                        return await Result<AccPettyCashDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                    }
                }
                //-------------------------------- '--جاب  رقم مرجع 

                long ReferenceNo = 0;
                ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode ?? "0");

                if (ReferenceNo == 0)
                {
                    return await Result<AccPettyCashDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

                }
                if (await accRepositoryManager.AccJournalMasterRepository.GetJournalMasterStatuse(entity.Id, 34) == 2)
                {
                    return await Result<AccPettyCashDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

                }
                AccJournalMasterDto accJournalMaster = new()
                {

                    DocTypeId = 34,
                    StatusId = Status_Id,
                    FacilityId = session.FacilityId,
                    FinYear = session.FinYear,
                    ReferenceNo = entity.Id,
                    JDateGregorian = entity.TDate,
                    JDateHijri = entity.TDate,
                    Amount = entity.Amount,
                    AmountWrite = "",
                    JDescription = entity.Description,
                    PaymentTypeId = 2,
                    PeriodId = entity.PeriodId,
                    ReferenceCode = entity.Code.ToString(),
                    JBian = entity.Description,
                    BankId = entity.BankId ?? 0,
                    CcId = entity.BranchId,
                    CurrencyId = 1,
                    ExchangeRate = 1,
                    ChequNo = "",
                    ChequDateHijri = "",
                };
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var addJournalMaster = await accRepositoryManager.AccJournalMasterRepository.AddACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);


                if (!addJournalMaster.Succeeded)
                    return await Result<AccPettyCashDto>.FailAsync(addJournalMaster.Status.message);

                entity.JId = addJournalMaster.Data.JId;
                long jId = addJournalMaster.Data.JId;
                decimal Debit = 0;
                var PettyCashDetailslist = new List<AccPettyCashDVw>();
                if (PropertyValue == "1")
                {
                    var CashDetails = await accRepositoryManager.AccPettyCashDRepository.GetPettyCashDetails2(entity.Id);
                    PettyCashDetailslist.AddRange(CashDetails);
                }
                else
                {
                    var CashDetails = await accRepositoryManager.AccPettyCashDRepository.GetAllVW(x => x.PettyCashId == entity.Id);
                    PettyCashDetailslist.AddRange(CashDetails);
                }
                decimal ExchangeRate = 0;
                int DefCurrency = await mainRepositoryManager.SysCurrencyRepository.GetDefaultCurrency();
                //-------------------------------------- تفاصيل القيد  الحساب

                #region "Acc Journal Detail Credit"


                foreach (var item in PettyCashDetailslist)
                {

                    if (item.AccAccountId == 0)
                    {
                        return await Result<AccPettyCashDto>.FailAsync($"{localization.GetMessagesResource("ExpensetypeNot")}");

                    }


                    //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 

                    int AccountCurreny = await accRepositoryManager.AccAccountRepository.GetCuureny(item.AccAccountId ?? 0, session.FacilityId);
                    //---------------------------- الدالة تاتي في العملة الرئسية للنظام  
                    ExchangeRate = await mainRepositoryManager.SysExchangeRateRepository.GetExchangeRateValue((long)AccountCurreny);

                    AccJournalDetaileDto accJournalDetailCredit = new()
                    {
                        JId = jId,
                        AccAccountId = item.AccAccountId,
                        Credit = item.Amount,
                        Debit = 0,
                        Description = item.Description,
                        JDateGregorian = entity.TDate,
                        ReferenceTypeId = 1,
                        CcId = item.CcId,
                        ReferenceNo = 0,
                        CurrencyId = AccountCurreny,
                        ExchangeRate = ExchangeRate,
                        ReferenceCode = entity.ReferenceCode,
                    };
                    var addJournalDetaiCredit = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(accJournalDetailCredit, cancellationToken);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    if (!addJournalDetaiCredit.Succeeded)
                        return await Result<AccPettyCashDto>.FailAsync(addJournalDetaiCredit.Status.message);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    // 'تسجيل الضريبة
                    if (item.VatAmount > 0)
                    {
                        long PurchasesVATAccount = await accRepositoryManager.AccFacilityRepository.GetPurchasesVATAccount(session.FacilityId);
                        AccountCurreny = await accRepositoryManager.AccAccountRepository.GetCuureny(PurchasesVATAccount, session.FacilityId);
                        AccJournalDetaileDto accJournalDetailVatAmount = new()
                        {
                            JId = jId,
                            AccAccountId = PurchasesVATAccount,
                            Credit = item.VatAmount,
                            Debit = 0,
                            Description = item.Description,
                            JDateGregorian = entity.TDate,
                            ReferenceTypeId = 1,
                            CcId = 0,
                            ReferenceNo = 0,
                            CurrencyId = AccountCurreny,
                            ExchangeRate = ExchangeRate,
                        };
                        var addJournalDetaiVatAmount = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(accJournalDetailVatAmount, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                        if (!addJournalDetaiVatAmount.Succeeded)
                            return await Result<AccPettyCashDto>.FailAsync(addJournalDetaiVatAmount.Status.message);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    }

                    Debit += item.Total ?? 0;
                }


                #endregion

                //-------------------------------------- تفاصيل القيد  الحساب   
                #region "Acc Journal Detail Credit"


                //---------------------------- الدالة تاتي في العملة الرئسية للنظام  
                ExchangeRate = await mainRepositoryManager.SysExchangeRateRepository.GetExchangeRateValue((long)DefCurrency);



                AccJournalDetaileDto accJournalDetail1Debit = new()
                {
                    JId = jId,
                    AccAccountId = AccAccountID,
                    Credit = 0,
                    Debit = Debit,
                    Description = entity.Description,
                    JDateGregorian = entity.TDate,
                    ReferenceTypeId = entity.ReferenceTypeId,
                    ReferenceNo = ReferenceNo,
                    CurrencyId = DefCurrency,
                    ExchangeRate = ExchangeRate,
                    ReferenceCode = entity.ReferenceCode,
                };

                var addJournalDetailDebit = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(accJournalDetail1Debit, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                if (!addJournalDetailDebit.Succeeded)
                    return await Result<AccPettyCashDto>.FailAsync(addJournalDetailDebit.Status.message);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                #endregion "Acc Journal Detail Debit"

                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);
                return await Result<AccPettyCashDto>.SuccessAsync(entity, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {
                return await Result<AccPettyCashDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }



        }

        //public async Task<IResult<IEnumerable<AccPettyCashTempVw>>> GetPettyCashTemp(Expression<Func<AccPettyCashTempVw, bool>> expression)
        //{
        //   var PettyCashTemp=await accRepositoryManager.AccPettyCashDRepository.GetPettyCashTemp(expression);
        //    return PettyCashTemp;

        //}
        public async Task<IResult<IEnumerable<AccPettyCashTempVw>>> GetPettyCashTemp(Expression<Func<AccPettyCashTempVw, bool>> expression)
        {
            try
            {
                var pettyCashTemp = await accRepositoryManager.AccPettyCashDRepository.GetPettyCashTemp(expression);

                return Result<IEnumerable<AccPettyCashTempVw>>.Success(pettyCashTemp);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<AccPettyCashTempVw>>.Fail($"An error occurred: {ex.Message}");
            }
        }
    }
}
