using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
    public class AccSettlementScheduleDFilterDto
    {

     
        public long Id { get; set; }
       
        public long? SsId { get; set; }
        
        public long? AccAccountId { get; set; }
       
        public long? CcId { get; set; }
      
        public decimal? Debit { get; set; }
      
        public decimal? Credit { get; set; }
        [StringLength(2000)]
        public string? Description { get; set; }
       
        public int? CurrencyId { get; set; }
       
        public decimal? ExchangeRate { get; set; }

       
        public bool? IsMain { get; set; }
      
        public int? ReferenceTypeId { get; set; }
     
        public long? ReferenceNo { get; set; }
     
        public long? Cc2Id { get; set; }
       
        public long? Cc3Id { get; set; }
      
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
     
        public long? Cc4Id { get; set; }
       
        public long? Cc5Id { get; set; }
       
        public long? BranchId { get; set; }
       
        public long? ActivityId { get; set; }
       
        public long? AssestId { get; set; }
       
        public long? EmpId { get; set; }
    }
    public class AccSettlementScheduleDDto
    {
        public long Id { get; set; }

        public long? SsId { get; set; }

        public long? AccAccountId { get; set; }

        public long? CcId { get; set; }

        public decimal? Debit { get; set; }

        public decimal? Credit { get; set; }
        [StringLength(2000)]
        public string? Description { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }


        public bool? IsMain { get; set; }

        public int? ReferenceTypeId { get; set; }

        public long? ReferenceNo { get; set; }

        public long? Cc2Id { get; set; }

        public long? Cc3Id { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }

        public long? Cc4Id { get; set; }

        public long? Cc5Id { get; set; }

        public long? BranchId { get; set; }

        public long? ActivityId { get; set; }

        public long? AssestId { get; set; }

        public long? EmpId { get; set; }
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
    }

    public class AccSettlementScheduleDEditDto
    {
        public long Id { get; set; }

        public long? SsId { get; set; }

        public long? AccAccountId { get; set; }

        public long? CcId { get; set; }

        public decimal? Debit { get; set; }

        public decimal? Credit { get; set; }
        [StringLength(2000)]
        public string? Description { get; set; }

        public int? CurrencyId { get; set; }

        public decimal? ExchangeRate { get; set; }


        public bool? IsMain { get; set; }

        public int? ReferenceTypeId { get; set; }

        public long? ReferenceNo { get; set; }

        public long? Cc2Id { get; set; }

        public long? Cc3Id { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }

        public long? Cc4Id { get; set; }

        public long? Cc5Id { get; set; }

        public long? BranchId { get; set; }

        public long? ActivityId { get; set; }

        public long? AssestId { get; set; }

        public long? EmpId { get; set; }
    }
}
