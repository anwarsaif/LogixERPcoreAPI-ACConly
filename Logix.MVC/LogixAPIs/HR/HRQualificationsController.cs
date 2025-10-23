using AutoMapper;
using DevExpress.CodeParser;
using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.HR;
using Logix.Infrastructure.Repositories;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Result = Logix.Application.Wrapper.Result;

namespace Logix.MVC.LogixAPIs.HR
{
    //  المؤهلات والخبرات
    public class HRQualificationsController : BaseHrApiController
    {
        private readonly IHrServiceManager hrServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IMapper mapper;



        public HRQualificationsController(IHrServiceManager hrServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager, IMapper _mapper)
        {
            this.permission = permission;
            this.session = session;
            this.hrServiceManager = hrServiceManager;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
            this.mapper = _mapper;
        }


        #region IndexPage

        [HttpPost("Search")]
        public async Task<IActionResult> Search(HrQualificationsFilterDto filter)
        {
            var chk = await permission.HasPermission(333, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await hrServiceManager.HrEmployeeService.HRQualificationsSearch(filter);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));

            }
        }

        [HttpPost("GetPagination")]
        public async Task<IActionResult> GetPagination([FromBody] HrQualificationsFilterDto filter, int take = 5, long? lastSeenId = null)
        {
            var chk = await permission.HasPermission(28, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                filter.BranchId ??= 0;
                filter.JobType ??= 0;
                filter.DeptId ??= 0;
                filter.NationalityId ??= 0;
                filter.JobCatagoriesId ??= 0;
                filter.Status ??= 0;
                var BranchesList = session.Branches.Split(',');
                var result = await hrServiceManager.HrEmployeeService.GetAllWithPaginationVW(
                    selector: x => x.Id,
                    expression: e => e.Isdel == false
               && (filter.BranchId != 0 ? e.BranchId == filter.BranchId : BranchesList.Contains(e.BranchId.ToString()))
               && (filter.JobType == 0 || e.JobType == filter.JobType)
               && (filter.DeptId == 0 || e.DeptId == filter.DeptId)
               && (filter.NationalityId == 0 || e.NationalityId == filter.NationalityId)
               && (filter.JobCatagoriesId == 0 || e.JobCatagoriesId == filter.JobCatagoriesId)
               && (filter.Status == 0 || e.StatusId == filter.Status)
               && (string.IsNullOrEmpty(filter.EmpName) || e.EmpName.ToLower().Contains(filter.EmpName.ToLower()) || e.EmpName2.ToLower().Contains(filter.EmpName.ToLower()))
               && (string.IsNullOrEmpty(filter.EmpCode) || e.EmpId == filter.EmpCode)
               && (string.IsNullOrEmpty(filter.IdNo) || e.IdNo == filter.IdNo)
               && (string.IsNullOrEmpty(filter.PassId) || e.PassportNo == filter.PassId)
               && (string.IsNullOrEmpty(filter.EntryNo) || e.EntryNo == filter.EntryNo),
                    take: take,
                    lastSeenId: lastSeenId
                );

                if (!result.Succeeded)
                    return StatusCode(result.Status.code, result.Status.message);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));

            }
        }
        #endregion


        #region Get Employee Qualifications region 
        [HttpGet("GetByEmpId")]
        public async Task<IActionResult> GetQualificationsData(long EmpId)
        {
            var chk = await permission.HasPermission(333, PermissionType.View);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var checkEmpExist = await mainServiceManager.InvestEmployeeService.GetOne(x => x.Id == EmpId && x.IsDeleted == false && x.Isdel == false);
                if (checkEmpExist.Data == null) return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeNotFound")));

                // جلب خبرات العمل
                var getWorkData = await hrServiceManager.HrWorkExperienceService.GetAll(x => x.IsDeleted == false && x.EmpId == EmpId);

                // جلب التعليم
                var getEducationData = await hrServiceManager.HrEducationService.GetAllVW(x => x.IsDeleted == false && x.EmpId == EmpId);

                // جلب المهارات
                var getSkillsData = await hrServiceManager.HrSkillService.GetAllVW(x => x.IsDeleted == false && x.EmpId == EmpId);

                // جلب اللغات
                var getLanguagesData = await hrServiceManager.HrLanguageService.GetAllVW(x => x.IsDeleted == false && x.EmpId == EmpId);

                // جلب التراخيص
                var getLicensesData = await hrServiceManager.HrLicenseService.GetAllVW(x => x.IsDeleted == false && x.EmpId == EmpId);

                // جلب المرفقات
                var getFilesData = await hrServiceManager.HrFileService.GetAllVW(x => x.IsDeleted == false && x.EmpId == EmpId);

                return Ok(await Result<object>.SuccessAsync(new { WorkData = getWorkData.Data.ToList(), EducationData = getEducationData.Data.ToList(), SkillsData = getSkillsData.Data.ToList(), LanguagesData = getLanguagesData.Data.ToList(), LicensesData = getLicensesData.Data.ToList(), FilesData = getFilesData.Data.ToList() }));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync(ex.Message));

            }
        }

        #endregion

        #region Add Data region 


        [HttpPost("AddLanguage")]
        public async Task<IActionResult> AddLanguage(HrLanguageDto obj)
        {
            var chk = await permission.HasPermission(333, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                var addRes = await hrServiceManager.HrLanguageService.Add(obj);

                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLanguageDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("AddSkill")]
        public async Task<IActionResult> AddSkill(HrSkillDto obj)
        {
            var chk = await permission.HasPermission(333, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                var addRes = await hrServiceManager.HrSkillService.Add(obj);

                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrSkillDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("AddFile")]
        public async Task<IActionResult> AddFile(HrFileDto obj)
        {
            var chk = await permission.HasPermission(333, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                var addRes = await hrServiceManager.HrFileService.Add(obj);

                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrFileDto>.FailAsync(ex.Message));
            }
        }
        [HttpPost("AddWorkExperience")]
        public async Task<IActionResult> AddWorkExperience(HrWorkExperienceDto obj)
        {
            var chk = await permission.HasPermission(333, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                var addRes = await hrServiceManager.HrWorkExperienceService.Add(obj);

                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrWorkExperienceDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("AddLicense")]
        public async Task<IActionResult> AddLicense(HrLicenseDto obj)
        {
            var chk = await permission.HasPermission(333, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));

                var addRes = await hrServiceManager.HrLicenseService.Add(obj);

                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrLicenseDto>.FailAsync(ex.Message));
            }
        }

        [HttpPost("AddEducation")]
        public async Task<IActionResult> AddEducation(HrEducationDto obj)
        {
            var chk = await permission.HasPermission(333, PermissionType.Add);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                if (string.IsNullOrEmpty(obj.EmpCode)) return Ok(await Result<object>.FailAsync(localization.GetResource1("EmployeeIsNumber")));


                var addRes = await hrServiceManager.HrEducationService.Add(obj);

                return Ok(addRes);
            }
            catch (Exception ex)
            {
                return Ok(await Result<HrEducationDto>.FailAsync(ex.Message));
            }
        }

        #endregion



        #region Delete Data region 
        [HttpDelete("DeleteLanguage")]
        public async Task<IActionResult> DeleteLanguage(long Id = 0)
        {
            var chk = await permission.HasPermission(333, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("ChooseDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrLanguageService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        [HttpDelete("DeleteSkill")]
        public async Task<IActionResult> DeleteSkill(long Id = 0)
        {
            var chk = await permission.HasPermission(333, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("ChooseDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrSkillService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }


        [HttpDelete("DeleteFile")]
        public async Task<IActionResult> DeleteFile(long Id = 0)
        {
            var chk = await permission.HasPermission(333, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("ChooseDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrFileService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }


        [HttpDelete("DeleteWorkExperience")]
        public async Task<IActionResult> DeleteWorkExperience(long Id = 0)
        {
            var chk = await permission.HasPermission(333, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("ChooseDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrWorkExperienceService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        [HttpDelete("DeleteLicense")]
        public async Task<IActionResult> DeleteLicense(long Id = 0)
        {
            var chk = await permission.HasPermission(333, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("ChooseDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrLicenseService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }

        [HttpDelete("DeleteEducation")]
        public async Task<IActionResult> DeleteEducation(long Id = 0)
        {
            var chk = await permission.HasPermission(333, PermissionType.Delete);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            if (Id == 0)
            {
                return Ok(await Result.FailAsync($"{localization.GetMessagesResource("ChooseDelete")}"));
            }

            try
            {
                var del = await hrServiceManager.HrEducationService.Remove(Id);

                return Ok(del);
            }
            catch (Exception exp)
            {
                return Ok(await Result.FailAsync($"{exp.Message}"));
            }
        }
        #endregion

    }
}