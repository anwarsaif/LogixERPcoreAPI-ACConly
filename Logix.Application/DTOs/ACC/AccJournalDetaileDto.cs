using System.ComponentModel.DataAnnotations;


namespace Logix.Application.DTOs.ACC
{
    public class AccJournalDetailefilterDto
    {
        public long? PeriodId { get; set; } = 0;


        [StringLength(10)]
        public string? JDateGregorian { get; set; }
        [StringLength(10)]
        public string? JDateGregorian2 { get; set; }
        public int? BranchId { get; set; } = 0;
        public int? StatusId { get; set; } = 0;

        public string? CostCenterCode { get; set; }
        public string? CostcenterName { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }

        public decimal? Amount { get; set; } = 0;

        public string? Description { get; set; }
        public string? ReferenceNoFrom { get; set; }
        public string? ReferenceNoTo { get; set; }
        public string? ReferenceCode { get; set; }
        public string? CollectionEmpCode { get; set; }
        public int? PaymentTypeId { get; set; }
        public int? ReferenceTypeId { get; set; }


    }

    public class AccJournalDetaileDto
    {

        public long JDetailesId { get; set; }

        public long? JId { get; set; }

        public string? JDateGregorian { get; set; }

        public long? AccAccountId { get; set; }

        public long? CcId { get; set; }

        public decimal? Debit { get; set; }

        public decimal? Credit { get; set; }

        public int? ReferenceTypeId { get; set; }


        public long? ReferenceNo { get; set; }

        public string? Description { get; set; }

        public bool? Auto { get; set; }
        public int? CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public long? Cc2Id { get; set; }
        public long? Cc3Id { get; set; }

        public string? ReferenceCode { get; set; }
        public long? Cc4Id { get; set; }
        public long? Cc5Id { get; set; }
        public long? BranchId { get; set; }

        public long? ActivityId { get; set; }

        public long? AssestId { get; set; }

        public long? EmpId { get; set; }
        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string? AccAccountCode { get; set; }
        public long? AccountType { get; set; }
        public string? AccAccountName { get; set; }

        public string? CostCenterCode { get; set; }

        public string? CostCenterName { get; set; }
        public string? CostCenterCode2 { get; set; }

        public string? CostCenterName2 { get; set; }
        public string? CostCenterCode3 { get; set; }

        public string? CostCenterName3 { get; set; }
        public string? CostCenterCode4 { get; set; }

        public string? CostCenterName4 { get; set; }
        public string? CostCenterCode5 { get; set; }

        public string? CostCenterName5 { get; set; }
        public long? PeriodId { get; set; }

    }
    public class AccJournalDetaileEditDto
    {

        public long JDetailesId { get; set; }

        public long? JId { get; set; }

        public string? JDateGregorian { get; set; }

        public long? AccAccountId { get; set; }

        public long? CcId { get; set; }

        public decimal? Debit { get; set; }

        public decimal? Credit { get; set; }

        public int? ReferenceTypeId { get; set; }


        public long? ReferenceNo { get; set; }

        public string? Description { get; set; }

        public bool? Auto { get; set; }
        public int? CurrencyId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public long? Cc2Id { get; set; }
        public long? Cc3Id { get; set; }

        public string? ReferenceCode { get; set; }
        public long? Cc4Id { get; set; }
        public long? Cc5Id { get; set; }
        public long? BranchId { get; set; }

        public long? ActivityId { get; set; }

        public long? AssestId { get; set; }

        public long? EmpId { get; set; }

        public int? ModifiedBy { get; set; }

        public int? DeleteUserId { get; set; }


        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
