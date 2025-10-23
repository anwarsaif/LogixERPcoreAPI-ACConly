using Logix.Application.DTOs.Main;

using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Logix.Application.DTOs.ACC
{
    public class PayrollResultpopup
    {
        public int RowNumber { get; set; }
        public string? MsTitle { get; set; }
        public long? MsCode { get; set; }
        public string? MsDate { get; set; }
        public string? MonthName { get; set; }
        public int? FinancelYear { get; set; }
        public long MsId { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public decimal Amount { get; set; }
    }
    public class PayrollResultPopupFilter
    {
        public int? MsId { get; set; }

        public string? MsMonth { get; set; }
        public int? FinancialYear { get; set; }
        public int? PayrollTypeId { get; set; }
        public string? ApplicationCode { get; set; }
    }


    public class TransactionResult
    {
        public int RowNumber { get; set; }
        public string Code { get; set; }
        public string Date1 { get; set; } // أو DateTime إذا كانت Date1 من نوع DateTime
        public decimal Subtotal { get; set; }
        public decimal Paid { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public decimal Total { get; set; }
    }
    public class TransactionUnPaidResult
    {
        public int RowNumber { get; set; }
        public string Code { get; set; }
        public string Date1 { get; set; } // أو DateTime إذا كانت Date1 من نوع DateTime
        public decimal Subtotal { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? ProjectName { get; set; }
        public decimal? Paid { get; set; }
        public string? ParentCCCode { get; set; }
        public string? ParentCCName { get; set; }
        public long? CusTypeId { get; set; }
        public decimal Total { get; set; }
    }

    public class AccRequestFilterDto

    {
        
        public long? DepId { get; set; }
        public long? ReferenceNo { get; set; }

        
        public long? ReferenceTypeId { get; set; }
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        
        public long? AppCode { get; set; }

        
        public int? StatusId { get; set; }

        
        public int? TypeId { get; set; }

        
        public string? StartDate { get; set; } = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

        
        public string? EndDate { get; set; } = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture); //DateTime.ParseExact(DateTime.Now.ToString(), "yyyy/MM/dd", new CultureInfo("en-US")).ToString();
        public decimal? Amount { get; set; }
        
        public long? BranchId { get; set; }
        public string? Iban { get; set; }


        public string? CustomerName { get; set; }

        public string? IdNo { get; set; }

        public int? Status2Id { get; set; }

        public string? ApplicationCode { get; set; }

        public string? Description { get; set; }
        public int? TransTypeId { get; set; }
    }
    public class AccRequestPopDto
    {
        public long? AppCode { get; set; }
        public string? AppDate { get; set; }
        public string? DepName { get; set; }
        public decimal? Amount { get; set; }
        public int? TypeId { get; set; }

        public long? DepId { get; set; }

        public int? Status2Id { get; set; }

        public string? Amountwrite { get; set; }
        public long? ReferenceTypeId { get; set; }


        public string? AccAccountCode { get; set; }
        [StringLength(255)]

        public string? AccAccountName { get; set; }

        public string? CostCenterCode { get; set; }
        public long? BranchId { get; set; }
        public string? CostCenterName { get; set; }
        public string? IdNo { get; set; }
        public int? BankId { get; set; }

        public string? Iban { get; set; }

        public string? CustomerName { get; set; }

        public int? CustomerCont { get; set; }
        public long? AppId { get; set; }
        public long? CcId { get; set; }

        public string? Description { get; set; }

    }
    public class AccRequestPopFilterDto
    {
        public long? AppCode { get; set; }


    }
    public class AccRequestDto
    {

        public long Id { get; set; }

        public long? AppCode { get; set; }
        [Required]
        public string? AppDate { get; set; }
        public int? TypeId { get; set; }

        [Required]
        public long? DepId { get; set; }

        public decimal? Amount { get; set; }

        public string? Description { get; set; }
        public string? Note { get; set; }
        public string? RefranceNo { get; set; }

        public long? AccountId { get; set; }
        public int? StatusId { get; set; }

        public long? FinUserId { get; set; }

        public DateTime? FinDate { get; set; }

        public int? HasCredit { get; set; }

        public int? BalanceStatusId { get; set; }

        public string? FinNote { get; set; }

        public long? GmUserId { get; set; }

        public DateTime? GmDate { get; set; }

        public int? ExchangeStatusId { get; set; }

        public string? GmNote { get; set; }


        public bool IsDeleted { get; set; }

        public int? Status2Id { get; set; }

        public long? AppId { get; set; }

        public long? ReferenceNo { get; set; }
        [Required]
        public long? ReferenceTypeId { get; set; }
        [Required]
        public long? BranchId { get; set; }

        public long? FacilityId { get; set; }
        public long? JId { get; set; }


        public string? Amountwrite { get; set; }
        public long? CcId { get; set; }
        public int? TransTypeId { get; set; }
        public long? RefranceId { get; set; }
        [Range(1, long.MaxValue)]
        public int? CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }

        public string? IdNo { get; set; }


        public int? BankId { get; set; }


        public string? Iban { get; set; }



        public string? CustomerName { get; set; }


        public int? CustomerCont { get; set; }


        public decimal? DeductionsTotal { get; set; }

        public string? DeductionsNote { get; set; }

        public long? BadgetNo { get; set; }

        public string? AppDateH { get; set; }


        public string StartDate { get; set; } = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

        public string? EndDate { get; set; } = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture); //DateTime.ParseExact(DateTime.Now.ToString(), "yyyy/MM/dd", new CultureInfo("en-US")).ToString();
        public long? AppCodeS { get; set; }

        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }
        [StringLength(50)]

        public string? AccAccountCode { get; set; }
        [StringLength(255)]

        public string? AccAccountName { get; set; }

        public long ApplicantType { get; set; }
        public string? EmpCode { get; set; }
        public string? EmpName { get; set; }
        public string? EmpName2 { get; set; }
        public string? LinkCode { get; set; }
        public decimal? AmountTotal { get; set; }
        public decimal? ExpensesAmount { get; set; }

        public decimal? LinkAmount { get; set; }


        public decimal? LinkAmountInitial { get; set; }
        public decimal? ExRequestAmount { get; set; }
        public decimal? ExRemainingِِAmount { get; set; }



        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }

        public string? ApplicationCode { get; set; }
        public int? PaidStatus { get; set; }
        public int? AppTypeId { get; set; } = 0;
        public bool? ISMulti { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }
    }

    public class AccRequestEditDto
    {

        public long Id { get; set; }

        public long? AppCode { get; set; }

        public string? AppDate { get; set; }

        public int? TypeId { get; set; }

        [Required]
        public long? DepId { get; set; }
        public decimal? Amount { get; set; }

        public string? Description { get; set; }
        public string? Note { get; set; }

        public string? RefranceNo { get; set; }

        public long? AccountId { get; set; }

        public int? StatusId { get; set; }

        public long? FinUserId { get; set; }

        public DateTime? FinDate { get; set; }

        public int? HasCredit { get; set; }

        public int? BalanceStatusId { get; set; }

        public string? FinNote { get; set; }

        public long? GmUserId { get; set; }

        public DateTime? GmDate { get; set; }

        public int? ExchangeStatusId { get; set; }

        public string? GmNote { get; set; }


        public bool IsDeleted { get; set; }
        public int? Status2Id { get; set; }


        public long? ReferenceNo { get; set; }
        [Required]
        public long? ReferenceTypeId { get; set; }
        [Required]
        public long? BranchId { get; set; }

        public long? JId { get; set; }
        public string? Amountwrite { get; set; }
        public long? CcId { get; set; }
        [Range(1, long.MaxValue)]
        public int? CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public string? IdNo { get; set; }
        public int? BankId { get; set; }
        public string? Iban { get; set; }

        public string? CustomerName { get; set; }

        public int? CustomerCont { get; set; }

        public decimal? DeductionsTotal { get; set; }

        public string? DeductionsNote { get; set; }

        public long? BadgetNo { get; set; }

        public string? AppDateH { get; set; }
        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [StringLength(255)]
        public string? AccAccountName { get; set; }

        public long ApplicantType { get; set; }
        public string? EmpCode { get; set; }

        public string? EmpName { get; set; }
        public string? EmpName2 { get; set; }

        public string? LinkCode { get; set; }

        public decimal? AmountTotal { get; set; }

        public decimal? ExpensesAmount { get; set; }


        public decimal? LinkAmount { get; set; }



        public decimal? LinkAmountInitial { get; set; }

        public decimal? ExRequestAmount { get; set; }

        public decimal? ExRemainingِِAmount { get; set; }




        public string? ItemCode { get; set; }

        public string? ItemName { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }

    }

    public class InformationBankDto
    {
        public long? Id { get; set; }

        public int? BankId { get; set; }

        public string? BankAccount { get; set; }

        public string? IdNo { get; set; }

        public string? Name { get; set; }

        public string? Name2 { get; set; }

    }
    public class AccRequestPaymentDto
    {

        public long Id { get; set; }

        public long? AppCode { get; set; }
        [Required]
        public string? AppDate { get; set; }
        public int? TypeId { get; set; }

        [Required]
        public long? DepId { get; set; }

        public decimal? Amount { get; set; }


        public string? Description { get; set; }


        public string? RefranceNo { get; set; }

        public long? AccountId { get; set; }


        public int? Status2Id { get; set; }

        public long? AppId { get; set; }

        [Required]

        public long? ReferenceTypeId { get; set; }
        [Required]

        public long? BranchId { get; set; }




        public string? Amountwrite { get; set; }
        public long? CcId { get; set; }
        [Range(1, long.MaxValue)]

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }


        public string? IdNo { get; set; }


        public int? BankId { get; set; }

        public string? Iban { get; set; }


        public string? CustomerName { get; set; }

        public int? CustomerCont { get; set; }


        public decimal? DeductionsTotal { get; set; }

        public string? DeductionsNote { get; set; }

        public int? AppTypeId { get; set; } = 0;





        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }
        [StringLength(50)]

        public string? AccAccountCode { get; set; }
        [StringLength(255)]

        public string? AccAccountName { get; set; }


        public string? EmpCode { get; set; }
        public string? EmpName { get; set; }
        public string? EmpName2 { get; set; }


        public int? PaidStatus { get; set; }




        public string? Note { get; set; }

        public long? FacilityId { get; set; }

        public int? StatusId { get; set; }

        public long? FinUserId { get; set; }

        public DateTime? FinDate { get; set; }

        public int? HasCredit { get; set; }

        public int? BalanceStatusId { get; set; }

        public string? FinNote { get; set; }

        public long? GmUserId { get; set; }

        public DateTime? GmDate { get; set; }

        public int? ExchangeStatusId { get; set; }

        public string? GmNote { get; set; }




        public long? ReferenceNo { get; set; }



        public long? JId { get; set; }


        public int? TransTypeId { get; set; }
        public long? RefranceId { get; set; }



        public long? BadgetNo { get; set; }

        public string? AppDateH { get; set; }


        public string StartDate { get; set; } = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

        public string? EndDate { get; set; } = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture); //DateTime.ParseExact(DateTime.Now.ToString(), "yyyy/MM/dd", new CultureInfo("en-US")).ToString();
        public long? AppCodeS { get; set; }



        public long? ApplicantType { get; set; }
        public long? refranceCode { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }




    }

    public class AccRequestPaymentEditDto
    {

        public long Id { get; set; }

        public long? AppCode { get; set; }

        public string? AppDate { get; set; }

        public int? TypeId { get; set; }

        public long? DepId { get; set; }

        public decimal? Amount { get; set; }
        public string? Description { get; set; }
        public string? Note { get; set; }

        public string? RefranceNo { get; set; }

        public long? AccountId { get; set; }


        public int? StatusId { get; set; }


        public DateTime? FinDate { get; set; }



        public string? FinNote { get; set; }



        public DateTime? GmDate { get; set; }


        

        public string? GmNote { get; set; }


        public bool IsDeleted { get; set; }

        public int? Status2Id { get; set; }


        public long? ReferenceNo { get; set; }
        [Required]
        
        public long? ReferenceTypeId { get; set; }
        [Required]
        
        public long? BranchId { get; set; }

        public string? Amountwrite { get; set; }
        public long? CcId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        public string? IdNo { get; set; }
        
        public int? BankId { get; set; }
        
        public string? Iban { get; set; }
        
        public string? CustomerName { get; set; }
        
        public int? CustomerCont { get; set; }
        
        public decimal? DeductionsTotal { get; set; }
        
        public string? DeductionsNote { get; set; }


        public string? AppDateH { get; set; }
        
        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        public long? RefraneCode { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }

    }

    public class AccRequestMultiDto
    {

        public long Id { get; set; }

        public long? AppCode { get; set; }
        [Required]
        public string? AppDate { get; set; }
        public int? TypeId { get; set; }

        [Required]
        public long? DepId { get; set; }

        public decimal? Amount { get; set; }

        public string? Description { get; set; }
        public string? Note { get; set; }
        public string? RefranceNo { get; set; }

        public long? AccountId { get; set; }
        public int? StatusId { get; set; }

        public long? FinUserId { get; set; }

        public DateTime? FinDate { get; set; }

        public int? HasCredit { get; set; }

        public int? BalanceStatusId { get; set; }

        public string? FinNote { get; set; }

        public long? GmUserId { get; set; }

        public DateTime? GmDate { get; set; }

        public int? ExchangeStatusId { get; set; }

        public string? GmNote { get; set; }


        public bool IsDeleted { get; set; }

        public int? Status2Id { get; set; }

        public long? AppId { get; set; }

        public long? ReferenceNo { get; set; }
        [Required]
        public long? ReferenceTypeId { get; set; }
        [Required]
        public long? BranchId { get; set; }

        public long? FacilityId { get; set; }
        public long? JId { get; set; }


        public string? Amountwrite { get; set; }
        public long? CcId { get; set; }
        public int? TransTypeId { get; set; }
        public long? RefranceId { get; set; }
        [Range(1, long.MaxValue)]
        public int? CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }

        public string? IdNo { get; set; }

        public int? BankId { get; set; }

        public string? Iban { get; set; }


        public string? CustomerName { get; set; }

        public int? CustomerCont { get; set; }


        public decimal? DeductionsTotal { get; set; }

        public string? DeductionsNote { get; set; }

        public long? BadgetNo { get; set; }

        public string? AppDateH { get; set; }


        public string StartDate { get; set; } = new DateTime(DateTime.Now.Year, 1, 1).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

        public string? EndDate { get; set; } = DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture); //DateTime.ParseExact(DateTime.Now.ToString(), "yyyy/MM/dd", new CultureInfo("en-US")).ToString();
        public long? AppCodeS { get; set; }

        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }
        [StringLength(50)]

        public string? AccAccountCode { get; set; }
        [StringLength(255)]

        public string? AccAccountName { get; set; }

        public long ApplicantType { get; set; }
        public string? EmpCode { get; set; }
        public string? EmpName { get; set; }
        public string? EmpName2 { get; set; }
        public string? LinkCode { get; set; }
        public decimal? AmountTotal { get; set; }
        public decimal? ExpensesAmount { get; set; }

        public decimal? LinkAmount { get; set; }


        public decimal? LinkAmountInitial { get; set; }
        public decimal? ExRequestAmount { get; set; }
        public decimal? ExRemainingِِAmount { get; set; }



        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }

        public string? ApplicationCode { get; set; }
        public int? PaidStatus { get; set; }
        public int? AppTypeId { get; set; } = 0;
        public bool? ISMulti { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }
        public List<AccRequestEmployeeDto>? AccRequestEmployeeDto { get; set; }
    }
}
