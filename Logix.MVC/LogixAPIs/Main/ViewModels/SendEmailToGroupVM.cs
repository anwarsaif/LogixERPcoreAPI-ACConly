using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.Main.ViewModels
{
    public class SendEmailToGroupVM
    {
        [Required]
        public int? TypeId { get; set; }
        [Required]
        public string? Subject { get; set; }
        [Required]
        public string? Message { get; set; }
    }
}