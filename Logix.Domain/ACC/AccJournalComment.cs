using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Domain.ACC
{
    [Table("ACC_Journal_Comments")]
    public partial class AccJournalComment
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("J_ID")]
        public long? JId { get; set; }
        [StringLength(10)]
        public string? Date1 { get; set; }
        public string? Note { get; set; }
        public long? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
    }
}
