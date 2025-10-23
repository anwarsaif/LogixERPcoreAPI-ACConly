using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{

    // المخالفات
    public class HrDisciplinaryCaseApiController : BaseHrApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IDDListHelper listHelper;
        private readonly ILocalizationService localization;

        public HrDisciplinaryCaseApiController(IHrServiceManager hrServiceManager, IDDListHelper listHelper, IMainServiceManager mainServiceManager, ICurrentData session, IPermissionHelper permission, ILocalizationService localization)
        {
            this.mainServiceManager = mainServiceManager;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.permission = permission;
            this.listHelper = listHelper;
            this.localization = localization;
        }


        [HttpPost("SearchDisciplinaryCase")]
        public async Task<IActionResult> GetAllSearch(HrDisciplinaryFilterVM filter)
        {
            var lang = session.Language;
            var chk = await permission.HasPermission(567, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                List<HrDisciplinaryFilterVM> DisciplinaryFilterVMs = new List<HrDisciplinaryFilterVM>();
                var getAllDisciplinaryCases = await hrServiceManager.HrDisciplinaryCaseService.GetAll(e => e.IsDeleted == false);
                var getAllDisciplinaryRules = await hrServiceManager.HrDisciplinaryRuleService.GetAll(r => r.DisciplinaryCaseId.HasValue);
                if (!string.IsNullOrEmpty(filter.CaseName))
                {
                    getAllDisciplinaryCases = await hrServiceManager.HrDisciplinaryCaseService.GetAll(e => e.IsDeleted == false && ((e.CaseName != null && e.CaseName.ToLower().Contains(filter.CaseName.ToLower()) || (e.CaseName2 != null && e.CaseName2.ToLower().Contains(filter.CaseName.ToLower())))));
                }
                if (getAllDisciplinaryCases.Succeeded && getAllDisciplinaryRules.Succeeded)
                {
                    var disciplinaryCases = getAllDisciplinaryCases.Data.ToList();
                    var disciplinaryRules = getAllDisciplinaryRules.Data.ToList();
                    foreach (var disciplinaryCase in disciplinaryCases)
                    {
                        var caseId = disciplinaryCase.Id;
                        var firstRepeat = disciplinaryRules.FirstOrDefault(r => r.DisciplinaryCaseId == caseId && r.ReptFrom == 1);
                        var secondRepeat = disciplinaryRules.FirstOrDefault(r => r.DisciplinaryCaseId == caseId && r.ReptFrom == 2);
                        var thirdRepeat = disciplinaryRules.FirstOrDefault(r => r.DisciplinaryCaseId == caseId && r.ReptFrom == 3);
                        var fourthRepeat = disciplinaryRules.FirstOrDefault(r => r.DisciplinaryCaseId == caseId && r.ReptFrom == 4);
                        var disciplinaryFilterVM = new HrDisciplinaryFilterVM
                        {
                            Id = disciplinaryCase.Id,
                            CaseName = (lang == 1) ? disciplinaryCase.CaseName : disciplinaryCase.CaseName2,
                            FirstRepeat = (lang == 1) ? firstRepeat?.Name : firstRepeat?.Name2,
                            SecondRepeat = (lang == 1) ? secondRepeat?.Name : secondRepeat?.Name2,
                            ThirdRepeat = (lang == 1) ? thirdRepeat?.Name : thirdRepeat?.Name2,
                            FourthRepeat = (lang == 1) ? fourthRepeat?.Name : fourthRepeat?.Name2,
                        };
                        DisciplinaryFilterVMs.Add(disciplinaryFilterVM);
                    }
                }
                return Ok(await Result<List<HrDisciplinaryFilterVM>>.SuccessAsync(DisciplinaryFilterVMs, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrDisciplinaryFilterVM filter, int take = Pagination.take, int? lastSeenId = null)
        {
            var lang = session.Language;
            var chk = await permission.HasPermission(567, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                // استدعاء القضايا مع pagination
                var getAllDisciplinaryCases = await hrServiceManager.HrDisciplinaryCaseService.GetAllWithPaginationVW(
                    selector: e => e.Id,
                    expression: e => e.IsDeleted == false &&
                        (string.IsNullOrEmpty(filter.CaseName) ||
                            (e.CaseName != null && e.CaseName.ToLower().Contains(filter.CaseName.ToLower())) ||
                            (e.CaseName2 != null && e.CaseName2.ToLower().Contains(filter.CaseName.ToLower()))),
                    take: take,
                    lastSeenId: lastSeenId
                );

                // استدعاء القوانين المرتبطة بدون pagination (لأنها تعتمد على القضايا)
                var getAllDisciplinaryRules = await hrServiceManager.HrDisciplinaryRuleService.GetAll(r => r.DisciplinaryCaseId.HasValue);

                if (!getAllDisciplinaryCases.Succeeded || !getAllDisciplinaryRules.Succeeded)
                    return Ok(await Result<List<HrDisciplinaryFilterVM>>.FailAsync("Error while fetching data."));

                if (getAllDisciplinaryCases.Data == null || !getAllDisciplinaryCases.Data.Any())
                    return Ok(await Result<List<HrDisciplinaryFilterVM>>.SuccessAsync(new List<HrDisciplinaryFilterVM>()));

                var disciplinaryCases = getAllDisciplinaryCases.Data.ToList();
                var disciplinaryRules = getAllDisciplinaryRules.Data.ToList();

                List<HrDisciplinaryFilterVM> DisciplinaryFilterVMs = new List<HrDisciplinaryFilterVM>();

                foreach (var disciplinaryCase in disciplinaryCases)
                {
                    var caseId = disciplinaryCase.Id;
                    var firstRepeat = disciplinaryRules.FirstOrDefault(r => r.DisciplinaryCaseId == caseId && r.ReptFrom == 1);
                    var secondRepeat = disciplinaryRules.FirstOrDefault(r => r.DisciplinaryCaseId == caseId && r.ReptFrom == 2);
                    var thirdRepeat = disciplinaryRules.FirstOrDefault(r => r.DisciplinaryCaseId == caseId && r.ReptFrom == 3);
                    var fourthRepeat = disciplinaryRules.FirstOrDefault(r => r.DisciplinaryCaseId == caseId && r.ReptFrom == 4);

                    var disciplinaryFilterVM = new HrDisciplinaryFilterVM
                    {
                        Id = disciplinaryCase.Id,
                        CaseName = (lang == 1) ? disciplinaryCase.CaseName : disciplinaryCase.CaseName2,
                        FirstRepeat = (lang == 1) ? firstRepeat?.Name : firstRepeat?.Name2,
                        SecondRepeat = (lang == 1) ? secondRepeat?.Name : secondRepeat?.Name2,
                        ThirdRepeat = (lang == 1) ? thirdRepeat?.Name : thirdRepeat?.Name2,
                        FourthRepeat = (lang == 1) ? fourthRepeat?.Name : fourthRepeat?.Name2,
                    };
                    DisciplinaryFilterVMs.Add(disciplinaryFilterVM);
                }

                var paginatedData = new PaginatedResult<object>
                {
                    Succeeded = true,
                    Data = DisciplinaryFilterVMs,
                    Status = getAllDisciplinaryCases.Status,
                    PaginationInfo = getAllDisciplinaryCases.PaginationInfo
                };

                return Ok(paginatedData);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }



        [HttpPost("Add")]
        public async Task<IActionResult> Add(HrDisciplinaryCaseDto obj)
        {
            var chk = await permission.HasPermission(567, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {
                obj.FromDay ??= 0;
                obj.ToDay ??= 0;
                obj.FromMinutes ??= 0;
                obj.ToMinutes ??= 0;
                obj.ApplyOfEarly ??= false;
                obj.ApplyOfDelay ??= false;
                obj.ApplyOfAbsence ??= false;
                var addRes = await hrServiceManager.HrDisciplinaryCaseService.Add(obj);
                return Ok(addRes);

            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long Id)
        {
            var chk = await permission.HasPermission(567, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));

            try
            {
                var getItem = await hrServiceManager.HrDisciplinaryCaseService.GetForUpdate<HrDisciplinaryCaseEditDto>(Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    return Ok(getItem);
                }
                return Ok(await Result.FailAsync(getItem.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message)); ;
            }
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(HrDisciplinaryCaseEditDto obj)
        {
            var chk = await permission.HasPermission(567, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            try
            {
                obj.FromDay ??= 0;
                obj.ToDay ??= 0;
                obj.FromMinutes ??= 0;
                obj.ToMinutes ??= 0;
                obj.ApplyOfEarly ??= false;
                obj.ApplyOfDelay ??= false;
                obj.ApplyOfAbsence ??= false;
                var addRes = await hrServiceManager.HrDisciplinaryCaseService.Update(obj);
                return Ok(addRes);
            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long Id = 0)
        {
            var chk = await permission.HasPermission(567, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync("Please choose an entity to delete it, there is no id passed"));
            }

            try
            {
                var del = await hrServiceManager.HrDisciplinaryCaseService.Remove(Id);
                if (del.Succeeded)
                {

                    return Ok(del);
                }
                return Ok(await Result.FailAsync($"{del.Status.message}"));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpGet("DDLDisciplinaryType")]
        public async Task<IActionResult> DDLDisciplinaryType()
        {
            var lang = session.Language;
            try
            {
                var allDisciplinaryTypes = await mainServiceManager.SysLookupDataService.GetDataByCategory(477);

                if (allDisciplinaryTypes.Succeeded && allDisciplinaryTypes.Data != null)
                {
                    return Ok(await Result<object>.SuccessAsync(allDisciplinaryTypes, ""));

                }
                return Ok(allDisciplinaryTypes);

            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("GetRules")]
        public async Task<IActionResult> GetRules(long Id)
        {
            var chk = await permission.HasPermission(567, PermissionType.Edit);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id.Equals(null))
            {
                return Ok(await Result.FailAsync("You do not Enter Any Value"));
            }

            try
            {
                var getItem = await hrServiceManager.HrDisciplinaryRuleService.GetAllVW(x => x.DisciplinaryCaseId == Id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    return Ok(getItem);
                }
                return Ok(await Result.FailAsync(getItem.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message)); ;
            }
        }
        [HttpPost("AddRule")]
        public async Task<IActionResult> AddRule(HrDisciplinaryRuleDto obj)
        {
            var chk = await permission.HasPermission(567, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            if (string.IsNullOrEmpty(obj.Name))
            {
                return Ok(await Result.FailAsync(" يجب ادخال الاسم"));

            }
            if (string.IsNullOrEmpty(obj.Name2))
            {
                return Ok(await Result.FailAsync("يجب ادخال الاسم بالانجليزي "));


            }
            if (obj.ReptFrom == null)
            {
                return Ok(await Result.FailAsync("يجب ادخال عدد التكرار من"));

            }
            if (obj.ReptTo == null)
            {
                return Ok(await Result.FailAsync("يجب ادخال عدد التكرار الى"));

            }
            if (obj.DeductedRate == null)
            {
                return Ok(await Result.FailAsync("يجب ادخال النسبة"));

            }
            try
            {

                var addRes = await hrServiceManager.HrDisciplinaryCaseService.AddNewRule(obj);
                return Ok(addRes);

            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }
        [HttpPost("EditRule")]
        public async Task<IActionResult> EditRule(HrDisciplinaryRuleEditDto obj)
        {
            var chk = await permission.HasPermission(567, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (!ModelState.IsValid)
            {
                return Ok(obj);
            }
            if (string.IsNullOrEmpty(obj.Name))
            {
                return Ok(await Result.FailAsync(" يجب ادخال الاسم"));

            }
            if (string.IsNullOrEmpty(obj.Name2))
            {
                return Ok(await Result.FailAsync("يجب ادخال الاسم بالانجليزي "));


            }
            if (obj.ReptFrom == null)
            {
                return Ok(await Result.FailAsync("يجب ادخال عدد التكرار من"));

            }
            if (obj.ReptTo == null)
            {
                return Ok(await Result.FailAsync("يجب ادخال عدد التكرار الى"));

            }
            if (obj.DeductedRate == null)
            {
                return Ok(await Result.FailAsync("يجب ادخال النسبة"));

            }
            try
            {

                var addRes = await hrServiceManager.HrDisciplinaryCaseService.EditRule(obj);
                return Ok(addRes);

            }

            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpDelete("DeleteRule")]
        public async Task<IActionResult> DeleteRule(long Id = 0)
        {
            var chk = await permission.HasPermission(567, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id <= 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrDisciplinaryRuleService.Remove(Id);

                return Ok(del);

            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));
            }
        }

    }

}