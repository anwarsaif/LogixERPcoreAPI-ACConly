using DocumentFormat.OpenXml.Spreadsheet;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    // العهد
    public class HROhadController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        public HROhadController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, ICurrentData session, IPermissionHelper permission, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.permission = permission;
            this.localization = localization;
        }

        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrOhadFilterDto filter)
        {
            List<HrOhadFilterDto> OhadList = new List<HrOhadFilterDto>();
            var chk = await permission.HasPermission(172, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var BranchesList = session.Branches.Split(',');

                filter.DeptId ??= 0;
                filter.BranchId ??= 0;
                filter.Location ??= 0;
                var items = await hrServiceManager.HrOhadService.GetAllVW(e => e.IsDeleted == false &&
                e.TransTypeId == 1 &&
                BranchesList.Contains(e.BranchId.ToString()) &&
                (filter.BranchId == 0 || filter.BranchId == e.BranchId) &&
                (filter.Location == 0 || filter.Location == e.Location) &&
                (filter.DeptId == 0 || filter.DeptId == e.DeptId) &&
                (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode) &&
                (string.IsNullOrEmpty(filter.EmpName) || (e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName.ToLower())))

                );

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));

                var res = items.Data.AsQueryable();
                if (!res.Any()) return Ok(await Result<object>.SuccessAsync(res, localization.GetResource1("NosearchResult")));


                res = res.OrderBy(e => e.DeptId);


                foreach (var item in res)
                {
                    var newRow = new HrOhadFilterDto
                    {
                        EmpCode = item.EmpCode,
                        BranchId = item.BranchId,
                        OhadId = item.OhdaId,
                        OhadDate = item.OhdaDate,
                        DeptId = item.DeptId,
                        EmpName = item.EmpName,
                        Location = item.Location,
                        Note = item.Note
                    };
                    OhadList.Add(newRow);
                }
                return Ok(await Result<List<HrOhadFilterDto>>.SuccessAsync(OhadList, ""));

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            HrOhadEditDto result = new HrOhadEditDto();
            result.OhadDetails = new List<HrOhadDetailDto>();
            result.fileDtos = new List<SaveFileDto>();
            try
            {
                var chk = await permission.HasPermission(172, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


                var ohadData = await hrServiceManager.HrOhadService.GetOneVW(x => x.IsDeleted == false && x.OhdaId == Id);
                if (!ohadData.Succeeded)
                    return Ok(await Result.FailAsync(ohadData.Status.message));
                result.Code = ohadData.Data.Code;
                result.OhdaDate = ohadData.Data.OhdaDate;
                result.EmpCode = ohadData.Data.EmpCode;
                result.EmpName = ohadData.Data.EmpName;
                result.Note = ohadData.Data.Note;

                var OhadDetails = await hrServiceManager.HrOhadDetailService.GetAllVW(x => x.OhdaId == Id && x.IsDeleted == false);
                if (OhadDetails.Data != null)
                {
                    foreach (var item in OhadDetails.Data)
                    {
                        var newRecord = new HrOhadDetailDto
                        {
                            OhadDetId = item.OhadDetId,
                            ItemName = item.ItemName,
                            ItemState = item.ItemStateName,
                            ItemDetails = item.ItemDetails,
                            QtyIn = item.QtyIn,
                            IsDeleted = item.IsDeleted,
                            Note = item.Note,
                        };
                        result.OhadDetails.Add(newRecord);
                    }
                }

                var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 104);
                result.fileDtos = getFiles.Data ?? new List<SaveFileDto>();

                return Ok(await Result<HrOhadEditDto>.SuccessAsync(result));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrOhadEditDto>.FailAsync($"====== Exp in HROhad getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(172, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrOhadService.Remove(Id);
                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }


        #endregion


        #region Add Page

        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrOhadDto obj)
        {
            var chk = await permission.HasPermission(172, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<List<HrOhadDto>>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                if (string.IsNullOrEmpty(obj.OhdaDate))
                    return Ok(await Result<List<HrOhadDto>>.FailAsync(localization.GetHrResource("CustodyDate")));
                if (!(obj.OhadDetails.Where(x => x.IsDeleted == false).Count() > 0))
                    return Ok(await Result<List<HrOhadDto>>.FailAsync(localization.GetResource1("AddItemsInFirst")));

                obj.EmpIdTo = 0;
                obj.EmpIdRecipient = 0;
                var addRes = await hrServiceManager.HrOhadService.Add(obj);
                return Ok(addRes);

            }

            catch (Exception ex)
            {
                return Ok(await Result<HrOhadDto>.FailAsync(ex.Message));
            }
        }
        #endregion

        #region Edit Page


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrOhadEditDto obj)
        {
            var chk = await permission.HasPermission(172, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (obj.OhdaId <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                if (string.IsNullOrEmpty(obj.EmpCode))
                    return Ok(await Result<List<HrOhadEditDto>>.FailAsync(localization.GetResource1("EmployeeIsNumber")));
                if (!obj.OhadDetails.Any())
                    return Ok(await Result<List<HrOhadEditDto>>.FailAsync(localization.GetResource1("AddItemsInFirst")));
                var edit = await hrServiceManager.HrOhadService.Update(obj);
                return Ok(edit);

            }

            catch (Exception ex)
            {
                return Ok(await Result<HrOhadEditDto>.FailAsync($"{ex.Message}"));
            }
        }

        #endregion

        //BINDLIST_Lang(Drp_Status, "Sys_lookup_Data_VW where ISDEL=0 and Catagories_ID=447", Resources.Resource1.SelectOneFromDDl)
        //BINDLIST_Lang(DrpItems, "Sys_lookup_Data_VW where ISDEL=0 and Catagories_ID=32", Resources.Resource1.SelectOneFromDDl)

    }
}
