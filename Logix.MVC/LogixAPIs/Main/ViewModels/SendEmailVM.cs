using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.Main.ViewModels
{
    public class SendEmailVM
    {
        [Required]
        public string? SendTo { get; set; } //ارسال الى
        public string? CcTo { get; set; } // نسخة الى
        public string? BCcTo { get; set; } //نسخة مخفية
        public long? MailServerId { get; set; }
        public long? TemplateId { get; set; }
        [Required]
        public string? Subject { get; set; }
        [Required]
        public string? Message { get; set; }
        public string? FilesUrl { get; set; }
    }
}