using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.Main.ViewModels
{
    public class SendSmsVM
    {
        [Required]
        public string ReceiverMobile { get; set; } = "";
        [Required]
        public string Message { get; set; } = "";
        public long? FacilityId { get; set; }

        public bool IsRepeat { get; set; }
        public bool IsArabic { get; set; }
        public int? UserId { get; set; }
    }
}