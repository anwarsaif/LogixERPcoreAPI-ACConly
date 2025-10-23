using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{

    //  مهام العمل 
    public class HRAssignmentsController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IApiDDLHelper ddlHelper;


        public HRAssignmentsController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IApiDDLHelper ddlHelper)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.ddlHelper = ddlHelper;
        }



        [HttpGet("EmpIdChanged")]
        public async Task<IActionResult> EmpIdChanged(string EmpId)
        {
            var chk = await permission.HasPermission(1563, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (string.IsNullOrEmpty(EmpId))
            {
                return Ok(await Result<EmpIdChangedVM>.SuccessAsync("there is no id passed"));
            }

            try
            {
                var checkEmpId = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpId == EmpId && i.Isdel == false);
                if (checkEmpId.Succeeded)
                {
                    if (checkEmpId.Data != null)
                    {
                        var item = new EmpIdChangedVM
                        {
                            EmpId = checkEmpId.Data.EmpId,
                            EmpName = checkEmpId.Data.EmpName,
                            BankId = checkEmpId.Data.BankId,
                            BranchId = checkEmpId.Data.BranchId,
                            Gender = checkEmpId.Data.Gender,
                            Iban = checkEmpId.Data.Iban,
                            IdNo = checkEmpId.Data.IdNo,
                            NationalityId = checkEmpId.Data.NationalityId,

                        };
                        return Ok(await Result<EmpIdChangedVM>.SuccessAsync(item));

                    }
                    else
                    {
                        return Ok(await Result<EmpIdChangedVM>.SuccessAsync($"There is No Employee with this Id:  {EmpId}"));

                    }
                }
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{checkEmpId.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrAssignmenFilterDto filter)
        {
            var chk = await permission.HasPermission(1563, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var BranchesList = session.Branches.Split(',');

                List<HrAssignmenFilterDto> resultList = new List<HrAssignmenFilterDto>();
                var items = await hrServiceManager.HrAssignmenService.GetAllVW(e => e.IsDeleted == false && e.FacilityId == session.FacilityId && BranchesList.Contains(e.BranchId.ToString()));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();

                        if (!string.IsNullOrEmpty(filter.EmpId))
                        {
                            res = res.Where(r => r.EmpCode != null && r.EmpCode.Equals(filter.EmpId));
                        }
                        if (!string.IsNullOrEmpty(filter.EmpName))
                        {
                            res = res.Where(c => (c.EmpName != null && c.EmpName.Contains(filter.EmpName)));
                        }

                        if (filter.Id > 0)
                        {
                            res = res.Where(c => c.Id.Equals(filter.Id));
                        }

                        if (filter.LocationId != null && filter.LocationId > 0)
                        {
                            res = res.Where(c => c.Location != null && c.Location == filter.LocationId);
                        }
                        if (filter.TypeId != null && filter.TypeId > 0)
                        {
                            res = res.Where(c => c.TypeId != null && c.TypeId == filter.TypeId);
                        }
                        if (filter.BranchId != null && filter.BranchId > 0)
                        {
                            res = res.Where(c => c.BranchId != null && c.BranchId == filter.BranchId);
                        }

                        if (!string.IsNullOrEmpty(filter.FromDate) && !string.IsNullOrEmpty(filter.ToDate))
                        {
                            var fromDate = DateHelper.StringToDate(filter.FromDate);
                            var toDate = DateHelper.StringToDate(filter.ToDate);
                            res = res.Where(r =>
                            (r.FromDate != null && DateHelper.StringToDate(r.FromDate) >= fromDate && DateHelper.StringToDate(r.FromDate) <= toDate) ||
                            (r.ToDate != null && DateHelper.StringToDate(r.ToDate) >= fromDate && DateHelper.StringToDate(r.ToDate) <= toDate)

                            );
                        }
                        foreach (var item in res)
                        {
                            var newRecord = new HrAssignmenFilterDto
                            {
                                Id = item.Id,
                                EmpName = item.EmpName,
                                EmpId = item.EmpCode,
                                FromDate = item.FromDate,
                                ToDate = item.ToDate,
                                Note = item.Note,
                                TypeName = item.TypeName,
                            };
                            resultList.Add(newRecord);
                        }
                        if (resultList.Any())
                            return Ok(await Result<List<HrAssignmenFilterDto>>.SuccessAsync(resultList, ""));
                        return Ok(await Result<List<HrAssignmenFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<HrAssignmenFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }
                return Ok(await Result<HrAssignmenFilterDto>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrTransferFilterDto>.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1563, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrAssignmenService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HR Assignments Controller, MESSAGE: {ex.Message}"));
            }
        }


        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            try
            {
                var chk = await permission.HasPermission(1563, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (Id <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrAssignmenService.GetOneVW(x => x.Id == Id);
                if (item.Succeeded)
                {
                    return Ok(item);
                }
                return Ok(await Result.FailAsync(item.Status.message));

            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAssignmenEditDto>.FailAsync($"====== Exp in HRTransfersController getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrAssignmenEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1563, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.TypeId <= 0)
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار نوع المهمة "));

                }
                if (string.IsNullOrEmpty(obj.empCode))
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار رقم الموظف "));

                }
                if (string.IsNullOrEmpty(obj.FromDate))
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار من تاريخ "));

                }
                if (string.IsNullOrEmpty(obj.ToDate))
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار إلى تاريخ  "));

                }
                if (string.IsNullOrEmpty(obj.AssignmentDate))
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار تاريخ المهمة  "));

                }
                var update = await hrServiceManager.HrAssignmenService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAssignmenEditDto>.FailAsync($"====== Exp in Edit HR Assignments  Controller, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrAssignmenDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1563, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrAssignmenDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.TypeId <= 0)
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار نوع المهمة "));

                }
                if (string.IsNullOrEmpty(obj.empCode))
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار رقم الموظف "));

                }
                if (string.IsNullOrEmpty(obj.FromDate))
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار من تاريخ "));

                }
                if (string.IsNullOrEmpty(obj.ToDate))
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار إلى تاريخ  "));

                }
                if (string.IsNullOrEmpty(obj.AssignmentDate))
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار تاريخ المهمة  "));

                }

                var add = await hrServiceManager.HrAssignmenService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HR   Assignment Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Assignment2Add")]
        public async Task<ActionResult> Assignment2Add(HrAssignmen2AddDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1563, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrAssignmen2AddDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.TypeId <= 0)
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار نوع المهمة "));

                }
                if (obj.descriptionsDtos.Count() <= 0)
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار موظف واحد على الأقل "));

                }
                if (obj.descriptionsDtos.Any(x => string.IsNullOrEmpty(x.empCode)))
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار أرقام الموظف "));

                }
                if (string.IsNullOrEmpty(obj.FromDate))
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار من تاريخ "));

                }
                if (string.IsNullOrEmpty(obj.ToDate))
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار إلى تاريخ  "));

                }
                if (string.IsNullOrEmpty(obj.AssignmentDate))
                {
                    return Ok(await Result<HrAssignmenEditDto>.FailAsync("يجب اختيار تاريخ المهمة  "));

                }

                var add = await hrServiceManager.HrAssignmenService.Assignment2Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrAssignmen2AddDto>.FailAsync($"====== Exp in Add HR   Assignment Controller  Add Method, MESSAGE: {ex.Message}"));
            }
        }
    }
}