using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccRequestHasCreditVw
    {
        [Column("Has_Credit_ID")]
        public long? HasCreditId { get; set; }
        [Column("Has_Credit_Name")]
        [StringLength(250)]
        public string? HasCreditName { get; set; }
        [Column("Catagories_ID")]
        public int? CatagoriesId { get; set; }
        [Column("ISDEL")]
        public bool? Isdel { get; set; }
        [Column("USER_ID")]
        public long? UserId { get; set; }
        [Column("Sort_no")]
        public int? SortNo { get; set; }
        [StringLength(50)]
        public string? Note { get; set; }
        [Column("Refrance_No")]
        [StringLength(250)]
        public string? RefranceNo { get; set; }
        [Column("Color_ID")]
        public int? ColorId { get; set; }
        [StringLength(250)]
        public string? Icon { get; set; }
    }
}
