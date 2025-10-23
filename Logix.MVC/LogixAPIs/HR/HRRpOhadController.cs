using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  تقرير بعهدة موظف
    public class HRRPOhadController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        public HRRPOhadController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRRPOhadFilterDto filter)
        {
            List<HRRPOhadFilterDto> results = new List<HRRPOhadFilterDto>();
            var chk = await permission.HasPermission(1262, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
        //filter.Branch ??= 0;
        //var BranchesList = session.Branches.Split(',');

        //var GetData = await hrServiceManager.HrOhadService.GetAllVW(e => e.IsDeleted == false
        //&& BranchesList.Contains(e.BranchId.ToString())
        //&& (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower()))
        //&& (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode)

        //);
        //var filteredResult = GetData.Data.Where(e => filter.Branch == 0 || filter.Branch == e.BranchId);

        //foreach (var item in filteredResult)
        //{

        //    var OhadDto = new HRRPOhadFilterDto
        //    {
        //        OhadId = item.OhdaId,
        //        OhdaDate = item.OhdaDate,
        //        EmpCode = item.EmpCode,
        //        EmpName = session.Language == 1 ? item.EmpName : item.EmpName2,
        //        StatusName = session.Language == 1 ? item.StatusName : item.StatusName2,
        //        ItemNo = item.TransTypeId,
        //        ItemName = item.TransTypeName,
        //        Note = item.Note,
        //    };

        //    results.Add(OhadDto);
        //}
        //if (results.Count > 0) return Ok(await Result<List<HRRPOhadFilterDto>>.SuccessAsync(results, ""));
        //return Ok(await Result<List<HRRPOhadFilterDto>>.SuccessAsync(results, localization.GetResource1("NosearchResult")));
                 var GetData = await hrServiceManager.HrOhadService.RPOhadSerach(filter);
                 return Ok(GetData);

            }
            catch (Exception ex)
            {
                return Ok(await Result<HRRPOhadFilterDto>.FailAsync(ex.Message));
            }
        }


        #endregion

    }
}
