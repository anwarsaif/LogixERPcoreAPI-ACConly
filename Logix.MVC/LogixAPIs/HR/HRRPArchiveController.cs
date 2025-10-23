using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //    تقرير الأرشيف
    public class HRRPArchiveController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        public HRRPArchiveController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HRRPArchiveFilterDto filter)
        {
            var chk = await permission.HasPermission(2043, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            if (filter.FacilityId <= 0)
            {
                return Ok(await Result<object>.FailAsync("الشركة"));

            }
            try
            {

                List<HRRPArchiveFilterDto> resultList = new List<HRRPArchiveFilterDto>();
                var items = await mainServiceManager.SysFileService.GetAll(e => e.IsDeleted == false &&
                (string.IsNullOrEmpty(filter.fileName) || e.FileName.ToLower().Contains(filter.fileName)) &&
                (filter.TransactionType == 0 || filter.TransactionType == null || filter.TransactionType == e.TableId) &&
                (filter.FacilityId == e.FacilityId)
                );
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();
                        if (!string.IsNullOrEmpty(filter.From) && !string.IsNullOrEmpty(filter.To))
                        {
                            res = res.Where(c => (c.FileDate != null && DateHelper.StringToDate(c.FileDate) >= DateHelper.StringToDate(filter.From) && DateHelper.StringToDate(c.FileDate) <= DateHelper.StringToDate(filter.To)));
                        }
                        if (res.Count() > 0)
                        {
                            foreach (var item in res)
                            {
                                var newItem = new HRRPArchiveFilterDto
                                {

                                    Id = item.Id,
                                    fileName = item.FileName,
                                    FileDescription = item.FileDescription,
                                    FileDate = item.FileDate,
                                    FileUrl = item.FileUrl
                                };
                                resultList.Add(newItem);
                            }
                            if (resultList.Count > 0) return Ok(await Result<List<HRRPArchiveFilterDto>>.SuccessAsync(resultList));
                            return Ok(await Result<List<HRRPArchiveFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                        }
                        return Ok(await Result<List<HRRPArchiveFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                    }
                    return Ok(await Result<List<HRRPArchiveFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HRRPArchiveFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HRRPArchiveFilterDto>.FailAsync(ex.Message));
            }
        }
        #endregion

    }
}