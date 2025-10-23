using iText.Commons.Bouncycastle.Asn1.X509;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    // التأمين لموظف أو تابع 
    public class HRInsuranceEmployeeController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ILocalizationService localization;
        private readonly IHrServiceManager hrServiceManager;
        private readonly ICurrentData session;

        public HRInsuranceEmployeeController(IMainServiceManager mainServiceManager, IPermissionHelper permission, ILocalizationService localization, IHrServiceManager hrServiceManager, ICurrentData session)
        {
            this.mainServiceManager = mainServiceManager;
            this.permission = permission;
            this.localization = localization;
            this.hrServiceManager = hrServiceManager;
            this.session = session;
        }

        #region Index Page


        [HttpPost("Search")]

        public async Task<IActionResult> Search(HrInsuranceFilterDto filter)
        {
            List<HrInsuranceFilterDto> resultList = new List<HrInsuranceFilterDto>();

            var chk = await permission.HasPermission(1251, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.Code ??= 0;
                filter.Total ??= 0;
                var items = await hrServiceManager.HrInsuranceService.GetAll(e => e.IsDeleted == false &&
                e.TransTypeId == 1 &&
                (filter.Code == 0 || e.Code == filter.Code) &&
                (filter.Total == 0 || e.Total == filter.Total)
                );

                if (!items.Succeeded)
                {
                    return Ok(await Result.FailAsync(items.Status.message));

                }
                var res = items.Data.AsQueryable();
                if (!string.IsNullOrEmpty(filter.StartDate) && (!string.IsNullOrEmpty(filter.EndDate)))
                {
                    var StartDate = DateHelper.StringToDate(filter.StartDate);
                    var EndDate = DateHelper.StringToDate(filter.EndDate);
                    res = res.Where(x => DateHelper.StringToDate(x.InsuranceDate) >= StartDate
                    && DateHelper.StringToDate(x.InsuranceDate) <= EndDate);
                }
                res = res.OrderBy(e => e.Id);
                if (res.Any())
                {
                    foreach (var item in res)
                    {
                        var newRow = new HrInsuranceFilterDto
                        {
                            Id = item.Id,
                            Code = item.Code,
                            InsuranceDate = item.InsuranceDate,
                            Total = item.Total,
                            Note = item.Note
                        };
                        resultList.Add(newRow);
                    }
                    return Ok(await Result<List<HrInsuranceFilterDto>>.SuccessAsync(resultList, ""));
                }
                return Ok(await Result<List<HrInsuranceFilterDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"{localization.GetResource1("NotAbleShowResults")}   {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id)
        {
            var chk = await permission.HasPermission(1252, PermissionType.Delete);
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
                var del = await hrServiceManager.HrInsuranceService.Remove(Id);


                return Ok(del);

            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            var chk = await permission.HasPermission(1252, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result<HrInsuranceEditDto>.FailAsync($"Access Denied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
            }
            try
            {
                List<HrInsuranceEmpVM>? HrInsuranceEmp = new List<HrInsuranceEmpVM>();
                string ToDependentName = "";
                string DependentName = "";

                var getInsurance = await hrServiceManager.HrInsuranceService.GetOne(g => g.Id == Id);
                if (!getInsurance.Succeeded)
                {
                    return Ok(await Result<object>.FailAsync(getInsurance.Status.message));

                }
                if (getInsurance.Data == null)
                {
                    return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

                }

                var InsuranceData = new HrInsuranceEditDto
                {
                    PolicyId = getInsurance.Data.PolicyId,
                    InsuranceType = getInsurance.Data.InsuranceType,
                    InsuranceDate = getInsurance.Data.InsuranceDate,
                    Total = getInsurance.Data.Total,
                    Note = getInsurance.Data.Note,
                    Code = getInsurance.Data.Code,
                    Id = (long)getInsurance.Data.Id,

                };


                var getInsuranceEmp = await hrServiceManager.HrInsuranceEmpService.GetAllVW(e => e.InsuranceId == Id && e.IsDeleted == false);
                if (getInsuranceEmp.Data.Any())
                {
                    var res = getInsuranceEmp.Data.AsQueryable();
                    var RefranceInsEmpID = await hrServiceManager.HrInsuranceEmpService.GetAll(x => x.RefranceInsEmpId);
                    res = res.Where(x => !RefranceInsEmpID.Data.Contains(x.Id));
                    foreach (var item in res)
                    {
                        if (item.ToDependents == false)
                        {
                            ToDependentName = "الموظف";
                            DependentName = "لايوجد";
                        }
                        else
                        {
                            ToDependentName = "تابع";
                            DependentName = item.DependentName ?? "";
                        }
                        var newRecord = new HrInsuranceEmpVM
                        {
                            Id = item.Id,
                            EmpCode = item.EmpCode,
                            EmpName = item.EmpName,
                            DependentId = item.DependentId,
                            ToDependents = item.ToDependents,
                            ToDependentsName = ToDependentName,
                            DependentName = DependentName,
                            ClassId = item.ClassId,
                            ClassName = item.ClassName,
                            Note = item.Note,
                            Amount = item.Amount,
                            InsuranceCardNo = item.InsuranceCardNo,
                        };
                        HrInsuranceEmp.Add(newRecord);
                    }
                }


                InsuranceData.InsuranceEmp = HrInsuranceEmp;

                return Ok(await Result<HrInsuranceEditDto>.SuccessAsync(InsuranceData));

            }
            catch (Exception exp)
            {
                return Ok(await Result<HrInsuranceEditDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}"));
            }
        }

        #endregion

        #region Add Page
        [HttpPost("Add")]
        public async Task<ActionResult> Add(HrInsuranceDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1252, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

                if (obj.InsuranceType <= 0)
                    return Ok(await Result.FailAsync($"يجب ادخال نوع التأمين"));
                if (obj.PolicyId <= 0)
                    return Ok(await Result.FailAsync($"يجب ادخال بوليصات التأمين"));
                if (string.IsNullOrEmpty(obj.InsuranceDate))
                    return Ok(await Result.FailAsync($"يجب ادخال تاريخ التأمين"));

                if (obj.Total <= 0)
                    return Ok(await Result.FailAsync($"يجب أن لا تساوي قيمة الاجمالي  0"));

                if (obj.InsuranceEmp.Count < 1)
                    return Ok(await Result.FailAsync($"يجب ادخال شخص واحد على الأقل للتأمين"));


                if (obj.Total != obj.InsuranceEmp.Sum(x => x.Amount)) return Ok(await Result<HrInsuranceDto>.FailAsync("  المبلغ الاجمالي  للتامين اعلى او اقل من القيمة "));
                var add = await hrServiceManager.HrInsuranceService.Add(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRInsuranceEmployeeController, MESSAGE: {ex.Message}"));
            }
        }
        [HttpGet("EmpIdChanged")]
        public async Task<IActionResult> EmpIdChanged(string EmpId)
        {
            var chk = await permission.HasPermission(1252, PermissionType.Add);
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
                List<HrDependentDto> DependentList = new List<HrDependentDto>();

                var checkEmpId = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpId == EmpId && i.Isdel == false && i.IsDeleted == false);

                if (!checkEmpId.Succeeded)
                {
                    return Ok(await Result<object>.FailAsync($"{checkEmpId.Status.message}"));

                }
                if (checkEmpId.Data == null)
                {
                    return Ok(await Result<object>.SuccessAsync(localization.GetResource1("EmployeeNotFound")));

                }

                var item = new EmpIdChangedVM
                {
                    EmpId = checkEmpId.Data.EmpId,
                    EmpName = (session.Language == 1) ? checkEmpId.Data.EmpName : checkEmpId.Data.EmpName2,
                    InsuranceCardNo = checkEmpId.Data.InsuranceCardNo,
                };

                var DependentID = await hrServiceManager.HrDependentService.GetAll(e => e.IsDeleted == false && e.EmpId == checkEmpId.Data.Id);
                if (DependentID.Succeeded && DependentID.Data != null)
                {
                    var res = DependentID.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    foreach (var items in res)
                    {
                        var newRow = new HrDependentDto
                        {
                            Id = items.Id,
                            Name = (session.Language == 1) ? items.Name : items.Name1,

                        };
                        DependentList.Add(newRow);
                    }
                }
                return Ok(await Result<object>.SuccessAsync(new { Dependent = DependentList, empName = checkEmpId.Data.EmpName }));



            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"{exp.Message}"));
            }
        }
        #endregion

        #region Edit Page

        [HttpPost("Edit")]
        public async Task<ActionResult> Edit(HrInsuranceEditDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1252, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (!ModelState.IsValid)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));
                if (obj.InsuranceType <= 0)
                    return Ok(await Result.FailAsync($"يجب ادخال نوع التأمين"));
                if (obj.PolicyId <= 0)
                    return Ok(await Result.FailAsync($"يجب ادخال بوليصات التأمين"));
                if (string.IsNullOrEmpty(obj.InsuranceDate))
                    return Ok(await Result.FailAsync($"يجب ادخال تاريخ التأمين"));

                if (obj.Total <= 0)
                    return Ok(await Result.FailAsync($"يجب أن لا تساوي قيمة الاجمالي  0"));

                if (obj.InsuranceEmp.Count < 1)
                    return Ok(await Result.FailAsync($"يجب ادخال شخص واحد على الأقل للتأمين"));


                if (obj.Total != obj.InsuranceEmp.Where(x => x.IsDeleted != false).Sum(x => x.Amount)) return Ok(await Result<HrInsuranceDto>.FailAsync("  المبلغ الاجمالي  للتامين اعلى او اقل من القيمة "));

                var EditRes = await hrServiceManager.HrInsuranceService.Update(obj);

                return Ok(EditRes);

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRInsuranceEmployeeController, MESSAGE: {ex.Message}"));
            }
        }

        #endregion

    }
}




