using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Logix.MVC.LogixAPIs.PM
{

    // الضمانات البنكية
    public class PMBankGuaranteesController : BasePMApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IPMServiceManager pMServiceManager;



        public PMBankGuaranteesController(IPermissionHelper permission, ICurrentData session, ILocalizationService localization, IMainServiceManager mainServiceManager, IPMServiceManager pMServiceManager)
        {
            this.permission = permission;
            this.session = session;
            this.localization = localization;
            this.mainServiceManager = mainServiceManager;
            this.pMServiceManager = pMServiceManager;
        }

        #region Index Page

        [HttpPost("Search")]
        public async Task<IActionResult> Search(PaginatedRequest<PmProjectsGuaranteeFilterDto> request)
        {

            try
            {
                var chk = await permission.HasPermission(1641, PermissionType.Show);

                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                var filter = request.Filter;
                filter.ProjectCode ??= 0;
                filter.StatusId ??= 0;
                filter.BankId ??= 0;
                filter.GuaranteeType ??= 0;
                filter.FacilityId = (int?)session.FacilityId;

                var items = await pMServiceManager.PmProjectsGuaranteeService.Search(filter);

                if (!items.Succeeded)
                    return Ok(await Result<object>.FailAsync(items.Status.message));


                if (request.PageNumber > 0 && request.PageSize > 0)
                {
                    var paginatedData = await PaginatedResult<object>.SuccessAsync(items.Data, request.PageNumber, request.PageSize);
                    return Ok(paginatedData);
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(Search)}: {ex.Message}"));
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                var chk = await permission.HasPermission(1641, PermissionType.Delete);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

                var delete = await pMServiceManager.PmProjectsGuaranteeService.Remove(id);

                return Ok(delete);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }

        }

        #endregion

        #region Edit Page



        [HttpGet("GetById")]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                // Check permissions
                var check = await permission.HasPermission(1641, PermissionType.Edit);

                if (!check)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                if (id <= 0)
                    return Ok(await Result<object>.FailAsync(localization.GetMessagesResource("NoIdInUpdate")));

                var item = await pMServiceManager.PmProjectsGuaranteeService.GetOneVW(x => x.IsDeleted == false && x.Id == id);
                if (!item.Succeeded || item.Data == null)
                    return Ok(await Result<object>.FailAsync(item.Succeeded ? localization.GetMessagesResource("NoIdInUpdate") : item.Status.message));

                // Retrieve Files
                var fileResult = await mainServiceManager.SysFileService.GetFilesForUser(id, 105);
                var fileDtos = fileResult.Data;

                // Prepare response
                var response = new
                {
                    item.Data,
                    FileDtos = fileDtos,

                };

                return Ok(await Result<object>.SuccessAsync(response, string.Empty, 200));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in PMChangeRequestController Edit method, MESSAGE: {ex.Message}"));
            }
        }


        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmProjectsGuaranteeEditDto obj)
        {

            try
            {

                var chk = await permission.HasPermission(252, PermissionType.Edit);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));
                if (obj.Id <= 0)
                    return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                obj.ProjectCode ??= 0;
                obj.FacilityId = (int?)session.FacilityId;
                //  المشروع 
                if (obj.ProjectCode <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("ProjectNo")));

                //التاريخ
                if (string.IsNullOrEmpty(obj.Date)) return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                //الجهة
                if (obj.SupType <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("EntityNo")));

                //رقم المورد
                if (string.IsNullOrEmpty(obj.SupCode)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Supplier")));


                //نوع الضمان
                if (obj.GuaranteeType <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("GuaranteesType")));
                //رقم الضمان 

                if (string.IsNullOrEmpty(obj.GuaranteeNo)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("GuaranteesNO")));

                //نسبة الضمان
                if (obj.Rate <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("GuaranteesRate")));

                //  البنك
                if (obj.BankId <= 0) return Ok(await Result<object>.FailAsync(localization.GetAccResource("Bank")));

                // رقم التواصل مع البنك
                if (string.IsNullOrEmpty(obj.BankMobil)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("BankcontactMobile")));

                // تاريخ الإصدار 
                if (string.IsNullOrEmpty(obj.IssueDate)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("IssueDate")));

                //  تاريخ النهاية
                if (string.IsNullOrEmpty(obj.ExpiryDate)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("EndDate")));


                var result = await pMServiceManager.PmProjectsGuaranteeService.Update(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion


        #region Add Page

        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmProjectsGuaranteeDto obj)
        {

            try
            {
                var chk = await permission.HasPermission(1641, PermissionType.Add);
                if (!chk)
                    return Ok(await Result.AccessDenied("AccessDenied"));

                obj.ProjectCode ??= 0;
                obj.FacilityId = (int?)session.FacilityId;
                obj.StatusId = 1;
                //  المشروع 
                if (obj.ProjectCode <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("ProjectNo")));

                //التاريخ
                if (string.IsNullOrEmpty(obj.Date)) return Ok(await Result<object>.FailAsync(localization.GetCommonResource("Tdate")));

                //الجهة
                if (obj.SupType <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("EntityNo")));

                //رقم المورد
                if (string.IsNullOrEmpty(obj.SupCode)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("Supplier")));


                //نوع الضمان
                if (obj.GuaranteeType <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("GuaranteesType")));
                //رقم الضمان 

                if (string.IsNullOrEmpty(obj.GuaranteeNo)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("GuaranteesNO")));

                //نسبة الضمان
                if (obj.Rate <= 0) return Ok(await Result<object>.FailAsync(localization.GetPMResource("GuaranteesRate")));

                //  البنك
                if (obj.BankId <= 0) return Ok(await Result<object>.FailAsync(localization.GetAccResource("Bank")));

                // رقم التواصل مع البنك
                if (string.IsNullOrEmpty(obj.BankMobil)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("BankcontactMobile")));

                // تاريخ الإصدار 
                if (string.IsNullOrEmpty(obj.IssueDate)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("IssueDate")));

                //  تاريخ النهاية
                if (string.IsNullOrEmpty(obj.ExpiryDate)) return Ok(await Result<object>.FailAsync(localization.GetPMResource("EndDate")));

                var result = await pMServiceManager.PmProjectsGuaranteeService.Add(obj);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));

            }

        }

        #endregion


    }
}
