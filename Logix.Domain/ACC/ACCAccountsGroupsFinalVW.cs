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
    public partial class AccAccountsGroupsFinalVw
    {
        [Column("Acc_Account_ID")]
        public long AccAccountId { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(255)]
        public string? AccAccountCode { get; set; }
        [Column("Acc_Account_Name")]
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [Column("isdel")]
        public bool? Isdel { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
    }
}
