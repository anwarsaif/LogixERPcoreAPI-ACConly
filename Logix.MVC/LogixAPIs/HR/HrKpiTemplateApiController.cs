using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.OPM;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Logix.MVC.LogixAPIs.HR
{

    // نماذج تقييم الأداء
    public class HrKpiTemplateApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;

        public HrKpiTemplateApiController(IHrServiceManager hrServiceManager, IDDListHelper listHelper, IMainServiceManager mainServiceManager, ICurrentData session, IPermissionHelper permission, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.permission = permission;
            this.listHelper = listHelper;
            this.localization = localization;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(336, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrKpiTemplateService.GetAllVW(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(items);
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("SearchHrKpiTemplates")]
        public async Task<IActionResult> GetAllSearch(HrKpiTemplatesVw filter)
        {
            var chk = await permission.HasPermission(336, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrKpiTemplateService.GetAllVW(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter == null)
                    {
                        return Ok(items);
                    }
                    if (filter.TypeId != null && filter.TypeId > 0)
                        res = res.Where(r => r.TypeId != null && r.TypeId.Equals(filter.TypeId));
                    if (!string.IsNullOrEmpty(filter.Name))
                    {
                        res = res.Where(c => (c.Name != null && c.Name.ToLower().Contains(filter.Name.ToLower())) || (c.Name2 != null && c.Name2.ToLower().Contains(filter.Name.ToLower())));
                    }

                    res = res.OrderBy(e => e.Id);
                    var final = res.ToList();
                    if (final.Count > 0)
                        return Ok(await Result<List<HrKpiTemplatesVw>>.SuccessAsync(final, ""));
                    return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrKpiTemplateDto obj)
        {
            var chk = await permission.HasPermission(336, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var addRes = await hrServiceManager.HrKpiTemplateService.Add(obj);
                return Ok(addRes);

            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            var chk = await permission.HasPermission(336, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));
            }


            try
            {
                var getItem = await hrServiceManager.HrKpiTemplateService.GetOneVW(x => x.IsDeleted == false && x.Id == Id);
                return Ok(getItem);

            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrKpiTemplateEditDto obj)
        {
            var chk = await permission.HasPermission(336, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {

                var Edit = await hrServiceManager.HrKpiTemplateService.Update(obj);
                return Ok(Edit);
            }

            catch (Exception ex)
            {
                return Ok($"{await Result.FailAsync(ex.Message)}");
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(336, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));


            try
            {
                var del = await hrServiceManager.HrKpiTemplateService.Remove(Id);
                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync(exp.Message));
            }
        }
        //[HttpGet("GetDDLKpiTemplate")]
        //public async Task<IActionResult> GetDDLKpiTemplate()
        //{
        //    int lang = session.Language;
        //    try
        //    {
        //        var list = await listHelper.GetList(513, hasDefault: true, defaultText: "all");
        //        return Ok(await Result<object>.SuccessAsync(list, ""));

        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(await Result.FailAsync(ex.Message));
        //    }
        //}

        [HttpGet("GetDDLSysGroup")]
        public async Task<IActionResult> GetDDLSysGroup()
        {
            try
            {
                var AllSysGroup = await mainServiceManager.SysGroupService.GetAll(s => s.IsDeleted == false && s.IsDeleted == false);
                var AllSysGroupList = AllSysGroup.Data.AsEnumerable().Select(e => new { Id = e.GroupId, Name = e.GroupName });
                return Ok(await Result<object>.SuccessAsync(AllSysGroupList, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
    }

}