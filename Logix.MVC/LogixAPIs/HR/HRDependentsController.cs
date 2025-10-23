using DevExpress.CodeParser;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.Pdf.Xmp;
using DocumentFormat.OpenXml.Wordprocessing;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logix.MVC.LogixAPIs.HR
{

    // التابعين 
    public class HRDependentsController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRDependentsController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrDependentVM filter)
        {
            var chk = await permission.HasPermission(576, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HrDependentVM> resultList = new List<HrDependentVM>();
                var BranchesList = session.Branches.Split(',');

                filter.BranchId ??= 0;
                var items = await hrServiceManager.HrDependentService.GetAllVW(e => e.IsDeleted == false && e.FacilityId == session.FacilityId &&
                (string.IsNullOrEmpty(filter.EmpId) || e.EmpCode == filter.EmpId) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.Contains(filter.EmpName)) &&
                (BranchesList == null || BranchesList.Contains(e.BranchId.ToString())) &&
                (filter.BranchId == 0 || e.BranchId == filter.BranchId));
                if (items.Succeeded)
                {
                    if (items.Data.Count() > 0)
                    {
                        var res = items.Data.AsQueryable();

                        string format = "yyyy/MM/dd";
                        int age = 0;

                        foreach (var item in res)
                        {
                            age = 0;
                            if (!string.IsNullOrEmpty(item.DateOfBirth))
                            {
                                DateTime birthDate = DateTime.ParseExact(item.DateOfBirth, format, System.Globalization.CultureInfo.InvariantCulture);
                                DateTime today = DateTime.Today;

                                age = today.Year - birthDate.Year;

                                if (birthDate > today.AddYears(-age))
                                {
                                    age -= 1;
                                }
                            }

                            var newRecord = new HrDependentVM
                            {
                                DependentId = item.Id,
                                DateOfBirth = item.DateOfBirth,
                                EmpId = item.EmpCode,
                                EmpName = item.EmpName,
                                Name = item.Name,
                                Name2 = item.Name1,
                                Relationship = Convert.ToInt32(item.Relationship),
                                RelationshipName = item.RelationshipName,
                                Insurance = item.Insurance,
                                Ticket = item.Ticket,
                                Age = age
                            };

                            resultList.Add(newRecord);
                        }

                        return Ok(await Result<List<HrDependentVM>>.SuccessAsync(resultList, ""));
                    }

                    return Ok(await Result<List<HrDependentVM>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));

                }

                return Ok(await Result<HrDependentVM>.FailAsync(items.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrDependentVM filter, int take = Pagination.take, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(576, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HrDependentVM> resultList = new List<HrDependentVM>();

                var BranchesList = session.Branches.Split(',');
                filter.BranchId ??= 0;

                var items = await hrServiceManager.HrDependentService.GetAllWithPaginationVW(selector: e => e.Id,
                expression: e => e.IsDeleted == false && e.FacilityId == session.FacilityId &&
                (string.IsNullOrEmpty(filter.EmpId) || e.EmpCode == filter.EmpId) &&
                (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.Contains(filter.EmpName)) &&
                (BranchesList == null || BranchesList.Contains(e.BranchId.ToString())) &&
                (filter.BranchId == 0 || e.BranchId == filter.BranchId),
                    take: take,
                    lastSeenId: lastSeenId);

                if (!items.Succeeded)
                    return Ok(await Result<List<HrDependentVM>>.FailAsync(items.Status.message));

                if (items.Data.Count() > 0)
                {

                    var res = items.Data.AsQueryable();

                    string format = "yyyy/MM/dd";
                    int age = 0;

                    foreach (var item in res)
                    {
                        age = 0;
                        if (!string.IsNullOrEmpty(item.DateOfBirth))
                        {
                            DateTime birthDate = DateTime.ParseExact(item.DateOfBirth, format, System.Globalization.CultureInfo.InvariantCulture);
                            DateTime today = DateTime.Today;

                            age = today.Year - birthDate.Year;

                            if (birthDate > today.AddYears(-age))
                            {
                                age -= 1;
                            }
                        }

                        var newRecord = new HrDependentVM
                        {
                            DependentId = item.Id,
                            DateOfBirth = item.DateOfBirth,
                            EmpId = item.EmpCode,
                            EmpName = item.EmpName,
                            Name = item.Name,
                            Name2 = item.Name1,
                            Relationship = Convert.ToInt32(item.Relationship),
                            RelationshipName = item.RelationshipName,
                            Insurance = item.Insurance,
                            Ticket = item.Ticket,
                            Age = age
                        };

                        resultList.Add(newRecord);
                    }
                    var paginatedData = new PaginatedResult<object>
                    {
                        Succeeded = items.Succeeded,
                        Data = resultList.ToList(),
                        Status = items.Status,
                        PaginationInfo = items.PaginationInfo
                    };
                    return Ok(paginatedData);

                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrDependentDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(576, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var add = await hrServiceManager.HrDependentService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRDependentsController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(576, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await hrServiceManager.HrDependentService.Remove(id);
                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Delete HRDependentsController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrDependentEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(576, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result<HrDependentEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                var update = await hrServiceManager.HrDependentService.Update(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDependentEditDto>.FailAsync($"====== Exp in Edit HRDependentsController, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long DependentId)
        {
            try
            {
                var chk = await permission.HasPermission(576, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (DependentId <= 0)
                {
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var item = await hrServiceManager.HrDependentService.GetOneVW(x => x.Id == DependentId);

                var response = new
                {
                    data = new
                    {
                        id = item.Data.Id,
                        relationshipName = item.Data.RelationshipName,
                        empId = item.Data.EmpId,
                        name = item.Data.Name,
                        name1 = item.Data.Name1,
                        relationshipId = Convert.ToInt64(item.Data.Relationship),
                        dateOfBirth = item.Data.DateOfBirth,
                        insurance = item.Data.Insurance,
                        ticket = item.Data.Ticket,
                        gender = item.Data.Gender,
                        insuranceNo = item.Data.InsuranceNo,
                        nationalityId = item.Data.NationalityId,
                        maritalStatus = item.Data.MaritalStatus,
                        empCode = item.Data.EmpCode,
                        isdel = item.Data.Isdel,
                        empName = item.Data.EmpName,
                        idNo = item.Data.IdNo,
                        nationalityName = item.Data.NationalityName,
                        nationalityName2 = item.Data.NationalityName2,
                        code = item.Data.Code,
                        branchId = item.Data.BranchId,
                        facilityId = item.Data.FacilityId
                    },
                    status = new
                    {
                        code = item.Status.code,
                        message = item.Status.message
                    },
                    succeeded = item.Succeeded
                };


                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrDependentsVw>.FailAsync($"====== Exp in HRDependentsController getById, MESSAGE: {ex.Message}"));
            }
        }

        [HttpGet("EmpIdChanged")]
        public async Task<IActionResult> EmpIdChanged(string EmpId)
        {
            var chk = await permission.HasPermission(576, PermissionType.Delete);
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
                        };
                        return Ok(await Result<EmpIdChangedVM>.SuccessAsync(item));

                    }
                    else
                    {
                        return Ok(await Result<EmpIdChangedVM>.SuccessAsync(localization.GetResource1("EmployeeNotFound")));

                    }
                }
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{checkEmpId.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<EmpIdChangedVM>.FailAsync($"{exp.Message}"));
            }
        }


    }
}