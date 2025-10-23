using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
    public class AccBranchAccountTypeDto
    {
        public long Id { get; set; }
        [StringLength(250)]
        public string? Name { get; set; }
        [StringLength(250)]
        public string? Name2 { get; set; }
        public long? CreatedBy { get; set; }
        
        public DateTime? CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
