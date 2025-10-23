using Logix.Application.DTOs.Main;
using Logix.Domain.ACC;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logix.Application.DTOs.ACC
{

    public class AccJournalVM
    {
        [StringLength(10)]
        public string? Date1 { get; set; }
        public long PeriodId { get; set; }
        public int? BranchId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public int? DocTypeId { get; set; }
    }
    public class JournalMasterVM
    {
        public long JId { get; set; }
        public string? JCode { get; set; }
        [StringLength(10)]
        public string? JDateGregorian { get; set; }
        [StringLength(50)]
        public string? DocTypeName { get; set; }
        [StringLength(50)]
        public string? DocTypeName2 { get; set; }
        [Column("BRA_NAME")]
        public string? BraName { get; set; }
        [Column("BRA_NAME2")]
        public string? BraName2 { get; set; }
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        [StringLength(50)]
        public string? StatusName { get; set; }
        [StringLength(50)]
        public string? StatusName2 { get; set; }
        public long? ReferenceNo { get; set; } = 0;
        public int? DocTypeId { get; set; } = 0;
        [StringLength(50)]
        public string? UserFullname { get; set; }
        public decimal? sumCredit { get; set; } = 0;
        public decimal? sumDebit { get; set; } = 0;
        public int? DeleteUserId { get; set; }
        public bool? IsDeleted { get; set; }
        public int? InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? CollectionEmpCode { get; set; }
        public List<AccJournalDetailesVw> Children { get; set; }
    }
    public class AccJournalMasterfilterDto
    {
        public long? PeriodId { get; set; } = 0;
        [StringLength(255)]
        public string? JCode { get; set; }
        [StringLength(255)]
        public string? JCode2 { get; set; }
        [StringLength(10)]
        public string? JDateGregorian { get; set; }
        [StringLength(10)]
        public string? JDateGregorian2 { get; set; }
        public int? BranchId { get; set; } = 0;
        public int? StatusId { get; set; } = 0;

        public int? CreatedBy { get; set; } = 0;


        public string? CostCenterCode { get; set; }
        public string? CostcenterName { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public decimal? Debit { get; set; } = 0;

        public decimal? Credit { get; set; } = 0;
        public string? Description { get; set; }
        public string? ReferenceNoFrom { get; set; }
        public string? ReferenceNoTo { get; set; }
        public string? ReferenceCode { get; set; }
        public string? CollectionEmpCode { get; set; }
        public int? PaymentTypeId { get; set; }
        public int? InsertUserId { get; set; } = 0;
        public long? ReferenceNo { get; set; } = 0;
        public int? DocTypeId { get; set; } = 0;


    }
    public class AccJournalMasterDtoVW
    {
        public AccJournalMasterDtoVW()
        {
            AccJournalMasterDto = new AccJournalMasterDto();

            DetailsList = new List<AccJournalDetaileDto>();
            FileDtos = new List<SaveFileDto>();
        }
        public AccJournalMasterDto AccJournalMasterDto { get; set; }
        public List<AccJournalDetaileDto> DetailsList { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }



    }
    public class AccJournalMasterEditDtoVW
    {
        public AccJournalMasterEditDtoVW()
        {
            AccJournalMasterDto = new AccJournalMasterEditDto();

            DetailsList = new List<AccJournalDetaileDto>();
            FileDtos = new List<SaveFileDto>();
        }
        public AccJournalMasterEditDto AccJournalMasterDto { get; set; }
        public List<AccJournalDetaileDto> DetailsList { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }


    }
    public class AccJournalMasterDto
    {
        public long JId { get; set; }

        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }

        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }
        public long? cashOnhandId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public string? AccAccountCode { get; set; }
        public bool? FlagDelete { get; set; }
        public int? InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? CollectionEmpCode { get; set; }

        public long? ReferenceTypeId { get; set; }
        public string? CostCenterCode { get; set; }

    }
    public class AccJournalMasterEditDto
    {
        public long JId { get; set; }

        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }



        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public string? CollectionEmpCode { get; set; }
        public long? cashOnhandId { get; set; }

    }
    public class AccJournalMasterPurBillDto
    {
        public long JId { get; set; }

        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }

        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }
        public long? cashOnhandId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public string? AccAccountCode { get; set; }
        public bool? FlagDelete { get; set; }
        public int? InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? CollectionEmpCode { get; set; }

        public long? ReferenceTypeId { get; set; }
        public string? CostCenterCode { get; set; }

    }

    //================================= سند القيض
    #region
    public class AccIncomeDto
    {
        public AccIncomeDto()
        {
            FileDtos = new List<SaveFileDto>();
        }
        public long JId { get; set; }
        public long? BranchId { get; set; }
        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }

        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }
        public long? cashOnhandId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public string? AccAccountCode { get; set; }
        public bool? FlagDelete { get; set; }
        public int? InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? CollectionEmpCode { get; set; }

        public long? ReferenceTypeId { get; set; }
        public string? CostCenterCode { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }

    }
    public class AccIncomeEditDto
    {
        public long JId { get; set; }
        public long? BranchId { get; set; }
        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }

        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public long? cashOnhandId { get; set; }
        public string? CollectionEmpCode { get; set; }
    }
    public class AccIncomeMasterDtoVW
    {
        public AccIncomeMasterDtoVW()
        {
            AccJournalMasterDto = new AccIncomeDto();
            DetailsList = new List<AccJournalDetaileDto>();
            FileDtos = new List<SaveFileDto>();
        }
        public AccIncomeDto? AccJournalMasterDto { get; set; }

        public List<AccJournalDetaileDto> DetailsList { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }


    }
    public class AccIncomeMasterEditDtoVW
    {
        public AccIncomeMasterEditDtoVW()
        {
            AccJournalMasterDto = new AccIncomeEditDto();
            DetailsList = new List<AccJournalDetaileDto>();
            FileDtos = new List<SaveFileDto>();
        }
        public AccIncomeEditDto? AccJournalMasterDto { get; set; }

        public List<AccJournalDetaileDto> DetailsList { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }


    }
    #endregion
    //================================= سند الصرف
    #region
    public class AccExpensesDto
    {
        public AccExpensesDto()
        {
            FileDtos = new List<SaveFileDto>();
        }
        public long JId { get; set; }
        public long? BranchId { get; set; }
        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }

        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }
        public long? cashOnhandId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public string? AccAccountCode { get; set; }
        public bool? FlagDelete { get; set; }
        public int? InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? CollectionEmpCode { get; set; }

        public long? ReferenceTypeId { get; set; }
        public string? CostCenterCode { get; set; }
        public long? RequestID { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }

    }
    public class AccExpensesEditDto
    {
        public long JId { get; set; }
        public long? BranchId { get; set; }
        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }

        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public long? cashOnhandId { get; set; }

    }

    public class AccExpensesMasterEditDtoVW
    {
        public AccExpensesMasterEditDtoVW()
        {
            AccJournalMasterDto = new AccExpensesEditDto();
            DetailsList = new List<AccJournalDetaileDto>();
            FileDtos = new List<SaveFileDto>();
        }
        public AccExpensesEditDto? AccJournalMasterDto { get; set; }

        public List<AccJournalDetaileDto> DetailsList { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }


    }
    #endregion
    //================================ القيد العكسي
    #region
    public class AccJournalReverseDto
    {

        public long JId { get; set; }
        public long? BranchId { get; set; }
        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }

        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }
        public long? cashOnhandId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public string? AccAccountCode { get; set; }
        public bool? FlagDelete { get; set; }
        public int? InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? CollectionEmpCode { get; set; }

        public long? ReferenceTypeId { get; set; }
        public string? CostCenterCode { get; set; }

    }
    public class AccJournalReverseEditDto
    {
        public long JId { get; set; }

        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }



        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public string? CollectionEmpCode { get; set; }
        public long? cashOnhandId { get; set; }

    }
    public class AccJournalReverseDtoVW
    {
        public AccJournalReverseDtoVW()
        {
            AccJournalMasterDto = new AccJournalReverseDto();

            DetailsList = new List<AccJournalDetaileDto>();
            FileDtos = new List<SaveFileDto>();
        }
        public AccJournalReverseDto AccJournalMasterDto { get; set; }
        public List<AccJournalDetaileDto> DetailsList { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }



    }
    #endregion
    //===========================================ترحيل القيود
    #region
    public class AccJournalMasterStatusDto
    {
        public long? JId { get; set; } = 0;
        public string? Note { get; set; }

        public int? StatusId { get; set; }
        public long Count { get; set; } = 0;
        public string? SelectedJId { get; set; }
    }
    #endregion
    //===========================================رصيد أول المدة
    #region  

    public class FirstTimeBalanceDto
    {

        public long JId { get; set; }
        public long? BranchId { get; set; }
        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }

        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }
        public long? cashOnhandId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public string? AccAccountCode { get; set; }
        public bool? FlagDelete { get; set; }
        public int? InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? CollectionEmpCode { get; set; }

        public long? ReferenceTypeId { get; set; }
        public string? CostCenterCode { get; set; }

    }
    public class FirstTimeBalanceEditDto
    {
        public long JId { get; set; }

        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }



        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public string? CollectionEmpCode { get; set; }
        public long? cashOnhandId { get; set; }

    }
    public class FirstTimeBalanceDtoVW
    {
        public FirstTimeBalanceDtoVW()
        {
            AccJournalMasterDto = new FirstTimeBalanceDto();

            DetailsList = new List<AccJournalDetaileDto>();
            FileDtos = new List<SaveFileDto>();
        }
        public FirstTimeBalanceDto AccJournalMasterDto { get; set; }
        public List<AccJournalDetaileDto> DetailsList { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }



    }
    public class FirstTimeBalanceEditDtoVW
    {
        public FirstTimeBalanceEditDtoVW()
        {
            AccJournalMasterDto = new FirstTimeBalanceEditDto();

            DetailsList = new List<AccJournalDetaileDto>();
            FileDtos = new List<SaveFileDto>();
        }
        public FirstTimeBalanceEditDto AccJournalMasterDto { get; set; }
        public List<AccJournalDetaileDto> DetailsList { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }


    }
    #endregion
    //=========================================== الرصيد الإفتتاحي
    #region  

    public class OpeningBalanceDto
    {

        public long JId { get; set; }
        public long? BranchId { get; set; }
        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? IsDeleted { get; set; }

        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }
        public long? cashOnhandId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public string? AccAccountCode { get; set; }
        public bool? FlagDelete { get; set; }
        public int? InsertUserId { get; set; }
        public DateTime? InsertDate { get; set; }
        public string? CollectionEmpCode { get; set; }

        public long? ReferenceTypeId { get; set; }
        public string? CostCenterCode { get; set; }

    }
    public class OpeningBalanceEditDto
    {
        public long JId { get; set; }

        [StringLength(255)]
        public string? JCode { get; set; }

        [StringLength(10)]
        public string? JDateHijri { get; set; }

        [StringLength(10)]
        public string? JDateGregorian { get; set; }

        [StringLength(50)]
        public string? JTime { get; set; }

        [StringLength(2000)]
        public string? JDescription { get; set; }

        public long? PeriodId { get; set; }

        public long? FinYear { get; set; }

        public long? FacilityId { get; set; }

        public long? CcId { get; set; }

        public int? DocTypeId { get; set; }



        public int? StatusId { get; set; }

        public int? PaymentTypeId { get; set; }

        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }

        public long? BankId { get; set; }

        [StringLength(2500)]
        public string? JBian { get; set; }

        public decimal? Amount { get; set; }
        [StringLength(4000)]
        public string? AmountWrite { get; set; }

        public long? ReferenceNo { get; set; }

        public long? CollectionEmpId { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }
        public string? CollectionEmpCode { get; set; }
        public long? cashOnhandId { get; set; }

    }
    public class OpeningBalanceDtoVW
    {
        public OpeningBalanceDtoVW()
        {
            AccJournalMasterDto = new OpeningBalanceDto();

            DetailsList = new List<AccJournalDetaileDto>();
            FileDtos = new List<SaveFileDto>();
        }
        public OpeningBalanceDto AccJournalMasterDto { get; set; }
        public List<AccJournalDetaileDto> DetailsList { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }



    }
    public class OpeningBalanceEditDtoVW
    {
        public OpeningBalanceEditDtoVW()
        {
            AccJournalMasterDto = new OpeningBalanceEditDto();

            DetailsList = new List<AccJournalDetaileDto>();
            FileDtos = new List<SaveFileDto>();
        }
        public OpeningBalanceEditDto AccJournalMasterDto { get; set; }
        public List<AccJournalDetaileDto> DetailsList { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }


    }
    #endregion
}
