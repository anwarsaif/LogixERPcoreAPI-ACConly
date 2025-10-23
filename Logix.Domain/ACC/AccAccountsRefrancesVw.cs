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
    public partial class AccAccountsRefrancesVw
    {
        [Column("Refrance_No")]
        public long RefranceNo { get; set; }
        [StringLength(50)]
        public string? Code { get; set; }
        [StringLength(4000)]
        public string? Name { get; set; }
        [Column("Refrance_Type")]
        public int RefranceType { get; set; }
        [Column("Status_ID")]
        public int? StatusId { get; set; }
    }
}
