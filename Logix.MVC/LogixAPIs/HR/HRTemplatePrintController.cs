using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.HR
{

    //   طباعة النماذج
    public class HRTemplatePrintController : BaseHrApiController
    {
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public HRTemplatePrintController(IPermissionHelper permission, ILocalizationService localization, ICurrentData session, IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IWebHostEnvironment hostingEnvironment)
        {
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.hrServiceManager = hrServiceManager;
            this.mainServiceManager = mainServiceManager;
            _hostingEnvironment = hostingEnvironment;
        }


        [HttpPost("Search")]

        //public async Task<IActionResult> Search(SysTemplatePrintFilterDto filter)
        //{
        //    List<HrInsuranceEmpResulteDto> InsuranceList = new List<HrInsuranceEmpResulteDto>();

        //    filter.TemplateId ??= 0;
        //    var chk = await permission.HasPermission(732, PermissionType.Show);
        //    if (!chk)
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    if (filter.TemplateId <= 0)
        //        return Ok(await Result.FailAsync(localization.GetHrResource("Template")));
        //    if (string.IsNullOrEmpty(filter.EmpName))
        //        return Ok(await Result.FailAsync(localization.GetHrResource("EmpName")));
        //    if (string.IsNullOrEmpty(filter.EmpCode))
        //        return Ok(await Result.FailAsync(localization.GetHrResource("EmpNo")));
        //    try
        //    {
        //        var checkEmpExist = await hrServiceManager.HrEmployeeService.GetOne(x => x.EmpId == filter.EmpCode);
        //        if (checkEmpExist.Data == null) return Ok(await Result.FailAsync(localization.GetResource1("EmployeeNotFound")));

        //        var GetSysTemplate = await mainServiceManager.SysTemplateService.GetOneVW(x => x.IsDeleted == false && x.Id == filter.TemplateId);
        //        if (GetSysTemplate.Data == null) return Ok(await Result.FailAsync("Template Not Found"));

        //        var typeId = GetSysTemplate.Data.TypeId;
        //        var empId = checkEmpExist.Data.Id;

        //        var newFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "TempImages", "Template_" + DateTime.Now.ToString("ddMMyyHHmmss") + ".docx");

        //        if (GetSysTemplate.Data.TypeId > 0)
        //        {
        //            if (typeId == 1) // Text template case
        //            {
        //                var xData = await mainServiceManager.SysTemplateService.ReplaceDate(newFilePath, (int)typeId, empId);
        //                return Content(xData); // Returns plain text content
        //            }
        //            else if (typeId == 2) // File template case
        //            {
        //                if (!System.IO.File.Exists(newFilePath) && !string.IsNullOrEmpty(GetSysTemplate.Data.Url))
        //                {
        //                    var sourceFile = Path.Combine(_hostingEnvironment.WebRootPath, GetSysTemplate.Data.Url);

        //                    System.IO.File.Copy(sourceFile, newFilePath);
        //                    await ReplaceDate(newFilePath, typeId, empId);

        //                    var fileBytes = System.IO.File.ReadAllBytes(newFilePath);
        //                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Contract.docx");
        //                }
        //            }
        //        }


        //        return Content(newFilePath);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result.FailAsync(ex.Message));
        //    }
        //}

        public async Task<IActionResult> Search(SysTemplatePrintFilterDto filter)
        {
            filter.TemplateId ??= 0;
            var hasPermission = await permission.HasPermission(732, PermissionType.Show);
            if (!hasPermission)
                return Ok(await Result.AccessDenied("AccessDenied"));

            if (filter.TemplateId <= 0)
                return Ok(await Result.FailAsync(localization.GetHrResource("Template")));

            if (string.IsNullOrEmpty(filter.EmpName))
                return Ok(await Result.FailAsync(localization.GetHrResource("EmpName")));

            if (string.IsNullOrEmpty(filter.EmpCode))
                return Ok(await Result.FailAsync(localization.GetHrResource("EmpNo")));

            try
            {
                var employee = await hrServiceManager.HrEmployeeService.GetOne(x => x.EmpId == filter.EmpCode);
                if (employee.Data == null)
                    return Ok(await Result.FailAsync(localization.GetResource1("EmployeeNotFound")));

                var sysTemplate = await mainServiceManager.SysTemplateService.GetOneVW(x => x.IsDeleted == false && x.Id == filter.TemplateId);
                if (sysTemplate.Data == null)
                    return Ok(await Result.FailAsync("Template Not Found"));

                var typeId = sysTemplate.Data.TypeId;
                var TemplateId = sysTemplate.Data.Id;
                var empId = employee.Data.Id;

                if (typeId > 0)
                {
                    if (typeId == 1) // Text template
                    {
                        var xData = await mainServiceManager.SysTemplateService.ReplaceDate(null, (int)typeId, empId, TemplateId);
                        return Ok(await Result<object>.SuccessAsync(xData,"",200));
                    }
                    else if (typeId == 2) // File template
                    {
                        var newFileName = $"Template_{DateTime.Now:ddMMyyHHmmss}.docx";
                        var newFilePath = Path.Combine(_hostingEnvironment.WebRootPath, "TempImages", newFileName);

                        // Check if the source template URL is available
                        if (!string.IsNullOrEmpty(sysTemplate.Data.Url))
                        {
                            var sourceFile = Path.Combine(_hostingEnvironment.WebRootPath, sysTemplate.Data.Url);

                            if (System.IO.File.Exists(sourceFile))
                            {
                                // Copy source file to a new path
                                System.IO.File.Copy(sourceFile, newFilePath, true);

                                // Await the file modification
                                var fileUrl = await mainServiceManager.SysTemplateService.ReplaceDate(newFilePath, (int)typeId, empId, TemplateId);

                                // Return the file URL as a result
                                if (!string.IsNullOrEmpty(fileUrl))
                                {
                                    return Ok(await Result<object>.SuccessAsync(fileUrl,"", 200));

                                }
                                else
                                {
                                    return Ok(await Result.FailAsync("Failed to generate file URL"));
                                }
                            }
                            else
                            {
                                return Ok(await Result.FailAsync("Source file not found"));
                            }
                        }
                        else
                        {
                            return Ok(await Result.FailAsync("Template URL is invalid"));
                        }
                    }
                }

                return Ok(await Result.FailAsync("Invalid template type"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, await Result.FailAsync(ex.Message));
            }
        }
    }
}