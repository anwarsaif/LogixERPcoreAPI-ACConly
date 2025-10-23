using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{


    //   المسيرات المعتمدة
    public class HRPayrollApprovedController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IMainServiceManager mainServiceManager;

        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly ISysConfigurationHelper sysConfigurationHelper;
        private readonly IAccServiceManager accServiceManager;


        public HRPayrollApprovedController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, ISysConfigurationHelper sysConfigurationHelper, IAccServiceManager accServiceManager, IMainServiceManager mainServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.sysConfigurationHelper = sysConfigurationHelper;
            this.accServiceManager = accServiceManager;
            this.mainServiceManager = mainServiceManager;
        }

        #region IndexPage


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrPayrollFilterDto filter)
        {
            var chk = await permission.HasPermission(435, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (filter.FinancelYear == 0 || filter.FinancelYear == null)
                {
                    return Ok(await Result<HrPayrollFilterDto>.FailAsync(" يجب اختيار السنة المالية"));
                }

                var BranchesList = session.Branches.Split(',');
                List<HrPayrollApproveFilterResultDto> resultList = new List<HrPayrollApproveFilterResultDto>();

                // Get all journals (assuming you want all for filtering)
                var allJournals = await accServiceManager.AccJournalMasterService.GetAll(x => x.FlagDelete == false && x.DocTypeId == 24 && x.ReferenceNo != null);

                // Get filtered payroll data
                var items = await hrServiceManager.HrPayrollService.GetAllVW(e => e.IsDeleted == false &&
                                                                            (e.BranchId == 0 || BranchesList.Contains(e.BranchId.ToString())) &&
                                                                             e.State == 4 &&
                                                                             e.FacilityId == session.FacilityId &&
                                                                             (filter.FinancelYear == null || filter.FinancelYear == 0 || filter.FinancelYear == e.FinancelYear) &&
                                                                             (filter.PayrollTypeId == null || filter.PayrollTypeId == 0 || filter.PayrollTypeId == e.PayrollTypeId) &&
                                                                             (filter.MsCode == null || filter.MsCode == 0 || filter.MsCode == e.MsCode) &&
                                                                             (filter.ApplicationCode == null || filter.ApplicationCode == 0 || filter.ApplicationCode == e.ApplicationCode) &&
                                                                             (string.IsNullOrEmpty(filter.MsMonth) || Convert.ToInt32(filter.MsMonth) == Convert.ToInt32(e.MsMonth)) &&
                                                                             (string.IsNullOrEmpty(filter.MsTitle) || Convert.ToInt32(filter.MsTitle) == Convert.ToInt32(e.MsTitle)));

                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        foreach (var payroll in items.Data)
                        {
                            var matchingJournal = allJournals.Data.FirstOrDefault(j => j.ReferenceNo == payroll.MsId);

                            resultList.Add(new HrPayrollApproveFilterResultDto
                            {
                                MsId = payroll.MsId,
                                MsCode = payroll.MsCode,
                                MsDate = payroll.MsDate,
                                FinancelYear = payroll.FinancelYear,
                                StatusName = (session.Language == 1) ? payroll.StatusName : payroll.StatusName2,
                                TypeName = (session.Language == 1) ? payroll.TypeName : payroll.TypeName2,
                                MsTitle = payroll.MsTitle,
                                MsMonth = payroll.MsMonth,
                                ApplicationCode = payroll.ApplicationCode,
                                PayrollTypeId = payroll.PayrollTypeId,
                                Status = payroll.State,
                                JCode = matchingJournal?.JCode,
                                JDateGregorian = matchingJournal?.JDateGregorian,
                            });
                        }

                        if (resultList.Any())
                        {
                            return Ok(await Result<List<HrPayrollApproveFilterResultDto>>.SuccessAsync(resultList, ""));
                        }
                        return Ok(await Result<List<HrPayrollApproveFilterResultDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrPayrollApproveFilterResultDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrPayrollApproveFilterResultDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollApproveFilterResultDto>.FailAsync(ex.Message));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(435, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrPayrollService.BindApprovedPayroll(Id);
                var files = await mainServiceManager.SysFileService.GetAll(x => x.IsDeleted == false && x.PrimaryKey == Id && x.TableId == 37);
                return Ok(await Result<object>.SuccessAsync(new { data = item.Data, fileDtos = files }, ""));
                //return Ok(item);

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrPayrollApprovedDto>.FailAsync($"====== Exp in HR Manual Payroll Controller getById, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

        #region AddPage Business
        [HttpPost("SearchInAdd")]
        public async Task<IActionResult> SearchInAdd(HrPayrollApprovedFilterAndAddDto filter)
        {
            var chk = await permission.HasPermission(435, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (filter.FinancelYear == 0 || filter.FinancelYear == null)
                {
                    return Ok(await Result<object>.FailAsync(" يجب اختيار السنة المالية"));

                }
                if (filter.MSId == 0 || filter.MSId == null)
                {
                    return Ok(await Result<object>.FailAsync(" يجب تحديد رقم المسير"));

                }
                if (string.IsNullOrEmpty(filter.MSMonth) || filter.MSMonth == "0" || filter.MSMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync(" يجب تحديد الشهر"));
                }
                var items = await hrServiceManager.HrPayrollDService.GetAllVW(e =>
                filter.MSId == e.MsId &&

                e.IsDeleted == false &&
                (filter.FinancelYear == null || filter.FinancelYear == 0 || filter.FinancelYear == e.FinancelYear) &&
                (filter.FacilityID == null || filter.FacilityID == 0 || filter.FacilityID == e.FacilityId) &&
                (filter.DeptID == null || filter.DeptID == 0 || filter.DeptID == e.DeptId) &&
                (filter.BRANCHID == null || filter.BRANCHID == 0 || filter.BRANCHID == e.BranchId) &&
                (filter.Location == null || filter.Location == 0 || filter.Location == e.Location) &&
                (filter.SponsorsID == null || filter.SponsorsID == 0 || filter.SponsorsID == e.SponsorsId) &&
                (string.IsNullOrEmpty(filter.MSMonth) || Convert.ToInt32(filter.MSMonth) == Convert.ToInt32(e.MsMonth)));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        return Ok(await Result<IEnumerable<object>>.SuccessAsync(items.Data.ToList(), ""));
                    }
                    return Ok(await Result<IEnumerable<object>>.SuccessAsync(items.Data.ToList(), localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<object>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrPayrollApprovedFilterAndAddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(435, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<string>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.FinancelYear == 0 || obj.FinancelYear == null)
                {
                    return Ok(await Result<object>.FailAsync(" يجب اختيار السنة المالية"));

                }
                if (obj.MSId == 0 || obj.MSId == null)
                {
                    return Ok(await Result<object>.FailAsync(" يجب تحديد رقم المسير"));

                }
                if (string.IsNullOrEmpty(obj.MSMonth) || obj.MSMonth == "0" || obj.MSMonth == "00")
                {
                    return Ok(await Result<object>.FailAsync(" يجب تحديد الشهر"));
                }
                //if (string.IsNullOrEmpty(obj.MSMonthText))
                //{
                //    return Ok(await Result<object>.FailAsync(" يجب ارسال اسم الشهر بشكل صحيح"));
                //}
                var add = await hrServiceManager.HrPayrollService.AddNewApprovedPayroll1(obj);
                if (add.Succeeded)
                {
                    // webhook
                    var send = await mainServiceManager.SysWebHookService.SendToWebHook(add.Data, 435, session.UserId, session.FacilityId, ProcessType.Added);
                }
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"====== Exp in Add HR   PayrollApproved Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }


        #endregion
    }
}