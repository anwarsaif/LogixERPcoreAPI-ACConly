using AutoMapper;
using Castle.Windsor.Installer;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;
using System.Data;
using DocumentFormat.OpenXml.Office2016.Excel;

namespace Logix.Application.Services.ACC
{
    public class AccReportsService : IAccReportsService
    {


        private readonly IMapper _mapper;
        private readonly ICurrentData currentData;
        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMainRepositoryManager mainRepositoryManager;
        private readonly ILocalizationService localization;
        private readonly IHrRepositoryManager hrRepositoryManager;

        public AccReportsService(IMapper mapper, ICurrentData currentData, IAccRepositoryManager accRepositoryManager, IMainRepositoryManager mainRepositoryManager, ILocalizationService localization, IHrRepositoryManager hrRepositoryManager)
        {

            this._mapper = mapper;
            this.currentData = currentData;
            this.accRepositoryManager = accRepositoryManager;
            this.mainRepositoryManager = mainRepositoryManager;
            this.localization = localization;
            this.hrRepositoryManager = hrRepositoryManager;
        }
        #region =====================================  كشف حساب
        public async Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetAccounttransactions(AccounttransactionsDto obj)
        {
            try
            {
                obj.facilityId = currentData.FacilityId;
                obj.FinYear = currentData.FinYear;
                obj.accountId = await accRepositoryManager.AccAccountRepository.GetAccountIdByCode(obj.AccountCode, currentData.FacilityId);
                IEnumerable<AccountBalanceSheetDto> items;

                if (obj.chkAllYear == false)
                {
                    // استدعاء الرصيد للسنة الحالية
                    items = await accRepositoryManager.AccReportsRepository.GetAccounttransactionsCurrentYearBalance(obj);



                    if (items == null || !items.Any())
                    {
                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataCurrentYearBalance"));
                    }
                }
                else
                {
                    // استدعاء الرصيد لكل السنوات
                    items = await accRepositoryManager.AccReportsRepository.GetAccounttransactionsAllYearBalance(obj);


                    if (items == null || !items.Any())
                    {

                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataAllYearBalance"));
                    }
                }


                return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("success"));
            }
            catch (Exception ex)
            {

                return await Result<IEnumerable<AccountBalanceSheetDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }

