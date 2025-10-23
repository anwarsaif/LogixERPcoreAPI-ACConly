using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Helpers;
using Logix.Application.Helpers.Acc;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using System.Globalization;

namespace Logix.Application.Services.ACC
{
    public class AccRequestService : GenericQueryService<AccRequest, AccRequestDto, AccRequestVw>, IAccRequestService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData currentData;
        private readonly IHrRepositoryManager hrRepositoryManager;
        private readonly IGetAccountIDByCodeHelper getAccountIDByCodeHelper;
        private readonly IGetRefranceByCodeHelper getRefranceByCodeHelper;
        private readonly ISysConfigurationAppHelper sysConfigurationAppHelper;
        private readonly IWorkflowHelper workflowHelper;
        private readonly IMainRepositoryManager mainRepositoryManager;
        private readonly ILocalizationService localization;
        private readonly IWFRepositoryManager wFRepositoryManager;

        public AccRequestService(IQueryRepository<AccRequest> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData currentData, IHrRepositoryManager hrRepositoryManager, IGetAccountIDByCodeHelper getAccountIDByCodeHelper, IGetRefranceByCodeHelper getRefranceByCodeHelper, ISysConfigurationAppHelper SysConfigurationAppHelper, IWorkflowHelper workflowHelper, IMainRepositoryManager mainRepositoryManager, ILocalizationService localization, IWFRepositoryManager wFRepositoryManager) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;

