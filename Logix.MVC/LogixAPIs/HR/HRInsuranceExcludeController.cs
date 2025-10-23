using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Interfaces.IServices.HR;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.HR
{
    // الإستبعاد من التأمين 
    public class HRInsuranceExcludeController : BaseHrApiController
    {
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IHrServiceManager hrServiceManager;
        public HRInsuranceExcludeController(
            IPermissionHelper permission,
            ILocalizationService localization,
            ICurrentData session,
            IHrServiceManager hrServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.hrServiceManager = hrServiceManager;
        }
        #region Index Page


        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrInsuranceFilterDto filter)
        {
            List<HrInsuranceFilterDto> InsuranceList = new List<HrInsuranceFilterDto>();

            var chk = await permission.HasPermission(1253, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                filter.Code ??= 0;
                filter.Total ??= 0;
                var items = await hrServiceManager.HrInsuranceService.GetAll(e =>
                e.IsDeleted == false &&
                e.TransTypeId == 2 &&
                (filter.Code == 0 || e.Code == filter.Code) &&
                (filter.Total == 0 || e.Total == filter.Total)

                );
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();

                    if (!string.IsNullOrEmpty(filter.StartDate) && (!string.IsNullOrEmpty(filter.EndDate)))
                    {
                        var StartDate = DateHelper.StringToDate(filter.StartDate);
                        var EndDate = DateHelper.StringToDate(filter.EndDate);
                        res = res.Where(x => DateHelper.StringToDate(x.InsuranceDate) >= StartDate
                        && DateHelper.StringToDate(x.InsuranceDate) <= EndDate);
                    }


                    res = res.OrderBy(e => e.Id);
                    var final = res.ToList();
                    if (final.Any())
                    {
                        foreach (var item in final)
                        {
                            var newRow = new HrInsuranceFilterDto
                            {
                                Id = item.Id,
                                Code = item.Code,
                                InsuranceDate = item.InsuranceDate,
                                Note = item.Note,
                                Total = item.Total,


                            };
                            InsuranceList.Add(newRow);
                        }
                        return Ok(await Result<List<HrInsuranceFilterDto>>.SuccessAsync(InsuranceList, ""));

                    }

                    return Ok(await Result<List<HrInsuranceFilterDto>>.SuccessAsync(InsuranceList, localization.GetResource1("NosearchResult")));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(1253, PermissionType.Delete);
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
            var chk = await permission.HasPermission(1253, PermissionType.Edit);
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
                            EmpId = item.EmpId,
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

		//[HttpPost("SearchOnAdd")]

		//public async Task<IActionResult> SearchOnAdd(HrInsuranceEmpfilterRPDto filter)
		//{
		//    var chk = await permission.HasPermission(1253, PermissionType.Add);
		//    if (!chk)
		//        return Ok(await Result.AccessDenied("AccessDenied"));
		//    List<HrInsuranceEmpResulteDto> InsuranceList = new List<HrInsuranceEmpResulteDto>();
		//    filter.PolicyId ??= 0;
		//    filter.DeptId ??= 0;
		//    filter.Location ??= 0;
		//    filter.InsuranceType ??= 0;
		//    filter.StatusId ??= 0;
		//    filter.ClassId ??= 0;
		//    filter.BranchId ??= 0;
		//    try
		//    {
		//        var BranchesList = session.Branches.Split(',');
		//        if (filter.PolicyId <= 0)
		//            return Ok(await Result<object>.FailAsync("يجب تحديد بوليصات التأمين"));

		//        if (filter.InsuranceType <= 0)
		//            return Ok(await Result<object>.FailAsync("يجب تحديد نوع التأمين "));

		//        var items = await hrServiceManager.HrInsuranceEmpService.GetAllVW(e => e.IsDeleted == false && e.TransTypeId != 2 &&
		//        BranchesList.Contains(e.BranchId.ToString()) &&
		//        (e.PolicyId == filter.PolicyId) &&
		//        (filter.EmpId == 0 || e.EmpId == filter.EmpId) &&
		//        (filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
		//        (filter.Location == 0 || e.Location == filter.Location) &&
		//        (e.InsuranceType == filter.InsuranceType) &&
		//        (filter.BranchId == 0 || e.BranchId == filter.BranchId) &&
		//       // (string.IsNullOrEmpty(filter.EmpCode) || e.EmpCode == filter.EmpCode) &&
		//        (string.IsNullOrEmpty(filter.EmpName) || (e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName.ToLower())))
		//        );

		//        if (!items.Succeeded)
		//        {
		//            return Ok(await Result.FailAsync(items.Status.message));
		//        }


		//        var res = items.Data.AsQueryable();
		//        var RefranceInsEmpID = await hrServiceManager.HrInsuranceEmpService.GetAll(x => x.RefranceInsEmpId);
		//        res = res.Where(x => !RefranceInsEmpID.Data.Contains(x.Id));


		//        if (res.Any())
		//        {
		//            foreach (var item in res)
		//            {
		//                var newRow = new HrInsuranceEmpResulteDto
		//                {
		//                    EmpCode = item.EmpCode,
		//                    EmpName = item.EmpName,
		//                    CreatedOn = item.CreatedOn.ToString("yyyy/mm/dd", CultureInfo.InvariantCulture),
		//                    DependentName = item.DependentName,
		//                    DepName = item.DepName,
		//                    LocationName = item.LocationName,
		//                    StatusName = item.StatusName,
		//                    PolicyCode = item.PolicyCode,
		//                    ClassName = item.ClassName,
		//                    Amount = item.Amount,
		//                };
		//                InsuranceList.Add(newRow);
		//            }
		//            if (InsuranceList.Count > 0)
		//                return Ok(await Result<List<HrInsuranceEmpResulteDto>>.SuccessAsync(InsuranceList, ""));
		//            return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

		//        }
		//        return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));



		//    }
		//    catch (Exception ex)
		//    {
		//        return Ok(await Result.FailAsync(ex.Message));
		//    }
		//}
		[HttpPost("SearchOnAdd")]
		public async Task<IActionResult> SearchOnAdd(HrInsuranceEmpfilterRPDto filter)
		{
			// 1. التحقق من الصلاحيات
			var chk = await permission.HasPermission(1253, PermissionType.Add);
			if (!chk)
				return Ok(await Result.AccessDenied("AccessDenied"));

			// 2. تجهيز القائمة الفارغة
			List<HrInsuranceEmpResulteDto> InsuranceList = new List<HrInsuranceEmpResulteDto>();

			// 3. تعيين القيم الافتراضية إذا لم يرسل المستخدم أي بيانات
			filter.PolicyId ??= 0;
			filter.DeptId ??= 0;
			filter.Location ??= 0;
			filter.InsuranceType ??= 0;
			filter.StatusId ??= 0;
			filter.ClassId ??= 0;
			filter.BranchId ??= 0;

			try
			{
				var BranchesList = session.Branches.Split(',');

				// 4. التحقق من الإدخالات الأساسية
				if (filter.PolicyId <= 0)
					return Ok(await Result<object>.FailAsync("يجب تحديد بوليصات التأمين"));

				if (filter.InsuranceType <= 0)
					return Ok(await Result<object>.FailAsync("يجب تحديد نوع التأمين"));

				// 5. جلب بيانات التأمين للموظفين بناءً على الفلترة
				var items = await hrServiceManager.HrInsuranceEmpService.GetAllVW(e =>
					e.IsDeleted == false &&
					e.TransTypeId != 2 
					//BranchesList.Contains(e.BranchId.ToString()) &&
					////(e.PolicyId == filter.PolicyId) &&
					//(filter.PolicyId == 0 || e.PolicyId == filter.PolicyId) &&
					//(filter.EmpId == 0 || e.EmpId == filter.EmpId) &&
					//(filter.DeptId == 0 || e.DeptId == filter.DeptId) &&
					//(filter.Location == 0 || e.Location == filter.Location) &&
					//(filter.InsuranceType == 0 || e.InsuranceType == filter.InsuranceType) &&
					////(e.InsuranceType == filter.InsuranceType) &&
					//(filter.BranchId == 0 || e.BranchId == filter.BranchId) &&
					//(string.IsNullOrEmpty(filter.EmpName) ||
					//	(e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName.ToLower())))
				);

				if (!items.Succeeded)
					return Ok(await Result.FailAsync(items.Status.message));

				// 6. استبعاد السجلات المرتبطة بسجلات أخرى (RefranceInsEmpID)
				var res = items.Data.AsQueryable();
				var RefranceInsEmpID = await hrServiceManager.HrInsuranceEmpService.GetAll(x => x.RefranceInsEmpId);
				res = res.Where(x => !RefranceInsEmpID.Data.Contains(x.Id));

				// 7. بناء النتيجة
				if (res.Any())
				{
					foreach (var item in res)
					{
						var newRow = new HrInsuranceEmpResulteDto
						{
							EmpCode = item.EmpCode,
							EmpName = item.EmpName,
							CreatedOn = item.CreatedOn.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture),
							DependentId = item.DependentId,
							DependentName = item.DependentName,
							DepName = item.DepName,
							LocationName = item.LocationName,
							StatusName = item.StatusName,
							PolicyCode = item.PolicyCode,
							ClassId = item.ClassId,
							ClassName = item.ClassName,
							Amount = item.Amount,
							EmpId = item.EmpId,
                            Note = item.Note,
							InsuranceDate = item.InsuranceDate,
							InsuranceType = item.InsuranceType,
							InsuranceCardNo = item.InsuranceCardNo,
						};
						InsuranceList.Add(newRow);
					}

					if (InsuranceList.Count > 0)
						return Ok(await Result<List<HrInsuranceEmpResulteDto>>.SuccessAsync(InsuranceList, ""));

					return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
				}

				return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));
			}
			catch (Exception ex)
			{
				return Ok(await Result.FailAsync(ex.Message));
			}
		}


		[HttpPost("Add")]
        public async Task<ActionResult> Add(HrInsuranceDto obj)
        {
            try
            {
                var chk = await permission.HasPermission(1253, PermissionType.Add);
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

                if (obj.InsuranceEmp.Count < 1)
                    return Ok(await Result.FailAsync($" حدد الموظف المراد استبعادة من التأمين  "));

                var add = await hrServiceManager.HrInsuranceService.AddInsuranceExclude(obj);
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp in Add HRInsuranceExcludeController, MESSAGE: {ex.Message}"));
            }
        }
        #endregion

    }
}
