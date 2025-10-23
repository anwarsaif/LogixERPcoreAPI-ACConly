using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Logix.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Table("ACC_Financial_Year")]
    public partial class AccFinancialYear: TraceEntity
    {
        [Key]
        [Column("Fin_year")]
        public long FinYear { get; set; }
        [Column("Fin_year_Gregorian")]
        public int FinYearGregorian { get; set; }
        [Column("Fin_year_Hijri")]
        public int? FinYearHijri { get; set; }
        [Column("Start_Date_Hijri")]
        [StringLength(50)]
        public string? StartDateHijri { get; set; }
        [Column("Start_Date_Gregorian")]
        [StringLength(50)]
        public string StartDateGregorian { get; set; } = null!;
        [Column("End_Date_Hijri")]
        [StringLength(50)]
        public string? EndDateHijri { get; set; }
        [Column("End_Date_Gregorian")]
        [StringLength(50)]
        public string EndDateGregorian { get; set; } = null!;
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
        public long FacilityId { get; set; }
        [Column("Fin_State")]
        public int? FinState { get; set; }
    }
}
