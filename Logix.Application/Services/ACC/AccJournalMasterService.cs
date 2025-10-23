using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers;
using Logix.Application.Helpers.Acc;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Globalization;
//using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Logix.Application.Services.ACC
{
    public class AccJournalMasterService : GenericQueryService<AccJournalMaster, AccJournalMasterDto, AccJournalMasterVw>, IAccJournalMasterService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMainRepositoryManager mainRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly IGetRefranceByCodeHelper getRefranceByCodeHelper;
        private readonly IHrRepositoryManager hrRepositoryManager;
        private readonly ILocalizationService localization;
        private readonly IGetAccountIDByCodeHelper getAccountIDByCodeHelper;
        private readonly IGetAccJournaCodeHelper getAccJournaCodeHelper;
        private readonly ISysConfigurationAppHelper sysConfigurationAppHelper;

        public AccJournalMasterService(IQueryRepository<AccJournalMaster> queryRepository, IAccRepositoryManager accRepositoryManager, IMainRepositoryManager mainRepositoryManager, IMapper mapper, ICurrentData session, IGetRefranceByCodeHelper getRefranceByCodeHelper, IHrRepositoryManager hrRepositoryManager,
            ILocalizationService localization, IGetAccountIDByCodeHelper getAccountIDByCodeHelper, IGetAccJournaCodeHelper getAccJournaCodeHelper, ISysConfigurationAppHelper SysConfigurationAppHelper) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this.mainRepositoryManager = mainRepositoryManager;
            this._mapper = mapper;

            this.session = session;
            this.getRefranceByCodeHelper = getRefranceByCodeHelper;
            this.hrRepositoryManager = hrRepositoryManager;
            this.localization = localization;
            this.getAccountIDByCodeHelper = getAccountIDByCodeHelper;
            this.getAccJournaCodeHelper = getAccJournaCodeHelper;
            sysConfigurationAppHelper = SysConfigurationAppHelper;
        }

        public async Task<IResult<AccJournalMasterDto>> Add(AccJournalMasterDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccJournalMasterDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");

            try
            {


                var item = _mapper.Map<AccJournalMaster>(entity);

                var newEntity = await accRepositoryManager.AccJournalMasterRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccJournalMasterDto>(newEntity);


                return await Result<AccJournalMasterDto>.SuccessAsync(entityMap, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<AccJournalMasterDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {

            try
            {
                var item = await accRepositoryManager.AccJournalMasterRepository.DeleteJournalWithDetailsByJId(Id);
                if (item != null)
                {
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    return await Result<AccJournalMasterDto>.SuccessAsync(_mapper.Map<AccJournalMasterDto>(item), localization.GetMessagesResource("DeletedSuccess"));
                }
                else
                {
                    return await Result<AccJournalMasterDto>.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}");

                }
            }
            catch (Exception exp)
            {
                return await Result<AccJournalMasterDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await accRepositoryManager.AccJournalMasterRepository.DeleteJournalWithDetailsByJId(Id);
                if (item != null)
                {
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    return await Result<AccJournalMasterDto>.SuccessAsync(_mapper.Map<AccJournalMasterDto>(item), localization.GetMessagesResource("DeletedSuccess"));
                }
                else
                {
                    return await Result<AccJournalMasterDto>.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}");

                }

            }

            catch (Exception exp)
            {
                return await Result<AccJournalMasterDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccJournalMasterEditDto>> Update(AccJournalMasterEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccJournalMasterEditDto>.FailAsync($"Error in {this.GetType()} : the passed entity IS NULL.");


            try
            {
                var item = await accRepositoryManager.AccJournalMasterRepository.GetById(entity.JId);

                if (item == null) return await Result<AccJournalMasterEditDto>.FailAsync($"--- there is no Data with this id: {entity.JId}---");

                item.UpdateDate = DateTime.UtcNow;
                item.UpdateUserId = (int)session.UserId;
                _mapper.Map(entity, item);

                accRepositoryManager.AccJournalMasterRepository.Update(item);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccJournalMasterEditDto>.SuccessAsync(_mapper.Map<AccJournalMasterEditDto>(item), "Item updated successfully");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccJournalMasterEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }


        public async Task<IResult<AccJournalMasterDto>> AddDetaile(AccJournalMasterDtoVW entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccJournalMasterDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {

                //====التشييك على المرفقات في القيد في حال كانت الزامية

                string AttachmentMandatory = await sysConfigurationAppHelper.GetValue(171, session.FacilityId);
                if (AttachmentMandatory == "1")
                {
                    if (entity.FileDtos.Count() == 0)
                    {
                        return await Result<AccJournalMasterDto>.FailAsync($"{localization.GetAccResource("AttachmentMandatory")}");

                    }
                }
                var AccJournalMasterDto = entity.AccJournalMasterDto;

                //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين 
                decimal sumCredit = entity.DetailsList.Sum(x => x.Credit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);
                decimal sumDebit = entity.DetailsList.Sum(x => x.Debit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);

                if (sumCredit != sumDebit)
                {
                    return await Result<AccJournalMasterDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }

                int? Status_Id = 0;
                //--------------------اضافة قيد موقت
                if (AccJournalMasterDto.StatusId != 5)
                {
                    Status_Id = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(session.FacilityId);

                }
                else
                {
                    Status_Id = AccJournalMasterDto.StatusId;
                }
                //------------------------تشيك التاريخ
                if (await DateHelper.CheckDate(AccJournalMasterDto.JDateGregorian ?? "0", session.FacilityId, session.CalendarType) == true)
                {
                    AccJournalMasterDto.JDateGregorian = AccJournalMasterDto.JDateGregorian;
                    AccJournalMasterDto.JDateHijri = AccJournalMasterDto.JDateGregorian;
                }
                else
                {
                    return await Result<AccJournalMasterDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }
                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(AccJournalMasterDto.PeriodId ?? 0, AccJournalMasterDto.JDateGregorian) == false)
                {
                    return await Result<AccJournalMasterDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }



                AccJournalMasterDto.FacilityId = session.FacilityId;
                AccJournalMasterDto.StatusId = Status_Id;
                AccJournalMasterDto.FinYear = session.FinYear;
                AccJournalMasterDto.ReferenceNo = 0;
                AccJournalMasterDto.PaymentTypeId = 0;
                AccJournalMasterDto.BankId = 0;
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var addJournalMaster = await accRepositoryManager.AccJournalMasterRepository.AddACCJournalMaster(AccJournalMasterDto, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                if (!addJournalMaster.Succeeded)
                    return await Result<AccJournalMasterDto>.FailAsync(addJournalMaster.Status.message);


                entity.AccJournalMasterDto.JCode = addJournalMaster.Data.JCode;
                entity.AccJournalMasterDto.JId = addJournalMaster.Data.JId;
                long jId = addJournalMaster.Data.JId;

                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<AccJournalMasterDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");
                foreach (var child in entity.DetailsList)

                {

                    //-------------------------------- '--جاب  رقم حساب 

                    long AccAccountID = 0;

                    if (!string.IsNullOrEmpty(child.AccAccountCode))
                    {
                        AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                        if (AccAccountID == 0)
                        {
                            return await Result<AccJournalMasterDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                        }
                    }

                    //-------------------------------- '--جاب  رقم مرجع 

                    long ReferenceNo = 0;
                    ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                    if (ReferenceNo == 0)
                    {
                        return await Result<AccJournalMasterDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

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
                    //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                    var DefCurrency = await accRepositoryManager.AccAccountRepository.GetCuureny(0, session.FacilityId);

                    if (AccJournalMasterDto.CurrencyId != DefCurrency)
                    {
                        if (child.CurrencyId == AccJournalMasterDto.CurrencyId)
                        {
                            child.ExchangeRate = AccJournalMasterDto.ExchangeRate;
                        }
                        else
                        {
                            child.ExchangeRate = 1;
                        }
                    }
                    else
                    {
                        child.ExchangeRate = 1;
                    }
                    child.JId = jId;
                    child.JDetailesId = 0;
                    child.AccAccountId = AccAccountID;
                    child.ReferenceNo = ReferenceNo;
                    child.CcId = CCId;
                    child.Cc2Id = CCId2;
                    child.Cc3Id = CCId3;
                    child.Cc4Id = CCId4;
                    child.Cc5Id = CCId5;
                    var addJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(child, cancellationToken);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                    if (!addJournalDetail1.Succeeded)
                        return await Result<AccJournalMasterDto>.FailAsync(addJournalDetail1.Status.message);

                }
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);


                var entityMap = _mapper.Map<AccJournalMasterDto>(addJournalMaster.Data);
                return await Result<AccJournalMasterDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {
                return await Result<AccJournalMasterDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }


        public async Task<long> GetCurrencyID(long AccountType, string code, long facilityId)
        {
            long CurrencyID = 0;
            switch (AccountType)
            {
                case 1: // حساب
                    CurrencyID = await accRepositoryManager.AccAccountRepository.GetCuurenyAccountCode(AccountType, code, facilityId);
                    break;
                case 2:
                case 21:
                case 22:
                case 23: // عميل
                    CurrencyID = await mainRepositoryManager.SysCustomerRepository.GetCurrencyCustomer(2, code, facilityId);
                    break;
                case 3:
                case 24:
                case 25:
                case 26: // مورد
                    CurrencyID = await mainRepositoryManager.SysCustomerRepository.GetCurrencyCustomer(3, code, facilityId);
                    break;
                default:
                    CurrencyID = await accRepositoryManager.AccAccountRepository.GetCuurenyAccountCode(3, code, facilityId);

                    break;
            }

            return CurrencyID;
        }


        public async Task<IResult<AccJournalMasterEditDto>> UpdateDetaile(AccJournalMasterEditDtoVW entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccJournalMasterEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");


            try
            {

                var AccJournalMasterDto = entity.AccJournalMasterDto;
                //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين  
                decimal sumCredit = entity.DetailsList.Where(d => d.IsDeleted == false).Sum(x => x.Credit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);
                decimal sumDebit = entity.DetailsList.Where(d => d.IsDeleted == false).Sum(x => x.Debit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);

                if (sumCredit != sumDebit)
                {
                    return await Result<AccJournalMasterEditDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }



                //====التشييك على المرفقات في القيد في حال كانت الزامية
                string AttachmentMandatory = await sysConfigurationAppHelper.GetValue(171, session.FacilityId);
                if (AttachmentMandatory == "1")
                {
                    if (entity.FileDtos.Count() == 0)
                    {
                        return await Result<AccJournalMasterEditDto>.FailAsync($"{localization.GetAccResource("NotAbleAnyAction")}");

                    }
                }

                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(AccJournalMasterDto.PeriodId ?? 0, AccJournalMasterDto.JDateGregorian) == false)
                {
                    return await Result<AccJournalMasterEditDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }


                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.UpdateACCJournalMaster(AccJournalMasterDto, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                if (!UpdateJournalMaster.Succeeded)
                    return await Result<AccJournalMasterEditDto>.FailAsync(UpdateJournalMaster.Status.message);


                entity.AccJournalMasterDto.JCode = UpdateJournalMaster.Data.JCode;
                entity.AccJournalMasterDto.JId = UpdateJournalMaster.Data.JId;
                long jId = UpdateJournalMaster.Data.JId;

                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<AccJournalMasterEditDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");
                foreach (var child in entity.DetailsList)
                {
                    //-------------------------------- '--جاب  رقم حساب 

                    long AccAccountID = 0;

                    if (!string.IsNullOrEmpty(child.AccAccountCode))
                    {
                        AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                        if (AccAccountID == 0)
                        {
                            return await Result<AccJournalMasterEditDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                        }
                    }



                    //-------------------------------- '--جاب  رقم مرجع 

                    long ReferenceNo = 0;
                    ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode ?? "0");

                    if (ReferenceNo == 0)
                    {
                        return await Result<AccJournalMasterEditDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

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
                        CCId5 = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(child.CostCenterCode5 ?? "0", session.FacilityId);

                    }
                    //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                    var DefCurrency = await accRepositoryManager.AccAccountRepository.GetCuureny(0, session.FacilityId);

                    if (AccJournalMasterDto.CurrencyId != DefCurrency)
                    {
                        if (child.CurrencyId == AccJournalMasterDto.CurrencyId)
                        {
                            child.ExchangeRate = AccJournalMasterDto.ExchangeRate;
                        }
                        else
                        {
                            child.ExchangeRate = 1;
                        }
                    }
                    else
                    {
                        child.ExchangeRate = 1;
                    }
                    if (child.JDetailesId == 0 && child.IsDeleted == false)
                    {
                        child.JId = jId;
                        child.AccAccountId = AccAccountID;
                        child.ReferenceNo = ReferenceNo;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        var addJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(child, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        if (!addJournalDetail1.Succeeded)
                            return await Result<AccJournalMasterEditDto>.FailAsync(addJournalDetail1.Status.message);
                    }
                    else
                    {
                        child.JId = jId;
                        child.AccAccountId = AccAccountID;
                        child.ReferenceNo = ReferenceNo;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        var UpdateJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.UpdateAccJournalDetail(child, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        if (!UpdateJournalDetail1.Succeeded)
                            return await Result<AccJournalMasterEditDto>.FailAsync(UpdateJournalDetail1.Status.message);

                    }




                }
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                var entityMap = _mapper.Map<AccJournalMasterEditDto>(UpdateJournalMaster.Data);


                return await Result<AccJournalMasterEditDto>.SuccessAsync(entityMap, "item added successfully");

            }
            catch (Exception exc)
            {
                return await Result<AccJournalMasterEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        //---------------------سند القبض
        #region Acc Income


        public async Task<IResult<AccIncomeDto>> AddIncome(AccIncomeDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccIncomeDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {


                //-------------------------------- '--الموظف غير موجود في قائمة الموظفين 
                long EmpId = 0;
                var chkEmpid = await hrRepositoryManager.HrEmployeeRepository.GetAll(x => x.IsDeleted == false && x.EmpId.Contains(entity.CollectionEmpCode));
                if (chkEmpid != null && chkEmpid.Count() == 0)
                {
                    return await Result<AccIncomeDto>.FailAsync(localization.GetResource1("EmployeeNotFound"));
                }
                else
                {
                    var employee = await mainRepositoryManager.InvestEmployeeRepository.GetOne(x => x.EmpId == entity.CollectionEmpCode);
                    if (employee == null)
                    {
                        return await Result<AccIncomeDto>.FailAsync(localization.GetResource1("EmployeeNotFound"));
                    }
                    EmpId = employee.Id;
                }
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

                //-------------------------------- '--جاب  رقم حساب 

                long AccAccountID = 0;

                if (!string.IsNullOrEmpty(entity.AccAccountCode))
                {
                    AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);

                    if (AccAccountID == 0)
                    {
                        return await Result<AccIncomeDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                    }
                }
                //-------------------------------- '--جاب  رقم مرجع 

                long ReferenceNo = 0;
                ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode ?? "0");

                if (ReferenceNo == 0)
                {
                    return await Result<AccIncomeDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

                }
                //------------------------تشيك التاريخ
                if (await DateHelper.CheckDate(entity.JDateGregorian, session.FacilityId, session.CalendarType) == true)
                {
                    entity.JDateGregorian = entity.JDateGregorian;
                    entity.JDateHijri = entity.JDateGregorian;
                }
                else
                {
                    return await Result<AccIncomeDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }
                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(entity.PeriodId ?? 0, entity.JDateGregorian) == false)
                {
                    return await Result<AccIncomeDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }
                //----------------------------طريقة الدفع
                if (entity.PaymentTypeId == 1 || entity.PaymentTypeId == 3)
                {
                    entity.ChequNo = null;
                    entity.BankId = 0;
                    entity.ChequDateHijri = null;
                }
                else
                {
                    entity.ChequNo = entity.ChequNo;
                    entity.BankId = entity.BankId;
                    entity.ChequDateHijri = entity.ChequDateHijri;
                }
                //----------------------ReferenceCode التشيك على 

                if (!string.IsNullOrEmpty(entity.ReferenceCode))
                {
                    if (await accRepositoryManager.AccJournalMasterRepository.NumberExists(entity.ReferenceCode, entity.CcId ?? 0, entity.DocTypeId ?? 0, session.FacilityId, session.PeriodId) > 0)
                    {
                        return await Result<AccIncomeDto>.FailAsync($"{localization.GetResource1("NumberExists")}");
                    }

                }

                AccJournalMasterDto accJournalMaster = new()
                {
                    CollectionEmpId = EmpId,
                    DocTypeId = 1,
                    StatusId = Status_Id,
                    FacilityId = session.FacilityId,
                    FinYear = session.FinYear,
                    ReferenceNo = 0,
                    JDateGregorian = entity.JDateGregorian,
                    JDateHijri = entity.JDateGregorian,
                    Amount = entity.Amount,
                    AmountWrite = entity.AmountWrite,
                    JDescription = entity.JDescription,
                    PaymentTypeId = entity.PaymentTypeId,
                    PeriodId = entity.PeriodId,
                    ReferenceCode = entity.ReferenceCode,
                    JBian = entity.JBian,
                    BankId = entity.BankId ?? 0,
                    CcId = entity.BranchId,
                    CurrencyId = entity.CurrencyId,
                    ExchangeRate = entity.ExchangeRate,
                    ChequNo = entity.ChequNo,
                    ChequDateHijri = entity.ChequDateHijri,
                };
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var addJournalMaster = await accRepositoryManager.AccJournalMasterRepository.AddACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                if (!addJournalMaster.Succeeded)
                    return await Result<AccIncomeDto>.FailAsync(addJournalMaster.Status.message);


                entity.JCode = addJournalMaster.Data.JCode;
                entity.JId = addJournalMaster.Data.JId;
                long jId = addJournalMaster.Data.JId;

                //----------------- الدالة تاتي في العملة الرئسية للنظام  
                decimal ExchangeRate = 0;
                int DefCurrency = await mainRepositoryManager.SysCurrencyRepository.GetDefaultCurrency();
                //-------------------------------------- تفاصيل القيد  الحساب الدين  
                #region "Acc Journal Detail Debit"
                //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 

                int AccountCurreny = await accRepositoryManager.AccAccountRepository.GetCuureny(entity.cashOnhandId ?? 0, session.FacilityId);
                if (entity.CurrencyId != DefCurrency)
                {
                    if (AccountCurreny != entity.CurrencyId)
                    {
                        ExchangeRate = entity.ExchangeRate ?? 0;
                    }
                    else
                    {
                        ExchangeRate = 1;
                    }
                }
                else
                {
                    ExchangeRate = 1;
                }

                AccJournalDetaileDto accJournalDetail1Debit = new()
                {
                    JId = jId,
                    AccAccountId = entity.cashOnhandId,
                    Credit = 0,
                    Debit = entity.Amount,
                    Description = entity.JBian,
                    JDateGregorian = entity.JDateGregorian,
                    ReferenceTypeId = 1,
                    ReferenceNo = 0,
                    CurrencyId = AccountCurreny,
                    ExchangeRate = ExchangeRate,
                    ReferenceCode = entity.ReferenceCode,
                };
                var addJournalDetailDebit = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(accJournalDetail1Debit, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                if (!addJournalDetailDebit.Succeeded)
                    return await Result<AccIncomeDto>.FailAsync(addJournalDetailDebit.Status.message);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                #endregion "Acc Journal Detail Debit"
                //-------------------------------------- تفاصيل القيد  الحساب المدين 
                #region "Acc Journal Detail Credit"

                long CCId = 0;
                if (!string.IsNullOrEmpty(entity.CostCenterCode))
                {
                    CCId = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(entity.CostCenterCode, session.FacilityId);

                }

                //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 

                AccountCurreny = await accRepositoryManager.AccAccountRepository.GetCuureny(AccAccountID, session.FacilityId);
                if (entity.CurrencyId != DefCurrency)
                {
                    if (AccountCurreny != entity.CurrencyId)
                    {
                        ExchangeRate = entity.ExchangeRate ?? 0;
                    }
                    else
                    {
                        ExchangeRate = 1;
                    }
                }
                else
                {
                    ExchangeRate = 1;
                }
                AccJournalDetaileDto accJournalDetail1Credit = new()
                {
                    JId = jId,
                    AccAccountId = AccAccountID,
                    Credit = entity.Amount,
                    JDateGregorian = entity.JDateGregorian,
                    Debit = 0,
                    Description = entity.JBian,
                    CcId = CCId,
                    ReferenceTypeId = (int)entity.ReferenceTypeId,
                    ReferenceNo = ReferenceNo,
                    CurrencyId = AccountCurreny,
                    ExchangeRate = ExchangeRate,
                    ReferenceCode = entity.ReferenceCode,
                };
                var addJournalDetailCredit = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(accJournalDetail1Credit, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                if (!addJournalDetailCredit.Succeeded)
                    return await Result<AccIncomeDto>.FailAsync(addJournalDetailCredit.Status.message);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                #endregion "Acc Journal Detail Credit"

                //----------------------save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }

                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                var entityMap = _mapper.Map<AccIncomeDto>(addJournalMaster.Data);


                return await Result<AccIncomeDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {
                return await Result<AccIncomeDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }
        public async Task<IResult<AccIncomeEditDto>> UpdateIncome(AccIncomeMasterEditDtoVW entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccIncomeEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");


            try
            {

                var AccJournalMasterDto = entity.AccJournalMasterDto;
                //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين  
                decimal sumCredit = entity.DetailsList.Where(d => d.IsDeleted == false).Sum(x => x.Credit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);
                decimal sumDebit = (AccJournalMasterDto.Amount ?? 0) * (AccJournalMasterDto.ExchangeRate ?? 0);

                if (sumCredit != sumDebit)
                {
                    return await Result<AccIncomeEditDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }

                //-------------------------------- '--الموظف غير موجود في قائمة الموظفين 
                long EmpId = 0;
                var chkEmpid = await hrRepositoryManager.HrEmployeeRepository.GetAll(x => x.IsDeleted == false && x.EmpId.Contains(AccJournalMasterDto.CollectionEmpCode));
                if (chkEmpid != null && chkEmpid.Count() == 0)
                {
                    return await Result<AccIncomeEditDto>.FailAsync(localization.GetResource1("EmployeeNotFound"));
                }
                else
                {
                    var employee = await mainRepositoryManager.InvestEmployeeRepository.GetOne(x => x.EmpId == AccJournalMasterDto.CollectionEmpCode);
                    if (employee == null)
                    {
                        return await Result<AccIncomeEditDto>.FailAsync(localization.GetResource1("EmployeeNotFound"));
                    }
                    EmpId = employee.Id;
                }

                //====التشييك على المرفقات في القيد في حال كانت الزامية
                string AttachmentMandatory = await sysConfigurationAppHelper.GetValue(171, session.FacilityId);
                if (AttachmentMandatory == "1")
                {
                    if (entity.FileDtos.Count() == 0)
                    {
                        return await Result<AccIncomeEditDto>.FailAsync($"{localization.GetAccResource("NotAbleAnyAction")}");

                    }
                }

                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(AccJournalMasterDto.PeriodId ?? 0, AccJournalMasterDto.JDateGregorian) == false)
                {
                    return await Result<AccIncomeEditDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }
                //----------------------------طريقة الدفع
                if (AccJournalMasterDto.PaymentTypeId == 1 || AccJournalMasterDto.PaymentTypeId == 3)
                {
                    AccJournalMasterDto.ChequNo = null;
                    AccJournalMasterDto.BankId = 0;
                    AccJournalMasterDto.ChequDateHijri = null;
                }
                else
                {
                    AccJournalMasterDto.ChequNo = AccJournalMasterDto.ChequNo;
                    AccJournalMasterDto.BankId = AccJournalMasterDto.BankId;
                    AccJournalMasterDto.ChequDateHijri = AccJournalMasterDto.ChequDateHijri;
                }
                AccJournalMasterDto.CollectionEmpId = EmpId;
                int? Status_Id = 0;
                //--------------------اضافة قيد موقت
                if (AccJournalMasterDto.StatusId != 5)
                {
                    Status_Id = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(session.FacilityId);

                }
                else
                {
                    Status_Id = AccJournalMasterDto.StatusId;
                }
                AccJournalMasterEditDto accJournalMaster = new()
                {
                    JId = AccJournalMasterDto.JId,
                    JCode = AccJournalMasterDto.JCode,
                    CollectionEmpId = EmpId,
                    DocTypeId = 1,
                    StatusId = Status_Id,
                    FacilityId = session.FacilityId,
                    FinYear = session.FinYear,
                    ReferenceNo = AccJournalMasterDto.ReferenceNo,
                    JDateGregorian = AccJournalMasterDto.JDateGregorian,
                    JDateHijri = AccJournalMasterDto.JDateGregorian,
                    Amount = AccJournalMasterDto.Amount,
                    AmountWrite = AccJournalMasterDto.AmountWrite,
                    JDescription = AccJournalMasterDto.JDescription,
                    PaymentTypeId = AccJournalMasterDto.PaymentTypeId,
                    PeriodId = AccJournalMasterDto.PeriodId,
                    ReferenceCode = AccJournalMasterDto.ReferenceCode,
                    JBian = AccJournalMasterDto.JBian,
                    BankId = AccJournalMasterDto.BankId ?? 0,
                    CcId = AccJournalMasterDto.CcId,
                    CurrencyId = AccJournalMasterDto.CurrencyId,
                    ExchangeRate = AccJournalMasterDto.ExchangeRate,
                    ChequNo = AccJournalMasterDto.ChequNo,
                    ChequDateHijri = AccJournalMasterDto.ChequDateHijri,
                };
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.UpdateACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                if (!UpdateJournalMaster.Succeeded)
                    return await Result<AccIncomeEditDto>.FailAsync(UpdateJournalMaster.Status.message);


                entity.AccJournalMasterDto.JCode = UpdateJournalMaster.Data.JCode;
                entity.AccJournalMasterDto.JId = UpdateJournalMaster.Data.JId;
                long jId = UpdateJournalMaster.Data.JId;

                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<AccIncomeEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");
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
                    //-------------------------------- '--جاب  رقم حساب 
                    if (child.Credit > 0)
                    {


                        long AccAccountID = 0;

                        if (!string.IsNullOrEmpty(child.AccAccountCode))
                        {
                            AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                            if (AccAccountID == 0)
                            {
                                return await Result<AccIncomeEditDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                            }
                        }



                        //-------------------------------- '--جاب  رقم مرجع 

                        long ReferenceNo = 0;
                        ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                        if (ReferenceNo == 0)
                        {
                            return await Result<AccIncomeEditDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

                        }


                        //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                        var DefCurrency = await accRepositoryManager.AccAccountRepository.GetCuureny(0, session.FacilityId);

                        if (AccJournalMasterDto.CurrencyId != DefCurrency)
                        {
                            if (child.CurrencyId == AccJournalMasterDto.CurrencyId)
                            {
                                child.ExchangeRate = AccJournalMasterDto.ExchangeRate;
                            }
                            else
                            {
                                child.ExchangeRate = 1;
                            }
                        }
                        else
                        {
                            child.ExchangeRate = 1;
                        }
                        if (child.JDetailesId == 0 && child.IsDeleted == false)
                        {
                            child.JId = jId;
                            child.AccAccountId = AccAccountID;
                            child.ReferenceNo = ReferenceNo;
                            child.CcId = CCId;
                            child.Cc2Id = CCId2;
                            child.Cc3Id = CCId3;
                            child.Cc4Id = CCId4;
                            child.Cc5Id = CCId5;
                            var addJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(child, cancellationToken);
                            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                            if (!addJournalDetail1.Succeeded)
                                return await Result<AccIncomeEditDto>.FailAsync(addJournalDetail1.Status.message);
                        }
                        else
                        {
                            child.JId = jId;
                            child.AccAccountId = AccAccountID;
                            child.ReferenceNo = ReferenceNo;
                            child.CcId = CCId;
                            child.Cc2Id = CCId2;
                            child.Cc3Id = CCId3;
                            child.Cc4Id = CCId4;
                            child.Cc5Id = CCId5;
                            var UpdateJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.UpdateAccJournalDetail(child, cancellationToken);
                            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                            if (!UpdateJournalDetail1.Succeeded)
                                return await Result<AccIncomeEditDto>.FailAsync(UpdateJournalDetail1.Status.message);

                        }




                    }
                    else
                    {
                        //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                        //----------------- الدالة تاتي في العملة الرئسية للنظام  
                        decimal ExchangeRate = 0;
                        int DefCurrency = await mainRepositoryManager.SysCurrencyRepository.GetDefaultCurrency();
                        int AccountCurreny = await accRepositoryManager.AccAccountRepository.GetCuureny(AccJournalMasterDto.cashOnhandId ?? 0, session.FacilityId);
                        if (AccJournalMasterDto.CurrencyId != DefCurrency)
                        {
                            if (AccountCurreny != AccJournalMasterDto.CurrencyId)
                            {
                                ExchangeRate = AccJournalMasterDto.ExchangeRate ?? 0;
                            }
                            else
                            {
                                ExchangeRate = 1;
                            }
                        }
                        else
                        {
                            ExchangeRate = 1;
                        }
                        child.JId = jId;
                        child.Debit = AccJournalMasterDto.Amount ?? 0;
                        child.AccAccountId = AccJournalMasterDto.cashOnhandId;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        child.ReferenceCode = AccJournalMasterDto.ReferenceCode;
                        var UpdateJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.UpdateAccJournalDetail(child, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        if (!UpdateJournalDetail1.Succeeded)
                            return await Result<AccIncomeEditDto>.FailAsync(UpdateJournalDetail1.Status.message);
                    }
                }
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                var entityMap = _mapper.Map<AccIncomeEditDto>(UpdateJournalMaster.Data);


                return await Result<AccIncomeEditDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));

            }
            catch (Exception exc)
            {
                return await Result<AccIncomeEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }


        public async Task<int> GetBookSerial(long facilityId, long branchId, int DocTypeId)
        {
            int ReferenceCode = 0;
            ReferenceCode = await accRepositoryManager.AccJournalMasterRepository.GetBookSerial(facilityId, branchId, DocTypeId);
            return ReferenceCode;
        }
        #endregion Acc Income
        //---------------------سند الصرف
        #region Acc Expenses


        public async Task<IResult<AccExpensesDto>> AddExpenses(AccExpensesDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccExpensesDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");
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

                //-------------------------------- '--جاب  رقم حساب 

                long AccAccountID = 0;

                if (!string.IsNullOrEmpty(entity.AccAccountCode))
                {
                    AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);

                    if (AccAccountID == 0)
                    {
                        return await Result<AccExpensesDto>.FailAsync(localization.GetResource1("ThereIsNoAccountWithThisNumber"));

                    }
                }
                //-------------------------------- '--جاب  رقم مرجع 

                long ReferenceNo = 0;
                ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);

                if (ReferenceNo == 0)
                {
                    return await Result<AccExpensesDto>.FailAsync(localization.GetAccResource("ThereIsNoReferenceForThisNumber"));

                }

                //====التشييك على المرفقات في القيد في حال كانت الزامية

                string AttachmentMandatory = await sysConfigurationAppHelper.GetValue(172, session.FacilityId);
                if (AttachmentMandatory == "1")
                {
                    if (entity.FileDtos.Count() == 0)
                    {
                        return await Result<AccExpensesDto>.FailAsync($"{localization.GetAccResource("AttachmentMandatory")}");

                    }
                }
                //-------------------------------- '--ركز التكلفة لا يقبل مع هذا الحساب 


                // '------------------بداية التشييك على مراكز التكلفة 
                string MsgErrorCostCenter = null;

                var CostCenter = await accRepositoryManager.AccAccountsCostcenterVWRepository.GetAll(X => X.IsDeleted == false && X.AccAccountId == AccAccountID);

                //  'هل مطلوب مركز تكلفة ام لا  CcNo1
                if (!string.IsNullOrEmpty(entity.CostCenterCode))
                {


                    var CostCenterOne = CostCenter.Where(x => x.CcNo == 1);
                    if (CostCenterOne != null)
                    {
                        foreach (var Cost in CostCenterOne)
                        {

                            if (!string.IsNullOrEmpty(Cost.CcIdFrom) && !string.IsNullOrEmpty(Cost.CcIdTo))
                            {
                                if (Int64.Parse(entity.CostCenterCode) > Int64.Parse(Cost.CcIdTo) ||
                                    Int64.Parse(entity.CostCenterCode) < Int64.Parse(Cost.CcIdFrom))
                                {

                                    return await Result<AccExpensesDto>.FailAsync(localization.GetAccResource("Allowablecostcenters") + Cost.CcIdFrom + " - " + Cost.CcIdTo);

                                }
                            }


                        }

                    }

                }
                else
                {
                    var CostCenterOne = CostCenter.Where(x => x.CcNo == 1);
                    if (CostCenterOne != null)
                    {
                        foreach (var Cost in CostCenterOne)
                        {
                            if (Cost.IsRequired == true)
                            {
                                MsgErrorCostCenter += localization.GetAccResource("CostcenterAccountRequired") + ",";


                            }


                        }

                    }

                }
                //  'هل مطلوب مركز تكلفة ام لا  CcNo2
                var CostCenterTow = CostCenter.Where(x => x.CcNo == 2);
                if (CostCenterTow != null)
                {
                    foreach (var Cost in CostCenterTow)
                    {
                        if (Cost.IsRequired == true)
                        {
                            MsgErrorCostCenter += localization.GetAccResource("Costcenter2AccountRequired") + ",";




                        }


                    }

                }
                //  'هل مطلوب مركز تكلفة ام لا  CcNo3
                var CostCenterThree = CostCenter.Where(x => x.CcNo == 3);
                if (CostCenterThree != null)
                {
                    foreach (var Cost in CostCenterThree)
                    {
                        if (Cost.IsRequired == true)
                        {

                            MsgErrorCostCenter += localization.GetAccResource("Costcenter3AccountRequired") + ",";


                        }


                    }

                }

                //  'هل مطلوب مركز تكلفة ام لا  CcNo4
                var CostCenterFour = CostCenter.Where(x => x.CcNo == 4);
                if (CostCenterFour != null)
                {
                    foreach (var Cost in CostCenterFour)
                    {
                        if (Cost.IsRequired == true)
                        {

                            MsgErrorCostCenter += localization.GetAccResource("Costcenter4AccountRequired") + ",";


                        }


                    }

                }

                //  'هل مطلوب مركز تكلفة ام لا  CcNo5
                var CostCenterFive = CostCenter.Where(x => x.CcNo == 5);
                if (CostCenterFive != null)
                {
                    foreach (var Cost in CostCenterFive)
                    {
                        if (Cost.IsRequired == true)
                        {


                            MsgErrorCostCenter += localization.GetAccResource("Costcenter5AccountRequired") + ",";

                        }


                    }

                }

                if (!string.IsNullOrEmpty(MsgErrorCostCenter))
                {
                    return await Result<AccExpensesDto>.FailAsync(MsgErrorCostCenter);

                }
                //-------------رصيد الحساب-------------------
                if (await sysConfigurationAppHelper.GetValue(344, session.FacilityId) == "1")
                {
                    decimal Account_Balance = await accRepositoryManager.AccJournalMasterRepository.GetBalanceForAccount(entity.cashOnhandId ?? 0, session.FacilityId, session.FinYear);
                    if (entity.Amount > Account_Balance)
                    {
                        return await Result<AccExpensesDto>.FailAsync($"{localization.GetResource1("TheAccountBalanceIsInsufficientForThisOperation")}");

                    }

                }

                //------------------------تشيك التاريخ
                if (Bahsas.IsHijri(entity.JDateGregorian, session))
                {
                    entity.JDateGregorian = entity.JDateGregorian;
                    entity.JDateHijri = entity.JDateGregorian;
                }
                else
                {
                    return await Result<AccExpensesDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }
                if (String.IsNullOrEmpty(entity.ChequDateHijri) == false)
                {
                    entity.ChequDateHijri = entity.ChequDateHijri;

                }
                else
                {
                    return await Result<AccExpensesDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }

                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(entity.PeriodId ?? 0, entity.JDateGregorian) == false)
                {
                    return await Result<AccExpensesDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }
                //----------------------------طريقة الدفع
                if (entity.PaymentTypeId == 1 || entity.PaymentTypeId == 3)
                {
                    entity.ChequNo = null;
                    entity.BankId = 0;
                    entity.ChequDateHijri = null;
                }
                else
                {
                    entity.ChequNo = entity.ChequNo;
                    entity.BankId = entity.BankId;
                    entity.ChequDateHijri = entity.ChequDateHijri;
                }


                AccJournalMasterDto accJournalMaster = new()
                {

                    DocTypeId = 2,
                    StatusId = Status_Id,
                    FacilityId = session.FacilityId,
                    FinYear = session.FinYear,
                    ReferenceNo = 0,
                    JDateGregorian = entity.JDateGregorian,
                    JDateHijri = entity.JDateGregorian,
                    Amount = entity.Amount,
                    AmountWrite = entity.AmountWrite,
                    JDescription = entity.JDescription,
                    PaymentTypeId = entity.PaymentTypeId,
                    PeriodId = entity.PeriodId,
                    ReferenceCode = entity.ReferenceCode,
                    JBian = entity.JBian,
                    BankId = entity.BankId ?? 0,
                    CcId = entity.BranchId,
                    CurrencyId = entity.CurrencyId,
                    ExchangeRate = entity.ExchangeRate,
                    ChequNo = entity.ChequNo,
                    ChequDateHijri = entity.ChequDateHijri,
                };
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var addJournalMaster = await accRepositoryManager.AccJournalMasterRepository.AddACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                if (!addJournalMaster.Succeeded)
                    return await Result<AccExpensesDto>.FailAsync(addJournalMaster.Status.message);


                entity.JCode = addJournalMaster.Data.JCode;
                entity.JId = addJournalMaster.Data.JId;
                long jId = addJournalMaster.Data.JId;

                //----------------- الدالة تاتي في العملة الرئسية للنظام  
                decimal ExchangeRate = 0;
                int DefCurrency = await mainRepositoryManager.SysCurrencyRepository.GetDefaultCurrency();
                //-------------------------------------- تفاصيل القيد  الحساب الدين  
                #region "Acc Journal Detail Debit"
                //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                long CCId = 0;
                if (!string.IsNullOrEmpty(entity.CostCenterCode))
                {
                    CCId = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(entity.CostCenterCode, session.FacilityId);

                }
                int AccountCurreny = await accRepositoryManager.AccAccountRepository.GetCuureny(AccAccountID, session.FacilityId);
                if (entity.CurrencyId != DefCurrency)
                {
                    if (AccountCurreny != entity.CurrencyId)
                    {
                        ExchangeRate = entity.ExchangeRate ?? 0;
                    }
                    else
                    {
                        ExchangeRate = 1;
                    }
                }
                else
                {
                    ExchangeRate = 1;
                }

                AccJournalDetaileDto accJournalDetail1Debit = new()
                {
                    JId = jId,
                    AccAccountId = AccAccountID,
                    Credit = 0,
                    Debit = entity.Amount,
                    Description = entity.JBian,
                    JDateGregorian = entity.JDateGregorian,
                    ReferenceTypeId = (int)entity.ReferenceTypeId,
                    ReferenceNo = ReferenceNo,
                    CurrencyId = AccountCurreny,
                    CcId = CCId,
                    ExchangeRate = ExchangeRate,
                };
                var addJournalDetailDebit = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(accJournalDetail1Debit, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                if (!addJournalDetailDebit.Succeeded)
                    return await Result<AccExpensesDto>.FailAsync(addJournalDetailDebit.Status.message);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                #endregion "Acc Journal Detail Debit"
                //-------------------------------------- تفاصيل القيد  الحساب المدين 
                #region "Acc Journal Detail Credit"

                if (!string.IsNullOrEmpty(entity.CostCenterCode))
                {
                    CCId = await accRepositoryManager.AccCostCenterRepository.GetCostCenterIdByCode(entity.CostCenterCode, session.FacilityId);

                }

                //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 

                AccountCurreny = await accRepositoryManager.AccAccountRepository.GetCuureny(entity.cashOnhandId ?? 0, session.FacilityId);
                if (entity.CurrencyId != DefCurrency)
                {
                    if (AccountCurreny != entity.CurrencyId)
                    {
                        ExchangeRate = entity.ExchangeRate ?? 0;
                    }
                    else
                    {
                        ExchangeRate = 1;
                    }
                }
                else
                {
                    ExchangeRate = 1;
                }
                AccJournalDetaileDto accJournalDetail1Credit = new()
                {
                    JId = jId,
                    AccAccountId = entity.cashOnhandId ?? 0,
                    Credit = entity.Amount,
                    JDateGregorian = entity.JDateGregorian,
                    Debit = 0,
                    Description = entity.JBian,
                    CcId = 0,
                    ReferenceTypeId = 1,
                    ReferenceNo = 0,
                    CurrencyId = AccountCurreny,
                    ExchangeRate = ExchangeRate,
                };
                var addJournalDetailCredit = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(accJournalDetail1Credit, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                if (!addJournalDetailCredit.Succeeded)
                    return await Result<AccExpensesDto>.FailAsync(addJournalDetailCredit.Status.message);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                #endregion "Acc Journal Detail Credit"

                //----------------------save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                //------------------------'Update J_ID for financial request
                if (entity.RequestID > 0)
                {
                    var item = await accRepositoryManager.AccRequestRepository.GetById(entity.RequestID ?? 0);
                    if (item == null) return Result<AccExpensesDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
                    item.JId = jId;
                    accRepositoryManager.AccRequestRepository.Update(item);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                var entityMap = _mapper.Map<AccExpensesDto>(addJournalMaster.Data);


                return await Result<AccExpensesDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {
                return await Result<AccExpensesDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }
        public async Task<IResult<AccExpensesEditDto>> UpdateExpenses(AccExpensesMasterEditDtoVW entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccExpensesEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");


            try
            {

                var AccJournalMasterDto = entity.AccJournalMasterDto;
                //====اجمالي الحساب المدين لايساوي اجمالي الحساب  الدائن  
                decimal sumDebit = entity.DetailsList.Where(d => d.IsDeleted == false).Sum(x => x.Debit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);
                decimal sumCredit = (AccJournalMasterDto.Amount ?? 0) * (AccJournalMasterDto.ExchangeRate ?? 0);

                if (sumCredit != sumDebit)
                {
                    return await Result<AccExpensesEditDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }
                int? Status_Id = 0;
                //--------------------اضافة قيد موقت
                if (AccJournalMasterDto.StatusId != 5)
                {
                    Status_Id = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(session.FacilityId);

                }
                else
                {
                    Status_Id = AccJournalMasterDto.StatusId;
                }


                //====التشييك على المرفقات في القيد في حال كانت الزامية
                string AttachmentMandatory = await sysConfigurationAppHelper.GetValue(172, session.FacilityId);
                if (AttachmentMandatory == "1")
                {
                    if (entity.FileDtos.Count() == 0)
                    {
                        return await Result<AccExpensesEditDto>.FailAsync($"{localization.GetAccResource("NotAbleAnyAction")}");

                    }
                }

                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(AccJournalMasterDto.PeriodId ?? 0, AccJournalMasterDto.JDateGregorian ?? "0") == false)
                {
                    return await Result<AccExpensesEditDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }
                //------------------------تشيك التاريخ
                if (Bahsas.IsHijri(AccJournalMasterDto.JDateGregorian, session))
                {
                    AccJournalMasterDto.JDateGregorian = AccJournalMasterDto.JDateGregorian;
                    AccJournalMasterDto.JDateHijri = AccJournalMasterDto.JDateGregorian;
                }
                else
                {
                    return await Result<AccExpensesEditDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }
                if (String.IsNullOrEmpty(AccJournalMasterDto.ChequDateHijri) == false)
                {
                    AccJournalMasterDto.ChequDateHijri = AccJournalMasterDto.ChequDateHijri;

                }
                else
                {
                    return await Result<AccExpensesEditDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }
                //----------------------------طريقة الدفع
                if (AccJournalMasterDto.PaymentTypeId == 1 || AccJournalMasterDto.PaymentTypeId == 3)
                {
                    AccJournalMasterDto.ChequNo = null;
                    AccJournalMasterDto.BankId = 0;
                    AccJournalMasterDto.ChequDateHijri = null;
                }
                else
                {
                    AccJournalMasterDto.ChequNo = AccJournalMasterDto.ChequNo;
                    AccJournalMasterDto.BankId = AccJournalMasterDto.BankId;
                    AccJournalMasterDto.ChequDateHijri = AccJournalMasterDto.ChequDateHijri;
                }
                AccJournalMasterDto.CollectionEmpId = 0;
                AccJournalMasterEditDto accJournalMaster = new()
                {
                    JId = AccJournalMasterDto.JId,
                    JCode = AccJournalMasterDto.JCode,
                    DocTypeId = 2,
                    StatusId = Status_Id,
                    JDateGregorian = AccJournalMasterDto.JDateGregorian,
                    JDateHijri = AccJournalMasterDto.JDateGregorian,
                    Amount = AccJournalMasterDto.Amount,
                    ReferenceNo = AccJournalMasterDto.ReferenceNo,
                    AmountWrite = AccJournalMasterDto.AmountWrite,
                    JDescription = AccJournalMasterDto.JDescription,
                    PaymentTypeId = AccJournalMasterDto.PaymentTypeId,
                    PeriodId = AccJournalMasterDto.PeriodId,
                    ReferenceCode = AccJournalMasterDto.ReferenceCode,
                    JBian = AccJournalMasterDto.JBian,
                    BankId = AccJournalMasterDto.BankId ?? 0,
                    CcId = AccJournalMasterDto.CcId,
                    CurrencyId = AccJournalMasterDto.CurrencyId,
                    ExchangeRate = AccJournalMasterDto.ExchangeRate,
                    ChequNo = AccJournalMasterDto.ChequNo,
                    ChequDateHijri = AccJournalMasterDto.ChequDateHijri,
                };
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.UpdateACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                if (!UpdateJournalMaster.Succeeded)
                    return await Result<AccExpensesEditDto>.FailAsync(UpdateJournalMaster.Status.message);


                entity.AccJournalMasterDto.JCode = UpdateJournalMaster.Data.JCode;
                entity.AccJournalMasterDto.JId = UpdateJournalMaster.Data.JId;
                long jId = UpdateJournalMaster.Data.JId;

                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<AccExpensesEditDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");
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
                    //-------------------------------- '--جاب  رقم حساب 
                    if (child.Debit > 0)
                    {


                        long AccAccountID = 0;

                        if (!string.IsNullOrEmpty(child.AccAccountCode))
                        {
                            AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                            if (AccAccountID == 0)
                            {
                                return await Result<AccExpensesEditDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                            }
                        }



                        //-------------------------------- '--جاب  رقم مرجع 

                        long ReferenceNo = 0;
                        ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                        if (ReferenceNo == 0)
                        {
                            return await Result<AccExpensesEditDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

                        }


                        //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                        var DefCurrency = await accRepositoryManager.AccAccountRepository.GetCuureny(0, session.FacilityId);

                        if (AccJournalMasterDto.CurrencyId != DefCurrency)
                        {
                            if (child.CurrencyId == AccJournalMasterDto.CurrencyId)
                            {
                                child.ExchangeRate = AccJournalMasterDto.ExchangeRate;
                            }
                            else
                            {
                                child.ExchangeRate = 1;
                            }
                        }
                        else
                        {
                            child.ExchangeRate = 1;
                        }
                        if (child.JDetailesId == 0 && child.IsDeleted == false)
                        {
                            child.JId = jId;
                            child.AccAccountId = AccAccountID;
                            child.ReferenceNo = ReferenceNo;
                            child.CcId = CCId;
                            child.Cc2Id = CCId2;
                            child.Cc3Id = CCId3;
                            child.Cc4Id = CCId4;
                            child.Cc5Id = CCId5;
                            var addJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(child, cancellationToken);
                            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                            if (!addJournalDetail1.Succeeded)
                                return await Result<AccExpensesEditDto>.FailAsync(addJournalDetail1.Status.message);
                        }
                        else
                        {
                            child.JId = jId;
                            child.AccAccountId = AccAccountID;
                            child.ReferenceNo = ReferenceNo;
                            child.CcId = CCId;
                            child.Cc2Id = CCId2;
                            child.Cc3Id = CCId3;
                            child.Cc4Id = CCId4;
                            child.Cc5Id = CCId5;
                            var UpdateJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.UpdateAccJournalDetail(child, cancellationToken);
                            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                            if (!UpdateJournalDetail1.Succeeded)
                                return await Result<AccExpensesEditDto>.FailAsync(UpdateJournalDetail1.Status.message);

                        }




                    }
                    else
                    {
                        //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                        //----------------- الدالة تاتي في العملة الرئسية للنظام  
                        decimal ExchangeRate = 0;
                        int DefCurrency = await mainRepositoryManager.SysCurrencyRepository.GetDefaultCurrency();
                        int AccountCurreny = await accRepositoryManager.AccAccountRepository.GetCuureny(AccJournalMasterDto.cashOnhandId ?? 0, session.FacilityId);
                        if (AccJournalMasterDto.CurrencyId != DefCurrency)
                        {
                            if (AccountCurreny != AccJournalMasterDto.CurrencyId)
                            {
                                ExchangeRate = AccJournalMasterDto.ExchangeRate ?? 0;
                            }
                            else
                            {
                                ExchangeRate = 1;
                            }
                        }
                        else
                        {
                            ExchangeRate = 1;
                        }
                        child.JId = jId;
                        child.Credit = AccJournalMasterDto.Amount ?? 0;
                        child.AccAccountId = AccJournalMasterDto.cashOnhandId;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        child.ReferenceCode = AccJournalMasterDto.ReferenceCode;
                        var UpdateJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.UpdateAccJournalDetail(child, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        if (!UpdateJournalDetail1.Succeeded)
                            return await Result<AccExpensesEditDto>.FailAsync(UpdateJournalDetail1.Status.message);
                    }
                }
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                var entityMap = _mapper.Map<AccExpensesEditDto>(UpdateJournalMaster.Data);


                return await Result<AccExpensesEditDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));

            }
            catch (Exception exc)
            {
                return await Result<AccExpensesEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }



        #endregion Acc Expenses
        //---------------------القيد العكسي
        #region Journal Reverse
        public async Task<IResult<AccJournalReverseDto>> AddJournalReverse(AccJournalReverseDtoVW entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccJournalReverseDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {

                //====التشييك على المرفقات في القيد في حال كانت الزامية

                string AttachmentMandatory = await sysConfigurationAppHelper.GetValue(171, session.FacilityId);
                if (AttachmentMandatory == "1")
                {
                    if (entity.FileDtos.Count() == 0)
                    {
                        return await Result<AccJournalReverseDto>.FailAsync($"{localization.GetAccResource("AttachmentMandatory")}");

                    }
                }
                var AccJournalMasterDto = entity.AccJournalMasterDto;

                //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين 
                decimal sumCredit = entity.DetailsList.Sum(x => x.Credit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);
                decimal sumDebit = entity.DetailsList.Sum(x => x.Debit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);

                if (sumCredit != sumDebit)
                {
                    return await Result<AccJournalReverseDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }
                int? Status_Id = 0;
                //--------------------اضافة قيد موقت
                if (AccJournalMasterDto.StatusId != 5)
                {
                    Status_Id = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(session.FacilityId);

                }
                else
                {
                    Status_Id = AccJournalMasterDto.StatusId;
                }
                //------------------------تشيك التاريخ
                if (await DateHelper.CheckDate(AccJournalMasterDto.JDateGregorian, session.FacilityId, session.CalendarType) == true)
                {
                    AccJournalMasterDto.JDateGregorian = AccJournalMasterDto.JDateGregorian;
                    AccJournalMasterDto.JDateHijri = AccJournalMasterDto.JDateGregorian;
                }
                else
                {
                    return await Result<AccJournalReverseDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }
                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(AccJournalMasterDto.PeriodId ?? 0, AccJournalMasterDto.JDateGregorian) == false)
                {
                    return await Result<AccJournalReverseDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }



                AccJournalMasterDto.FacilityId = session.FacilityId;
                AccJournalMasterDto.StatusId = Status_Id;
                AccJournalMasterDto.FinYear = session.FinYear;
                AccJournalMasterDto.ReferenceNo = 0;
                AccJournalMasterDto.PaymentTypeId = 0;
                AccJournalMasterDto.BankId = 0;
                AccJournalMasterDto accJournalMaster = new()
                {

                    DocTypeId = 35,
                    StatusId = Status_Id,
                    FacilityId = session.FacilityId,
                    FinYear = session.FinYear,
                    ReferenceNo = AccJournalMasterDto.JId,
                    JDateGregorian = AccJournalMasterDto.JDateGregorian,
                    JDateHijri = AccJournalMasterDto.JDateGregorian,
                    Amount = AccJournalMasterDto.Amount,
                    AmountWrite = AccJournalMasterDto.AmountWrite,
                    JDescription = AccJournalMasterDto.JDescription += "قيد عكسي للقيد رقم" + " " + AccJournalMasterDto.JCode,
                    PaymentTypeId = AccJournalMasterDto.PaymentTypeId,
                    PeriodId = AccJournalMasterDto.PeriodId,
                    ReferenceCode = AccJournalMasterDto.JCode,
                    JBian = AccJournalMasterDto.JBian,
                    BankId = AccJournalMasterDto.BankId ?? 0,
                    CcId = AccJournalMasterDto.CcId,
                    CurrencyId = AccJournalMasterDto.CurrencyId,
                    ExchangeRate = AccJournalMasterDto.ExchangeRate,
                    ChequNo = AccJournalMasterDto.ChequNo,
                    ChequDateHijri = AccJournalMasterDto.ChequDateHijri,
                };
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var addJournalMaster = await accRepositoryManager.AccJournalMasterRepository.AddACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                if (!addJournalMaster.Succeeded)
                    return await Result<AccJournalReverseDto>.FailAsync(addJournalMaster.Status.message);


                entity.AccJournalMasterDto.JCode = addJournalMaster.Data.JCode;
                entity.AccJournalMasterDto.JId = addJournalMaster.Data.JId;
                long jId = addJournalMaster.Data.JId;

                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<AccJournalReverseDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");
                foreach (var child in entity.DetailsList)

                {

                    //-------------------------------- '--جاب  رقم حساب 

                    long AccAccountID = 0;

                    if (!string.IsNullOrEmpty(child.AccAccountCode))
                    {
                        AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                        if (AccAccountID == 0)
                        {
                            return await Result<AccJournalReverseDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                        }
                    }

                    //-------------------------------- '--جاب  رقم مرجع 

                    long ReferenceNo = 0;
                    ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                    if (ReferenceNo == 0)
                    {
                        return await Result<AccJournalReverseDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

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
                    //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                    var DefCurrency = await accRepositoryManager.AccAccountRepository.GetCuureny(0, session.FacilityId);

                    if (AccJournalMasterDto.CurrencyId != DefCurrency)
                    {
                        if (child.CurrencyId == AccJournalMasterDto.CurrencyId)
                        {
                            child.ExchangeRate = AccJournalMasterDto.ExchangeRate;
                        }
                        else
                        {
                            child.ExchangeRate = 1;
                        }
                    }
                    else
                    {
                        child.ExchangeRate = 1;
                    }
                    child.JId = jId;
                    child.JDetailesId = 0;
                    child.AccAccountId = AccAccountID;
                    child.ReferenceNo = ReferenceNo;
                    child.CcId = CCId;
                    child.Cc2Id = CCId2;
                    child.Cc3Id = CCId3;
                    child.Cc4Id = CCId4;
                    child.Cc5Id = CCId5;
                    var addJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(child, cancellationToken);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                    if (!addJournalDetail1.Succeeded)
                        return await Result<AccJournalReverseDto>.FailAsync(addJournalDetail1.Status.message);

                }
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);


                var entityMap = _mapper.Map<AccJournalReverseDto>(addJournalMaster.Data);
                return await Result<AccJournalReverseDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {
                return await Result<AccJournalReverseDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }
        public async Task<IResult<AccJournalReverseEditDto>> UpdateJournalReverse(AccExpensesMasterEditDtoVW entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccJournalReverseEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");


            try
            {


                var AccJournalMasterDto = entity.AccJournalMasterDto;
                //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين  
                decimal sumCredit = entity.DetailsList.Where(d => d.IsDeleted == false).Sum(x => x.Credit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);
                decimal sumDebit = entity.DetailsList.Where(d => d.IsDeleted == false).Sum(x => x.Debit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);

                if (sumCredit != sumDebit)
                {
                    return await Result<AccJournalReverseEditDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }



                //====التشييك على المرفقات في القيد في حال كانت الزامية
                string AttachmentMandatory = await sysConfigurationAppHelper.GetValue(171, session.FacilityId);
                if (AttachmentMandatory == "1")
                {
                    if (entity.FileDtos.Count() == 0)
                    {
                        return await Result<AccJournalReverseEditDto>.FailAsync($"{localization.GetAccResource("NotAbleAnyAction")}");

                    }
                }

                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(AccJournalMasterDto.PeriodId ?? 0, AccJournalMasterDto.JDateGregorian) == false)
                {
                    return await Result<AccJournalReverseEditDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }
                int? Status_Id = 0;
                //--------------------اضافة قيد موقت
                if (AccJournalMasterDto.StatusId != 5)
                {
                    Status_Id = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(session.FacilityId);

                }
                else
                {
                    Status_Id = AccJournalMasterDto.StatusId;
                }
                AccJournalMasterEditDto accJournalMaster = new()
                {
                    JId = AccJournalMasterDto.JId,
                    JCode = AccJournalMasterDto.JCode,
                    DocTypeId = 35,
                    StatusId = Status_Id,
                    FacilityId = session.FacilityId,
                    FinYear = session.FinYear,
                    ReferenceNo = AccJournalMasterDto.ReferenceNo,
                    JDateGregorian = AccJournalMasterDto.JDateGregorian,
                    JDateHijri = AccJournalMasterDto.JDateGregorian,
                    Amount = AccJournalMasterDto.Amount,
                    AmountWrite = AccJournalMasterDto.AmountWrite,
                    JDescription = AccJournalMasterDto.JDescription += "قيد عكسي للقيد رقم" + " " + AccJournalMasterDto.JCode,
                    PaymentTypeId = AccJournalMasterDto.PaymentTypeId,
                    PeriodId = AccJournalMasterDto.PeriodId,
                    ReferenceCode = AccJournalMasterDto.ReferenceCode,
                    JBian = AccJournalMasterDto.JBian,
                    BankId = AccJournalMasterDto.BankId ?? 0,
                    CcId = AccJournalMasterDto.CcId,
                    CurrencyId = AccJournalMasterDto.CurrencyId,
                    ExchangeRate = AccJournalMasterDto.ExchangeRate,
                    ChequNo = AccJournalMasterDto.ChequNo,
                    ChequDateHijri = AccJournalMasterDto.ChequDateHijri,
                };

                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.UpdateACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                if (!UpdateJournalMaster.Succeeded)
                    return await Result<AccJournalReverseEditDto>.FailAsync(UpdateJournalMaster.Status.message);


                entity.AccJournalMasterDto.JCode = UpdateJournalMaster.Data.JCode;
                entity.AccJournalMasterDto.JId = UpdateJournalMaster.Data.JId;
                long jId = UpdateJournalMaster.Data.JId;

                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<AccJournalReverseEditDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");
                foreach (var child in entity.DetailsList)
                {
                    //-------------------------------- '--جاب  رقم حساب 

                    long AccAccountID = 0;

                    if (!string.IsNullOrEmpty(child.AccAccountCode))
                    {
                        AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                        if (AccAccountID == 0)
                        {
                            return await Result<AccJournalReverseEditDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                        }
                    }



                    //-------------------------------- '--جاب  رقم مرجع 

                    long ReferenceNo = 0;
                    ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                    if (ReferenceNo == 0)
                    {
                        return await Result<AccJournalReverseEditDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

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
                    //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                    var DefCurrency = await accRepositoryManager.AccAccountRepository.GetCuureny(0, session.FacilityId);

                    if (AccJournalMasterDto.CurrencyId != DefCurrency)
                    {
                        if (child.CurrencyId == AccJournalMasterDto.CurrencyId)
                        {
                            child.ExchangeRate = AccJournalMasterDto.ExchangeRate;
                        }
                        else
                        {
                            child.ExchangeRate = 1;
                        }
                    }
                    else
                    {
                        child.ExchangeRate = 1;
                    }
                    if (child.JDetailesId == 0 && child.IsDeleted == false)
                    {
                        child.JId = jId;
                        child.AccAccountId = AccAccountID;
                        child.ReferenceNo = ReferenceNo;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        var addJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(child, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        if (!addJournalDetail1.Succeeded)
                            return await Result<AccJournalReverseEditDto>.FailAsync(addJournalDetail1.Status.message);
                    }
                    else
                    {
                        child.JId = jId;
                        child.AccAccountId = AccAccountID;
                        child.ReferenceNo = ReferenceNo;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        var UpdateJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.UpdateAccJournalDetail(child, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        if (!UpdateJournalDetail1.Succeeded)
                            return await Result<AccJournalReverseEditDto>.FailAsync(UpdateJournalDetail1.Status.message);

                    }




                }
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                var entityMap = _mapper.Map<AccJournalReverseEditDto>(UpdateJournalMaster.Data);


                return await Result<AccJournalReverseEditDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));

            }
            catch (Exception exc)
            {
                return await Result<AccJournalReverseEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }


        #endregion Journal Reverse
        //===============================الترحيل للقيود
        #region Journal Transfer  To
        public async Task<IResult<AccJournalMasterStatusDto>> UpdateTransferGeneralTo(AccJournalMasterStatusDto accJournalMasterDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (accJournalMasterDto == null) return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");
                if (await accRepositoryManager.AccFinancialYearRepository.CheckFinyearStatus(session.FinYear) != 1)
                    return await Result<AccJournalMasterStatusDto>.SuccessAsync(accJournalMasterDto, localization.GetResource1("theclosureyear"));
                string TransfertoGeneralFacilityID = "";
                TransfertoGeneralFacilityID = await sysConfigurationAppHelper.GetValue(332, session.FacilityId);
                string massErorr = "";
                var selctedIdsarr = accJournalMasterDto.SelectedJId.Split(',');
                long count = 0;
                long XCount = 0;
                long RCount = 0;


                if (TransfertoGeneralFacilityID != "1")
                {
                    await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

                    foreach (var Jid in selctedIdsarr)
                    {
                        var itemMaster = await accRepositoryManager.AccJournalMasterRepository.GetById(Convert.ToInt64(Jid));

                        if (itemMaster != null)
                        {
                            var JournalDetaile = await accRepositoryManager.AccJournalDetaileRepository.SelectACCJournalDetailesFacilityByID(Convert.ToInt64(Jid));
                            //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين  
                            decimal sumCredit = JournalDetaile.Where(d => d.IsDeleted == false).Sum(x => x.Credit) ?? 0;
                            decimal sumDebit = JournalDetaile.Where(d => d.IsDeleted == false).Sum(x => x.Debit) ?? 0;

                            if (sumCredit == sumDebit || itemMaster.DocTypeId == 4)
                            {

                                AccJournalMasterEditDto accJournalMaster = new()
                                {
                                    JId = itemMaster.JId,
                                    StatusId = accJournalMasterDto.StatusId,


                                };
                                var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.TransferOprations(accJournalMaster, cancellationToken);
                                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                                XCount += 1;
                            }
                            else
                            {
                                RCount += 1;
                            }


                        }
                    }
                    if (XCount > 0)
                    {
                        await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                        return await Result<AccJournalMasterStatusDto>.SuccessAsync($"{localization.GetResource1("ActionSuccess")}");

                    }
                    else
                    {
                        if (RCount > 0)
                        {
                            return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetResource1("RowColorNotAbleApproce")}");

                        }
                        else
                        {
                            return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetResource1("SelectRecord")}");

                        }
                    }
                }
                else
                {
                    await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

                    foreach (var Jid in selctedIdsarr)
                    {
                        var itemMaster = await accRepositoryManager.AccJournalMasterRepository.GetById(Convert.ToInt64(Jid));

                        if (itemMaster != null)
                        {
                            var JournalDetaile1 = await accRepositoryManager.AccJournalDetaileRepository.SelectACCJournalDetailesFacilityByID(Convert.ToInt64(Jid));
                            //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين  
                            decimal sumCredit1 = JournalDetaile1.Where(d => d.IsDeleted == false).Sum(x => x.Credit) ?? 0;
                            decimal sumDebit1 = JournalDetaile1.Where(d => d.IsDeleted == false).Sum(x => x.Debit) ?? 0;

                            if (sumCredit1 == sumDebit1 || itemMaster.DocTypeId == 4)
                            {
                                AccJournalMasterEditDto accJournalMaster = new()
                                {
                                    JId = itemMaster.JId,
                                    StatusId = accJournalMasterDto.StatusId,


                                };
                                var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.TransferOprations(accJournalMaster, cancellationToken);
                                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken); ;

                                XCount += 1;
                            }
                            else
                            {
                                RCount += 1;
                            }


                        }


                        var JournalMaster = await accRepositoryManager.AccJournalMasterRepository.SelectACCJournalFacilityByID(Convert.ToInt64(Jid));
                        long JIDJornal = 0;
                        int DocTypeIDJornal = 0;
                        if (JournalMaster != null)
                        {


                            JIDJornal = JournalMaster.JId;
                            DocTypeIDJornal = JournalMaster.DocTypeId ?? 0;
                            var JournalDetaile = await accRepositoryManager.AccJournalDetaileRepository.SelectACCJournalDetailesFacilityByID(JIDJornal);
                            //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين  
                            decimal sumCredit = JournalDetaile.Where(d => d.IsDeleted == false).Sum(x => x.Credit) ?? 0;
                            decimal sumDebit = JournalDetaile.Where(d => d.IsDeleted == false).Sum(x => x.Debit) ?? 0;

                            if (sumCredit == sumDebit || DocTypeIDJornal == 4)
                            {
                                AccJournalMasterEditDto accJournalMaster = new()
                                {
                                    JId = JIDJornal,
                                    StatusId = accJournalMasterDto.StatusId,


                                };
                                var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.TransferOprations(accJournalMaster, cancellationToken);
                                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                                XCount += 1;
                            }
                            else
                            {
                                RCount += 1;
                            }
                        }
                    }
                    if (RCount == 0)
                    {
                        await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                    }
                    else
                    {

                        XCount = 0;
                    }
                    if (XCount > 0)
                    {
                        return await Result<AccJournalMasterStatusDto>.SuccessAsync($"{localization.GetResource1("ActionSuccess")}");

                    }
                    else
                    {
                        if (RCount > 0)
                        {
                            return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetResource1("RowColorNotAbleApproce")}");

                        }
                        else
                        {
                            return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetResource1("SelectRecord")}");

                        }
                    }
                }


            }
            catch (Exception exc)
            {
                return await Result<AccJournalMasterStatusDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }
        }
        public async Task<IResult<AccJournalMasterStatusDto>> UpdateTransferGeneralAllTo(AccJournalMasterStatusDto accJournalMasterDto, CancellationToken cancellationToken = default)
        {
            try
            {

                if (accJournalMasterDto == null) return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");
                if (await accRepositoryManager.AccFinancialYearRepository.CheckFinyearStatus(session.FinYear) != 1)
                    return await Result<AccJournalMasterStatusDto>.SuccessAsync(accJournalMasterDto, localization.GetResource1("theclosureyear"));
                string massErorr = "";
                var selctedIdsarr = accJournalMasterDto.SelectedJId.Split(',');
                var JournalIds = await accRepositoryManager.AccJournalDetaileRepository.GetJournalIds(accJournalMasterDto.SelectedJId, session.FacilityId);
                int count = 0;
                int XCount = 0;
                int RCount = 0;
                foreach (var Jid in JournalIds)
                {
                    var itemMaster = await accRepositoryManager.AccJournalMasterRepository.GetById(Jid);

                    if (itemMaster != null)
                    {

                        AccJournalMasterEditDto accJournalMaster = new()
                        {
                            JId = itemMaster.JId,
                            StatusId = accJournalMasterDto.StatusId,


                        };
                        var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.TransferOprations(accJournalMaster, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken); ;

                        XCount += 1;
                    }



                }
                if (XCount > 0)
                {
                    accJournalMasterDto.Count = XCount;
                    return await Result<AccJournalMasterStatusDto>.SuccessAsync($"{localization.GetResource1("ActionSuccess")}");

                }
                else
                {

                    return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetResource1("SelectRecord")}");


                }



            }
            catch (Exception exc)
            {
                return await Result<AccJournalMasterStatusDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }
        }

        public async Task<IResult<AccJournalMasterStatusDto>> UpdateACCJournalComment(AccJournalMasterStatusDto accJournalMasterDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (accJournalMasterDto == null) return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");

                string massErorr = "";
                var selctedIdsarr = accJournalMasterDto.SelectedJId.Split(',');
                long count = 0;
                foreach (var Jid in selctedIdsarr)
                {
                    var itemMaster = await accRepositoryManager.AccJournalMasterRepository.GetById(Convert.ToInt64(Jid));

                    if (itemMaster != null)
                    {
                        await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                        AccJournalCommentDto AccJournalComment = new()
                        {
                            JId = itemMaster.JId,
                            Note = accJournalMasterDto.Note,
                            CreatedBy = session.UserId,
                            Date1 = accJournalMasterDto.Note,

                        };
                        var AddAccJournalComment = _mapper.Map<AccJournalComment>(AccJournalComment);
                        var JournalComment = await accRepositoryManager.AccJournalCommentRepository.AddAndReturn(AddAccJournalComment);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        itemMaster.StatusId = accJournalMasterDto.StatusId;
                        itemMaster.UpdateDate = DateTime.UtcNow;
                        itemMaster.UpdateUserId = (int)session.UserId;
                        accRepositoryManager.AccJournalMasterRepository.Update(itemMaster);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        SysNotificationDto sysNotification = new()
                        {
                            UserId = itemMaster.InsertUserId,
                            Url = "/Apps/Accounting/GL/Journal_Auto/Journal_Auto_Edit?ID=" + Jid,
                            CreatedBy = session.UserId,
                            MsgTxt = accJournalMasterDto.Note,

                        };
                        var AddNotification = _mapper.Map<SysNotification>(sysNotification);
                        var Notification = await mainRepositoryManager.SysNotificationRepository.AddAndReturn(AddNotification);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);
                        count += 1;
                    }

                    else
                    {
                        massErorr += Jid + ",";
                    }
                }
                accJournalMasterDto.Count = count;
                return await Result<AccJournalMasterStatusDto>.SuccessAsync(accJournalMasterDto, localization.GetMessagesResource("success"));

            }
            catch (Exception exc)
            {
                return await Result<AccJournalMasterStatusDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }
        }

        #endregion Journal Transfer To
        //===============================الغاء الترحيل للقيود من الاستاذ العام
        #region Journal Transfer  From
        public async Task<IResult<AccJournalMasterStatusDto>> UpdateTransferGeneralFrom(AccJournalMasterStatusDto accJournalMasterDto, CancellationToken cancellationToken = default)
        {
            try
            {
                if (accJournalMasterDto == null) return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");
                if (await accRepositoryManager.AccFinancialYearRepository.CheckFinyearStatus(session.FinYear) != 1)
                    return await Result<AccJournalMasterStatusDto>.SuccessAsync(accJournalMasterDto, localization.GetResource1("theclosureyear"));
                string TransfertoGeneralFacilityID = "";
                TransfertoGeneralFacilityID = await sysConfigurationAppHelper.GetValue(332, session.FacilityId);
                string massErorr = "";
                var selctedIdsarr = accJournalMasterDto.SelectedJId.Split(',');
                long count = 0;
                long XCount = 0;
                long RCount = 0;


                if (TransfertoGeneralFacilityID != "1")
                {
                    await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

                    foreach (var Jid in selctedIdsarr)
                    {

                        AccJournalMasterEditDto accJournalMaster = new()
                        {
                            JId = Convert.ToInt64(Jid),
                            StatusId = accJournalMasterDto.StatusId,
                        };
                        var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.TransferOprations(accJournalMaster, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                        XCount += 1;


                    }
                    if (XCount > 0)
                    {
                        await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                        return await Result<AccJournalMasterStatusDto>.SuccessAsync($"{localization.GetResource1("ActionSuccess")}");

                    }
                    else
                    {
                        if (RCount > 0)
                        {
                            return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetResource1("RowColorNotAbleApproce")}");

                        }
                        else
                        {
                            return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetResource1("SelectRecord")}");

                        }
                    }
                }
                else
                {
                    await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

                    foreach (var Jid in selctedIdsarr)
                    {
                        AccJournalMasterEditDto accJournalMaster = new()
                        {
                            JId = Convert.ToInt64(Jid),
                            StatusId = accJournalMasterDto.StatusId,


                        };
                        var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.TransferOprations(accJournalMaster, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken); ;
                        if (Convert.ToInt64(Jid) > 0)
                        {
                            XCount += 1;
                        }


                        var JournalMaster = await accRepositoryManager.AccJournalMasterRepository.SelectACCJournalFacilityByID(Convert.ToInt64(Jid));
                        long JIDJornal = 0;
                        int DocTypeIDJornal = 0;
                        if (JournalMaster != null)
                        {
                            JIDJornal = JournalMaster.JId;
                            DocTypeIDJornal = JournalMaster.DocTypeId ?? 0;
                            AccJournalMasterEditDto accJournalMaster2 = new()
                            {
                                JId = JIDJornal,
                                StatusId = accJournalMasterDto.StatusId,
                            };
                            var UpdateJournalMaster2 = await accRepositoryManager.AccJournalMasterRepository.TransferOprations(accJournalMaster2, cancellationToken);
                            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);



                        }
                    }

                    if (XCount > 0)
                    {
                        accJournalMasterDto.Count = XCount;
                        return await Result<AccJournalMasterStatusDto>.SuccessAsync($"{localization.GetResource1("ActionSuccess")}");

                    }
                    else
                    {

                        return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetResource1("SelectRecord")}");


                    }


                }


            }
            catch (Exception exc)
            {
                return await Result<AccJournalMasterStatusDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }
        }
        public async Task<IResult<AccJournalMasterStatusDto>> UpdateTransferGeneralAllFrom(AccJournalMasterStatusDto accJournalMasterDto, CancellationToken cancellationToken = default)
        {
            try
            {

                if (accJournalMasterDto == null) return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");
                if (await accRepositoryManager.AccFinancialYearRepository.CheckFinyearStatus(session.FinYear) != 1)
                    return await Result<AccJournalMasterStatusDto>.SuccessAsync(accJournalMasterDto, localization.GetResource1("theclosureyear"));
                string massErorr = "";
                var selctedIdsarr = accJournalMasterDto.SelectedJId.Split(',');
                int count = 0;
                int XCount = 0;
                int RCount = 0;
                foreach (var Jid in selctedIdsarr)
                {


                    AccJournalMasterEditDto accJournalMaster = new()
                    {
                        JId = Convert.ToInt64(Jid),
                        StatusId = accJournalMasterDto.StatusId,


                    };
                    var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.TransferOprations(accJournalMaster, cancellationToken);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken); ;

                    XCount += 1;


                }
                if (XCount > 0)
                {
                    accJournalMasterDto.Count = XCount;
                    return await Result<AccJournalMasterStatusDto>.SuccessAsync($"{localization.GetResource1("ActionSuccess")}");

                }
                else
                {

                    return await Result<AccJournalMasterStatusDto>.FailAsync($"{localization.GetResource1("SelectRecord")}");


                }



            }
            catch (Exception exc)
            {
                return await Result<AccJournalMasterStatusDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }
        }


        #endregion Journal Transfer  c
        //--------------------- رصيد أول المدة
        #region First Time Balances
        public async Task<IResult<FirstTimeBalanceDto>> AddFirstTimeBalances(FirstTimeBalanceDtoVW entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<FirstTimeBalanceDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {

                //====التشييك على المرفقات في القيد في حال كانت الزامية

                string AttachmentMandatory = await sysConfigurationAppHelper.GetValue(171, session.FacilityId);
                if (AttachmentMandatory == "1")
                {
                    if (entity.FileDtos.Count() == 0)
                    {
                        return await Result<FirstTimeBalanceDto>.FailAsync($"{localization.GetAccResource("AttachmentMandatory")}");

                    }
                }
                var AccJournalMasterDto = entity.AccJournalMasterDto;

                //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين 
                decimal sumCredit = entity.DetailsList.Sum(x => x.Credit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);
                decimal sumDebit = entity.DetailsList.Sum(x => x.Debit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);

                if (sumCredit != sumDebit)
                {
                    return await Result<FirstTimeBalanceDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }
                int? Status_Id = 0;
                //--------------------اضافة قيد موقت
                if (AccJournalMasterDto.StatusId != 5)
                {
                    Status_Id = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(session.FacilityId);

                }
                else
                {
                    Status_Id = AccJournalMasterDto.StatusId;
                }
                //------------------------تشيك التاريخ
                if (await DateHelper.CheckDate(AccJournalMasterDto.JDateGregorian, session.FacilityId, session.CalendarType) == true)
                {
                    AccJournalMasterDto.JDateGregorian = AccJournalMasterDto.JDateGregorian;
                    AccJournalMasterDto.JDateHijri = AccJournalMasterDto.JDateGregorian;
                }
                else
                {
                    return await Result<FirstTimeBalanceDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }
                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(AccJournalMasterDto.PeriodId ?? 0, AccJournalMasterDto.JDateGregorian) == false)
                {
                    return await Result<FirstTimeBalanceDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }



                AccJournalMasterDto.FacilityId = session.FacilityId;
                AccJournalMasterDto.StatusId = Status_Id;
                AccJournalMasterDto.FinYear = session.FinYear;
                AccJournalMasterDto.ReferenceNo = 0;
                AccJournalMasterDto.PaymentTypeId = 0;
                AccJournalMasterDto.BankId = 0;
                AccJournalMasterDto accJournalMaster = new()
                {

                    DocTypeId = 27,
                    StatusId = Status_Id,
                    FacilityId = session.FacilityId,
                    FinYear = session.FinYear,
                    ReferenceNo = AccJournalMasterDto.JId,
                    JDateGregorian = AccJournalMasterDto.JDateGregorian,
                    JDateHijri = AccJournalMasterDto.JDateGregorian,
                    Amount = AccJournalMasterDto.Amount,
                    AmountWrite = AccJournalMasterDto.AmountWrite,
                    JDescription = AccJournalMasterDto.JDescription += "رصيد إفتتاحي أول المدة",
                    PaymentTypeId = AccJournalMasterDto.PaymentTypeId,
                    PeriodId = AccJournalMasterDto.PeriodId,
                    ReferenceCode = AccJournalMasterDto.JCode,
                    JBian = AccJournalMasterDto.JBian,
                    BankId = AccJournalMasterDto.BankId ?? 0,
                    CcId = AccJournalMasterDto.CcId,
                    CurrencyId = AccJournalMasterDto.CurrencyId,
                    ExchangeRate = AccJournalMasterDto.ExchangeRate,
                    ChequNo = AccJournalMasterDto.ChequNo,
                    ChequDateHijri = AccJournalMasterDto.ChequDateHijri,
                };
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var addJournalMaster = await accRepositoryManager.AccJournalMasterRepository.AddACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                if (!addJournalMaster.Succeeded)
                    return await Result<FirstTimeBalanceDto>.FailAsync(addJournalMaster.Status.message);


                entity.AccJournalMasterDto.JCode = addJournalMaster.Data.JCode;
                entity.AccJournalMasterDto.JId = addJournalMaster.Data.JId;
                long jId = addJournalMaster.Data.JId;

                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<FirstTimeBalanceDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");
                foreach (var child in entity.DetailsList)

                {

                    //-------------------------------- '--جاب  رقم حساب 

                    long AccAccountID = 0;

                    if (!string.IsNullOrEmpty(child.AccAccountCode))
                    {
                        AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                        if (AccAccountID == 0)
                        {
                            return await Result<FirstTimeBalanceDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                        }
                    }

                    //-------------------------------- '--جاب  رقم مرجع 

                    long ReferenceNo = 0;
                    ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                    if (ReferenceNo == 0)
                    {
                        return await Result<FirstTimeBalanceDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

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
                    //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                    var DefCurrency = await accRepositoryManager.AccAccountRepository.GetCuureny(0, session.FacilityId);

                    if (AccJournalMasterDto.CurrencyId != DefCurrency)
                    {
                        if (child.CurrencyId == AccJournalMasterDto.CurrencyId)
                        {
                            child.ExchangeRate = AccJournalMasterDto.ExchangeRate;
                        }
                        else
                        {
                            child.ExchangeRate = 1;
                        }
                    }
                    else
                    {
                        child.ExchangeRate = 1;
                    }
                    child.JId = jId;
                    child.JDetailesId = 0;
                    child.AccAccountId = AccAccountID;
                    child.ReferenceNo = ReferenceNo;
                    child.CcId = CCId;
                    child.Cc2Id = CCId2;
                    child.Cc3Id = CCId3;
                    child.Cc4Id = CCId4;
                    child.Cc5Id = CCId5;
                    var addJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(child, cancellationToken);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                    if (!addJournalDetail1.Succeeded)
                        return await Result<FirstTimeBalanceDto>.FailAsync(addJournalDetail1.Status.message);

                }
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);


                var entityMap = _mapper.Map<FirstTimeBalanceDto>(addJournalMaster.Data);
                return await Result<FirstTimeBalanceDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {
                return await Result<FirstTimeBalanceDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }
        public async Task<IResult<FirstTimeBalanceEditDto>> UpdateFirstTimeBalance(FirstTimeBalanceEditDtoVW entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<FirstTimeBalanceEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");


            try
            {


                var AccJournalMasterDto = entity.AccJournalMasterDto;
                //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين  
                decimal sumCredit = entity.DetailsList.Where(d => d.IsDeleted == false).Sum(x => x.Credit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);
                decimal sumDebit = entity.DetailsList.Where(d => d.IsDeleted == false).Sum(x => x.Debit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);

                if (sumCredit != sumDebit)
                {
                    return await Result<FirstTimeBalanceEditDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }



                //====التشييك على المرفقات في القيد في حال كانت الزامية
                string AttachmentMandatory = await sysConfigurationAppHelper.GetValue(171, session.FacilityId);
                if (AttachmentMandatory == "1")
                {
                    if (entity.FileDtos.Count() == 0)
                    {
                        return await Result<FirstTimeBalanceEditDto>.FailAsync($"{localization.GetAccResource("NotAbleAnyAction")}");

                    }
                }

                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(AccJournalMasterDto.PeriodId ?? 0, AccJournalMasterDto.JDateGregorian) == false)
                {
                    return await Result<FirstTimeBalanceEditDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }
                int? Status_Id = 0;
                //--------------------اضافة قيد موقت
                if (AccJournalMasterDto.StatusId != 5)
                {
                    Status_Id = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(session.FacilityId);

                }
                else
                {
                    Status_Id = AccJournalMasterDto.StatusId;
                }
                AccJournalMasterEditDto accJournalMaster = new()
                {
                    JId = AccJournalMasterDto.JId,
                    JCode = AccJournalMasterDto.JCode,
                    DocTypeId = 27,
                    StatusId = Status_Id,
                    FacilityId = session.FacilityId,
                    FinYear = session.FinYear,
                    ReferenceNo = AccJournalMasterDto.ReferenceNo,
                    JDateGregorian = AccJournalMasterDto.JDateGregorian,
                    JDateHijri = AccJournalMasterDto.JDateGregorian,
                    Amount = AccJournalMasterDto.Amount,
                    AmountWrite = AccJournalMasterDto.AmountWrite,
                    JDescription = AccJournalMasterDto.JDescription,
                    PaymentTypeId = AccJournalMasterDto.PaymentTypeId,
                    PeriodId = AccJournalMasterDto.PeriodId,
                    ReferenceCode = AccJournalMasterDto.ReferenceCode,
                    JBian = AccJournalMasterDto.JBian,
                    BankId = AccJournalMasterDto.BankId ?? 0,
                    CcId = AccJournalMasterDto.CcId,
                    CurrencyId = AccJournalMasterDto.CurrencyId,
                    ExchangeRate = AccJournalMasterDto.ExchangeRate,
                    ChequNo = AccJournalMasterDto.ChequNo,
                    ChequDateHijri = AccJournalMasterDto.ChequDateHijri,

                };

                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.UpdateACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                if (!UpdateJournalMaster.Succeeded)
                    return await Result<FirstTimeBalanceEditDto>.FailAsync(UpdateJournalMaster.Status.message);


                entity.AccJournalMasterDto.JCode = UpdateJournalMaster.Data.JCode;
                entity.AccJournalMasterDto.JId = UpdateJournalMaster.Data.JId;
                long jId = UpdateJournalMaster.Data.JId;

                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<FirstTimeBalanceEditDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");
                foreach (var child in entity.DetailsList)
                {
                    //-------------------------------- '--جاب  رقم حساب 

                    long AccAccountID = 0;

                    if (!string.IsNullOrEmpty(child.AccAccountCode))
                    {
                        AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                        if (AccAccountID == 0)
                        {
                            return await Result<FirstTimeBalanceEditDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                        }
                    }



                    //-------------------------------- '--جاب  رقم مرجع 

                    long ReferenceNo = 0;
                    ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                    if (ReferenceNo == 0)
                    {
                        return await Result<FirstTimeBalanceEditDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

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
                    //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                    var DefCurrency = await accRepositoryManager.AccAccountRepository.GetCuureny(0, session.FacilityId);

                    if (AccJournalMasterDto.CurrencyId != DefCurrency)
                    {
                        if (child.CurrencyId == AccJournalMasterDto.CurrencyId)
                        {
                            child.ExchangeRate = AccJournalMasterDto.ExchangeRate;
                        }
                        else
                        {
                            child.ExchangeRate = 1;
                        }
                    }
                    else
                    {
                        child.ExchangeRate = 1;
                    }
                    if (child.JDetailesId == 0 && child.IsDeleted == false)
                    {
                        child.JId = jId;
                        child.AccAccountId = AccAccountID;
                        child.ReferenceNo = ReferenceNo;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        var addJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(child, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        if (!addJournalDetail1.Succeeded)
                            return await Result<FirstTimeBalanceEditDto>.FailAsync(addJournalDetail1.Status.message);
                    }
                    else
                    {
                        child.JId = jId;
                        child.AccAccountId = AccAccountID;
                        child.ReferenceNo = ReferenceNo;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        var UpdateJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.UpdateAccJournalDetail(child, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        if (!UpdateJournalDetail1.Succeeded)
                            return await Result<FirstTimeBalanceEditDto>.FailAsync(UpdateJournalDetail1.Status.message);

                    }




                }
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                var entityMap = _mapper.Map<FirstTimeBalanceEditDto>(UpdateJournalMaster.Data);


                return await Result<FirstTimeBalanceEditDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));

            }
            catch (Exception exc)
            {
                return await Result<FirstTimeBalanceEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }


        #endregion First Time Balances
        //---------------------  الرصيد الإفتتاحي
        #region Opening Balance
        public async Task<IResult<OpeningBalanceDto>> AddOpeningBalance(OpeningBalanceDtoVW entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<OpeningBalanceDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {


                var AccJournalMasterDto = entity.AccJournalMasterDto;

                //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين 
                decimal sumCredit = entity.DetailsList.Sum(x => x.Credit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);
                decimal sumDebit = entity.DetailsList.Sum(x => x.Debit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);

                if (sumCredit != sumDebit)
                {
                    return await Result<OpeningBalanceDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }
                int? Status_Id = 0;
                //--------------------اضافة قيد موقت
                if (AccJournalMasterDto.StatusId != 5)
                {
                    Status_Id = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(session.FacilityId);

                }
                else
                {
                    Status_Id = AccJournalMasterDto.StatusId;
                }
                //------------------------تشيك التاريخ
                if (await DateHelper.CheckDate(AccJournalMasterDto.JDateGregorian, session.FacilityId, session.CalendarType) == true)
                {
                    AccJournalMasterDto.JDateGregorian = AccJournalMasterDto.JDateGregorian;
                    AccJournalMasterDto.JDateHijri = AccJournalMasterDto.JDateGregorian;
                }
                else
                {
                    return await Result<OpeningBalanceDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }
                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(AccJournalMasterDto.PeriodId ?? 0, AccJournalMasterDto.JDateGregorian) == false)
                {
                    return await Result<OpeningBalanceDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }



                AccJournalMasterDto.FacilityId = session.FacilityId;
                AccJournalMasterDto.StatusId = Status_Id;
                AccJournalMasterDto.FinYear = session.FinYear;
                AccJournalMasterDto.ReferenceNo = 0;
                AccJournalMasterDto.PaymentTypeId = 0;
                AccJournalMasterDto.BankId = 0;
                AccJournalMasterDto accJournalMaster = new()
                {

                    DocTypeId = 4,
                    StatusId = Status_Id,
                    FacilityId = session.FacilityId,
                    FinYear = session.FinYear,
                    ReferenceNo = AccJournalMasterDto.JId,
                    JDateGregorian = AccJournalMasterDto.JDateGregorian,
                    JDateHijri = AccJournalMasterDto.JDateGregorian,
                    Amount = AccJournalMasterDto.Amount,
                    AmountWrite = AccJournalMasterDto.AmountWrite,
                    JDescription = AccJournalMasterDto.JDescription += "رصيد إفتتاحي",
                    PaymentTypeId = AccJournalMasterDto.PaymentTypeId,
                    PeriodId = AccJournalMasterDto.PeriodId,
                    ReferenceCode = AccJournalMasterDto.JCode,
                    JBian = AccJournalMasterDto.JBian,
                    BankId = AccJournalMasterDto.BankId ?? 0,
                    CcId = AccJournalMasterDto.CcId,
                    CurrencyId = AccJournalMasterDto.CurrencyId,
                    ExchangeRate = AccJournalMasterDto.ExchangeRate,
                    ChequNo = AccJournalMasterDto.ChequNo,
                    ChequDateHijri = AccJournalMasterDto.ChequDateHijri,
                };
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var addJournalMaster = await accRepositoryManager.AccJournalMasterRepository.AddACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                if (!addJournalMaster.Succeeded)
                    return await Result<OpeningBalanceDto>.FailAsync(addJournalMaster.Status.message);


                entity.AccJournalMasterDto.JCode = addJournalMaster.Data.JCode;
                entity.AccJournalMasterDto.JId = addJournalMaster.Data.JId;
                long jId = addJournalMaster.Data.JId;

                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<OpeningBalanceDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");
                foreach (var child in entity.DetailsList)

                {

                    //-------------------------------- '--جاب  رقم حساب 

                    long AccAccountID = 0;

                    if (!string.IsNullOrEmpty(child.AccAccountCode))
                    {
                        AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                        if (AccAccountID == 0)
                        {
                            return await Result<OpeningBalanceDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                        }
                    }

                    //-------------------------------- '--جاب  رقم مرجع 

                    long ReferenceNo = 0;
                    ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                    if (ReferenceNo == 0)
                    {
                        return await Result<OpeningBalanceDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

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
                    //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                    var DefCurrency = await accRepositoryManager.AccAccountRepository.GetCuureny(0, session.FacilityId);

                    if (AccJournalMasterDto.CurrencyId != DefCurrency)
                    {
                        if (child.CurrencyId == AccJournalMasterDto.CurrencyId)
                        {
                            child.ExchangeRate = AccJournalMasterDto.ExchangeRate;
                        }
                        else
                        {
                            child.ExchangeRate = 1;
                        }
                    }
                    else
                    {
                        child.ExchangeRate = 1;
                    }
                    child.JId = jId;
                    child.JDetailesId = 0;
                    child.AccAccountId = AccAccountID;
                    child.ReferenceNo = ReferenceNo;
                    child.CcId = CCId;
                    child.Cc2Id = CCId2;
                    child.Cc3Id = CCId3;
                    child.Cc4Id = CCId4;
                    child.Cc5Id = CCId5;
                    var addJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(child, cancellationToken);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                    if (!addJournalDetail1.Succeeded)
                        return await Result<OpeningBalanceDto>.FailAsync(addJournalDetail1.Status.message);

                }
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);


                var entityMap = _mapper.Map<OpeningBalanceDto>(addJournalMaster.Data);
                return await Result<OpeningBalanceDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {
                return await Result<OpeningBalanceDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }
        public async Task<IResult<OpeningBalanceEditDto>> UpdateOpeningBalance(OpeningBalanceEditDtoVW entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<OpeningBalanceEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");


            try
            {


                var AccJournalMasterDto = entity.AccJournalMasterDto;
                //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين  
                decimal sumCredit = entity.DetailsList.Where(d => d.IsDeleted == false).Sum(x => x.Credit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);
                decimal sumDebit = entity.DetailsList.Where(d => d.IsDeleted == false).Sum(x => x.Debit) ?? 0 * (AccJournalMasterDto.ExchangeRate ?? 0);

                if (sumCredit != sumDebit)
                {
                    return await Result<OpeningBalanceEditDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }





                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(AccJournalMasterDto.PeriodId ?? 0, AccJournalMasterDto.JDateGregorian) == false)
                {
                    return await Result<OpeningBalanceEditDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }
                int? Status_Id = 0;
                //--------------------اضافة قيد موقت
                if (AccJournalMasterDto.StatusId != 5)
                {
                    Status_Id = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(session.FacilityId);

                }
                else
                {
                    Status_Id = AccJournalMasterDto.StatusId;
                }
                AccJournalMasterEditDto accJournalMaster = new()
                {
                    JId = AccJournalMasterDto.JId,
                    JCode = AccJournalMasterDto.JCode,
                    DocTypeId = 4,
                    StatusId = Status_Id,
                    FacilityId = session.FacilityId,
                    FinYear = session.FinYear,
                    ReferenceNo = AccJournalMasterDto.ReferenceNo,
                    JDateGregorian = AccJournalMasterDto.JDateGregorian,
                    JDateHijri = AccJournalMasterDto.JDateGregorian,
                    Amount = AccJournalMasterDto.Amount,
                    AmountWrite = AccJournalMasterDto.AmountWrite,
                    JDescription = AccJournalMasterDto.JDescription,
                    PaymentTypeId = AccJournalMasterDto.PaymentTypeId,
                    PeriodId = AccJournalMasterDto.PeriodId,
                    ReferenceCode = AccJournalMasterDto.ReferenceCode,
                    JBian = AccJournalMasterDto.JBian,
                    BankId = AccJournalMasterDto.BankId ?? 0,
                    CcId = AccJournalMasterDto.CcId,
                    CurrencyId = AccJournalMasterDto.CurrencyId,
                    ExchangeRate = AccJournalMasterDto.ExchangeRate,
                    ChequNo = AccJournalMasterDto.ChequNo,
                    ChequDateHijri = AccJournalMasterDto.ChequDateHijri,

                };

                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var UpdateJournalMaster = await accRepositoryManager.AccJournalMasterRepository.UpdateACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                if (!UpdateJournalMaster.Succeeded)
                    return await Result<OpeningBalanceEditDto>.FailAsync(UpdateJournalMaster.Status.message);


                entity.AccJournalMasterDto.JCode = UpdateJournalMaster.Data.JCode;
                entity.AccJournalMasterDto.JId = UpdateJournalMaster.Data.JId;
                long jId = UpdateJournalMaster.Data.JId;

                if (entity.DetailsList == null || entity.DetailsList.Count == 0) return await Result<OpeningBalanceEditDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");
                foreach (var child in entity.DetailsList)
                {
                    //-------------------------------- '--جاب  رقم حساب 

                    long AccAccountID = 0;

                    if (!string.IsNullOrEmpty(child.AccAccountCode))
                    {
                        AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                        if (AccAccountID == 0)
                        {
                            return await Result<OpeningBalanceEditDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                        }
                    }



                    //-------------------------------- '--جاب  رقم مرجع 

                    long ReferenceNo = 0;
                    ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(child.ReferenceTypeId ?? 0, child.AccAccountCode);

                    if (ReferenceNo == 0)
                    {
                        return await Result<OpeningBalanceEditDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

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
                    //-------------------------------------- الدالة تاتي في العملة الرئسية للنظام في حالة ارسال رقم الحساب 
                    var DefCurrency = await accRepositoryManager.AccAccountRepository.GetCuureny(0, session.FacilityId);

                    if (AccJournalMasterDto.CurrencyId != DefCurrency)
                    {
                        if (child.CurrencyId == AccJournalMasterDto.CurrencyId)
                        {
                            child.ExchangeRate = AccJournalMasterDto.ExchangeRate;
                        }
                        else
                        {
                            child.ExchangeRate = 1;
                        }
                    }
                    else
                    {
                        child.ExchangeRate = 1;
                    }
                    if (child.JDetailesId == 0 && child.IsDeleted == false)
                    {
                        child.JId = jId;
                        child.AccAccountId = AccAccountID;
                        child.ReferenceNo = ReferenceNo;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        var addJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(child, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        if (!addJournalDetail1.Succeeded)
                            return await Result<OpeningBalanceEditDto>.FailAsync(addJournalDetail1.Status.message);
                    }
                    else
                    {
                        child.JId = jId;
                        child.AccAccountId = AccAccountID;
                        child.ReferenceNo = ReferenceNo;
                        child.CcId = CCId;
                        child.Cc2Id = CCId2;
                        child.Cc3Id = CCId3;
                        child.Cc4Id = CCId4;
                        child.Cc5Id = CCId5;
                        var UpdateJournalDetail1 = await accRepositoryManager.AccJournalDetaileRepository.UpdateAccJournalDetail(child, cancellationToken);
                        await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        if (!UpdateJournalDetail1.Succeeded)
                            return await Result<OpeningBalanceEditDto>.FailAsync(UpdateJournalDetail1.Status.message);

                    }




                }
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, jId, 4);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                var entityMap = _mapper.Map<OpeningBalanceEditDto>(UpdateJournalMaster.Data);


                return await Result<OpeningBalanceEditDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));

            }
            catch (Exception exc)
            {
                return await Result<OpeningBalanceEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }




        #endregion Opening Balance

        public async Task<long> GetJIDByJCode2(string JCode, int DocTypeId, long facilityId, long Finyear)
        {

            long JID = 0;
            JID = await accRepositoryManager.AccJournalMasterRepository.GetJIDByJCode2(JCode, DocTypeId, facilityId, Finyear);

            return JID;
        }
        public async Task<string?> GetJCodeByReferenceNo(long ReferenceNo, int DocTypeID)
        {
            string JCode = "";

            var journal = await accRepositoryManager.AccJournalMasterRepository.GetJCodeByReferenceNo(ReferenceNo, DocTypeID);

            if (journal != null)
            {
                JCode = journal ?? "";
            }

            return JCode;
        }

        public async Task<int?> GetJournalMasterStatuse(long ReferenceNo, int DocTypeID)
        {
            int? JID;
            JID = await accRepositoryManager.AccJournalMasterRepository.GetJournalMasterStatuse(ReferenceNo, DocTypeID);

            return JID;
        }

        public async Task<IResult<List<AccJournalMasterVw>>> Search(AccJournalMasterfilterDto filter, CancellationToken cancellationToken = default)
        {
            try
            {
                string StatusId = "1,2";
                filter.PaymentTypeId ??= 0;
                filter.StatusId ??= 0;
                filter.PeriodId ??= 0;
                filter.InsertUserId ??= 0;
                filter.BranchId ??= 0;
                filter.Debit ??= 0;
                filter.Credit ??= 0;
                var branchsId = session.Branches.Split(',');
                filter.BranchId ??= 0;
                var items = await accRepositoryManager.AccJournalMasterRepository.GetAllVw(x =>
    x.FacilityId == session.FacilityId && x.FlagDelete == false &&
    x.DocTypeId == 3 &&
               x.FinYear == session.FinYear &&
    (filter.InsertUserId > 0 ? x.InsertUserId.Equals(filter.InsertUserId) : true) &&
    (filter.PeriodId > 0 ? x.PeriodId.Equals(filter.PeriodId) : true) &&
    (string.IsNullOrEmpty(filter.JCode) || (x.JCode != null && x.JCode.CompareTo(filter.JCode) >= 0 && x.JCode.CompareTo(filter.JCode2) <= 0))
    && (filter.BranchId == 0 || (x.BranchId == filter.BranchId))
       && ((filter.BranchId != 0) || branchsId.Contains(x.BranchId.ToString())) &&
    (filter.StatusId != null && filter.StatusId > 0 ?
        (
            filter.StatusId == 2 ? x.StatusId.Equals(2) :
            filter.StatusId == 1 ? x.StatusId.Equals(1) :
            filter.StatusId == 3 ? x.FlagDelete == true :
            filter.StatusId == 4 ? StatusId.Contains(x.StatusId.ToString()) :
            filter.StatusId == 5 ? x.StatusId.Equals(0) : true) : true)
);
                if (items != null)
                {
                    var res = items.AsQueryable();
                    if (filter == null)
                    {
                        return await Result<List<AccJournalMasterVw>>.SuccessAsync(new List<AccJournalMasterVw>(items.Where(x => x.FlagDelete == false)));
                    }

                    if (!string.IsNullOrEmpty(filter.CostCenterCode) || filter.Debit > 0 || filter.Credit > 0 || !string.IsNullOrEmpty(filter.AccountCode) || !string.IsNullOrEmpty(filter.AccountName) || !string.IsNullOrEmpty(filter.Description))
                    {
                        var details = await accRepositoryManager.AccJournalDetaileRepository.GetAllVw(x => x.FlagDelete == false);
                        if (!string.IsNullOrEmpty(filter.CostCenterCode))
                        {

                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.CostCenterCode != null && d.CostCenterCode == filter.CostCenterCode));

                        }
                        if (!string.IsNullOrEmpty(filter.CostcenterName))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.CostCenterName != null && d.CostCenterName.Contains(filter.CostcenterName)));

                        }

                        if (filter.Debit > 0)
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.Debit == filter.Debit));


                        }
                        if (filter.Credit > 0)
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.Credit == filter.Credit));


                        }
                        if (!string.IsNullOrEmpty(filter.AccountCode))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.AccAccountCode != null && d.AccAccountCode == filter.AccountCode));

                        }
                        if (!string.IsNullOrEmpty(filter.AccountName))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.AccAccountName != null && d.AccAccountName.Contains(filter.AccountName)));

                        }

                        if (!string.IsNullOrEmpty(filter.Description))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.Description.IndexOf(filter.Description, StringComparison.OrdinalIgnoreCase) >= 0));
                        }

                    }

                    if (!string.IsNullOrEmpty(filter.JDateGregorian))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
                    }

                    if (!string.IsNullOrEmpty(filter.JDateGregorian2))
                    {
                        DateTime endDate = DateTime.ParseExact(filter.JDateGregorian2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }





                    var final = res.ToList();
                    return await Result<List<AccJournalMasterVw>>.SuccessAsync(final, "");
                }
                return await Result<List<AccJournalMasterVw>>.SuccessAsync(new List<AccJournalMasterVw>());
            }
            catch (Exception ex)
            {
                return await Result<List<AccJournalMasterVw>>.FailAsync($"======= Exp in Search AccJournalMasterVw, MESSAGE: {ex.Message}");
            }
        }

        public async Task<IResult<List<AccJournalMasterVw>>> RepIncomeSearch(AccJournalMasterfilterDto filter, CancellationToken cancellationToken = default)
        {
            try
            {
                filter.PaymentTypeId ??= 0;
                filter.StatusId ??= 0;
                filter.InsertUserId ??= 0;
                filter.BranchId ??= 0;
                filter.PeriodId ??= 0;

                long fromParsed, toParsed;
                var branchsId = session.Branches.Split(',');
                var items = await accRepositoryManager.AccJournalMasterRepository.GetAllVw(x => x.FlagDelete == false
                   && x.FacilityId == session.FacilityId && x.DocTypeId == 1 && x.FinYear == session.FinYear
                && (filter.InsertUserId == 0 || x.InsertUserId == filter.InsertUserId)
                && (filter.PeriodId == 0 || x.PeriodId == filter.PeriodId)
                && (filter.Credit == 0 || x.Amount == filter.Credit)
                && (string.IsNullOrEmpty(filter.ReferenceCode) || (x.ReferenceCode != null && x.ReferenceCode.Contains(filter.ReferenceCode)))
                 && ((filter.BranchId != 0 ? x.BranchId == filter.BranchId : branchsId.Contains(x.BranchId.ToString())))

                 && (string.IsNullOrEmpty(filter.CollectionEmpCode) || (x.CollectionEmpCode != null && x.CollectionEmpCode.Contains(filter.CollectionEmpCode)))
                 && (filter.StatusId == 0 || (x.StatusId == filter.StatusId))
                 && (filter.PaymentTypeId == 0 || (x.PaymentTypeId == filter.PaymentTypeId))
                 && (string.IsNullOrEmpty(filter.ReferenceNoFrom) ||
                (x.ReferenceNo != null && long.TryParse(filter.ReferenceNoFrom, out fromParsed) && long.TryParse(filter.ReferenceNoTo, out toParsed) &&
                 x.ReferenceNo >= fromParsed && x.ReferenceNo <= toParsed))
                );
                if (items != null)
                {
                    var res = items.AsQueryable();

                    if (!string.IsNullOrEmpty(filter.CostCenterCode) || !string.IsNullOrEmpty(filter.AccountCode) || !string.IsNullOrEmpty(filter.AccountName) || !string.IsNullOrEmpty(filter.Description))
                    {
                        var details = await accRepositoryManager.AccJournalDetaileRepository.GetAllVw(x => x.FlagDelete == false);
                        if (!string.IsNullOrEmpty(filter.CostCenterCode))
                        {

                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.CostCenterCode != null && d.CostCenterCode == filter.CostCenterCode));

                        }
                        if (!string.IsNullOrEmpty(filter.CostcenterName))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.CostCenterName != null && d.CostCenterName.Contains(filter.CostcenterName)));

                        }


                        if (!string.IsNullOrEmpty(filter.AccountCode))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.AccAccountCode != null && d.AccAccountCode == filter.AccountCode));

                        }
                        if (!string.IsNullOrEmpty(filter.AccountName))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.AccAccountName != null && d.AccAccountName.Contains(filter.AccountName)));

                        }

                        if (!string.IsNullOrEmpty(filter.Description))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.Description.IndexOf(filter.Description, StringComparison.OrdinalIgnoreCase) >= 0));
                        }

                    }

                    if (!string.IsNullOrEmpty(filter.JDateGregorian))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.JDateGregorian) || DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
                    }

                    if (!string.IsNullOrEmpty(filter.JDateGregorian2))
                    {
                        DateTime endDate = DateTime.ParseExact(filter.JDateGregorian2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.JDateGregorian) || DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }





                    var final = res.ToList();
                    return await Result<List<AccJournalMasterVw>>.SuccessAsync(final, "");
                }
                return await Result<List<AccJournalMasterVw>>.SuccessAsync(new List<AccJournalMasterVw>());
            }
            catch (Exception ex)
            {
                return await Result<List<AccJournalMasterVw>>.FailAsync($"======= Exp in Search AccJournalMasterVw, MESSAGE: {ex.Message}");
            }
        }

        public async Task<IResult<List<AccJournalMasterVw>>> RepExpensesSearch(AccJournalMasterfilterDto filter, CancellationToken cancellationToken = default)
        {
            try
            {
                long fromParsed, toParsed;
                filter.PaymentTypeId ??= 0; filter.BranchId ??= 0;
                filter.StatusId ??= 0; filter.PeriodId ??= 0;
                filter.InsertUserId ??= 0; filter.Debit ??= 0;
                filter.Credit ??= 0;
                var branchsId = session.Branches.Split(',');

                var items = await accRepositoryManager.AccJournalMasterRepository.GetAllVw(x => x.FlagDelete == false
                  && x.FacilityId == session.FacilityId && x.DocTypeId == 2 && x.FinYear == session.FinYear
                 && (filter.InsertUserId == 0 || x.InsertUserId == filter.InsertUserId)
              && (filter.PeriodId == 0 || x.PeriodId == filter.PeriodId)
               && (string.IsNullOrEmpty(filter.ReferenceNoFrom) || (x.ReferenceNo != null && long.TryParse(filter.ReferenceNoFrom, out fromParsed) && long.TryParse(filter.ReferenceNoTo, out toParsed) &&
              x.ReferenceNo >= fromParsed && x.ReferenceNo <= toParsed))
              && (filter.Credit == 0 || x.Amount == filter.Credit)
               && (string.IsNullOrEmpty(filter.ReferenceCode) || (x.ReferenceCode != null && x.ReferenceCode.Contains(filter.ReferenceCode)))
               && (filter.StatusId == 0 || x.StatusId == filter.StatusId)
               && (filter.PaymentTypeId == 0 || x.PaymentTypeId == filter.PaymentTypeId)
                 && ((filter.BranchId > 0 && x.BranchId == filter.BranchId)
                    || (filter.BranchId == 0 && branchsId.Contains(x.BranchId.ToString())))
                );
                if (items != null)
                {
                    var res = items.AsQueryable();

                    if (!string.IsNullOrEmpty(filter.CostCenterCode) || !string.IsNullOrEmpty(filter.AccountCode) || !string.IsNullOrEmpty(filter.AccountName) || !string.IsNullOrEmpty(filter.Description))
                    {
                        var details = await accRepositoryManager.AccJournalDetaileRepository.GetAllVw(x => x.FlagDelete == false);
                        if (!string.IsNullOrEmpty(filter.CostCenterCode))
                        {

                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.CostCenterCode != null && d.CostCenterCode == filter.CostCenterCode));

                        }
                        if (!string.IsNullOrEmpty(filter.CostcenterName))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.CostCenterName != null && d.CostCenterName.Contains(filter.CostcenterName)));

                        }


                        if (!string.IsNullOrEmpty(filter.AccountCode))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.AccAccountCode != null && d.AccAccountCode == filter.AccountCode));

                        }
                        if (!string.IsNullOrEmpty(filter.AccountName))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.AccAccountName != null && d.AccAccountName.Contains(filter.AccountName)));

                        }

                        if (!string.IsNullOrEmpty(filter.Description))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.Description.IndexOf(filter.Description, StringComparison.OrdinalIgnoreCase) >= 0));
                        }

                    }


                    if (!string.IsNullOrEmpty(filter.JDateGregorian))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.JDateGregorian) || DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
                    }

                    if (!string.IsNullOrEmpty(filter.JDateGregorian2))
                    {
                        DateTime endDate = DateTime.ParseExact(filter.JDateGregorian2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => string.IsNullOrEmpty(s.JDateGregorian) || DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }





                    var final = res.ToList();
                    return await Result<List<AccJournalMasterVw>>.SuccessAsync(final, "");
                }
                return await Result<List<AccJournalMasterVw>>.SuccessAsync(new List<AccJournalMasterVw>());
            }
            catch (Exception ex)
            {
                return await Result<List<AccJournalMasterVw>>.FailAsync($"======= Exp in Search AccJournalMasterVw, MESSAGE: {ex.Message}");
            }
        }

        public async Task<IResult<List<AccJournalMasterfilterDto>>> TransferFromgeneralledgerSearch(AccJournalMasterfilterDto filter, CancellationToken cancellationToken = default)
        {
            try
            {
                string Posting_By_User_Doc_Type = "1";
                Posting_By_User_Doc_Type = await sysConfigurationAppHelper.GetValue(20, session.FacilityId);
                string Acc_Posting = "0";
                Acc_Posting = await mainRepositoryManager.SysUserRepository.GetUserPosting(session.UserId) ?? "0";


                var branchsId = session.Branches.Split(',');
                filter.PaymentTypeId ??= 0;
                filter.StatusId ??= 0;
                filter.DocTypeId ??= 0;
                filter.ReferenceNo ??= 0;
                filter.PeriodId ??= 0;
                filter.InsertUserId ??= 0;
                filter.BranchId ??= 0;
                filter.Debit ??= 0;
                filter.Credit ??= 0;
                var items = await accRepositoryManager.AccJournalMasterRepository.GetAllVw(x =>
    x.FacilityId == session.FacilityId && x.FlagDelete == false && x.StatusId == 1 && (Posting_By_User_Doc_Type == "1" || Acc_Posting.Contains(x.DocTypeId.ToString()))
    && x.FinYear == session.FinYear
     && (filter.PeriodId == 0 || (x.PeriodId.Equals(filter.PeriodId)))
     && (filter.BranchId == 0 || (x.BranchId == filter.BranchId))
   && ((filter.BranchId != 0) || branchsId.Contains(x.BranchId.ToString()))
   && (filter.DocTypeId == 0 || x.DocTypeId.Equals(filter.DocTypeId))
    && (string.IsNullOrEmpty(filter.JCode) || (x.JCode != null && x.JCode.CompareTo(filter.JCode) >= 0 && x.JCode.CompareTo(filter.JCode2) <= 0))
     && (filter.ReferenceNo == 0 || x.ReferenceNo.Equals(filter.ReferenceNo))
     && (string.IsNullOrEmpty(filter.ReferenceCode) || (x.ReferenceCode != null && x.ReferenceCode.Contains(filter.ReferenceCode)))

);
                if (items != null)
                {
                    var res = items.AsQueryable();
                    if (filter == null)
                    {
                        return await Result<List<AccJournalMasterfilterDto>>.SuccessAsync(new List<AccJournalMasterfilterDto>());
                    }

                    if (!string.IsNullOrEmpty(filter.CostCenterCode) || filter.Debit > 0 || filter.Credit > 0 || !string.IsNullOrEmpty(filter.AccountCode) || !string.IsNullOrEmpty(filter.AccountName) || !string.IsNullOrEmpty(filter.Description))
                    {
                        var details = await accRepositoryManager.AccJournalDetaileRepository.GetAllVw(x => x.FlagDelete == false);
                        if (!string.IsNullOrEmpty(filter.CostCenterCode))
                        {

                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.CostCenterCode != null && d.CostCenterCode == filter.CostCenterCode));

                        }
                        if (!string.IsNullOrEmpty(filter.CostcenterName))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.CostCenterName != null && d.CostCenterName.Contains(filter.CostcenterName)));

                        }

                        if (filter.Debit > 0)
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.Debit == filter.Debit));


                        }
                        if (filter.Credit > 0)
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.Credit == filter.Credit));


                        }
                        if (!string.IsNullOrEmpty(filter.AccountCode))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.AccAccountCode != null && d.AccAccountCode == filter.AccountCode));

                        }
                        if (!string.IsNullOrEmpty(filter.AccountName))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.AccAccountName != null && d.AccAccountName.Contains(filter.AccountName)));

                        }

                        if (!string.IsNullOrEmpty(filter.Description))
                        {
                            res = res.Where(s => details.Any(d => d.JId == s.JId && d.Description.IndexOf(filter.Description, StringComparison.OrdinalIgnoreCase) >= 0));
                        }

                    }

                    if (!string.IsNullOrEmpty(filter.JDateGregorian))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate);
                    }

                    if (!string.IsNullOrEmpty(filter.JDateGregorian2))
                    {
                        DateTime endDate = DateTime.ParseExact(filter.JDateGregorian2, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        res = res.Where(s => DateTime.ParseExact(s.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }




                    var final = res.ToList();
                    var result = new List<JournalMasterVM>();
                    foreach (var item in final)
                    {
                        var children = await accRepositoryManager.AccJournalDetaileRepository.GetAllVw(x => x.FlagDelete == false && x.JId == item.JId);
                        var result2 = new JournalMasterVM
                        {
                            JCode = item.JCode,
                            JId = item.JId,
                            JDateGregorian = item.JDateGregorian,
                            DocTypeName = item.DocTypeName,
                            DocTypeName2 = item.DocTypeName,
                            BraName = item.BraName,
                            BraName2 = item.BraName2,
                            ReferenceCode = item.ReferenceCode,
                            InsertDate = item.InsertDate,
                            StatusName = item.StatusName,
                            StatusName2 = item.StatusName2,
                            DocTypeId = item.DocTypeId,
                            ReferenceNo = item.ReferenceNo,
                            UserFullname = item.UserFullname,
                            sumCredit = children.ToList().Sum(a => a.Credit),
                            sumDebit = children.ToList().Sum(b => b.Debit),
                            Children = children.ToList()
                        };
                        result.Add(result2);
                    }

                    return await Result<List<AccJournalMasterfilterDto>>.SuccessAsync(new List<AccJournalMasterfilterDto>(), "");
                }
                return await Result<List<AccJournalMasterfilterDto>>.SuccessAsync(new List<AccJournalMasterfilterDto>());
            }
            catch (Exception ex)
            {
                return await Result<List<AccJournalMasterfilterDto>>.FailAsync($"======= Exp in Search AccJournalMasterVw, MESSAGE: {ex.Message}");
            }
        }
    }
}
