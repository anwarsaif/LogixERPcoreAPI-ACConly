using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.Main.ViewModels
{
    public class SendSmsToGroupVM
    {
        [Required]
        public int? GroupId { get; set; }
        public int? BranchId { get; set; }
        [Required]
        public string Message { get; set; } = "";

    }
}