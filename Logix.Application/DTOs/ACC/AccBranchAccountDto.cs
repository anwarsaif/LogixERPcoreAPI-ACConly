using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
    public class AccBranchAccountDto
    {
        public long Id { get; set; }
        
        public long? BrAccTypeId { get; set; }
        
        public long? BranchId { get; set; }
       
        public long? AccountId { get; set; }
        public long CreatedBy { get; set; }
        
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
       
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
