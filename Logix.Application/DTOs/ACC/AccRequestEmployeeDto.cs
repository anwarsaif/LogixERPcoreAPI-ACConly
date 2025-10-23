using System.ComponentModel.DataAnnotations;

namespace Logix.Application.DTOs.ACC
{
    public class AccRequestEmployeeDto
    {


        public long? Id { get; set; }


        public long? AccRequestId { get; set; }


        public long? ReferenceTypeId { get; set; }

        public long? AccAccountId { get; set; }


        [StringLength(255)]
        public string? AccAccountName { get; set; }

        [StringLength(50)]
        public string? ReferenceTypeName { get; set; }

        [StringLength(50)]
        public string? AccAccountCode { get; set; }

        public long? ReferenceNo { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(4000)]
        public string? Name { get; set; }

        public decimal? Amount { get; set; }

        public string? Amountwrite { get; set; }

        public long? CcId { get; set; }

        [StringLength(50)]
        public string? CostCenterCode { get; set; }

        [StringLength(150)]
        public string? CostCenterName { get; set; }

        public string? Note { get; set; }

        [StringLength(250)]
        public string? RefranceNo { get; set; }

        public int? StatusId { get; set; }
    }
}
