using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.Main
{
    public class SysLicenseController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IFilesHelper filesHelper;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public SysLicenseController(IMainServiceManager mainServiceManager,
            IAccServiceManager accServiceManager,
            IPermissionHelper permission,
            IFilesHelper filesHelper,
            ILocalizationService localization,
            ICurrentData session)
        {
            this.mainServiceManager = mainServiceManager;
            this.accServiceManager = accServiceManager;
            this.permission = permission;
            this.filesHelper = filesHelper;
            this.localization = localization;
            this.session = session;
        }

        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysLicenseFilterDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(497, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var items = await mainServiceManager.SysLicenseService.GetAllVW();
                if (items.Succeeded)
                {
                    var res = items.Data.OrderBy(s => s.Id).AsQueryable();
                    
                    if (filter.FacilityId != null && filter.FacilityId > 0)
                    {
                        res = res.Where(r => r.FacilityId.Equals(filter.FacilityId));
                    }
                    if (filter.LicenseType != null && filter.LicenseType > 0)
                    {
                        res = res.Where(r => r.LicenseType.Equals(filter.LicenseType));
                    }
                    if (filter.BranchId != null && filter.BranchId > 0)
                    {
                        res = res.Where(r => r.BranchId.Equals(filter.BranchId));
                    }
                    if (!string.IsNullOrEmpty(filter.LicenseNo))
                    {
                        res = res.Where(r => r.LicenseNo != null && r.LicenseNo.Equals(filter.LicenseNo));
                    }
                    if (!string.IsNullOrEmpty(filter.ExpireFrom))
                    {
                        res = res.Where(r => r.ExpiryDate != null && DateTime.ParseExact(r.ExpiryDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(filter.ExpireFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }
                    if (!string.IsNullOrEmpty(filter.ExpireTo))
                    {
                        //IssuedDate I used it for "Expiry to"
                        res = res.Where(r => r.ExpiryDate != null && DateTime.ParseExact(r.ExpiryDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= DateTime.ParseExact(filter.ExpireTo, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                    }

                    var final = res.ToList();

                    return Ok(await Result<List<SysLicensesVw>>.SuccessAsync(final));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAlll SysLicenseController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(SysLicenseDto entity/*, IFormFile? file*/)
        {
            try
            {
                var chk = await permission.HasPermission(497, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                //if (file != null && file.Length > 0)
                //{
                //    if (!ChkExtension(file))
                //        return Ok(await Result.FailAsync($"{localization.GetResource1("ImageFormat")}"));
                    
                //    var addFile = await filesHelper.SaveFile(file, "AllFiles");
                //    if (!string.IsNullOrEmpty(addFile))
                //    {
                //        entity.FileUrl = addFile;
                //    }
                //}
                var addLicense = await mainServiceManager.SysLicenseService.Add(entity);
                return Ok(addLicense);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add SysLicenseController, MESSAGE: {ex.Message}"));
            }
        }

        private bool ChkExtension(IFormFile file)
        {
            string[] acceptedExtension = { ".gif", ".png", ".jpeg", ".jpg", ".pdf" };
            string extension = "." + file.FileName.Split('.').Last().ToLower();

            if (acceptedExtension.Contains(extension))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(497, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await mainServiceManager.SysLicenseService.GetForUpdate<SysLicenseDto>(id);
                return Ok(item);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetByIdForEdit SysLicenseController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(SysLicenseDto entity/*, IFormFile? file*/)
        {
            try
            {
                var chk = await permission.HasPermission(497, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                //if (file != null && file.Length > 0)
                //{
                //    if (!ChkExtension(file))
                //        return Ok(await Result.FailAsync($"{localization.GetResource1("ImageFormat")}"));
                //    var addFile = await filesHelper.SaveFile(file, "AllFiles");
                //    if (!string.IsNullOrEmpty(addFile))
                //    {
                //        entity.FileUrl = addFile;
                //    }
                //}

                var update = await mainServiceManager.SysLicenseService.Update(entity);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Edit SysLicenseController, MESSAGE: {ex.Message}"));
            }
        }


        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(497, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await mainServiceManager.SysLicenseService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete SysLicenseController, MESSAGE: {ex.Message}"));
            }
        }
        
    }
}
