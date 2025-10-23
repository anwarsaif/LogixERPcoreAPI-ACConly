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
    public class HRReturnOhadEmpController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRReturnOhadEmpController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, ICurrentData session, IPermissionHelper permission, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.permission = permission;
            this.localization = localization;

        }

        [NonAction]
        private void setErrors()
        {
            var errors = new ErrorsHelper(ModelState);
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrOhadFilterDto filter)
        {
            List<HrOhadFilterDto> OhadList = new List<HrOhadFilterDto>();

            var chk = await permission.HasPermission(1260, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrOhadService.GetAllVW(e => e.IsDeleted == false && e.TransTypeId == 3);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    //if (filter.EmpId != null && filter.EmpId > 0)
                    //{
                    //    res = res.Where(x => x.EmpId == filter.EmpId);
                    //}
                    if (filter.DeptId != null && filter.DeptId > 0)
                    {
                        res = res.Where(x => x.DeptId == filter.DeptId);
                    }
                    if (filter.BranchId != null && filter.BranchId > 0)
                    {
                        res = res.Where(x => x.BranchId == filter.BranchId);
                    }
                    if (filter.Location != null && filter.Location > 0)
                    {
                        res = res.Where(x => x.Location == filter.Location);
                    }


                    res = res.OrderBy(e => e.DeptId);
                    var final = res.ToList();
                    if (final.Any())
                    {
                        foreach (var item in final)
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
                    return Ok(await Result<List<HrOhadFilterDto>>.SuccessAsync(OhadList, localization.GetResource1("NosearchResult")));
                }
                return Ok(items);
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
				var chk = await permission.HasPermission(1260, PermissionType.Edit);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				if (Id <= 0)
					return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


				var ohadData = await hrServiceManager.HrOhadService.GetOneVW(x => x.IsDeleted == false && x.OhdaId == Id);
				if (!ohadData.Succeeded)
					return Ok(await Result.FailAsync(ohadData.Status.message));
				result.OhdaId = ohadData.Data.OhdaId;
				result.Code = ohadData.Data.Code;
				result.OhdaDate = ohadData.Data.OhdaDate;
				result.EmpCode = ohadData.Data.EmpCode;
				result.EmpName = ohadData.Data.EmpName;
				result.EmpCodeTo = ohadData.Data.EmpCodeTo;
				result.EmpNameTo = ohadData.Data.EmpNameTo;
				result.Note = ohadData.Data.Note;

				var OhadDetails = await hrServiceManager.HrOhadDetailService.GetAllVW(x => x.OhdaId == Id && x.IsDeleted == false);
				if (OhadDetails.Data != null)
				{
					foreach (var item in OhadDetails.Data)
					{
						var newRecord = new HrOhadDetailDto
						{
							OhadDetId = item.OhadDetId,
							OhdaDate = item.OhdaDate,
							ItemId = item.ItemId,
							ItemName = item.ItemName,
							ItemStateId = item.ItemStateId,
							ItemState = item.ItemStateName,
							ItemDetails = item.ItemDetails,
							QtyIn = item.QtyIn,
							QtyOut = item.QtyOut,
							IsDeleted = item.IsDeleted,
							Note = item.Note,
						};
						result.OhadDetails.Add(newRecord);
					}
					var LASTENTITY = OhadDetails.Data.GroupBy(x => x.ItemId).Select(g => new
					{
						ItemId = g.Key,
						ItemName = g.Select(x => x.ItemName).FirstOrDefault(),
						RemainingQuantity = g.Sum(x => x.QtyOut) + g.Sum(x => x.QtyIn),
						TotalQtyIn = g.Sum(x => x.QtyIn),
						OhadDate = g.Select(x => x.OhdaDate).FirstOrDefault(),
						OhadWantDroping = g.Select(x => x.QtyOut).FirstOrDefault(),
						Note = ""


					});
				}

				var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 104);
				result.fileDtos = getFiles.Data ?? new List<SaveFileDto>();

				return Ok(await Result<HrOhadEditDto>.SuccessAsync(result));
			}
			catch (Exception ex)
			{
				return Ok(await Result<HrOhadEditDto>.FailAsync($"====== Exp in HRDroppingOhad getById, MESSAGE: {ex.Message}"));
			}
		}

		//[HttpGet("EmpIdChanged")]
		//public async Task<IActionResult> EmpIdChanged(string EmpId)
		//{
		//    setErrors();
		//    var chk = await permission.HasPermission(763, PermissionType.Delete);
		//    if (!chk)
		//    {
		//        return Ok(await Result.AccessDenied("AccessDenied"));
		//    }
		//    if (string.IsNullOrEmpty(EmpId))
		//    {
		//        return Ok(await Result<EmpIdChangedVM>.SuccessAsync("there is no id passed"));
		//    }

		//    try
		//    {
		//        var checkEmpId = await mainServiceManager.InvestEmployeeService.GetOne(i => i.EmpId == EmpId && i.Isdel == false);
		//        if (checkEmpId.Succeeded)
		//        {
		//            if (checkEmpId.Data != null)
		//            {
		//                var item = new EmpIdChangedVM
		//                {
		//                    EmpId = checkEmpId.Data.EmpId,
		//                    EmpName = checkEmpId.Data.EmpName,
		//                    BankId = checkEmpId.Data.BankId,
		//                    BranchId = checkEmpId.Data.BranchId,
		//                    Gender = checkEmpId.Data.Gender,
		//                    Iban = checkEmpId.Data.Iban,
		//                    IdNo = checkEmpId.Data.IdNo,
		//                    NationalityId = checkEmpId.Data.NationalityId,

		//                };
		//                return Ok(await Result<EmpIdChangedVM>.SuccessAsync(item));

		//            }
		//            else
		//            {
		//                return Ok(await Result<EmpIdChangedVM>.SuccessAsync($"There is No Employee with this Id:  {EmpId}"));

		//            }
		//        }
		//        return Ok(await Result<EmpIdChangedVM>.FailAsync($"{checkEmpId.Status.message}"));
		//    }
		//    catch (Exception exp)
		//    {
		//        return Ok(await Result<EmpIdChangedVM>.FailAsync($"{exp.Message}"));
		//    }
		//}

		//[HttpGet("removeHrReturnOhad")]
		//public async Task<IActionResult> removeHrReturnOhad(long Id = 0)
		//{
		//    setErrors();
		//    var chk = await permission.HasPermission(1260, PermissionType.Delete);
		//    if (!chk)
		//    {
		//        return Ok(await Result.AccessDenied("AccessDenied"));
		//    }
		//    if (Id <= 0)
		//    {
		//        return Ok(await Result<object>.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
		//    }

		//    try
		//    {
		//        var del = await hrServiceManager.HrOhadService.Remove(Id);
		//        if (del.Succeeded)
		//        {

		//            return Ok(await Result<object>.SuccessAsync("Return Ohad deleted successfully"));
		//        }
		//        return Ok(await Result<object>.FailAsync($"{del.Status.message}"));
		//    }
		//    catch (Exception exp)
		//    {
		//        return Ok(await Result<object>.FailAsync($"{exp.Message}"));
		//    }
		//}

		//[HttpPost("searchohadDetail")]
		//public async Task<IActionResult> searchohadDetail(long empId)

		//{
		//    var chk = await permission.HasPermission(1259, PermissionType.Add);
		//    if (!chk)
		//    {
		//        return Ok(await Result.AccessDenied("AccessDenied"));
		//    }
		//    setErrors();
		//    try
		//    {
		//        if (empId <= 0)
		//        {
		//            return Ok(await Result<List<object>>.FailAsync(localization.GetResource1("يجب إدخال رقم الموظف")));
		//        }
		//        var getDetails = await hrServiceManager.HrOhadDetailService.GetAllVW(x => x.EmpId == empId && x.TransTypeId == 1 && x.IsDeleted == false && x.QtyIn - x.QtyOut > 0);

		//        if (getDetails == null)
		//        {
		//            return Ok(await Result<List<object>>.FailAsync(localization.GetResource1("   رقم الموظف غير متوفر")));

		//        }
		//        var LASTENTITY = getDetails.Data.GroupBy(x => x.ItemId).Select(g => new
		//        {
		//            ItemId = g.Key,
		//            ItemName = g.Select(x => x.ItemName).FirstOrDefault(),
		//            ActualQuantity = g.Sum(x => x.QtyIn),
		//            RemainingQuantity = g.Sum(x => x.QtyIn) - g.Sum(x => x.QtyOut),
		//            QuantityReturned = 0,
		//            //g.Sum(x => x.QtyIn) - g.Sum(x => x.QtyOut),

		//            OhadDate = g.Select(x => x.OhdaDate).FirstOrDefault(),
		//            ItemStateId = g.Select(x => x.ItemStateId).FirstOrDefault(),

		//            Note = ""
		//        });

		//        return Ok(await Result<object>.SuccessAsync(LASTENTITY));
		//    }

		//    catch (Exception ex)
		//    {
		//        return Ok(await Result<HrOhadDetailDto>.FailAsync(ex.Message));
		//    }
		//}

		[HttpPost("Add")]
        public async Task<IActionResult> Add(HrOhadDto obj)
        {
			var chk = await permission.HasPermission(1260, PermissionType.Add);
			if (!chk)
			{
				return Ok(await Result.AccessDenied("AccessDenied"));
			}

			try
			{
				if (string.IsNullOrEmpty(obj.EmpCodeTo))
					return Ok(await Result<List<HrOhadDto>>.FailAsync("رقم  الموظف المستلم"));

				if (string.IsNullOrEmpty(obj.EmpCode))
					return Ok(await Result<List<HrOhadDto>>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

				if (string.IsNullOrEmpty(obj.OhdaDate))
					return Ok(await Result<List<HrOhadDto>>.FailAsync(localization.GetHrResource("CustodyDate")));

				var addRes = await hrServiceManager.HrOhadService.AddReturnOhad(obj);
				return Ok(addRes);
			}
			catch (Exception ex)
			{
				return Ok(await Result<HrOhadDto>.FailAsync(ex.Message));
			}
        }

		[HttpPost("EmpCodeChanged")]
		public async Task<IActionResult> EmpCodeChanged(string EmpCode)
		{
			var chk = await permission.HasPermission(1260, PermissionType.Add);
			if (!chk)
			{
				return Ok(await Result.AccessDenied("AccessDenied"));
			}

			try
			{
				// التحقق من إدخال كود الموظف
				if (string.IsNullOrEmpty(EmpCode))
					return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

				// البحث عن الموظف في قاعدة البيانات
				var checkEmpExist = await mainServiceManager.InvestEmployeeService.GetOne(
					e => e.EmpId == EmpCode && e.IsDeleted == false
				);

				// إذا لم يتم العثور على الموظف
				if (checkEmpExist.Data == null)
					return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));

				// جلب بيانات العهد الخاصة بالموظف
				var getDetails = await hrServiceManager.HrOhadDetailService.GetAllVW(
					x => x.EmpId == checkEmpExist.Data.Id &&
						 x.TransTypeId == 1 && // ثابتة كما طلبت
						 x.IsDeleted == false
				);

				// التحقق إذا لم تكن هناك عهد سابقة على الموظف
				if (!getDetails.Data.Any())
				{
					return Ok(await Result<object>.SuccessAsync("ليس هناك عهد سابقة على الموظف"));
				}

				var ohadDetails = getDetails.Data.AsQueryable();

				// تجهيز البيانات النهائية
				var result = ohadDetails
					.Select(ohad => new
					{
						ohad.IdOhda,
						ohad.EmpId,
						ohad.ItemId,
						ohad.ItemStateId,
						ohad.OrgnalId,
						ohad.OhdaId,
						ohad.QtyIn,
						ohad.QtyOut,
						Code = ohad.Code,
						OhadDetId = ohad.OhadDetId,
						Note = ohad.Note,
						ItemName = ohad.ItemName,
						OhdaDate = ohad.OhdaDate,
						ItemStateName = ohad.ItemStateName,

						// مجموع الكميات المصروفة مع استثناء TransTypeId = 4
						ItemQtyOut = ohadDetails
							.Where(d => d.EmpId == ohad.EmpId &&
										d.ItemId == ohad.ItemId &&
										d.ItemStateId == ohad.ItemStateId &&
										d.OrgnalId == ohad.OrgnalId &&
										d.IsDeleted == false &&
										d.TransTypeId != 4)
							.Sum(d => d.QtyOut),

						// مجموع الكميات المستلمة مع استثناء TransTypeId = 4
						ItemQtyIn = ohadDetails
							.Where(d => d.EmpId == ohad.EmpId &&
										d.ItemId == ohad.ItemId &&
										d.ItemStateId == ohad.ItemStateId &&
										d.OrgnalId == ohad.OrgnalId &&
										d.IsDeleted == false &&
										d.TransTypeId != 4)
							.Sum(d => d.QtyIn),
					})
					.AsEnumerable()
					.Select(ohad => new
					{
						ohad.IdOhda,
						ohad.EmpId,
						ohad.ItemId,
						ohad.ItemStateId,
						ohad.OrgnalId,
						ohad.OhdaId,
						ohad.QtyIn,
						ohad.QtyOut,
						Code = ohad.Code,
						OhadDetId = ohad.OhadDetId,
						Note = ohad.Note,
						ItemName = ohad.ItemName,
						OhdaDate = ohad.OhdaDate,
						ItemStateName = ohad.ItemStateName,
						ItemQtyOut = ohad.ItemQtyOut,
						ItemQtyIn = ohad.ItemQtyIn,

						// الكمية المتبقية = مجموع الداخل - مجموع الخارج
						RemainingQuantity = (ohad.ItemQtyIn - ohad.ItemQtyOut),

						// الكمية الفعلية المستلمة
						ActualQuantity = ohad.ItemQtyIn
					})
					// فلترة السجلات التي فيها كمية متبقية فقط
					.Where(ohad => ohad.RemainingQuantity > 0)
					.ToList();

				// إرجاع النتيجة النهائية
				return Ok(await Result<object>.SuccessAsync(result));
			}
			catch (Exception ex)
			{
				// في حال حدوث خطأ
				return Ok(await Result<object>.FailAsync(ex.Message));
			}
		}

	}
}
