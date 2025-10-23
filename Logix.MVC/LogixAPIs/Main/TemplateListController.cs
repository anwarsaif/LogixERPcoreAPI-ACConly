using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    public class TemplateListController : BaseMainApiController
    {
        private readonly IPermissionHelper permission;
        private readonly IWebHostEnvironment env;

        public TemplateListController(IPermissionHelper permission,
            IWebHostEnvironment env)
        {
            this.permission = permission;
            this.env = env;
        }

        [HttpGet("GetAllTemplateExcel")]
        public async Task<ActionResult> GetAllTemplateExcel()
        {
            try
            {
                var chk = await permission.HasPermission(747, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                string folderName = "TemplateExcel";
                var folderPath = Path.Combine(env.WebRootPath, folderName);

                if (!Directory.Exists(folderPath))
                {
                    return Ok(await Result.FailAsync("Folder not found"));
                }

                var allFiles = Directory.GetFiles(folderPath);
                List<string> allFilesName = new();
                foreach (var file in allFiles)
                {
                    string fileName = Path.GetFileName(file);
                    if (!string.IsNullOrEmpty(fileName))
                        allFilesName.Add(fileName);
                }
                return Ok(await Result<List<string>>.SuccessAsync(allFilesName));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Exp, MESSAGE: {ex.Message}"));
            }
        }
    }
}