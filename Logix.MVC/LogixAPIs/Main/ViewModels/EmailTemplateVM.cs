using Logix.Application.DTOs.Main;

namespace Logix.MVC.LogixAPIs.Main.ViewModels
{
    public class EmailTemplateVM
    {
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public List<SysFileDto> TemlateFiles { get; set; }

        public EmailTemplateVM()
        {
            TemlateFiles = new List<SysFileDto>();
        }
    }
}