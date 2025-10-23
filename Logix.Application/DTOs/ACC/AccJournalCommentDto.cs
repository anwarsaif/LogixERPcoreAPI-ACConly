using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
  public  class AccJournalCommentDto
    {
        
        public long Id { get; set; }
      
        public long? JId { get; set; }
        [StringLength(10)]
        public string? Date1 { get; set; }
        public string? Note { get; set; }
        public long? CreatedBy { get; set; }
 
        public DateTime? CreatedOn { get; set; }
    }
}
