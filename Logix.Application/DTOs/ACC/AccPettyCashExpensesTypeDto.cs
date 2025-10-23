using Castle.MicroKernel.SubSystems.Conversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Logix.Application.DTOs.ACC
{
    public class AccPettyCashExpensesTypeFilterDto
    {
        public string? Name { get; set; }

        public string? Name2 { get; set; }
    }
    public class AccPettyCashExpensesTypeDto
    {
        
        public long Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Name2 { get; set; }
        public decimal? VatRate { get; set; }
        public bool? LinkAccounting { get; set; }
        public long? AccAccountId { get; set; }
   
        public bool? IsDeleted { get; set; }
        public int? FacilityId { get; set; }
        public string? AccAccountCode { get; set; }
        public string? AccAccountName { get; set; }
    }
    public class AccPettyCashExpensesTypeEditDto
    {

        public long Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Name2 { get; set; }
        public decimal? VatRate { get; set; }
        public bool? LinkAccounting { get; set; }
        public long? AccAccountId { get; set; }

        public int? FacilityId { get; set; }
        public string? AccAccountCode { get; set; }
        public string? AccAccountName { get; set; }
    }
}
