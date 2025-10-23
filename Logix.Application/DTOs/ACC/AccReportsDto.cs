using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Logix.Application.DTOs.GB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
    #region =====================================   كشف حساب العملاء من رقم الى رقم 

    public class CustomerTransactionDataDto
    {
        public long Id { get; set; }                  // معرف العميل
        public string Name { get; set; }              // اسم العميل
        public string Name2 { get; set; }             // الاسم الثاني (إذا كان موجودًا)
        public string Code { get; set; }              // كود العميل
        public string CollectorName { get; set; }    // اسم المحصل (اختياري، حسب الحاجة)
    }

    public class CustomerTransactionFilterDto
    {
        public long FacilityId { get; set; }         
        public int? BranchId { get; set; }
        public long FinYear { get; set; }
        public long? GroupId { get; set; }            
        public string StartCode { get; set; }         
        public string EndCode { get; set; }           
        public string StartDate { get; set; }        
        public string EndDate { get; set; }          
        public long? EmpId { get; set; }             
        public bool NoZero { get; set; }              
    }

    public class CustomerTransactionDto
    {
        public string Code { get; set; }             
        public string Name { get; set; }            
        public string Name2 { get; set; }            
        public string CollectorName1 { get; set; }   
        public decimal AmountPrev { get; set; }     
        public decimal Debit { get; set; }            
        public decimal Credit { get; set; }         
        public decimal AmountNext { get; set; }       
    }

    #endregion

    #region =====================================   كشف حساب الموردين من الى 

    public class CustomerSupTransactionDataDto
    {
        public long Id { get; set; }                  // معرف العميل
        public string Name { get; set; }              // اسم العميل
        public string Name2 { get; set; }             // الاسم الثاني (إذا كان موجودًا)
        public string Code { get; set; }              // كود العميل
        public string CollectorName { get; set; }    // اسم المحصل (اختياري، حسب الحاجة)
    }

    public class CustomerSupTransactionFilterDto
    {
        public long FacilityId { get; set; }         
        public int? BranchId { get; set; }
        public long FinYear { get; set; }
        public long? GroupId { get; set; }            
        public string StartCode { get; set; }         
        public string EndCode { get; set; }           
        public string StartDate { get; set; }        
        public string EndDate { get; set; }          
        public long? EmpId { get; set; }             
        public bool NoZero { get; set; }              
    }

    public class CustomerSupTransactionDto
    {
        public string Code { get; set; }             
        public string Name { get; set; }            
        public string Name2 { get; set; }            
        public string CollectorName1 { get; set; }   
        public decimal AmountPrev { get; set; }     
        public decimal Debit { get; set; }            
        public decimal Credit { get; set; }         
        public decimal AmountNext { get; set; }       
    }

    #endregion


    #region =====================================  كشف حساب من الى
    public class AccounttransactionsFromToFilterDto
    {

        public long facilityId { get; set; }

        public string? AccountCode { get; set; }
        public string? AccountCode2 { get; set; }
        public long FinYear { get; set; }

        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
        public int? branchId { get; set; }
        public string? CostCenterCodeFrom { get; set; }
        public string? CostCenterCodeTo { get; set; }
    }
    public class AccounttransactionsFromToDto
    {
        public string AccAccountName { get; set; }
        public string AccAccountName2 { get; set; }
        public string AccAccountCode { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }

        public decimal? Rate { get; set; }
    }
    #endregion

    #region =====================================  كشف حساب بتاريخ العملية
    public class AccountTransactionDateFilterDto
    {
        public string? AccountCode { get; set; }
        public string? CostCenterCode { get; set; }
        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
        public int? branchId { get; set; }
        public int? CurrencyId { get; set; }
        public long? accountId { get; set; }
        public long facilityId { get; set; }
        public long FinYear { get; set; }
        public bool chkAllYear { get; set; } = false;
        public long? ReferenceDNo { get; set; }
        public int? ParentReferenceTypeId { get; set; }
        public long? ccId { get; set; }
        public int? ReferenceTypeId { get; set; }
    }

    #endregion

    #region =====================================  كشف حساب بالعملة الأجنبية
    public class CurrencytransactionsFilterDto
    {
        public string? AccountCode { get; set; }
        public string? CostCenterCode { get; set; }
        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
        public int? branchId { get; set; }
        public long? accountId { get; set; }
        public long facilityId { get; set; }
        public long FinYear { get; set; }
        public bool chkAllYear { get; set; } = false;
        public long? ReferenceDNo { get; set; }
        public int? ParentReferenceTypeId { get; set; }
        public long? ccId { get; set; }
        public int? ReferenceTypeId { get; set; }

        public int? CurrencyId { get; set; }
    }
    #endregion


    #region =====================================  كشف حساب مجموعة مركز تكلفة
    public class CostcentertransactionsGroupFilterDto
    {
        public string? AccountCode { get; set; }
        public string? CostCenterCode { get; set; }
        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
        public int? branchId { get; set; }
        public long? accountId { get; set; }
        public long facilityId { get; set; }
        public long FinYear { get; set; }
        public bool chkAllYear { get; set; } = false;
        public long? ccId { get; set; }
    }

    public class CostcentertransactionsGroupDto
    {
        public string CostCenterName { get; set; }
        public string CostCenterName2 { get; set; }
        public string CostCenterCode { get; set; }
        public int RowID { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }

        public decimal? Net { get; set; }
    }
    #endregion

    #region =====================================  كشف حساب  مركز تكلفة
    public class CostcentertransactionsFilterDto
    {
        public string? AccountCode { get; set; }
        public string? CostCenterCode { get; set; }
        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
        public int? branchId { get; set; }
        public long? accountId { get; set; }
        public long facilityId { get; set; }
        public long FinYear { get; set; }
        public bool chkAllYear { get; set; } = false;
        public long? ccId { get; set; }
        public string? AccGrouplist { get; set; }
    }

    public class CostcentertransactionsDto
    {
        public int RowID { get; set; }
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
        public decimal? Balance { get; set; }
        public string? JDateGregorian { get; set; }
        public string? ReferenceDNo { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }
        public int? SortNo { get; set; }
        public string? Description { get; set; }
        public string? CostCenterName2 { get; set; }
        public string? CostCenterName { get; set; }
        public string? JCode { get; set; }
        public string? DocTypeName { get; set; }
        public string? DocTypeName2 { get; set; }
        public string? ReferenceCode { get; set; }
        public string? ReferenceTypeName { get; set; }
        public string? ReferenceTypeName2 { get; set; }
        public int? NatureAccount { get; set; }
        public int? DocTypeId { get; set; }
        public long? JId { get; set; }
        public long? JDetailesId { get; set; }
        public long? ReferenceNo { get; set; }
        public decimal? CostsCenter { get; set; }
        public string? ChequNo { get; set; }
    }
    #endregion

    #region =====================================  كشف حساب مركز تكلفة من الى 
    public class CostcenterTransactionsFromToFilterDto
    {
        public string? dateFrom { get; set; } 
        public string? dateTo { get; set; } 
        public long? facilityId { get; set; } 
        public long? FinYear { get; set; } 
        public string? AccountCode { get; set; } 
        public string? AccountCode2 { get; set; } 
        public string? CostCenterCodeFrom { get; set; } 
        public string? CostCenterCodeTo { get; set; } 
        public string? AccGrouplist { get; set; } 
    }

    public class CostcenterTransactionsFromToDto
    {
        public string? CostCenterCode { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? CostCenterName { get; set; }  
        public string? CostCenterName2 { get; set; }  
        public decimal? Debit { get; set; } 
        public decimal? Credit { get; set; } 
        public decimal? Balance { get; set; } 

    }



    #endregion


    #region ========================================== ميزان المراجعة

    public class TrialBalanceSheetDto
    {
        public string? periodStartDate { get; set; }
        public  string? periodEndDate { get; set; }
        public long? facilityId { get; set; }
        public int? branchId { get; set; }
        public long? finYear { get; set; }
        public int? accountLevel { get; set; }
        public bool noZero { get; set; }
        public string? accountFrom { get; set; }
        public string? accountTo { get; set; }
        public string? ccFrom { get; set; }
        public string? ccTo { get; set; }
        public int? showAllLevel { get; set; }
   

    }


    public class TrialBalanceSheetDtoResult
    {
        public bool? IsSub { get; set; }
        public long? AccAccountId { get; set; }  // تغيير إلى long? لأن القيمة قد تكون NULL
        public string? AccAccountName { get; set; }
        public string? AccAccountName2 { get; set; }
        public string? AccAccountCode { get; set; }
        public int? AccountLevel { get; set; }
        public int? NatureAccount { get; set; }  // تغيير إلى int? لأن القيمة قد تكون NULL
        public long? AccGroupId { get; set; }  // تغيير إلى long? لأن القيمة قد تكون NULL
        public decimal? AMOUNTPrev { get; set; }  // تغيير إلى decimal? لأن القيمة قد تكون NULL
        public decimal? Debit { get; set; }  // تغيير إلى decimal? لأن القيمة قد تكون NULL
        public decimal? Credit { get; set; }  // تغيير إلى decimal? لأن القيمة قد تكون NULL
        public decimal? AMOUNTNext { get; set; }  // تغيير إلى decimal? لأن القيمة قد تكون NULL
        public decimal? PrevDebit { get; set; }
        public decimal? NextDebit { get; set; }
        public TrialBalanceTransactionDto Transactions { get; set; }
    }


    public class TrialBalanceTransactionDto
    {
        public decimal? AMOUNTPrev { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? AMOUNTNext { get; set; }
    }

    #endregion ================================== ميزان المراجعة
    #region ========================================== الاستاذ العام
    public class GeneralLedgerDto
    {
        public long? facilityId { get; set; }

        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

    }
    public class GeneralLedgerDtoResult
    {
        public string AccAccountName { get; set; }
        public string AccAccountName2 { get; set; }
        public string AccAccountCode { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }

    }


    #endregion ================================== الاستاذ العام

    #region ========================================== قائمة الدخل

    public class IncomeStatementDtoResult
    {
        public bool? IsSub { get; set; }
        public decimal? Value { get; set; }
        public long? accountId { get; set; }

        public string? AccAccountCode { get; set; }
        public string? AccAccountName { get; set; }
        public string? AccAccountName2 { get; set; }
    }
    public class IncomeStatementDto
    {
        
        public string? JDateFrom { get; set; }
        public string? JDateTo { get; set; }
        public long? FacilityId { get; set; }
        public int? BranchId { get; set; }
        public int? FinYear { get; set; }
        public int? AccountLevel { get; set; }
        public string? CCCodeFrom { get; set; }
        public string? CCCodeTo { get; set; }
        public long? GroupIncome { get; set; }
        public long? GroupExpenses { get; set; }
    }
    public class IncomeStatementDetailsDto
    {
        public long? accountId { get; set; }
        public long? ccId { get; set; }
        public string? JDateFrom { get; set; }
        public string? JDateTo { get; set; }
        public long? FacilityId { get; set; }
    }
    public class IncomeStatementDetailsDtoResult
    {
        public int RowId { get; set; }
        public string JDateHijri { get; set; }
        public string Description { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }
        public string CostCenterName { get; set; }
        public string JCode { get; set; }
        public int? NatureAccount { get; set; }
    }

    #endregion ================================== قائمة الدخل


    #region ==========================================   قائمة المركز المالي
    public class FinancialCenterListBindDataDto
    {
        public long? accountId { get; set; }
        public long? ccId { get; set; }
        public string? JDateFrom { get; set; }
        public string? JDateTo { get; set; }
        public long? FacilityId { get; set; }
        public int? BranchId { get; set; }
        public long FinYear { get; set; }
        public int? AccountLevel { get; set; }

        public long CMDTYPE { get; set; }
    }
    public class FinancialCenterListBindDataDtoResult
    {
        public long AccAccountId { get; set; } // رقم الحساب
        public string AccAccountName { get; set; } // اسم الحساب
        public string AccAccountName2 { get; set; } // اسم الحساب

        public string AccAccountCode { get; set; } // كود الحساب
        public int? AccountLevel { get; set; } // مستوى الحساب
        public int? NatureAccount { get; set; } // طبيعة الحساب (دائن أو مدين)
        public long? AccGroupId { get; set; } // معرف المجموعة المرتبطة بالحساب
        public decimal? Net { get; set; } // صافي الرصيد (الحركات)
        public List<FinancialCenterListDtoResult> Details { get; set; }
        public FinancialCenterListBindDataDtoResult()
        {
            Details = new List<FinancialCenterListDtoResult>();

        }
    }


    public class FinancialCenterListDto
    {
        public long? accountId { get; set; }
        public long? ccId { get; set; }
        public string? JDateFrom { get; set; }
        public string? JDateTo { get; set; }
        public long? FacilityId { get; set; }
        public int? BranchId { get; set; }
        public long FinYear { get; set; }
        public long CMDTYPE { get; set; }
    }
    public class FinancialCenterListDtoResult
    {
        public long AccAccountId { get; set; } // رقم الحساب
        public string AccAccountName { get; set; } // اسم الحساب
        public string AccAccountCode { get; set; } // كود الحساب
        public int? AccountLevel { get; set; } // مستوى الحساب
        public int? NatureAccount { get; set; } // طبيعة الحساب (دائن أو مدين)
        public long? AccGroupId { get; set; } // معرف المجموعة المرتبطة بالحساب
        public decimal? Net { get; set; } // صافي الرصيد (الحركات)
       
    }


    #endregion ==================================  قائمة المركز المالي

    #region ==========================================   قائمة الدخل شهري
    public class IncomeStatementMonthtDto
    {
        public string? FinyearGregorian { get; set; }

        public long? facilityId { get; set; }

        public long? finYear { get; set; }
        public long? ccId { get; set; }



    }
    public class IncomeStatementMonthResultDto
    {
        public string AccountName { get; set; }
        public Dictionary<string, decimal> MonthlyValues { get; set; }
    }
    #endregion ==================================  قائمة الدخل شهري

    #region ==========================================   الأرباح والخسائر
    public class ProfitandLossDto
    {
        public int? AccountLevel { get; set; }

        public long? ccId { get; set; }
        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }



    }
    public class ProfitandLossResultDto
    {
        public string AccAccountName { get; set; }
        public string AccAccountName2 { get; set; }
        public string AccAccountCode { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
    }

    #endregion ==================================  الأرباح والخسائر

    #region ==========================================  قائمة التدفقات النقدية

    public class CashFlowsDto
    {

        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }



    }
    public class CashFlowsResultDto
    {
        public string Description { get; set; }

        public decimal? Amount { get; set; }
    }

    #endregion ==================================   قائمة التدفقات النقدية


    #region ==========================================  اعمار الديون
    public class AgedReceivablesDto
    {
        public string? CustomerCode { get; set; }
        public string? CustomerCode2 { get; set; }
        public long? BRANCHID { get; set; }
        public long? GroupID { get; set; }
        public string? CurDate { get; set; }
        public string? EmpCode { get; set; }
        public string? EmpName { get; set; }
        public long? EmpID { get; set; }
        public long? facilityId { get; set; }

    }
    public class AgedReceivablesResultDto
    {
          public string? CustomerCode { get; set; }
          public string? CustomerName { get; set; }
          public string? EmpName { get; set; }
           public decimal Balance { get; set; }
            public decimal BalanceDue { get; set; }
            public decimal BlanceNotDue { get; set; }
            public decimal Payment { get; set; }
            public decimal Amount1_90 { get; set; }
            public decimal Amount91_180 { get; set; }
            public decimal Amount181_270 { get; set; }
            public decimal AmountMoreThan270 { get; set; }
        public decimal Creditlimit { get; set; }
        public int DuePeriodDays { get; set; }



    }
    #endregion ==================================  اعمار الديون
    #region ========================================== أعمار الديون - شهري
    public class AgedReceivablesMonthlyDto
    {
        public string? CustomerCode { get; set; }
        public string? CustomerCode2 { get; set; }
        public long? BRANCHID { get; set; }
        public long? GroupID { get; set; }
        public string? CurDate { get; set; }
        public string? EmpCode { get; set; }
        public string? EmpName { get; set; }
        public long? EmpID { get; set; }
        public long? facilityId { get; set; }

    }
    public class AgedReceivablesMonthlyResultDto
    {
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? EmpName { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceDue { get; set; }
        public decimal BlanceNotDue { get; set; }
        public decimal Payment { get; set; }
        public decimal Amount1_90 { get; set; }
        public decimal Amount91_180 { get; set; }
        public decimal Amount181_270 { get; set; }
        public decimal AmountMoreThan270 { get; set; }
        public decimal Creditlimit { get; set; }
        public int DuePeriodDays { get; set; }



    }
    #endregion ==================================  أعمار الديون - شهري
    #region ==========================================مقارنة بالسنوات
    public class CompareyearsDto
    {
        public long? Finyear { get; set; }
        public long? Finyear2 { get; set; }
        public long?  Finyear3 { get; set; }
        public string? AccgroupID { get; set; }
        public long? FacilityID { get; set; }
        public long? BranchID{ get; set; }
        public string? AccAccountCode { get; set; }
        public string? AccAccountCode2 { get; set; }
        public string? CCCodeFrom { get; set; }
        public string? CCCodeTo { get; set; }

    }
    public class CompareyearsDtoResultDto
    {
        public string? AccAccountName { get; set; }

        public string? AccAccountCode { get; set; }
        public decimal NetYear1 { get; set; }
        public decimal NetYear2 { get; set; }

        public decimal NetYear3 { get; set; }





    }

   

    #endregion ================================== مقارنة بالسنوات 

    #region ========================================== تقارير احصائي
    public class DashboardRequestDto
    {
       
        public string CurrDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

    }
    public class DashboardResultDto
    {
        public List<DashboardStatusResultDto> DashboardStatusResultDto { get; set; }

        public List<DashboardChartData> DashboardChartDataExpenses { get; set; }
        public List<DashboardChartData> DashboardChartDataRevenues { get; set; }
        public List<DashboardEstimatedactualResultDto> DashboardChartDataEstimatedactual { get; set; }


    }

    public class DashboardStatusResultDto
    {
        public decimal Cnt { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }


    }
    public class DashboardChartData
    {
        public string Labels { get; set; }
        public string Dataset { get; set; }
    }
    public class DashboardEstimatedactualResultDto
    {
        public long AccAccountId { get; set; }

        public string? AccAccountName { get; set; }

        public string? AccAccountCode { get; set; }
        public decimal Budget { get; set; }
        public decimal Actual { get; set; }
    }

    #endregion ========================================== تقارير احصائي
}