        public async Task<IResult<IEnumerable<AccounttransactionsFromToDto>>> GetAccounttransactionsFromTo(AccounttransactionsFromToFilterDto obj)
        {
            try
            {
                obj.facilityId = currentData.FacilityId;
                obj.FinYear = currentData.FinYear;
                IEnumerable<AccounttransactionsFromToDto> items = await accRepositoryManager.AccReportsRepository.GetAccounttransactionsFromTo(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<AccounttransactionsFromToDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<AccounttransactionsFromToDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<AccounttransactionsFromToDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }

        #endregion

        #region =====================================             كشف حساب صندوق
        public async Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetFundsstatementtransactions(FundsstatementtransactionsDto obj)
        {
            try
            {

                IEnumerable<AccountBalanceSheetDto> items;
                var accountTransactionsDto = new AccounttransactionsDto
                {
                    facilityId = currentData.FacilityId,
                    FinYear = currentData.FinYear,
                    accountId = obj.accountId != 0 ? obj.accountId : 0,
                    dateFrom = obj.dateFrom,
                    dateTo = obj.dateTo,
                    branchId = obj.branchId,

                };
                if (obj.chkAllYear == false)
                {
                    // استدعاء الرصيد للسنة الحالية
                    items = await accRepositoryManager.AccReportsRepository.GetAccounttransactionsCurrentYearBalance(accountTransactionsDto);



                    if (items == null || !items.Any())
                    {
                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataCurrentYearBalance"));
                    }
                }
                else
                {
                    // استدعاء الرصيد لكل السنوات
                    items = await accRepositoryManager.AccReportsRepository.GetAccounttransactionsAllYearBalance(accountTransactionsDto);


                    if (items == null || !items.Any())
                    {

                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataAllYearBalance"));
                    }
                }


                return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("success"));
            }
            catch (Exception ex)
            {

                return await Result<IEnumerable<AccountBalanceSheetDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }

        #endregion
        #region =====================================  كشف حساب عميل
        public async Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetCustomerAccounttransactions(CustomerAccountStatementDto obj)
        {
            try
            {

                var AccountID = await mainRepositoryManager.SysCustomerRepository.GetOne(s => s.Id, x => x.Code == obj.AccountCode && x.CusTypeId == 2 && x.FacilityId == currentData.FacilityId && x.IsDeleted == false);
                var CCid = await accRepositoryManager.AccCostCenterRepository.GetOne(s => s.CcId, x => x.CostCenterCode == obj.CostCenterCode && x.FacilityId == currentData.FacilityId && x.IsDeleted == false);


                IEnumerable<AccountBalanceSheetDto> items;
                var accountTransactionsDto = new AccounttransactionsDto
                {
                    facilityId = currentData.FacilityId,
                    FinYear = currentData.FinYear,
                    ReferenceDNo = AccountID != 0 ? AccountID : 0,
                    ParentReferenceTypeId = 2,
                    ccId = CCid != 0 ? CCid : 0,
                    ReferenceTypeId = obj.ReferenceTypeId,
                    dateFrom = obj.dateFrom,
                    dateTo = obj.dateTo,
                    branchId = obj.branchId,

                };

                if (obj.chkAllYear == false)
                {
                    // استدعاء الرصيد للسنة الحالية
                    items = await accRepositoryManager.AccReportsRepository.GetCustomerCurrentYearBalance(accountTransactionsDto);



                    if (items == null || !items.Any())
                    {
                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataCurrentYearBalance"));
                    }
                }
                else
                {
                    // استدعاء الرصيد لكل السنوات
                    items = await accRepositoryManager.AccReportsRepository.GetCustomerAllYearBalance(accountTransactionsDto);


                    if (items == null || !items.Any())
                    {
                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataAllYearBalance"));
                    }
                }


                return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("success"));
            }
            catch (Exception ex)
            {

                return await Result<IEnumerable<AccountBalanceSheetDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
            ;
        }
        #endregion

        #region =====================================  كشف حساب   مقاول 
        public async Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetContractorsAccounttransactions(ContractorsAccountStatementDto obj)
        {
            try
            {

                var AccountID = await mainRepositoryManager.SysCustomerRepository.GetOne(s => s.Id, x => x.Code == obj.AccountCode && x.CusTypeId == 3 && x.FacilityId == currentData.FacilityId && x.IsDeleted == false);
                var CCid = await accRepositoryManager.AccCostCenterRepository.GetOne(s => s.CcId, x => x.CostCenterCode == obj.CostCenterCode && x.FacilityId == currentData.FacilityId && x.IsDeleted == false);


                IEnumerable<AccountBalanceSheetDto> items;

                var accountTransactionsDto = new AccounttransactionsDto
                {
                    facilityId = currentData.FacilityId,
                    FinYear = currentData.FinYear,
                    ReferenceDNo = AccountID != 0 ? AccountID : 0,
                    ParentReferenceTypeId = 20,
                    ccId = CCid != 0 ? CCid : 0,
                    ReferenceTypeId = obj.ReferenceTypeId,
                    dateFrom = obj.dateFrom,
                    dateTo = obj.dateTo,
                    branchId = obj.branchId,
                };
                if (obj.chkAllYear == false)
                {
                    // استدعاء الرصيد للسنة الحالية
                    items = await accRepositoryManager.AccReportsRepository.GetSupplierCurrentYearBalance(accountTransactionsDto);



                    if (items == null || !items.Any())
                    {
                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataCurrentYearBalance"));
                    }
                }
                else
                {
                    // استدعاء الرصيد لكل السنوات
                    items = await accRepositoryManager.AccReportsRepository.GetSupplierAllYearBalance(accountTransactionsDto);


                    if (items == null || !items.Any())
                    {
                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataAllYearBalance"));
                    }
                }


                return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("success"));
            }
            catch (Exception ex)
            {

                return await Result<IEnumerable<AccountBalanceSheetDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
            ;
        }
        #endregion

        #region =====================================  كشف حساب  مورد  
        public async Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetSupplierAccounttransactions(SupplierAccountStatementDto obj)
        {
            try
            {

                var AccountID = await mainRepositoryManager.SysCustomerRepository.GetOne(s => s.Id, x => x.Code == obj.AccountCode && x.CusTypeId == 1 && x.FacilityId == currentData.FacilityId && x.IsDeleted == false);
                var CCid = await accRepositoryManager.AccCostCenterRepository.GetOne(s => s.CcId, x => x.CostCenterCode == obj.CostCenterCode && x.FacilityId == currentData.FacilityId && x.IsDeleted == false);


                IEnumerable<AccountBalanceSheetDto> items;
                var accountTransactionsDto = new AccounttransactionsDto
                {
                    facilityId = currentData.FacilityId,
                    FinYear = currentData.FinYear,
                    ReferenceDNo = AccountID != 0 ? AccountID : 0,
                    ParentReferenceTypeId = 3,
                    ccId = CCid != 0 ? CCid : 0,
                    ReferenceTypeId = obj.ReferenceTypeId,
                    dateFrom = obj.dateFrom,
                    dateTo = obj.dateTo,
                    branchId = obj.branchId,
                };

                if (obj.chkAllYear == false)
                {
                    // استدعاء الرصيد للسنة الحالية
                    items = await accRepositoryManager.AccReportsRepository.GetSupplierCurrentYearBalance(accountTransactionsDto);



                    if (items == null || !items.Any())
                    {
                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataCurrentYearBalance"));
                    }
                }
                else
                {
                    // استدعاء الرصيد لكل السنوات
                    items = await accRepositoryManager.AccReportsRepository.GetSupplierAllYearBalance(accountTransactionsDto);


                    if (items == null || !items.Any())
                    {
                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataAllYearBalance"));
                    }
                }


                return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("success"));
            }
            catch (Exception ex)
            {

                return await Result<IEnumerable<AccountBalanceSheetDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
            ;
        }

        #endregion

        #region =====================================  كشف حساب مجموعة
        public async Task<IResult<IEnumerable<AccounttransactionsGroupDto>>> GetAccounttransactionsGroup(AccounttransactionsFilterGroupDto obj)
        {
            try
            {
                obj.facilityId = currentData.FacilityId;
                obj.FinYear = currentData.FinYear;
                IEnumerable<AccounttransactionsGroupDto> items = await accRepositoryManager.AccReportsRepository.GetAccounttransactionsGroup(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<AccounttransactionsGroupDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<AccounttransactionsGroupDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<AccounttransactionsGroupDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }

        #endregion

        #region =====================================  كشف حساب  مركز تكلفة
        public async Task<IResult<IEnumerable<CostcentertransactionsDto>>> GetCostcentertransactions(CostcentertransactionsFilterDto obj)
        {
            try
            {
                obj.facilityId = currentData.FacilityId;
                obj.FinYear = currentData.FinYear;
                var CCid = await accRepositoryManager.AccCostCenterRepository.GetOne(s => s.CcId, x => x.CostCenterCode == obj.CostCenterCode && x.FacilityId == currentData.FacilityId && x.IsDeleted == false);
                obj.ccId = CCid;
                IEnumerable<CostcentertransactionsDto> items = await accRepositoryManager.AccReportsRepository.GetCostcentertransactions(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<CostcentertransactionsDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<CostcentertransactionsDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<CostcentertransactionsDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }
        #endregion
        #region =====================================  كشف حساب بتاريخ العملية
        public async Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetAccountStatementTransactionDate(AccountTransactionDateFilterDto obj)
        {
            try
            {
                obj.facilityId = currentData.FacilityId;
                obj.FinYear = currentData.FinYear;
                obj.accountId = await accRepositoryManager.AccAccountRepository.GetAccountIdByCode(obj.AccountCode, currentData.FacilityId);
                IEnumerable<AccountBalanceSheetDto> items;

                if (obj.chkAllYear == false)
                {
                    // استدعاء الرصيد للسنة الحالية
                    items = await accRepositoryManager.AccReportsRepository.GetAccountStatementTransactionDateYear(obj);



                    if (items == null || !items.Any())
                    {
                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataCurrentYearBalance"));
                    }
                }
                else
                {
                    // استدعاء الرصيد لكل السنوات
                    items = await accRepositoryManager.AccReportsRepository.GetAccountStatementTransactionDateAllYear(obj);


                    if (items == null || !items.Any())
                    {

                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataAllYearBalance"));
                    }
                }


                return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("success"));
            }
            catch (Exception ex)
            {

                return await Result<IEnumerable<AccountBalanceSheetDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }


        #endregion

        #region =====================================  كشف حساب بالعملة الأجنبية

        public async Task<IResult<IEnumerable<AccountBalanceSheetDto>>> GetCurrencytransactions(CurrencytransactionsFilterDto obj)
        {
            try
            {
                obj.facilityId = currentData.FacilityId;
                obj.FinYear = currentData.FinYear;
                if (obj.ReferenceTypeId == 1)
                {
                    obj.accountId = await accRepositoryManager.AccAccountRepository.GetAccountIdByCode(obj.AccountCode, currentData.FacilityId);

                }
                else if (obj.ReferenceTypeId == 2)
                {
                    obj.ReferenceDNo = await mainRepositoryManager.SysCustomerRepository.GetOne(s => s.Id, x => x.Code == obj.AccountCode && x.CusTypeId == 2 && x.FacilityId == currentData.FacilityId && x.IsDeleted == false);
                    obj.ParentReferenceTypeId = 2;
                }
                else if (obj.ReferenceTypeId == 3)
                {
                    obj.ReferenceDNo = await mainRepositoryManager.SysCustomerRepository.GetOne(s => s.Id, x => x.Code == obj.AccountCode && x.CusTypeId == 3 && x.FacilityId == currentData.FacilityId && x.IsDeleted == false);
                    obj.ParentReferenceTypeId = 3;
                }
                IEnumerable<AccountBalanceSheetDto> items;

                if (obj.chkAllYear == false)
                {
                    // استدعاء الرصيد للسنة الحالية
                    items = await accRepositoryManager.AccReportsRepository.GetCurrencytransactionsCurrentYearBalance(obj);



                    if (items == null || !items.Any())
                    {
                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataCurrentYearBalance"));
                    }
                }
                else
                {
                    // استدعاء الرصيد لكل السنوات
                    items = await accRepositoryManager.AccReportsRepository.GetCurrencytransactionsAllYearBalance(obj);


                    if (items == null || !items.Any())
                    {

                        return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("NoDataAllYearBalance"));
                    }
                }


                return await Result<IEnumerable<AccountBalanceSheetDto>>.SuccessAsync(items, localization.GetMessagesResource("success"));
            }
            catch (Exception ex)
            {

                return await Result<IEnumerable<AccountBalanceSheetDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }

        #endregion


        #region =====================================  كشف حساب مجموعة مركز تكلفة


        public async Task<IResult<IEnumerable<CostcentertransactionsGroupDto>>> GetCostcentertransactionsGroup(CostcentertransactionsGroupFilterDto obj)
        {
            try
            {
                obj.facilityId = currentData.FacilityId;
                obj.FinYear = currentData.FinYear;
                var CCid = await accRepositoryManager.AccCostCenterRepository.GetOne(s => s.CcId, x => x.CostCenterCode == obj.CostCenterCode && x.FacilityId == currentData.FacilityId && x.IsDeleted == false && x.IsActive == true);
                obj.ccId = CCid;
                IEnumerable<CostcentertransactionsGroupDto> items = await accRepositoryManager.AccReportsRepository.GetCostcentertransactionsGroup(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<CostcentertransactionsGroupDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<CostcentertransactionsGroupDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<CostcentertransactionsGroupDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }


        #endregion

        #region =====================================  كشف حساب مركز تكلفة من الى 

        public async Task<IResult<IEnumerable<CostcenterTransactionsFromToDto>>> GetCostcenterTransactionsFromTo(CostcenterTransactionsFromToFilterDto obj)
        {
            try
            {
                obj.facilityId = currentData.FacilityId;
                obj.FinYear = currentData.FinYear;
                IEnumerable<CostcenterTransactionsFromToDto> items = await accRepositoryManager.AccReportsRepository.GetCostcenterTransactionsFromTo(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<CostcenterTransactionsFromToDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<CostcenterTransactionsFromToDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<CostcenterTransactionsFromToDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }


        #endregion



        #region ========================================== ميزان المراجعة

        public async Task<IResult<IEnumerable<TrialBalanceSheetDtoResult>>> GetTrialBalanceSheet(TrialBalanceSheetDto obj)
        {
            try
            {
                obj.facilityId = currentData.FacilityId;
                obj.finYear = currentData.FinYear;
                var items = await mainRepositoryManager.StoredProceduresRepository.GetTrialBalanceSheet(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<TrialBalanceSheetDtoResult>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<TrialBalanceSheetDtoResult>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<TrialBalanceSheetDtoResult>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }


        #endregion ================================== ميزان المراجعة



        #region =====================================   كشف حساب العملاء من رقم الى رقم 

        public async Task<IResult<IEnumerable<CustomerTransactionDto>>> GetCustomerTransactionsFromTo(CustomerTransactionFilterDto obj)
        {
            try
            {
                obj.FacilityId = currentData.FacilityId;
                obj.FinYear = currentData.FinYear;
                IEnumerable<CustomerTransactionDto> items = await accRepositoryManager.AccReportsRepository.GetCustomerTransactionsFromTo(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<CustomerTransactionDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<CustomerTransactionDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<CustomerTransactionDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }
        #endregion



        #region ========================================== الاستاذ العام

        public async Task<IResult<IEnumerable<GeneralLedgerDtoResult>>> GetGeneralLedger(GeneralLedgerDto obj)
        {
            try
            {
                obj.facilityId = currentData.FacilityId;
                var items = await accRepositoryManager.AccReportsRepository.GetGeneralLedger(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<GeneralLedgerDtoResult>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<GeneralLedgerDtoResult>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<GeneralLedgerDtoResult>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }




        #endregion ================================== الاستاذ العام

        #region ========================================== قائمة الدخل

        public async Task<IResult<IEnumerable<IncomeStatementDtoResult>>> GetIncomeStatement(IncomeStatementDto obj)
        {
            try
            {
                obj.FacilityId = currentData.FacilityId;
                var items = await accRepositoryManager.AccReportsRepository.GetIncomeStatement(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<IncomeStatementDtoResult>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<IncomeStatementDtoResult>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<IncomeStatementDtoResult>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }

        public async Task<IResult<IEnumerable<IncomeStatementDetailsDtoResult>>> IncomeStatementDetails(IncomeStatementDetailsDto obj)
        {
            try
            {
                obj.FacilityId = currentData.FacilityId;
                var items = await accRepositoryManager.AccReportsRepository.IncomeStatementDetails(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<IncomeStatementDetailsDtoResult>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<IncomeStatementDetailsDtoResult>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<IncomeStatementDetailsDtoResult>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }



        #endregion ================================== قائمة الدخل


        #region ==========================================   قائمة المركز المالي 

        //public async Task<IResult<IEnumerable<FinancialCenterListDtoResult>>> FinancialCenterList(FinancialCenterListDto obj)
        //{
        //    try
        //    {
        //        obj.FacilityId = currentData.FacilityId;
        //        obj.FinYear = currentData.FinYear;
        //        var items = await accRepositoryManager.AccReportsRepository.FinancialCenterList(obj);

        //        if (items == null || !items.Any())
        //        {
        //            return await Result<IEnumerable<FinancialCenterListDtoResult>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
        //        }

        //        return await Result<IEnumerable<FinancialCenterListDtoResult>>.SuccessAsync(items);
        //    }
        //    catch (Exception ex)
        //    {
        //        return await Result<IEnumerable<FinancialCenterListDtoResult>>.FailAsync($"An error occurred: {ex.Message}");
        //    }
        //}

        public async Task<IResult<IEnumerable<FinancialCenterListBindDataDtoResult>>> FinancialCenterListBindData(FinancialCenterListBindDataDto obj)
        {
            try
            {
                obj.FacilityId = currentData.FacilityId;
                obj.FinYear = currentData.FinYear;
                var items = await accRepositoryManager.AccReportsRepository.FinancialCenterListBindData(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<FinancialCenterListBindDataDtoResult>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<FinancialCenterListBindDataDtoResult>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<FinancialCenterListBindDataDtoResult>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }



        #endregion ==================================  قائمة المركز المالي



        #region ==========================================   قائمة الدخل شهري

        public async Task<IResult<IEnumerable<IncomeStatementMonthResultDto>>> GetIncomeStatementMonth(IncomeStatementMonthtDto obj)
        {
            try
            {
                obj.facilityId = currentData.FacilityId;
                obj.finYear ??= 0;
                obj.ccId ??= 0;
                var FinyearGregorian = await accRepositoryManager.AccFinancialYearRepository.GetOne(s => s.FinYearGregorian, x => x.FinYear == obj.finYear);
                obj.FinyearGregorian = FinyearGregorian.ToString();
                var items = await mainRepositoryManager.StoredProceduresRepository.GetIncomeStatementMonth(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<IncomeStatementMonthResultDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<IncomeStatementMonthResultDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<IncomeStatementMonthResultDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }

        #endregion ==================================  قائمة الدخل شهري

        #region ==========================================  الأرباح والخسائر

        public async Task<IResult<IEnumerable<ProfitandLossResultDto>>> GetProfitandLoss(ProfitandLossDto obj)
        {
            try
            {

                obj.ccId ??= 0;
                obj.AccountLevel ??= 0;
                var items = await mainRepositoryManager.StoredProceduresRepository.GetProfitandLoss(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<ProfitandLossResultDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<ProfitandLossResultDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<ProfitandLossResultDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }





        #endregion ==================================  الأرباح والخسائر


        #region ==========================================  قائمة التدفقات النقدية


        public async Task<IResult<IEnumerable<CashFlowsResultDto>>> GetCashFlows(CashFlowsDto obj)
        {
            try
            {


                var items = await mainRepositoryManager.StoredProceduresRepository.GetCashFlows(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<CashFlowsResultDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<CashFlowsResultDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<CashFlowsResultDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }


        #endregion ==========================================  قائمة التدفقات النقدية



        #region ==========================================  اعمار الديون

        public async Task<IResult<IEnumerable<AgedReceivablesResultDto>>> GetAgedReceivables(AgedReceivablesDto obj)
        {
            try
            {

                long Empid = 0;

                if (currentData.SalesType.ToString() != "0")
                {
                    Empid = currentData.EmpId;
                }
                else
                {
                    Empid = await hrRepositoryManager.HrEmployeeRepository.GetEmpId(currentData.FacilityId, obj.EmpCode);

                }
                obj.EmpID = Empid;
                obj.facilityId = currentData.FacilityId;
                obj.BRANCHID ??= 0;
                obj.GroupID ??= 0;
                var items = await mainRepositoryManager.StoredProceduresRepository.GetAgedReceivables(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<AgedReceivablesResultDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<AgedReceivablesResultDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<AgedReceivablesResultDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }


        #endregion ==========================================  اعمار الديون

        #region ==========================================  أعمار الديون - شهري

        public async Task<IResult<IEnumerable<AgedReceivablesMonthlyResultDto>>> GetAgedReceivablesMonthly(AgedReceivablesMonthlyDto obj)
        {
            try
            {

                long Empid = 0;

                if (currentData.SalesType.ToString() != "0")
                {
                    Empid = currentData.EmpId;
                }
                else
                {
                    Empid = await hrRepositoryManager.HrEmployeeRepository.GetEmpId(currentData.FacilityId, obj.EmpCode);

                }
                obj.EmpID = Empid;
                obj.facilityId = currentData.FacilityId;
                obj.BRANCHID ??= 0;
                obj.GroupID ??= 0;
                var items = await mainRepositoryManager.StoredProceduresRepository.GetAgedReceivablesMonthly(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<AgedReceivablesMonthlyResultDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<AgedReceivablesMonthlyResultDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<AgedReceivablesMonthlyResultDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }


        #endregion ==========================================  أعمار الديون - شهري


        #region ==========================================  مقارنة بالسنوات

        public async Task<IResult<IEnumerable<CompareyearsDtoResultDto>>> GetBudgetEstimateCompareyears(CompareyearsDto obj)
        {
            try
            {

                obj.Finyear ??= 0;
                obj.Finyear2 ??= 0;
                obj.Finyear3 ??= 0;
                obj.FacilityID = currentData.FacilityId;
                obj.BranchID ??= 0;
                var items = await mainRepositoryManager.StoredProceduresRepository.GetBudgetEstimateCompareyears(obj);

                if (items == null || !items.Any())
                {
                    return await Result<IEnumerable<CompareyearsDtoResultDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<CompareyearsDtoResultDto>>.SuccessAsync(items);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<CompareyearsDtoResultDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }


        #endregion ================================== مقارنة بالسنوات


        #region ========================================== تقارير احصائي



        //public async Task<IResult<IEnumerable<DashboardResultDto>>> GetDashboardData(DashboardRequestDto obj)
        //{

        //    try
        //    { 

        //        var DashboardData = new DashboardResultDto();
        //        obj.FacilityId = currentData.FacilityId;
        //        obj.FinYear = currentData.FinYear;
        //        obj.UserId = currentData.UserId;
        //        obj.EmpId = currentData.EmpId;
        //        obj.BranchId = currentData.BranchId;
        //        var items = await mainRepositoryManager.StoredProceduresRepository.GetDashboardData(obj);

        //        obj.CmdType = 3;

        //        var items3 = await mainRepositoryManager.StoredProceduresRepository.GetDashboardData(obj);



        //        obj.CmdType = 4;

        //        var items4 = await mainRepositoryManager.StoredProceduresRepository.GetDashboardData(obj);




        //        obj.CmdType = 5;

        //        var items5 = await mainRepositoryManager.StoredProceduresRepository.GetDashboardData(obj);


        //        if (items == null || !items.Any())
        //        {
        //            return await Result<IEnumerable<DashboardResultDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
        //        }
        //        if (items == null || !items.Any())
        //        {
        //            return await Result<IEnumerable<DashboardResultDto>>.SuccessAsync(items, localization.GetResource1("NosearchResult"));
        //        }



        //        return await Result<IEnumerable<DashboardResultDto>>.SuccessAsync(items);
        //    }
        //    catch (Exception ex)
        //    {
        //        return await Result<IEnumerable<DashboardResultDto>>.FailAsync($"An error occurred: {ex.Message}");
        //    }
        //}
        public async Task<IResult<IEnumerable<DashboardResultDto>>> GetDashboardData(DashboardRequestDto obj)
        {
            try
            {
                

                var result = new List<DashboardResultDto>();
                var dashboardResult = new DashboardResultDto();

                // 1- ملخص الأرصدة
                var items = await mainRepositoryManager.StoredProceduresRepository.GetDashboardData(obj,1);
                if (items != null && items.Any())
                {
                    dashboardResult.DashboardStatusResultDto = items.Select(item => new DashboardStatusResultDto
                    {
                        Cnt = item.Cnt,
                        Name = item.Name,
                        Name2 = item.Name2,
                        Color = item.Color,
                        Icon = item.Icon,
                        Url = item.Url
                    }).ToList();
                }

                // 2- إحصائيات المصاريف (type = 3)
                var expenseItems = await mainRepositoryManager.StoredProceduresRepository.GetDashboardData(obj,3);
                if (expenseItems != null && expenseItems.Any())
                {
                    dashboardResult.DashboardChartDataExpenses = expenseItems.Select(x => new DashboardChartData
                    {
                        Labels = x.Name,
                        Dataset = x.Cnt.ToString()
                    }).ToList();
                }

                // 3- إحصائيات الإيرادات (type = 4)
                var incomeItems = await mainRepositoryManager.StoredProceduresRepository.GetDashboardData(obj,4);
                if (incomeItems != null && incomeItems.Any())
                {
                    dashboardResult.DashboardChartDataRevenues = incomeItems.Select(x => new DashboardChartData
                    {
                        Labels = x.Name,
                        Dataset = x.Cnt.ToString()
                    }).ToList();
                }

                // 4- التقديري بالفعلي (type = 5)
                var budgetVsActualItems = await mainRepositoryManager.StoredProceduresRepository.GetDashboardDataEstimatedactual(obj,5);
                if (budgetVsActualItems != null && budgetVsActualItems.Any())
                {

                    dashboardResult.DashboardChartDataEstimatedactual = budgetVsActualItems.Select(item => new DashboardEstimatedactualResultDto
                    {
                        AccAccountCode = item.AccAccountCode,
                        AccAccountName = item.AccAccountName,
                        AccAccountId = item.AccAccountId,
                        Budget = item.Budget,
                        Actual = item.Actual,
                    }).ToList();






                }

                // إضافة النتيجة إلى القائمة إذا كانت تحتوي على بيانات
                if ((dashboardResult.DashboardStatusResultDto != null && dashboardResult.DashboardStatusResultDto.Any()) ||
                    (dashboardResult.DashboardChartDataExpenses != null && dashboardResult.DashboardChartDataExpenses.Any()) ||
                    (dashboardResult.DashboardChartDataRevenues != null && dashboardResult.DashboardChartDataRevenues.Any()) ||
                    (dashboardResult.DashboardChartDataEstimatedactual != null && dashboardResult.DashboardChartDataEstimatedactual.Any()))
                {
                    result.Add(dashboardResult);
                }

                if (!result.Any())
                {
                    return await Result<IEnumerable<DashboardResultDto>>.SuccessAsync(result, localization.GetResource1("NosearchResult"));
                }

                return await Result<IEnumerable<DashboardResultDto>>.SuccessAsync(result);
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<DashboardResultDto>>.FailAsync($"An error occurred: {ex.Message}");
            }
        }


        #endregion ========================================== تقارير احصائي


    }
}
