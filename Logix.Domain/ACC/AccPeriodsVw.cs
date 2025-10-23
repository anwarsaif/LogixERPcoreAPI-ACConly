using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccPeriodsVw
    {
        [Column("Fin_year_Hijri")]
        public int? FinYearHijri { get; set; }
        [Column("Period_ID")]
        public long PeriodId { get; set; }
        [Column("Period_Start_Date_Hijri")]
        [StringLength(10)]
        public string? PeriodStartDateHijri { get; set; }
        [Column("Period_Start_Date_Gregorian")]
        [StringLength(10)]
        public string? PeriodStartDateGregorian { get; set; }
        [Column("Period_End_Date_Hijri")]
        [StringLength(10)]
        public string? PeriodEndDateHijri { get; set; }
        [Column("Period_End_Date_Gregorian")]
        [StringLength(10)]
        public string? PeriodEndDateGregorian { get; set; }
        [Column("Fin_year")]
        public long? FinYear { get; set; }
        [Column("Insert_User_ID")]
        public int? InsertUserId { get; set; }
        [Column("Update_User_ID")]
        public int? UpdateUserId { get; set; }
        [Column("Delete_User_ID")]
        public int? DeleteUserId { get; set; }
        [Column("Insert_Date", TypeName = "datetime")]
        public DateTime InsertDate { get; set; }
        [Column("Update_Date", TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [Column("Delete_Date", TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        public bool? FlagDelete { get; set; }
        [Column("Facility_ID")]
        public long FacilityId { get; set; }
        [Column("Period_state")]
        public int? PeriodState { get; set; }
    }
}
