using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.HR.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cmp;

namespace Logix.MVC.LogixAPIs.HR
{

    public class HRDropingOhadEmpController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRDropingOhadEmpController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, ICurrentData session, IPermissionHelper permission, ILocalizationService localization)

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
            var chk = await permission.HasPermission(1259, PermissionType.Show);
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
                e.TransTypeId == 2 &&
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
                        EmpName2 = item.EmpName2,
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


		//[HttpGet("GetById")]
		//public async Task<IActionResult> Edit(long Id)
		//{
		//	HrOhadEditDto result = new HrOhadEditDto();
		//	result.OhadDetails = new List<HrOhadDetailDto>();
		//	result.fileDtos = new List<SaveFileDto>();

		//	try
		//	{
		//		var chk = await permission.HasPermission(1259, PermissionType.Edit);
		//		if (!chk)
		//			return Ok(await Result.AccessDenied("AccessDenied"));

		//		if (Id <= 0)
		//			return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

		//		// جلب بيانات العهدة الرئيسية
		//		var ohadData = await hrServiceManager.HrOhadService.GetOneVW(x => x.IsDeleted == false && x.OhdaId == Id);
		//		if (!ohadData.Succeeded)
		//			return Ok(await Result.FailAsync(ohadData.Status.message));

		//		result.OhdaId = ohadData.Data.OhdaId;
		//		result.Code = ohadData.Data.Code;
		//		result.OhdaDate = ohadData.Data.OhdaDate;
		//		result.EmpCode = ohadData.Data.EmpCode;
		//		result.EmpName = ohadData.Data.EmpName;
		//		result.EmpCodeTo = ohadData.Data.EmpCodeTo;
		//		result.EmpNameTo = ohadData.Data.EmpNameTo;
		//		result.Note = ohadData.Data.Note;

		//		// جلب تفاصيل العهدة
		//		var OhadDetails = await hrServiceManager.HrOhadDetailService.GetAllVW(
		//			x => x.OhdaId == Id && x.IsDeleted == false
		//		);

		//		if (OhadDetails.Data != null)
		//		{
		//			var ohadDetailsQuery = OhadDetails.Data.AsQueryable();

		//			foreach (var item in OhadDetails.Data)
		//			{
		//				var newRecord = new HrOhadDetailDto
		//				{
		//					OhadDetId = item.OhadDetId,
		//					OhdaDate = item.OhdaDate,
		//					ItemId = item.ItemId,
		//					ItemName = item.ItemName,
		//					ItemStateId = item.ItemStateId,
		//					ItemState = item.ItemStateName,
		//					ItemDetails = item.ItemDetails,
		//					QtyIn = item.QtyIn,
		//					QtyOut = item.QtyOut,
		//					IsDeleted = item.IsDeleted,
		//					Note = item.Note,

		//					// حساب الكمية المصروفة
		//					ItemQtyOut = ohadDetailsQuery
		//						.Where(d => d.EmpId == item.EmpId &&
		//									d.ItemId == item.ItemId &&
		//									d.ItemStateId == item.ItemStateId &&
		//									d.OrgnalId == item.OrgnalId &&
		//									d.IsDeleted == false)
		//						.Sum(d => d.QtyOut),

		//					// حساب الكمية المستلمة
		//					ItemQtyIn = ohadDetailsQuery
		//						.Where(d => d.EmpId == item.EmpId &&
		//									d.ItemId == item.ItemId &&
		//									d.ItemStateId == item.ItemStateId &&
		//									d.OrgnalId == item.OrgnalId &&
		//									d.IsDeleted == false)
		//						.Sum(d => d.QtyIn),

		//					// الكمية المتبقية = مجموع الداخل - مجموع الخارج
		//					RemainingQuantity = ohadDetailsQuery
		//						.Where(d => d.EmpId == item.EmpId &&
		//									d.ItemId == item.ItemId &&
		//									d.ItemStateId == item.ItemStateId &&
		//									d.OrgnalId == item.OrgnalId &&
		//									d.IsDeleted == false)
		//						.Sum(d => d.QtyIn) -
		//						ohadDetailsQuery
		//						.Where(d => d.EmpId == item.EmpId &&
		//									d.ItemId == item.ItemId &&
		//									d.ItemStateId == item.ItemStateId &&
		//									d.OrgnalId == item.OrgnalId &&
		//									d.IsDeleted == false)
		//						.Sum(d => d.QtyOut)
		//				};

		//				result.OhadDetails.Add(newRecord);
		//			}
		//		}

		//		// جلب الملفات المرفقة
		//		var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 104);
		//		result.fileDtos = getFiles.Data ?? new List<SaveFileDto>();

		//		return Ok(await Result<HrOhadEditDto>.SuccessAsync(result));
		//	}
		//	catch (Exception ex)
		//	{
		//		return Ok(await Result<HrOhadEditDto>.FailAsync($"====== Exp in HRDroppingOhad getById, MESSAGE: {ex.Message}"));
		//	}
		//}



		[HttpGet("GetById")]
		public async Task<IActionResult> Edit(long Id)
		{
			HrOhadEditDto result = new HrOhadEditDto();
			result.OhadDetails = new List<HrOhadDetailDto>();
			result.fileDtos = new List<SaveFileDto>();
			try
			{
				var chk = await permission.HasPermission(1259, PermissionType.Edit);
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

		[HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(1259, PermissionType.Delete);
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
            var chk = await permission.HasPermission(1259, PermissionType.Add);
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

                var addRes = await hrServiceManager.HrOhadService.AddDropOhad(obj);
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
			var chk = await permission.HasPermission(1259, PermissionType.Add);
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



		//[HttpPost("EmpCodeChanged")]
		//public async Task<IActionResult> EmpCodeChanged(string EmpCode)
		//{
		//    var chk = await permission.HasPermission(1259, PermissionType.Add);
		//    if (!chk)
		//    {
		//        return Ok(await Result.AccessDenied("AccessDenied"));
		//    }

		//    try
		//    {
		//        if (string.IsNullOrEmpty(EmpCode))
		//            return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

		//        var checkEmpExist = await mainServiceManager.InvestEmployeeService.GetOne(e => e.EmpId == EmpCode && e.IsDeleted == false);

		//        if (checkEmpExist.Data == null)
		//            return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));

		//        var getDetails = await hrServiceManager.HrOhadDetailService.GetAllVW(x => x.EmpId == checkEmpExist.Data.Id && x.TransTypeId == 1 && x.IsDeleted == false);

		//        // Check if the result is empty
		//        if (!getDetails.Data.Any())
		//        {
		//            return Ok(await Result<object>.SuccessAsync(" ليس هناك عهد سابقة على الموظف"));
		//        }

		//        var ohadDetails = getDetails.Data.AsQueryable();

		//        var result = ohadDetails
		//            .Select(ohad => new
		//            {
		//                ohad.IdOhda,
		//                ohad.EmpId,
		//                ohad.ItemId,
		//                ohad.ItemStateId,
		//                ohad.OrgnalId,
		//                ohad.OhdaId,
		//                ohad.QtyIn,
		//                ohad.QtyOut,
		//                Code = ohad.Code,
		//                OhadDetId = ohad.OhadDetId,
		//                Note = ohad.Note,
		//                ItemName = ohad.ItemName,
		//                OhdaDate = ohad.OhdaDate,
		//                ItemStateName = ohad.ItemStateName,
		//                ItemQtyOut = ohadDetails
		//                    .Where(d => d.EmpId == ohad.EmpId && d.ItemId == ohad.ItemId
		//                                && d.ItemStateId == ohad.ItemStateId && d.OrgnalId == ohad.OhdaId
		//                                && d.IsDeleted == false)
		//                    .Sum(d => d.QtyOut),

		//                ItemQtyIn = ohadDetails
		//                    .Where(d => d.EmpId == ohad.EmpId && d.ItemId == ohad.ItemId
		//                                && d.ItemStateId == ohad.ItemStateId && d.OrgnalId == ohad.OrgnalId
		//                                && d.OhdaId == ohad.OhdaId && d.IsDeleted == false)
		//                    .Sum(d => d.QtyIn),
		//            })
		//            .AsEnumerable() 
		//            .Select(ohad => new
		//            {
		//                ohad.IdOhda,
		//                ohad.EmpId,
		//                ohad.ItemId,
		//                ohad.ItemStateId,
		//                ohad.OrgnalId,
		//                ohad.OhdaId,
		//                ohad.QtyIn,
		//                ohad.QtyOut,
		//                Code = ohad.Code,
		//                OhadDetId = ohad.OhadDetId,
		//                Note = ohad.Note,
		//                ItemName = ohad.ItemName,
		//                OhdaDate = ohad.OhdaDate,
		//                ItemStateName = ohad.ItemStateName,
		//                ItemQtyOut = ohad.ItemQtyOut,
		//                ItemQtyIn = ohad.ItemQtyIn,
		//                RemainingQuantity = (ohad.ItemQtyIn - ohad.ItemQtyOut),
		//                ActualQuantity = ohad.ItemQtyIn
		//            })
		//            .Where(ohad => ohad.RemainingQuantity > 0) 
		//            .ToList();

		//        return Ok(await Result<object>.SuccessAsync(result));
		//    }
		//    catch (Exception ex)
		//    {
		//        return Ok(await Result<object>.FailAsync(ex.Message));
		//    }
		//}

		[HttpPost("EmpCodeChangedtest")]
        public async Task<IActionResult> EmpCodeChangedtest(string empCode)
        {
            var chk = await permission.HasPermission(1259, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (string.IsNullOrEmpty(empCode))
                    return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                var checkEmpExist = await mainServiceManager.InvestEmployeeService.GetOne(e => e.EmpId == empCode && e.IsDeleted == false);
                if (checkEmpExist.Data == null)
                    return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));

                var empId = checkEmpExist.Data.Id;

                var getDetails = await hrServiceManager.HrOhadDetailService.GetAllVW(x => x.EmpId == empId && x.TransTypeId == 1 && x.IsDeleted == false);
                if (!getDetails.Data.Any())
                {
                    return Ok(await Result<object>.SuccessAsync("ليس هناك عهد سابقة على الموظف"));
                }

                var ohadDetails = getDetails.Data.AsQueryable();

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

                        // Calculate ItemQtyOut
                        ItemQtyOut = ohadDetails
                            .Where(d => d.EmpId == ohad.EmpId && d.ItemId == ohad.ItemId
                                        && d.ItemStateId == ohad.ItemStateId && d.OrgnalId == ohad.OhdaId
                                        && d.IsDeleted == false)
                            .Sum(d => d.QtyOut),

                        // Calculate ItemQtyIn
                        ItemQtyIn = ohadDetails
                            .Where(d => d.EmpId == ohad.EmpId && d.ItemId == ohad.ItemId
                                        && d.ItemStateId == ohad.ItemStateId && d.OrgnalId == ohad.OrgnalId
                                        && d.OhdaId == ohad.OhdaId && d.IsDeleted == false)
                            .Sum(d => d.QtyIn)
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
                        RemainingQuantity = ohad.ItemQtyIn - ohad.ItemQtyOut
                    })
                    .Where(ohad => ohad.RemainingQuantity > 0)
                    .ToList();

                return Ok(await Result<object>.SuccessAsync(result));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

		#endregion

		#region Edity Region
		//[HttpGet("GetById")]
		//public async Task<IActionResult> GetById(long Id)
		//{
		//	HrOhadEditDto result = new HrOhadEditDto();
		//	result.OhadDetails = new List<HrOhadDetailDto>();
		//	result.fileDtos = new List<SaveFileDto>();
		//	try
		//	{
		//		var chk = await permission.HasPermission(1259, PermissionType.Edit);
		//		if (!chk)
		//			return Ok(await Result.AccessDenied("AccessDenied"));

		//		if (Id <= 0)
		//			return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));


		//		var ohadData = await hrServiceManager.HrOhadService.GetOneVW(x => x.IsDeleted == false && x.OhdaId == Id);
		//		if (!ohadData.Succeeded)
		//			return Ok(await Result.FailAsync(ohadData.Status.message));
		//		result.Code = ohadData.Data.Code;
		//		result.OhdaDate = ohadData.Data.OhdaDate;
		//		result.EmpCode = ohadData.Data.EmpCode;
		//		result.EmpName = ohadData.Data.EmpName;
		//		result.Note = ohadData.Data.Note;

		//		var OhadDetails = await hrServiceManager.HrOhadDetailService.GetAllVW(x => x.OhdaId == Id && x.IsDeleted == false);
		//		if (OhadDetails.Data != null)
		//		{
		//			foreach (var item in OhadDetails.Data)
		//			{
		//				var newRecord = new HrOhadDetailDto
		//				{
		//					OhadDetId = item.OhadDetId,
		//					ItemName = item.ItemName,
		//					ItemState = item.ItemStateName,
		//					ItemDetails = item.ItemDetails,
		//					QtyIn = item.QtyIn,
		//					IsDeleted = item.IsDeleted,
		//					Note = item.Note,
		//				};
		//				result.OhadDetails.Add(newRecord);
		//			}
		//		}

		//		var getFiles = await mainServiceManager.SysFileService.GetFilesForUser(Id, 104);
		//		result.fileDtos = getFiles.Data ?? new List<SaveFileDto>();

		//		return Ok(await Result<HrOhadEditDto>.SuccessAsync(result));
		//	}
		//	catch (Exception ex)
		//	{
		//		return Ok(await Result<HrOhadEditDto>.FailAsync($"====== Exp in HROhad getById, MESSAGE: {ex.Message}"));
		//	}
		//}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(HrOhadEditDto obj)
		{
			var chk = await permission.HasPermission(1259, PermissionType.Edit);
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

        //[HttpGet("removeHrDropingOhad")]
        //public async Task<IActionResult> removeHrDropingOhad(long Id = 0)
        //{
        //    var chk = await permission.HasPermission(1259, PermissionType.Delete);
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
        //        var del = await hrServiceManager.HrOhadService.removeHrDropingOhad(Id);
        //        if (del.Succeeded)
        //        {

        //            return Ok(await Result<object>.SuccessAsync("DropingOhad deleted successfully"));
        //        }
        //        return Ok(await Result<object>.FailAsync($"{del.Status.message}"));
        //    }
        //    catch (Exception exp)
        //    {
        //        return Ok(await Result<object>.FailAsync($"{exp.Message}"));
        //    }
        //}

        #endregion

    }

}
