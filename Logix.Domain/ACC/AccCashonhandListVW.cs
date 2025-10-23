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
    public partial class AccCashOnHandListVw
    {
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
        [StringLength(255)]
        public string? Name { get; set; }
        [StringLength(255)]
        public string? Name2 { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        public bool? IsDeleted { get; set; }
        public int Type { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        [Column("Users_Permission")]
        public string? UsersPermission { get; set; }
    }
}
