using System.Globalization;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs
{
    public class SaveFileVM
    {
        public IFormFile? File { get; set; }
        public string? SavePath { get; set; }
    }

    [Route($"api/{ApiConfig.ApiVersion}/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IFilesHelper filesHelper;
        private readonly IMainServiceManager mainServiceManager;


        public FilesController(IFilesHelper filesHelper, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.filesHelper = filesHelper;
            this.session = session;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        [HttpPost("SaveFile")]
        [DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> SaveFile([FromForm] SaveFileVM obj)
        {
            try
            {
                if (obj.File == null)
                    return Ok(await Result<string>.FailAsync("File is null"));

                if (obj.SavePath == "Files")
                    obj.SavePath = FolderNameUpload(); // only Files folder must be like: Files/Facility/Year/Month

                var addFile = await filesHelper.SaveFile(obj.File, obj.SavePath ?? "AllFiles");
                return Ok(await Result<string>.SuccessAsync(addFile));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"{ex.Message}"));
            }
        }

        [HttpPost("SaveFile2")]
        //[Consumes("multipart/form-data")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> SaveFile2(IFormFile file, string savePath)
        {
            try
            {
                var addFile = await filesHelper.SaveFile(file, savePath);
                return Ok(await Result<string>.SuccessAsync(addFile));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"{ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysFileService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete File , MESSAGE: {ex.Message}"));
            }
        }

        //  لايجاد جميع الملفات التابعه لعنصر وقت التعديل  
        [HttpGet("GetFiles")]
        public async Task<IActionResult> GetFiles(long primaryKey, long tableId)
        {
            try
            {
                var item = await mainServiceManager.SysFileService.GetAll(x => x.TableId == tableId && x.PrimaryKey == primaryKey && x.IsDeleted == false);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysFileDto>.FailAsync($"====== Exp in GetFiles, MESSAGE: {ex.Message}"));
            }
        }


        //  تستخدم هذه الدالة غالبا لاضافة ملف من شاشة التعديل 
        [HttpPost("Add")]
        public async Task<ActionResult> Add(SaveFileEditDto obj)
        {
            try
            {
                var newFile = new SysFileDto
                {
                    CreatedBy = session.UserId,
                    CreatedOn = DateTime.Now,
                    FileUrl = obj.FileURL,
                    IsDeleted = false,
                    FileName = obj.FileName,
                    FileDate = DateTime.UtcNow.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture),
                    PrimaryKey = obj.primaryKey,
                    TableId = obj.tableId
                };
                var addFile = await mainServiceManager.SysFileService.Add(newFile);
                return Ok(addFile);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"{ex.Message}"));
            }
        }

        private string FolderNameUpload()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            string month = DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Now.Month);
            string year = DateTime.Now.Year.ToString();

            string folderName = $"Files/Facility{session.FacilityId}/{year}/{month}";
            return folderName;
        }
    }
}