            this.currentData = currentData;
            this.hrRepositoryManager = hrRepositoryManager;
            this.getAccountIDByCodeHelper = getAccountIDByCodeHelper;
            this.getRefranceByCodeHelper = getRefranceByCodeHelper;
            this.sysConfigurationAppHelper = SysConfigurationAppHelper;
            this.workflowHelper = workflowHelper;
            this.mainRepositoryManager = mainRepositoryManager;
            this.localization = localization;
            this.wFRepositoryManager = wFRepositoryManager;
        }

        public async Task<IResult<AccRequestDto>> Add(AccRequestDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccRequestDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {

                //-------------------------------- '--الموظف غير موجود في قائمة الموظفين 
                long EmpId = 0;
                var chkEmpid = await hrRepositoryManager.HrEmployeeRepository.chkEmpid(entity.EmpCode);
                if (chkEmpid != null && chkEmpid == 0)
                {
                    return await Result<AccRequestDto>.FailAsync(localization.GetResource1("EmployeeNotFound"));
                }
                else
                {
                    var Id = await hrRepositoryManager.HrEmployeeRepository.GetEmpId(currentData.FacilityId, entity.EmpCode);
                    if (Id == null)
                    {
                        return await Result<AccRequestDto>.FailAsync(localization.GetResource1("EmployeeNotFound"));
                    }
                    EmpId = Id;
                }
                ////-------------------------------------------------- كود مركز التكلفة لابد أن يكون رقماً
                //var ValdJournalDetailes = await accRepositoryManager.AccJournalDetaileRepository.ValdJournalDetailes(entity.AccAccountCode, entity.CostCenterCode);
                //if (ValdJournalDetailes == true)
                //{
                //    return await Result<AccRequestDto>.FailAsync(localization.GetResource1("CostCenterDoesNotAcceptThisAccount"));


                //}

                //-------------------------------- '--جاب  رقم مراكز التكلفة 
                long CCId = 0;
                if (!string.IsNullOrEmpty(entity.CostCenterCode))
                {
                    CCId = await accRepositoryManager.AccCostCenterRepository.getCostCenterByCode(entity.CostCenterCode, currentData.FacilityId);

                }

                //-------------------------------- '--جاب  رقم حساب 

                long AccAccountID = 0;

                if (!string.IsNullOrEmpty(entity.AccAccountCode))
                {
                    AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);


                    if (AccAccountID == 0)
                    {
                        return await Result<AccRequestDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                    }
                }


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

                                    return await Result<AccRequestDto>.FailAsync(localization.GetAccResource("Allowablecostcenters") + Cost.CcIdFrom + " - " + Cost.CcIdTo);

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
                    return await Result<AccRequestDto>.FailAsync(MsgErrorCostCenter);

                }

                //  رقم المرجع مرجع getRefranceByCodeHelper
                long ReferenceNo = 0;
                ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);

                if (ReferenceNo == 0)
                {
                    return await Result<AccRequestDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

                }
                long AppID = await workflowHelper.Send(EmpId, 641, entity.AppTypeId ?? 0, 0, entity.Description ?? "");


                ///  'ارسال الى سير العمل
                entity.AppId = AppID;


                //------------------------------توليد الكود
                long codeAut = 0;
                codeAut = await accRepositoryManager.AccRequestRepository.GetAccRequestCode(entity.AppDate);
                entity.TransTypeId = 1;
                entity.AppCode = codeAut;
                entity.AccountId = AccAccountID;
                entity.ReferenceNo = ReferenceNo;
                entity.CcId = CCId;
                entity.FacilityId = currentData.FacilityId;
                entity.StatusId = 1;
                entity.RefranceId = 0;
                entity.ExchangeStatusId = 0;
                entity.HasCredit = 0;
                entity.BadgetNo = 0;
                entity.JId = 0;
                entity.GmUserId = 0;
                entity.BalanceStatusId = 0;
                entity.FinUserId = 0;
                entity.ISMulti = false;
                var item = _mapper.Map<AccRequest>(entity);

                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

                var newEntity = await accRepositoryManager.AccRequestRepository.AddAndReturn(item);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, newEntity.Id, 21);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }

                var entityMap = _mapper.Map<AccRequestDto>(newEntity);
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);


                return await Result<AccRequestDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {

                return await Result<AccRequestDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {

            var item = await accRepositoryManager.AccRequestRepository.GetById(Id);

            if (item == null) return Result<AccRequestDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));
            string CheckStatus_ID = "0";
            CheckStatus_ID = await sysConfigurationAppHelper.GetValue(310, currentData.FacilityId);
            if (CheckStatus_ID == "1")
            {
                if (item.StatusId == 3)
                {
                    return Result<AccRequestDto>.Fail(localization.GetResource1("RequestApprovedAuthorizedPersonAndCannotDeleted"));
                }
                else if (item.StatusId == 4)
                {
                    return Result<AccRequestDto>.Fail(localization.GetResource1("RequestHasBeenRejectedAndCannotDeleted"));

                }

            }
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            accRepositoryManager.AccRequestRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccRequestDto>.SuccessAsync(_mapper.Map<AccRequestDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccRequestDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccRequestRepository.GetById(Id);
            if (item == null) return Result<AccRequestDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            string CheckStatus_ID = "0";
            CheckStatus_ID = await sysConfigurationAppHelper.GetValue(310, currentData.FacilityId);
            if (CheckStatus_ID == "1")
            {
                if (item.StatusId == 3)
                {
                    return Result<AccRequestDto>.Fail(localization.GetResource1("RequestApprovedAuthorizedPersonAndCannotDeleted"));
                }
                else if (item.StatusId == 4)
                {
                    return Result<AccRequestDto>.Fail(localization.GetResource1("RequestHasBeenRejectedAndCannotDeleted"));

                }

            }
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            accRepositoryManager.AccRequestRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccRequestDto>.SuccessAsync(_mapper.Map<AccRequestDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccRequestDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccRequestEditDto>> Update(AccRequestEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccRequestEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");

            //-------------------------------- '--جاب  رقم حساب 

            long AccAccountID = 0;

            if (!string.IsNullOrEmpty(entity.AccAccountCode))
            {
                AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);
                if (AccAccountID == 0)
                {
                    return await Result<AccRequestEditDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                }
            }

            ////-------------------------------------------------- كود مركز التكلفة لابد أن يكون رقماً
            //var ValdJournalDetailes = await accRepositoryManager.AccJournalDetaileRepository.ValdJournalDetailes(entity.AccAccountCode, entity.CostCenterCode);
            //if (ValdJournalDetailes == true)
            //{
            //    return await Result<AccRequestEditDto>.FailAsync(localization.GetResource1("CostCenterDoesNotAcceptThisAccount"));


            //}



            //-------------------------------- '--جاب  رقم مراكز التكلفة 
            long CCId = 0;
            if (!string.IsNullOrEmpty(entity.CostCenterCode))
            {
                CCId = await accRepositoryManager.AccCostCenterRepository.GetOne(s => s.CcId, X => X.IsDeleted == false && X.CostCenterCode == entity.CostCenterCode);

            }



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

                                return await Result<AccRequestEditDto>.FailAsync(localization.GetAccResource("Allowablecostcenters") + Cost.CcIdFrom + " - " + Cost.CcIdTo);

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
                return await Result<AccRequestEditDto>.FailAsync(MsgErrorCostCenter);

            }

            //  رقم المرجع مرجع getRefranceByCodeHelper
            long ReferenceNo = 0;
            ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);

            if (ReferenceNo == 0)
            {
                return await Result<AccRequestEditDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

            }

            var item = await accRepositoryManager.AccRequestRepository.GetById(entity.Id);

            if (item == null) return await Result<AccRequestEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));

            //--------------الطلب معتمد من قبل صاحب الصلاحية ولايمكن التعديل عليه

            string CheckStatus_ID = "0";

            CheckStatus_ID = await sysConfigurationAppHelper.GetValue(310, currentData.FacilityId);
            if (CheckStatus_ID == "1")
            {
                if (item.StatusId == 3)
                {
                    return await Result<AccRequestEditDto>.FailAsync(localization.GetAccResource("TheRequestApproved"));

                }

            }


            _mapper.Map(entity, item);
            item.CcId = CCId;
            item.ReferenceNo = ReferenceNo;
            item.AccountId = AccAccountID;
            item.IsDeleted = false;
            item.StatusId = entity.StatusId;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
            accRepositoryManager.AccRequestRepository.Update(item);
            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

            //save files
            if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
            {
                var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, item.Id, 21);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

            }
            if (item.AppId.HasValue && item.AppId > 0)
            {
                var itemwf = await wFRepositoryManager.WfApplicationRepository.GetById(item.AppId.Value);

                if (itemwf == null)
                    return await Result<AccRequestEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));

                itemwf.Subject = entity.Description;
                itemwf.ModifiedBy = currentData.UserId;

                wFRepositoryManager.WfApplicationRepository.Update(itemwf);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
            }

            try
            {
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                return await Result<AccRequestEditDto>.SuccessAsync(_mapper.Map<AccRequestEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccRequestEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccRequestPaymentDto>> AddPaymentDecision(AccRequestPaymentDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccRequestPaymentDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {

                //-------------------------------- '--الموظف غير موجود في قائمة الموظفين 
                long EmpId = 0;
                entity.TransTypeId = 2;
                var chkEmpid = await hrRepositoryManager.HrEmployeeRepository.GetAll(x => x.IsDeleted == false);
                if (chkEmpid != null && chkEmpid.Count() == 0)
                {
                    return await Result<AccRequestPaymentDto>.FailAsync(localization.GetResource1("EmployeeNotFound"));
                }
                else
                {
                    var employee = await mainRepositoryManager.InvestEmployeeRepository.GetOne(x => x.EmpId == entity.EmpCode);
                    if (employee == null)
                    {
                        return await Result<AccRequestPaymentDto>.FailAsync(localization.GetResource1("EmployeeNotFound"));
                    }
                    EmpId = employee.Id;
                }
                //-------------------------------- '--جاب  رقم مراكز التكلفة 
                long CCId = 0;
                if (!string.IsNullOrEmpty(entity.CostCenterCode))
                {
                    CCId = await accRepositoryManager.AccCostCenterRepository.GetOne(s => s.CcId, X => X.IsDeleted == false && X.CostCenterCode == entity.CostCenterCode);

                }

                //-------------------------------- '--جاب  رقم حساب 

                long AccAccountID = 0;

                if (!string.IsNullOrEmpty(entity.AccAccountCode))
                {
                    AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);
                    if (AccAccountID == 0)
                    {
                        return await Result<AccRequestPaymentDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                    }
                }


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

                                    return await Result<AccRequestPaymentDto>.FailAsync(localization.GetAccResource("Allowablecostcenters") + Cost.CcIdFrom + " - " + Cost.CcIdTo);

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
                    return await Result<AccRequestPaymentDto>.FailAsync(MsgErrorCostCenter);

                }

                //  رقم المرجع مرجع getRefranceByCodeHelper
                long ReferenceNo = 0;

                ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);

                if (ReferenceNo == 0)
                {
                    return await Result<AccRequestPaymentDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

                }
                ///رقم طلب الصرف 
                if (entity.refranceCode != 0)
                {
                    var Refrance = await accRepositoryManager.AccRequestRepository.GetRequestWaitingTransfer(currentData.FacilityId, 1, 3, entity.refranceCode ?? 0);

                    if (Refrance != null)
                    {
                        var firstRefrance = Refrance.FirstOrDefault();
                        if (firstRefrance != null)
                        {
                            entity.RefranceId = firstRefrance.Id;
                        }
                        if (entity.RefranceId == 0)
                        {
                            return await Result<AccRequestPaymentDto>.FailAsync(localization.GetMessagesResource("RequestNumberIsNot"));

                        }
                    }

                }
                long AppID = await workflowHelper.Send(EmpId, 1011, entity.AppTypeId ?? 0);

                ///  'ارسال الى سير العمل
                entity.AppId = AppID;

                //------------------------------توليد الكود
                long codeAut = 0;
                codeAut = await accRepositoryManager.AccRequestRepository.GetAccRequestCode(entity.AppDate);
                entity.AppCode = codeAut;
                entity.TransTypeId = 2;
                entity.AppCode = codeAut;
                entity.AccountId = AccAccountID;
                entity.ReferenceNo = ReferenceNo;
                entity.CcId = CCId;
                entity.FacilityId = currentData.FacilityId;
                entity.StatusId = 3;
                entity.ExchangeStatusId = 0;
                entity.HasCredit = 0;
                entity.BadgetNo = 0;
                entity.JId = 0;
                entity.GmUserId = 0;
                entity.BalanceStatusId = 0;
                entity.FinUserId = 0;
                var item = _mapper.Map<AccRequest>(entity);
                var newEntity = await accRepositoryManager.AccRequestRepository.AddAndReturn(item);
                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, newEntity.Id, 21);
                }
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccRequestPaymentDto>(newEntity);


                return await Result<AccRequestPaymentDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {

                return await Result<AccRequestPaymentDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }
        public async Task<IResult<AccRequestPaymentEditDto>> UpdatePaymentDecision(AccRequestPaymentEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccRequestPaymentEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");

            var item = await accRepositoryManager.AccRequestRepository.GetById(entity.Id);

            if (item == null) return await Result<AccRequestPaymentEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));


            //-------------------------------- '--جاب  رقم مراكز التكلفة 
            long CCId = 0;
            if (!string.IsNullOrEmpty(entity.CostCenterCode))
            {
                CCId = await accRepositoryManager.AccCostCenterRepository.GetOne(s => s.CcId, X => X.IsDeleted == false && X.CostCenterCode == entity.CostCenterCode);

            }

            //-------------------------------- '--جاب  رقم حساب 

            long AccAccountID = 0;

            if (!string.IsNullOrEmpty(entity.AccAccountCode))
            {
                AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);
                if (AccAccountID == 0)
                {
                    return await Result<AccRequestPaymentEditDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                }
            }


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

                                return await Result<AccRequestPaymentEditDto>.FailAsync(localization.GetAccResource("Allowablecostcenters") + Cost.CcIdFrom + " - " + Cost.CcIdTo);

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
                return await Result<AccRequestPaymentEditDto>.FailAsync(MsgErrorCostCenter);

            }

            //  رقم المرجع مرجع getRefranceByCodeHelper
            long ReferenceNo = 0;
            ReferenceNo = await getRefranceByCodeHelper.GetRefranceByCode(entity.ReferenceTypeId ?? 0, entity.AccAccountCode);

            if (ReferenceNo == 0)
            {
                return await Result<AccRequestPaymentEditDto>.FailAsync(localization.GetAccResource("ReferenceNotfind"));

            }




            _mapper.Map(entity, item);
            item.CcId = CCId;
            item.ReferenceNo = ReferenceNo;
            item.AccountId = AccAccountID;
            //item.StatusId = entity.StatusId;
            item.StatusId = item.StatusId;
            item.IsDeleted = false;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

            accRepositoryManager.AccRequestRepository.Update(item);
            //save files
            if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
            {
                var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, item.Id, 21);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

            }
            try
            {
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                return await Result<AccRequestPaymentEditDto>.SuccessAsync(_mapper.Map<AccRequestPaymentEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccRequestPaymentEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<List<AccRequestVw>>> Search(AccRequestFilterDto filter, CancellationToken cancellationToken = default)
        {
            try
            {
                filter.Status2Id ??= 0;
                filter.Amount ??= 0;
                filter.StatusId ??= 0;
                filter.ReferenceTypeId ??= 0;
                filter.TypeId ??= 0;
                filter.AppCode ??= 0;
                filter.DepId ??= 0;
                var branchsId = currentData.Branches.Split(',');
                filter.BranchId ??= 0;
                var items = await accRepositoryManager.AccRequestRepository.GetAllVw(x => x.IsDeleted == false && x.FacilityId == currentData.FacilityId &&
                           (filter.AppCode == 0 || x.AppCode == filter.AppCode) &&
                             (filter.Amount == 0 || x.Amount == filter.Amount) &&
                              (filter.BranchId == 0 || x.BranchId == filter.BranchId) &&
                              (filter.TypeId == 0 || x.TypeId == filter.TypeId) &&
                              (filter.DepId == 0 || x.DepId == filter.DepId) &&

                              (filter.StatusId == 0 || x.StatusId == filter.StatusId) &&
                               (string.IsNullOrEmpty(filter.AccAccountName) || (x.AccountName != null && x.AccountName.Contains(filter.AccAccountName))) &&
                              (string.IsNullOrEmpty(filter.ApplicationCode) || (x.ApplicationCode != null && x.ApplicationCode.Equals(filter.ApplicationCode))) &&
                              (string.IsNullOrEmpty(filter.Description) || (x.Description != null && x.Description.Contains(filter.Description))) &&
                              (string.IsNullOrEmpty(filter.Iban) || (x.Iban != null && x.Iban.Contains(filter.Iban))) &&
                               (string.IsNullOrEmpty(filter.IdNo) || (x.IdNo != null && x.IdNo.Contains(filter.IdNo))) &&
                               (string.IsNullOrEmpty(filter.CustomerName) || (x.CustomerName != null && x.CustomerName.Contains(filter.CustomerName))) &&
                               (filter.Status2Id == 0 || x.Status2Id == filter.Status2Id) &&
                               (filter.ReferenceTypeId == 0 || x.ReferenceTypeId == filter.ReferenceTypeId) &&
                               (!string.IsNullOrEmpty(filter.AccAccountCode) ? (filter.ReferenceTypeId == 1 ? x.AccountCode == filter.AccAccountCode : x.RefraneCode.Equals(filter.AccAccountCode)) : true)
                                  && (filter.BranchId == 0 || (x.BranchId == filter.BranchId))
                        && ((filter.BranchId != 0) || branchsId.Contains(x.BranchId.ToString()))



                );
                if (items != null)
                {
                    var res = items.AsQueryable();
                    if (filter == null)
                    {
                        return await Result<List<AccRequestVw>>.SuccessAsync(new List<AccRequestVw>());
                    }


                    if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
                    {
                        DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
                        DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

                        res = res.Where(r => !string.IsNullOrEmpty(r.AppDate) && DateTime.ParseExact(r.AppDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
                            && DateTime.ParseExact(r.AppDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
                    }

                    var final = res.ToList();
                    return await Result<List<AccRequestVw>>.SuccessAsync(final, "");
                }
                return await Result<List<AccRequestVw>>.SuccessAsync(new List<AccRequestVw>());
            }
            catch (Exception ex)
            {
                return await Result<List<AccRequestVw>>.FailAsync($"======= Exp in Search AccRequestVw, MESSAGE: {ex.Message}");
            }
        }

        public async Task<IResult<List<TransactionResult>>> GetUnPaidPO(string transTypeIds, string code, string dateText)
        {
            try
            {
                var unpaidPOList = await accRepositoryManager.AccRequestRepository.GetUnPaidPO(transTypeIds, code, dateText);

                return Result<List<TransactionResult>>.Success(unpaidPOList);
            }
            catch (Exception ex)
            {
                // يمكنك تخصيص الرسالة أو استخدام logging هنا
                return await Result<List<TransactionResult>>.FailAsync($"An error occurred, MESSAGE: {ex.Message}");
            }
        }

        public async Task<IResult<List<TransactionUnPaidResult>>> GetUnPaidSubEx(string code, string dateText)
        {
            try
            {
                var UnPaidSubExList = await accRepositoryManager.AccRequestRepository.GetUnPaidSubEx(code, dateText, 2);

                return Result<List<TransactionUnPaidResult>>.Success(UnPaidSubExList);
            }
            catch (Exception ex)
            {
                // يمكنك تخصيص الرسالة أو استخدام logging هنا
                return await Result<List<TransactionUnPaidResult>>.FailAsync($"An error occurred, MESSAGE: {ex.Message}");
            }
        }

        public async Task<IResult<List<PayrollResultpopup>>> GetApprovedPayroll(PayrollResultPopupFilter filter)
        {
            try
            {
                var ApprovedPayrollList = await accRepositoryManager.AccRequestRepository.GetApprovedPayroll(filter);

                return Result<List<PayrollResultpopup>>.Success(ApprovedPayrollList);
            }
            catch (Exception ex)
            {
                // يمكنك تخصيص الرسالة أو استخدام logging هنا
                return await Result<List<PayrollResultpopup>>.FailAsync($"An error occurred, MESSAGE: {ex.Message}");
            }
        }

        public async Task<IResult<AccRequestMultiDto>> AddMulti(AccRequestMultiDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccRequestMultiDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {

                //-------------------------------- '--الموظف غير موجود في قائمة الموظفين 
                long EmpId = 0;
                var chkEmpid = await hrRepositoryManager.HrEmployeeRepository.chkEmpid(entity.EmpCode);
                if (chkEmpid != null && chkEmpid == 0)
                {
                    return await Result<AccRequestMultiDto>.FailAsync(localization.GetResource1("EmployeeNotFound"));
                }
                else
                {
                    var Id = await hrRepositoryManager.HrEmployeeRepository.GetEmpId(currentData.FacilityId, entity.EmpCode);
                    if (Id == null)
                    {
                        return await Result<AccRequestMultiDto>.FailAsync(localization.GetResource1("EmployeeNotFound"));
                    }
                    EmpId = Id;
                }
                //-------------------------------------------- التفاصيل
                foreach (var requestEmployee in entity.AccRequestEmployeeDto)
                {
                    //-------------------------------- '--جاب  رقم حساب 

                    long AccAccountID = 0;


                    AccAccountID = await getAccountIDByCodeHelper.GetAccountIDByCode(requestEmployee.ReferenceTypeId ?? 0, requestEmployee.AccAccountCode);


                    if (AccAccountID == 0)
                    {
                        return await Result<AccRequestMultiDto>.FailAsync(localization.GetAccResource("AccAccountNotfind"));

                    }


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

                                        return await Result<AccRequestMultiDto>.FailAsync(localization.GetAccResource("Allowablecostcenters") + Cost.CcIdFrom + " - " + Cost.CcIdTo);

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
                    ////-------------------------------------------------- كود مركز التكلفة لابد أن يكون رقماً
                    var ValdJournalDetailes = await accRepositoryManager.AccJournalDetaileRepository.ValdJournalDetailes(requestEmployee.AccAccountCode, entity.CostCenterCode);
                    if (ValdJournalDetailes == true)
                    {
                        return await Result<AccRequestMultiDto>.FailAsync(localization.GetResource1("CostCenterDoesNotAcceptThisAccount"));


                    }

                }


                //-------------------------------- '--جاب  رقم مراكز التكلفة 

                long AppID = await workflowHelper.Send(EmpId, 641, entity.AppTypeId ?? 0, 0, entity.Description ?? "");


                ///  'ارسال الى سير العمل
                entity.AppId = AppID;


                //------------------------------توليد الكود
                long codeAut = 0;
                codeAut = await accRepositoryManager.AccRequestRepository.GetAccRequestCode(entity.AppDate);
                entity.TransTypeId = 1;
                entity.AppCode = codeAut;
                //entity.AccountId = AccAccountID;
                //entity.ReferenceNo = ReferenceNo;
                //entity.CcId = CCId;
                entity.FacilityId = currentData.FacilityId;
                entity.StatusId = 1;
                entity.RefranceId = 0;
                entity.ExchangeStatusId = 0;
                entity.HasCredit = 0;
                entity.BadgetNo = 0;
                entity.JId = 0;
                entity.GmUserId = 0;
                entity.BalanceStatusId = 0;
                entity.FinUserId = 0;
                entity.ISMulti = true;
                var item = _mapper.Map<AccRequest>(entity);

                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

                var newEntity = await accRepositoryManager.AccRequestRepository.AddAndReturn(item);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                //save files
                if (entity.FileDtos != null && entity.FileDtos.Count() > 0)
                {
                    var addFiles = await mainRepositoryManager.SysFileRepository.SaveFiles(entity.FileDtos, newEntity.Id, 21);

                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                }

                var entityMap = _mapper.Map<AccRequestMultiDto>(newEntity);
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);


                return await Result<AccRequestMultiDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {

                return await Result<AccRequestMultiDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }
        }
    }
}
