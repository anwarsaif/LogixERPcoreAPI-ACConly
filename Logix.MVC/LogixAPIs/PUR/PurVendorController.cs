using HarfBuzzSharp;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PUR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.PUR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.PUR
{
    public class PurVendorController : BasePurApiController
    {
        private readonly IPurServiceManager purServiceManager;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;

        public PurVendorController(IPurServiceManager purServiceManager,
            IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ICurrentData CurrentData,
            ILocalizationService localization,
            ICurrentData session)
        {
            this.purServiceManager = purServiceManager;
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
            this.session = session;
        }
        #region "GetAll - Search - GetAllFileTypes"
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(1043, PermissionType.Show);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));
            try
            {
                var items = await mainServiceManager.SysCustomerService.GetAll(e => e.IsDeleted == false);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAll, MESSAGE: {ex.Message}"));
            }
        }
        
        [HttpPost("Search")]
        public async Task<ActionResult> Search(SysCustomerFilterForPURDto filter)
        {
            try
            {
                var chk = await permission.HasPermission(1043, PermissionType.Show);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                filter.FacilityId ??= 0;
                filter.BranchId ??= 0;
                filter.GroupId ??= 0;
                var language = session.Language;
                var items = await mainServiceManager.SysCustomerService.GetAll(x => x.IsDeleted == false
                && (string.IsNullOrEmpty(filter.Name) || (language == 2 ? x.Name2 : x.Name)  == filter.Name)
                && (string.IsNullOrEmpty(filter.Code) || x.Code == filter.Code)
                && (filter.FacilityId == 0 || x.FacilityId == filter.FacilityId)
                && (filter.BranchId == 0 || x.BranchId == filter.BranchId)
                && (filter.GroupId == 0 || x.GroupId == filter.GroupId)
                && (string.IsNullOrEmpty(filter.Mobile) || x.Mobile == filter.Mobile)
                && (x.CusTypeId == 8)
                );
                if ((items.Succeeded))
                {
                    var resultData = items.Data.Select(x => new SysCustomerDisplayForPURDto
                    {
                        Code = x.Code,
                        Name = x.Name,
                        Name2 = x.Name2,
                        Mobile = x.Mobile,
                        Phone = x.Phone,
                        Fax = x.Fax,
                        Email = x.Email
                    }).ToList();

                    return Ok(await Result<List<SysCustomerDisplayForPURDto>>.SuccessAsync(resultData, $"{resultData.Count}"));
                }
                return Ok(await Result<SysCustomerFilterForPURDto>.SuccessAsync(filter, localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Search, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetAllFileTypes")]
        public async Task<IActionResult> GetAllFileTypes()
        {
            var chk = await permission.HasPermission(1043, PermissionType.Show);
            if (!chk)
                return Ok(await Result.AccessDenied("AccessDenied"));
            try
            {
                var fileList = new List<GetAllSysCustomerFileTypesDto>();

                bool isArabic = session.Language == 1;

                fileList.Add(new GetAllSysCustomerFileTypesDto
                {
                    FileType = 1,
                    FileTypeName = isArabic ? "السجل التجاري" : "C.R",
                    FileTypeName2 = "C.R",
                    FileName = "",
                    Date = DateTime.Now
                });

                fileList.Add(new GetAllSysCustomerFileTypesDto
                {
                    FileType = 2,
                    FileTypeName = isArabic ? "الغرفة التجارية" : "Chamber of Commerce",
                    FileTypeName2 = "Chamber of Commerce",
                    FileName = "",
                    Date = DateTime.Now
                });

                fileList.Add(new GetAllSysCustomerFileTypesDto
                {
                    FileType = 3,
                    FileTypeName = isArabic ? "شهادة السعودة" : "Saudization certificate",
                    FileTypeName2 = "Saudization certificate",
                    FileName = "",
                    Date = DateTime.Now
                });

                fileList.Add(new GetAllSysCustomerFileTypesDto
                {
                    FileType = 4,
                    FileTypeName = isArabic ? "شهادة مصلحة الزكاة والدخل" : "Certificate of Zakat and Income Tax",
                    FileTypeName2 = "Certificate of Zakat and Income Tax",
                    FileName = "",
                    Date = DateTime.Now
                });

                fileList.Add(new GetAllSysCustomerFileTypesDto
                {
                    FileType = 5,
                    FileTypeName = isArabic ? "شهادة ضريبة القيمة المضافة" : "Certificate of VAT",
                    FileTypeName2 = "Certificate of VAT",
                    FileName = "",
                    Date = DateTime.Now
                });

                fileList.Add(new GetAllSysCustomerFileTypesDto
                {
                    FileType = 6,
                    FileTypeName = isArabic ? "التامينات الإجتماعية" : "Certificate of GOSI",
                    FileTypeName2 = "Certificate of GOSI",
                    FileName = "",
                    Date = DateTime.Now
                });

                fileList.Add(new GetAllSysCustomerFileTypesDto
                {
                    FileType = 7,
                    FileTypeName = isArabic ? "وثيقة العمل الحر" : "Freelance work document",
                    FileTypeName2 = "Freelance work document",
                    FileName = "",
                    Date = DateTime.Now
                });

                fileList.Add(new GetAllSysCustomerFileTypesDto
                {
                    FileType = 8,
                    FileTypeName = isArabic ? "صورة بطاقة الأحوال" : "ID",
                    FileTypeName2 = "ID",
                    FileName = "",
                    Date = DateTime.Now
                });
                return Ok(fileList);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in GetAll, MESSAGE: {ex.Message}"));
            }
        }

        #endregion "GetAll - Search - GetAllFileTypes"

        #region "Add - AddPortalVendor - Edit - ApproveVendor"
        [HttpPost("Add")]
        public async Task<IActionResult> Add(SysCustomerAddQVDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1043, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await mainServiceManager.SysCustomerService.AddQualifiedVendor(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }
        [HttpPost("AddPortalVendor")]
        public async Task<IActionResult> AddPortalVendor(SysCustomerAddPVDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1043, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync(localization.GetMessagesResource("dataRequire")));

                var add = await mainServiceManager.SysCustomerService.AddPortalVendor(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(SysCustomerEditQVDto obj)
        {
            var chk = await permission.HasPermission(1043, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<SysCustomerEditQVDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var Edit = await mainServiceManager.SysCustomerService.UpdateQualifiedVendor(obj, false);
                if (Edit.Succeeded)
                {
                    return Ok(Edit);
                }
                else
                {
                    return Ok(await Result<SysCustomerEditQVDto>.FailAsync(localization.GetResource1("UpdateError")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysCustomerEditQVDto>.FailAsync($"======= Exp in PurItemsPriceM edit: {ex.Message}"));
            }
        }

        [HttpPost("ApproveVendor")]
        public async Task<IActionResult> ApproveVendor(SysCustomerEditQVDto obj)
        {
            var chk = await permission.HasPermission(1043, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<SysCustomerEditQVDto>.FailAsync(localization.GetMessagesResource("dataRequire")));
                }
                var Edit = await mainServiceManager.SysCustomerService.UpdateQualifiedVendor(obj, true);
                if (Edit.Succeeded)
                {
                    return Ok(Edit);
                }
                else
                {
                    return Ok(await Result<SysCustomerEditQVDto>.FailAsync(localization.GetResource1("UpdateError")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysCustomerEditQVDto>.FailAsync($"======= Exp in PurItemsPriceM edit: {ex.Message}"));
            }
        }
        #endregion "Add - Edit - AddPortalVendor - ApproveVendor"

        #region "Delete"
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(1043, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var removedItem = await mainServiceManager.SysCustomerService.RemoveQualifiedVendor(Id);
                if (removedItem.Succeeded)
                {
                    return Ok(removedItem);
                }
                else
                {
                    return Ok(await Result<SysCustomerAddQVDto>.FailAsync(localization.GetResource1("DeleteFail")));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysCustomerAddQVDto>.FailAsync($"======= Exp in Delete: {ex.Message}"));
            }
        }
        #endregion "Delete"

        #region "GetByIdForEdit - GetById - GetFilesByCustomerId"

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1043, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<SysCustomerEditQVDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await mainServiceManager.SysCustomerService.GetForUpdate<SysCustomerEditQVDto>(id);
                if (getItem.Succeeded)
                {
                    var files = await mainServiceManager.SysCustomerFileService.GetAll(x => x.IsDeleted == false && x.CustomerId == id);
                    getItem.Data.FileDtos = files.Data.ToList();
                    var contacts = await mainServiceManager.SysCustomerContactService.GetAll(x => x.IsDeleted == false && x.CusId == id);
                    getItem.Data.CustomerContactDtos = contacts.Data.ToList();
                    return Ok(await Result<SysCustomerEditQVDto>.SuccessAsync(getItem.Data, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1043, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AcessDenied"));
                }

                if (id <= 0)
                {
                    return Ok(await Result<SysCustomerDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
                }

                var getItem = await mainServiceManager.SysCustomerService.GetOne(s => s.Id == id && s.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<SysCustomerDto>.SuccessAsync(obj, $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysCustomerDto>.FailAsync($"====== Exp MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetFilesByCustomerId")]
        public async Task<IActionResult> GetFilesByCustomerId(long id)
        {
            try
            {
                if (id <= 0)
                {
                    return Ok(await Result<SysCustomerFileDto>.FailAsync(localization.GetMessagesResource("InCorrectId")));
                }

                var getItem = await mainServiceManager.SysCustomerFileService.GetAll(x => x.CustomerId == id && x.IsDeleted == false);
                if (getItem.Succeeded)
                {
                    var obj = getItem.Data;
                    return Ok(await Result<List<SysCustomerFileDto>>.SuccessAsync(obj.ToList(), $""));
                }
                else
                {
                    return Ok(getItem);
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<SysCustomerFileDto>>.FailAsync($"====== Exp in GetFilesByCustomerId, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "GetByIdForEdit - GetById - GetFilesByCustomerId"
    }
}
