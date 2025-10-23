using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
    public class AccSettlementInstalFilterDto
    {
        [StringLength(255)]
        public long? Code { get; set; }
        [StringLength(255)]
        public long? Code2 { get; set; }

        public long Id { get; set; }
     
        public long? InstallmentNo { get; set; }
      
        public long? SsId { get; set; }
       
        public decimal? InstallmentValue { get; set; }
     
        [StringLength(10)]
        public string? InstallmentDate { get; set; }
        public string? Description { get; set; }
        public int? BranchId { get; set; } = 0;
        public int? StatusId { get; set; } = 0;

        public int? CreatedBy { get; set; } = 0;
        public string? CostCenterCode { get; set; }
        public string? CostcenterName { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public decimal? Debit { get; set; } = 0;

        public decimal? Credit { get; set; } = 0;
        public long? ReferenceNo { get; set; } = 0;
        public int? DocTypeId { get; set; } = 0;
        [StringLength(10)]
        public string? StartDate { get; set; }
        [StringLength(10)]
        public string? EndDate { get; set; }
        public string? ReferenceCode { get; set; }
        public int? InsertUserId { get; set; } = 0;
    }

    public class AccSettlementInstallmentDto
    {
        public long Id { get; set; }

        public long? InstallmentNo { get; set; }

        public long? SsId { get; set; }

        public decimal? InstallmentValue { get; set; }

        [StringLength(10)]
        public string? InstallmentDate { get; set; }
        public string? Description { get; set; }
        public bool? IsDeleted { get; set; }
        public string? AccAccountCode { get; set; }
        public string? AccAccountName { get; set; }
        public string? JDateGregorian { get; set; }
    }
    public class AccSettlementInstallmentEditDto
    {
        public long Id { get; set; }

        public long? InstallmentNo { get; set; }

        public long? SsId { get; set; }

        public decimal? InstallmentValue { get; set; }

        [StringLength(10)]
        public string? InstallmentDate { get; set; }
        public string? Description { get; set; }
    }
}
