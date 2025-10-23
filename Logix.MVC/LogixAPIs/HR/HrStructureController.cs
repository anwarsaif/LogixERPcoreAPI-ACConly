using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    public class HrStructureController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        private readonly IAccServiceManager accServiceManager;
        private readonly IPMServiceManager pmServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IApiDDLHelper ddlHelper;

        public HrStructureController(IMainServiceManager mainServiceManager,
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session,
            IAccServiceManager accServiceManager,
            IPMServiceManager pmServiceManager,
            IHrServiceManager hrServiceManager,
            IApiDDLHelper ddlHelper
            )
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.localization = localization;

            this.accServiceManager = accServiceManager;
            this.pmServiceManager = pmServiceManager;
            this.hrServiceManager = hrServiceManager;
            this.ddlHelper = ddlHelper;
        }


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(2281, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrStructureService.GetAllVW(c => c.IsDeleted == false);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrStructureDto>.FailAsync($"======= Exp in Search HrStructureVw, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrStructureFilterDto filter)
        {
            var chk = await permission.HasPermission(2281, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.StatusId ??= 0;
                var items = await hrServiceManager.HrStructureService.GetAllVW(c => c.IsDeleted == false &&
                (filter.StatusId == 0 || c.StatusId == filter.StatusId) &&
                (string.IsNullOrEmpty(filter.Name) || (c.Name != null && c.Name.ToLower().Contains(filter.Name)) || (c.Name2 != null && c.Name2.ToLower().Contains(filter.Name.ToLower()))) &&
                (string.IsNullOrEmpty(filter.Code) || (c.Code != null && c.Code.Contains(filter.Code))) &&
                (filter.StatusId == 0 || c.StatusId == filter.StatusId)
                );
                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                if (!items.Succeeded || !items.Data.Any())
                    return Ok(await Result<object>.SuccessAsync( localization.GetResource1("NosearchResult")));

                var res = items.Data.AsQueryable();
                var resultList = new List<object>();
                foreach (var item in res)
                {
                    var newItem = new 
                    {
                        Id = item.Id,
                        Code = item.Code,
                        Name = item.Name,
                        Name2 = item.Name2,
                        Note = item.Note,
                        StatusName = item.StatusName,
                    };
                    resultList.Add(newItem);
                }

                return Ok(await Result<List<object>>.SuccessAsync(resultList, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in Search HrStructureVw, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrStructureDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2281, PermissionType.Add);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (string.IsNullOrEmpty(obj.Name)) return Ok(await Result<object>.FailAsync(localization.GetHrResource("DeptLocationArName")));


                if (string.IsNullOrEmpty(obj.Name2)) return Ok(await Result<object>.FailAsync(localization.GetHrResource("DeptLocationEnName")));

                var add = await hrServiceManager.HrStructureService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrStructureDto>.FailAsync($"======= Exp in Add HrStructureDto, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            try
            {
                var chk = await permission.HasPermission(2281, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }
                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var getItem = await hrServiceManager.HrStructureService.GetOneVW(s => s.Id == id && s.IsDeleted == false);
                return Ok(getItem);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"====== Exp in GetById HrStructureDto, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrStructureEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(2281, PermissionType.Edit);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (string.IsNullOrEmpty(obj.Name)) return Ok(await Result<object>.FailAsync(localization.GetHrResource("DeptLocationArName")));


                if (string.IsNullOrEmpty(obj.Name2)) return Ok(await Result<object>.FailAsync(localization.GetHrResource("DeptLocationEnName")));

                var update = await hrServiceManager.HrStructureService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrStructureDto>.FailAsync($"======= Exp in Edit HrStructureDto, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(2281, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var delete = await hrServiceManager.HrStructureService.Remove(Id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrStructureDto>.FailAsync($"======= Exp in Delete HrStructureDto, MESSAGE: {ex.Message}"));
            }
        }

    }
}
