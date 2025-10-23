using Logix.Application.DTOs.Main;
using Logix.Domain.ACC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
   
    public class AccPettyCashFilterDto
    {
        public long? Code { get; set; }
        [StringLength(255)]
        public string? JCode { get; set; }
        public long? ApplicationCode { get; set; }
        
        public int? StatusId { get; set; }

        public int? TypeId { get; set; }
        
        public int? PettyCashType { get; set; }
        [StringLength(10)]
        public string? StartDate { get; set; }
        [StringLength(10)]
        public string? EndDate { get; set; }
        public string? EmpCode { get; set; } 
        public string? EmpName { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public long? BranchId { get; set; }
        public string? AccAccountCode { get; set; }
        public string? AccAccountName { get; set; }
        public int? ReferenceTypeId { get; set; }
        public string? Description { get; set; }
        public string? ReferenceCode { get; set; }
        public long? ExpenseId { get; set; }

    }
    public class AccPettyCashDto
    {

     
        public long Id { get; set; }
        public long? Code { get; set; }
    
        [StringLength(10)]
        public string? TDate { get; set; }
   
        public decimal? Amount { get; set; }
     
        public long? EmpId { get; set; }
     
        public int? TypeId { get; set; }
      
        public int? PettyCashType { get; set; }
        public string? Description { get; set; }
     
        public int? PaymentTypeId { get; set; }
      
        [StringLength(50)]
        public string? ChequNo { get; set; }
       
        [StringLength(10)]
        public string? ChequDateHijri { get; set; }
       
        public long? BankId { get; set; }
      
        public long? CashOrBankAccountId { get; set; }
       
        public int? StatusId { get; set; }
       
        public long? FacilityId { get; set; }
        
        public long? BranchId { get; set; }
       
        public long? PeriodId { get; set; }

       
        public long? AppId { get; set; }
       
        public long? JId { get; set; }
        public string? Note { get; set; }
       
        public long? AccAccountId { get; set; }
        
        public int? ReferenceTypeId { get; set; }
       
        public long? ReferenceNo { get; set; }

        public bool? IsDeleted { get; set; }
        public string? ReferenceCode { get; set; }
        public string? AccAccountCode { get; set; }
        public int? AppTypeId { get; set; } = 0;
        public List<AccPettyCashDDto>? DetailsList { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }

    }

    public class AccPettyCashEditDto
    {
        public long Id { get; set; }
        public long? Code { get; set; }

        [StringLength(10)]
        public string? TDate { get; set; }

        public decimal? Amount { get; set; }

        public long? EmpId { get; set; }

        public int? TypeId { get; set; }

        public int? PettyCashType { get; set; }
        public string? Description { get; set; }


        [StringLength(50)]
        public string? ChequNo { get; set; }

        [StringLength(10)]
        public string? ChequDateHijri { get; set; }




        public int? StatusId { get; set; }


        public long? BranchId { get; set; }

        public long? PeriodId { get; set; }



        public long? JId { get; set; }
        public string? Note { get; set; }

        public long? AccAccountId { get; set; }

        public int? ReferenceTypeId { get; set; }

        public long? ReferenceNo { get; set; }
        public List<AccPettyCashDEditDto>? DetailsList { get; set; }
        public List<SaveFileDto>? FileDtos { get; set; }
        public AccPettyCashVw? AccPettyCashVw { get; set; }
        public string? ReferenceCode { get; set; }
        public string? AccAccountCode { get; set; }

        public int? StatusJournal { get; set; }

    }
}
