using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Keyless]
    public class AccPeriodDateVws
    {

        [Column("Period_ID")]
        public long PeriodId { get; set; }
        [Column("Period_date")]
        [StringLength(35)]
        public string? PeriodDate { get; set; }
        [Column("Period_date2")]
        [StringLength(36)]
        public string? PeriodDate2 { get; set; }
        [Column("Fin_year")]
        public long? FinYear { get; set; }
        [Column("Facility_ID")]
        public long FacilityId { get; set; }
        [Column("Period_state")]
        public int? PeriodState { get; set; }
        public bool? FlagDelete { get; set; }
    }
}
