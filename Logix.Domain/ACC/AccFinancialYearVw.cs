using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccFinancialYearVw
    {
        [Column("Fin_year")]
        public long FinYear { get; set; }
        [Column("Fin_year_Gregorian")]
        public int FinYearGregorian { get; set; }
        [Column("Fin_year_Hijri")]
        public int? FinYearHijri { get; set; }
        public bool? FlagDelete { get; set; }
        [Column("Fin_State")]
        public int? FinState { get; set; }
        [Column("Facility_ID")]
        public long FacilityId { get; set; }
    }
}
