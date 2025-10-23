using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.OPM;
using Logix.Application.DTOs.PM;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;

namespace Logix.MVC.LogixAPIs.Main
{
    public class PERMISSIONACCTransfer : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPMServiceManager pmServiceManager;
        private readonly IOPMServiceManager opmServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IDDListHelper listHelper;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly IScreenPropertiesHelper screenPropertiesHelper;
        private readonly IGbServiceManager gbServiceManager;
        private readonly ILocalizationService localization;
        private readonly ICurrentData session;
        public PERMISSIONACCTransfer(
            IMainServiceManager mainServiceManager,
            IHrServiceManager hrServiceManager,
            IAccServiceManager accServiceManager,
            IPMServiceManager pmServiceManager,
            IOPMServiceManager opmServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            IDDListHelper listHelper,
            ISysConfigurationHelper configurationHelper,
            IScreenPropertiesHelper screenPropertiesHelper,
            IGbServiceManager gbServiceManager,
               ILocalizationService localization

            )
        {
            this.mainServiceManager = mainServiceManager;
            this.hrServiceManager = hrServiceManager;
            this.accServiceManager = accServiceManager;
            this.pmServiceManager = pmServiceManager;
            this.opmServiceManager = opmServiceManager;
            this.permission = permission;
            this.listHelper = listHelper;
            this.configurationHelper = configurationHelper;
            this.screenPropertiesHelper = screenPropertiesHelper;
            this.gbServiceManager = gbServiceManager;
            this.localization = localization;
            this.session = session;
        }

        #region "transactions"


        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(850, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await mainServiceManager.SysUserService.GetAllVW(x => x.Isdel == false && x.FacilityId == session.FacilityId && x.UserTypeId==1);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.UserId);
                    return Ok(items);
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        [HttpPost("Search")]
        public async Task<IActionResult> Search(SysUserfilterDto filter)
        {
            var chk = await permission.HasPermission(850, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {
                var items = await mainServiceManager.SysUserService.GetAllVW(x => x.Isdel == false && x.FacilityId == session.FacilityId && x.UserTypeId == 1);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter == null)
                    {
                        return Ok(items);
                    }
                    if(session.GroupId == 1)
                    {
                      
                            var systemIds = (await mainServiceManager.SysGroupService.GetAll(x => x.IsDeleted==false && x.GroupId == session.GroupId)).Data.Select(g => g.SystemId).ToList();
                            //res = res.Where(x => !x.IsDeleted && systemIds.Contains(x.sys));
                      
                        //string SystemId = null;
                        //var Groupdata=await mainServiceManager.SysGroupService.GetAll(x=>x.IsDeleted== false && x.GroupId==session.GroupId);
                        //foreach (var item in Groupdata.Data)
                        //{
                        //    SystemId += item.SystemId + ",";
                        //    if (SystemId != null)
                        //    {
                        //        var SystemIdAll = SystemId.Split(',');
                        //        clean archt
                        //        res = res.Where(x => x.IsDeleted == false &&SystemIdAll.Contains(x.sys));
                        //    }

                        //}
                    }
                    if (filter.BranchId != null && filter.BranchId > 0)
                    {
                        res = res.Where(s => s.UserPkId.Equals(filter.BranchId));
                    }
                    if (filter.Enable != null && filter.Enable > 0)
                    {
                        res = res.Where(s => s.Enable.Equals(filter.Enable));
                    }
                    if (filter.GroupsId != null && Convert.ToInt64(filter.GroupsId) > 0)
                    {
                        res = res.Where(s => s.GroupsId.Equals(Convert.ToInt64(filter.GroupsId)));
                    }
                    if (!string.IsNullOrEmpty(filter.UserFullname))
                    {
                        res = res.Where(s => s.UserFullname != null && s.UserFullname.Contains(filter.UserFullname));
                    }
                    if (!string.IsNullOrEmpty(filter.EmpCode))
                    {
                        res = res.Where(s => s.EmpCode != null && s.EmpCode.Contains(filter.EmpCode));
                    }

                    var final = res.ToList();
                    return Ok(await Result<List<SysUserVw>>.SuccessAsync(final, ""));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<SysUserVw>.FailAsync($"======= Exp in Search SysUserVw, MESSAGE: {ex.Message}"));
            }
        }
        #endregion "transactions"


        [HttpGet("GetACCTransfer")]
        public async Task<IActionResult> GetACCTransferByUserId(long UserId)
        {
            try
            {
                var chk = await permission.HasPermission(850, PermissionType.Show);
                if (!chk)
                {
                    return Ok(await Result.AccessDenied("AccessDenied"));
                }

                if (UserId <= 0)
                {
                    return Ok(await Result<AccDocumentTypeVM>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }

                var getItem = await mainServiceManager.SysUserService.GetForUpdate<SysUserDto>(UserId);
                if (getItem.Succeeded)
                {

                    var AccDoList = new List<AccDocumentTypeVM>();
                    var getAccD = await accServiceManager.AccDocumentTypeListVwService.GetAll(x=>x.FlagDelete==false);
                    if (getAccD.Succeeded && getAccD.Data != null)
                    {
                        foreach (var item in getAccD.Data)
                        {
                            AccDoList.Add(new AccDocumentTypeVM { DocTypeID = item.DocTypeId, DocTypeName = item.DocTypeName ?? "", DocTypeName2 = item.DocTypeName2 ?? "", Selected = false });
                        }
                    }

                    var currentAccD = getItem.Data.AccTransfer;
                    var currList = currentAccD.Split(",");
                    if (!string.IsNullOrEmpty(currentAccD))
                    {
                        foreach (var d in currList)
                        {
                            foreach (var day in AccDoList)
                            {
                                if (day.DocTypeID == int.Parse(d))
                                {
                                    day.Selected = true;
                                    break;
                                }
                            }
                        }
                    }


                    return Ok(await Result<IEnumerable<AccDocumentTypeVM>>.SuccessAsync(AccDoList));

                }
                else
                {
                    return Ok(await Result<AccDocumentTypeVM>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
                }
            }
            catch (Exception ex)
            {
                return Ok(await Result<AccDocumentTypeVM>.FailAsync($"====== Exp in Get ACC Transfer Acc Bank, MESSAGE: {ex.Message}"));
            }
        }
    }
}

