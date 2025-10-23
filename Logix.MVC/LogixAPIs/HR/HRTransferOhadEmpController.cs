using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
    public class HRTransferOhadEmpController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public HRTransferOhadEmpController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, ICurrentData session, IPermissionHelper permission, ILocalizationService localization)

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

            var chk = await permission.HasPermission(1261, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await hrServiceManager.HrOhadService.GetAllVW(e => e.IsDeleted == false && e.TransTypeId == 4);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (!String.IsNullOrEmpty(filter.EmpCode))
                    {
                        res = res.Where(x => x.EmpCode == filter.EmpCode);
                    }
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

                    if (!String.IsNullOrEmpty(filter.FromDate))
                    {
                        res = res.Where(x => DateHelper.StringToDate(x.OhdaDate) >= DateHelper.StringToDate(filter.FromDate));
                    }
                    if (!String.IsNullOrEmpty(filter.ToDate))
                    {
                        res = res.Where(x => DateHelper.StringToDate(x.OhdaDate) <= DateHelper.StringToDate(filter.ToDate));
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
                                EmpName = item.EmpName,
                                EmpName2 = item.EmpName2,
                                EmpIdTo = item.EmpCodeTo,
                                EmpNameTo = item.EmpNameTo,
                                BranchId = item.BranchId,
                                OhadId = item.OhdaId,
                                OhadDate = item.OhdaDate,
                                DeptId = item.DeptId,
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

    [HttpPost("Add")]
    public async Task<IActionResult> Add(HrOhadDto obj)
    {
      var chk = await permission.HasPermission(1261, PermissionType.Add);
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

        var addRes = await hrServiceManager.HrOhadService.AddTransferOhad(obj);
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
      var chk = await permission.HasPermission(1261, PermissionType.Add);
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

    //[HttpGet("HrTransferOhad")]
    //public async Task<IActionResult> HrTransferOhad(long Id = 0)
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
    //        var del = await hrServiceManager.HrOhadService.HrTransferOhad(Id);
    //        if (del.Succeeded)
    //        {

    //            return Ok(await Result<object>.SuccessAsync("Transfer  Ohad deleted successfully"));
    //        }
    //        return Ok(await Result<object>.FailAsync($"{del.Status.message}"));
    //    }
    //    catch (Exception exp)
    //    {
    //        return Ok(await Result<object>.FailAsync($"{exp.Message}"));
    //    }
    //}

    [HttpPost("searchohadDetail")]

        public async Task<IActionResult> searchohadDetail(long empId)

        {
            var chk = await permission.HasPermission(1259, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            setErrors();
            try
            {
                if (empId <= 0)
                {
                    return Ok(await Result<List<object>>.FailAsync(localization.GetResource1("يجب إدخال رقم الموظف")));


                }
                var getDetails = await hrServiceManager.HrOhadDetailService.GetAllVW(x => x.EmpId == empId && x.TransTypeId == 1 && x.IsDeleted == false && x.QtyIn - x.QtyOut > 0);

                if (getDetails == null)
                {
                    return Ok(await Result<List<object>>.FailAsync(localization.GetResource1("   رقم الموظف غير متوفر")));

                }
                var LASTENTITY = getDetails.Data.GroupBy(x => x.ItemId).Select(g => new
                {
                    ItemId = g.Key,
                    ItemName = g.Select(x => x.ItemName).FirstOrDefault(),
                    ActualQuantity = g.Sum(x => x.QtyIn),
                    RemainingQuantity = g.Sum(x => x.QtyIn) - g.Sum(x => x.QtyOut),
                    QuantityReturned = 0,
                    //g.Sum(x => x.QtyIn) - g.Sum(x => x.QtyOut),

                    OhadDate = g.Select(x => x.OhdaDate).FirstOrDefault(),
                    ItemStateId = g.Select(x => x.ItemStateId).FirstOrDefault(),

                    Note = ""

                });

                return Ok(await Result<object>.SuccessAsync(LASTENTITY));

            }

            catch (Exception ex)
            {
                return Ok(await Result<HrOhadDetailDto>.FailAsync(ex.Message));
            }
        }

        //[HttpPost("AddTransferOhad")]
        //public async Task<IActionResult> AddTransferOhad(List<HrOhadDetailDto> obj)
        //{
        //    var chk = await permission.HasPermission(1259, PermissionType.Add);
        //    if (!chk)
        //    {
        //        return Ok(await Result.AccessDenied("AccessDenied"));
        //    }
        //    setErrors();
        //    if (!ModelState.IsValid)
        //    {
        //        return Ok(obj);
        //    }
        //    try
        //    {
        //        //var user = session.GetData<Domain.Main.SysUser>("user");
        //        var user = session.UserId;
        //        if (user == null)
        //        {
        //            return Ok(obj);
        //        }
        //        foreach (var item in obj)
        //        {

        //            if (item.EmpIdTo <= 0)
        //            {
        //                return Ok(await Result<bool>.FailAsync(localization.GetResource1(" يجب إدخال رقم المستلم")));
        //            }
        //            if (item.EmpId <= 0)
        //            {
        //                return Ok(await Result<bool>.FailAsync(localization.GetResource1(" يجب إدخال رقم الموظف")));
        //            }
        //        }

        //        var addRes = await hrServiceManager.HrOhadService.AddTransferOhad(obj);
        //        if (addRes.Succeeded)
        //        {
        //            return Ok(addRes);

        //        }

        //        else
        //        {
        //            return Ok(await Result<HrOhadDto>.FailAsync(addRes.Status.message));
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        return Ok(await Result<HrOhadDto>.FailAsync(ex.Message));
        //    }
        //}
    }

}
