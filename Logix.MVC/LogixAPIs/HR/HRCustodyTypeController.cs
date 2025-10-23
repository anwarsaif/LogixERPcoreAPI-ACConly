using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Interfaces.IServices.HR;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
namespace Logix.MVC.LogixAPIs.HR
{
    public class HRCustodyTypeController : BaseHrApiController
        {
            private readonly IMainServiceManager mainServiceManager;
            private readonly IAccServiceManager accServiceManager;
            private readonly IPermissionHelper permission;
            private readonly IDDListHelper listHelper;
            private readonly ICurrentData session;
            private readonly ILocalizationService localization;
            private readonly IHrServiceManager hrServiceManager;
            private readonly IHrArchiveFilesDetailService hrArchiveFilesDetailService;

            public HRCustodyTypeController(IMainServiceManager mainServiceManager,
                IPermissionHelper permission,
                IDDListHelper listHelper,
                ILocalizationService localization,
                ICurrentData session,
                IHrServiceManager hrServiceManager,
                IAccServiceManager accServiceManager,
                IHrArchiveFilesDetailService hrArchiveFilesDetailService
                )
            {
                this.mainServiceManager = mainServiceManager;
                this.permission = permission;
                this.listHelper = listHelper;
                this.session = session;
                this.localization = localization;
                this.hrServiceManager = hrServiceManager;
                this.accServiceManager = accServiceManager;
                this.hrArchiveFilesDetailService = hrArchiveFilesDetailService;
            }
            [NonAction]
            private void setErrors()
            {
                var errors = new ErrorsHelper(ModelState);
            }
            [HttpGet("GetAll")]
            public async Task<IActionResult> GetAll()
            {
                //var chk = await permission.HasPermission(1478, PermissionType.Show);
                //if (!chk)
                //{
                //    return Ok(await Result.AccessDenied("AccessDenied"));
                //}

                try
                {
                    var items = await hrServiceManager.HrCustodyTypeService.GetAll();
                    if (items.Succeeded)
                    {
                        var res = items.Data.AsQueryable();
                        res = res.OrderBy(e => e.Id);
                        return Ok(await Result<List<HrCustodyTypeDto>>.SuccessAsync(res.ToList(), items.Status.message));
                    }
                    return Ok(items);
                }
                catch (Exception ex)
                {
                    return Ok(await Result.FailAsync(ex.Message));
                }
            }
        }  
            }

        
    

