using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
    public class AccBranchAccountsVwsDto
    {
        public long Id { get; set; }
        [StringLength(255)]
        public string? AccAccountCode { get; set; }
        public long? BrAccTypeId { get; set; }
        public long? BranchId { get; set; }
        public long? AccountId { get; set; }
        [StringLength(250)]
        public string? Name { get; set; }
        [StringLength(250)]
        public string? Name2 { get; set; }
        public bool IsDeleted { get; set; }

        //[StringLength(255)]
        //public string? AccAccountName { get; set; }
        //[StringLength(255)]
        //public string? AccAccountName2 { get; set; }
        //public long CreatedBy { get; set; }
        //public DateTime CreatedOn { get; set; }
        //public long? ModifiedBy { get; set; }
        //public DateTime? ModifiedOn { get; set; }
    }
}
