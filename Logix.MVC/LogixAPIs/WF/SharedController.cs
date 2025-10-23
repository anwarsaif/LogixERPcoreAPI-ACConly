using Logix.Application.Common;
using Logix.Application.DTOs.WF;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.WF.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WF
{
    public class SharedController : BaseWfController
    {
        private readonly IWFServiceManager wfServiceManager;
        private readonly ICurrentData session;

        public SharedController(IWFServiceManager wfServiceManager,
            ICurrentData session)
        {
            this.wfServiceManager = wfServiceManager;
            this.session = session;
        }

        [HttpGet("GetStepper")]
        public async Task<ActionResult> GetStepper(long appTypeId, long applicationId)
        {
            try
            {
                var lang = session.Language;
                List<WfStepperVm> final = new();
                var getStepTrans = await wfServiceManager.WfStepsTransactionService.GetAllVW(x => x.AppTypeId == appTypeId && x.IsDeleted == false);
                if (getStepTrans.Succeeded)
                {
                    var res = getStepTrans.Data.OrderBy(x => x.SortNo).ToList();
                    foreach (var item in res)
                    {
                        // get count
                        var getAppStatus = await wfServiceManager.WfApplicationsStatusService.GetAll(x => x.Id, x => x.ApplicationsId == applicationId
                        && x.OldStepId.ToString() == item.FromStepId);
                        int cnt = getAppStatus.Data.Count();

                        final.Add(new WfStepperVm
                        {
                            SortNo = item.SortNo ?? 0,
                            FromStepName = lang == 1 ? item.FromStepName : item.FromStepName2,
                            Count = cnt
                        });
                    }
                }
                return Ok(await Result<List<WfStepperVm>>.SuccessAsync(final));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"====== Exp, MESSAGE: {ex.Message}"));
            }
        }

    }
}
