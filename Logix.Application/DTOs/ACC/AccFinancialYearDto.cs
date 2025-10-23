using System.ComponentModel.DataAnnotations;



namespace Logix.Application.DTOs.ACC
{
    public class AccFinancialYearDto
    {
        public long FinYear { get; set; }
        [Required]
        public int FinYearGregorian { get; set; }

        public int? FinYearHijri { get; set; }
        public string? StartDateHijri { get; set; }

        [StringLength(50)]
        [Required]
        public string StartDateGregorian { get; set; } = null!;

        public string? EndDateHijri { get; set; }

        [StringLength(50)]
        [Required]
        public string EndDateGregorian { get; set; } = null!;
        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }


        public bool? IsDeleted { get; set; }

        public long? FacilityId { get; set; }

        public int? FinState { get; set; }
    }
    public class AccFinancialYearEditDto
    {

        public long FinYear { get; set; }

        [Required]
        public int FinYearGregorian { get; set; }

        public int? FinYearHijri { get; set; }
        [StringLength(50)]
        public string? StartDateHijri { get; set; }

        [StringLength(50)]
        [Required]
        public string StartDateGregorian { get; set; } = null!;

        [StringLength(50)]
        public string? EndDateHijri { get; set; }

        [StringLength(50)]
        [Required]
        public string EndDateGregorian { get; set; } = null!;

        public int? FinState { get; set; }
    }
    public class BalanceSheetFinancialYear
    {
        public string? ReferenceTypeName { get; set; }
        public string? ReferenceTypeName2 { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? AccAccountCode { get; set; }
        public string? AccAccountName { get; set; }
        public string? AccAccountName2 { get; set; }
        public string? JDateGregorian { get; set; }
        public int? CCID { get; set; }
        public long? AccAccountID { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public int? ReferenceTypeID { get; set; }
        public int? ReferenceDNo { get; set; }
        public string Description { get; set; }
        public int? CC2ID { get; set; }
        public int? CC3ID { get; set; }
        public int? CC4ID { get; set; }
        public int? CC5ID { get; set; }
        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }
    }
    //public class BalanceSheetFinancialYear
    //{
    //    public string ReferenceTypeName { get; set; }
    //    public int Code { get; set; }
    //    public string Name { get; set; }
    //    public string AccAccountCode { get; set; }
    //    public string AccAccountName { get; set; }
    //    public string JDateGregorian { get; set; }
    //    public int CCID { get; set; }
    //    public long AccAccountID { get; set; }
    //    public decimal Debit { get; set; }
    //    public decimal Credit { get; set; }
    //    public int ReferenceTypeID { get; set; }
    //    public int ReferenceDNo { get; set; }
    //    public string Description { get; set; }
    //}

    public class BalanceSheetFinancialYearFilter
    {

        public int FinYearFrom { get; set; }
        public int PeriodIdFrom { get; set; }
        public int FinYearTo { get; set; }
        public int PeriodIdTo { get; set; }
        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
    }


    public class ClosingFinancialYearDto
    {

        public int FinYearFrom { get; set; }
        public int PeriodIdFrom { get; set; }
        public int FinYearTo { get; set; }
        public int PeriodIdTo { get; set; }
        public string? dateFrom { get; set; }
        public string? dateTo { get; set; }
        public int? CurrencyId { get; set; }
        public string? jCode { get; set; }
        public decimal? ExchangeRate { get; set; }

        public List<BalanceSheetFinancialYear> DetailsList { get; set; }
    }
}
