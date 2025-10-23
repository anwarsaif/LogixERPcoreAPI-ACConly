using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Logix.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Table("ACC_Group")]
    public partial class AccGroup: TraceEntity
    {
        [Key]
        [Column("Acc_group_ID")]
        public long AccGroupId { get; set; }
        [Column("Acc_group_Name")]
        [StringLength(50)]
        public string AccGroupName { get; set; } = null!;
        [Column("Acc_group_Name2")]
        [StringLength(50)]
        public string? AccGroupName2 { get; set; }
        //[Column("Insert_User_ID")]
        //public int InsertUserId { get; set; }
        //[Column("Update_User_ID")]
        //public int? UpdateUserId { get; set; }
        [Column("Delete_User_ID")]
        public int? DeleteUserId { get; set; }
        //[Column("Insert_Date", TypeName = "datetime")]
        //public DateTime InsertDate { get; set; }
        //[Column("Update_Date", TypeName = "datetime")]
        //public DateTime? UpdateDate { get; set; }
        [Column("Delete_Date", TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        //public bool? FlagDelete { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Acc_group_Code")]
        [StringLength(50)]
        public string AccGroupCode { get; set; } = null!;
        [Column("Nature_account")]
        public int? NatureAccount { get; set; }
    }
}
