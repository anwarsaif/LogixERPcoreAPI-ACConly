using System.ComponentModel.DataAnnotations;

namespace Logix.Application.DTOs.ACC
{
    public class AccPeriodsDto
    {

        public long PeriodId { get; set; }

        public string? PeriodStartDateHijri { get; set; }
        [Required]
        public string? PeriodStartDateGregorian { get; set; }

        public string? PeriodEndDateHijri { get; set; }
        [Required]
        public string? PeriodEndDateGregorian { get; set; }
        [Range(1, long.MaxValue)]
        public long? FinYear { get; set; }

         public int? InsertUserId { get; set; }
        //public long? CreatedBy { get; set; }


          public DateTime InsertDate { get; set; }
        //public DateTime CreatedOn { get; set; }


          public bool? FlagDelete { get; set; }
        //public bool? IsDeleted { get; set; }

        public long? FacilityId { get; set; }

        public int? PeriodState { get; set; }
        public string? PeriodStateName { get; set; }

    }
    public class AccPeriodsEditDto
    {

        public long PeriodId { get; set; }

        public string? PeriodStartDateHijri { get; set; }
        [Required]

        public string? PeriodStartDateGregorian { get; set; }
        public string? PeriodEndDateHijri { get; set; }

        [Required]
        public string? PeriodEndDateGregorian { get; set; }

        [Range(1, long.MaxValue)]
        public long? FinYear { get; set; }


        public int? PeriodState { get; set; }
    }
}
