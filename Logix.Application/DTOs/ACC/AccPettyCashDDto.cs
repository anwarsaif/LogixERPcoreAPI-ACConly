using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
    public class AccPettyCashDFilterDto
    {

    }
    public class AccPettyCashDDto
    {
        
        public long Id { get; set; }
      
        public long? PettyCashId { get; set; }
       
        public int? ExpenseId { get; set; }
        
        public long? CcId { get; set; }
       
        public decimal? Amount { get; set; }
        
        public decimal? VatAmount { get; set; }
       
        public decimal? Total { get; set; }
       
        [StringLength(50)]
        public string? ReferenceCode { get; set; }
       
        [StringLength(50)]
        public string? ReferenceDate { get; set; }
      
        [StringLength(50)]
        public string? MeterReading { get; set; }
        
        [StringLength(50)]
        public string? MeterReadingPrevious { get; set; }
        public string? Description { get; set; }

       
        public long? Cc2Id { get; set; }
      
        public long? Cc3Id { get; set; }
      
        public long? Cc4Id { get; set; }
       
        public long? Cc5Id { get; set; }
       
        public long? BranchId { get; set; }
     
        public long? ActivityId { get; set; }
       
        public long? AssestId { get; set; }
      
        public long? EmpId { get; set; }
        [StringLength(2500)]
        public string? SupplierName { get; set; }
        [StringLength(250)]
        public string? SupplierVatNumber { get; set; }
       
        public long? TempId { get; set; }
       
        public decimal? Vat { get; set; }
        public bool? IsDeleted { get; set; }
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
        public string? ExpenseName { get; set; }
      
    }
    public class AccPettyCashDEditDto
    {
        public long Id { get; set; }

        public long? PettyCashId { get; set; }

        public int? ExpenseId { get; set; }

        public long? CcId { get; set; }

        public decimal? Amount { get; set; }

        public decimal? VatAmount { get; set; }

        public decimal? Total { get; set; }

        [StringLength(50)]
        public string? ReferenceCode { get; set; }

        [StringLength(50)]
        public string? ReferenceDate { get; set; }

        [StringLength(50)]
        public string? MeterReading { get; set; }

        [StringLength(50)]
        public string? MeterReadingPrevious { get; set; }
        public string? Description { get; set; }


        public long? Cc2Id { get; set; }

        public long? Cc3Id { get; set; }

        public long? Cc4Id { get; set; }

        public long? Cc5Id { get; set; }

        public long? BranchId { get; set; }

        public long? ActivityId { get; set; }

        public long? AssestId { get; set; }

        public long? EmpId { get; set; }
        [StringLength(2500)]
        public string? SupplierName { get; set; }
        [StringLength(250)]
        public string? SupplierVatNumber { get; set; }

        public long? TempId { get; set; }

        public decimal? Vat { get; set; }
        public bool? IsDeleted { get; set; }
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
        public string? ExpenseName { get; set; }
    }
}
