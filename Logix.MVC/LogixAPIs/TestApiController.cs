using System.Globalization;
using EInvoiceKSADemo.Helpers.Zatca.Models;
using Logix.Application.Common;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs
{
    [Route("api/main/[controller]")]
    [ApiController]
    public class TestApiController : Controller
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly ICurrentData currentData;
        private readonly ILocalizationService localization;

        public TestApiController(
            IMainServiceManager mainServiceManager,
            ICurrentData currentData,
            ILocalizationService localization,
            IHrServiceManager hrServiceManager)
        {
            this.mainServiceManager = mainServiceManager;
            this.currentData = currentData;
            this.localization = localization;
            this.hrServiceManager = hrServiceManager;
        }


        private async Task<(InvoiceDataModel? InvDataModel, long CustomerId, long? ProductsInvoiceId, bool IsThereVate)>
            GetPM_Extract_TransactionsData(int x)
        {
            InvoiceDataModel InvoiceDataModel = new();
            long CustomerId = 3; long? ProductsInvoiceId = null; bool IsThereVate = true;
            return (null, CustomerId, ProductsInvoiceId, IsThereVate);
        }

        [AllowAnonymous]
        [HttpPost("TestDate")]
        public async Task<IActionResult> TestDate(int calanderType)
        {
            try
            {
                var x = DateHelper.YearHijri(calanderType.ToString());

                var arCul = new CultureInfo("en-US");
                var enCul = new CultureInfo("ar-SA");

                var h = new UmAlQuraCalendar();
                var g = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
                string res = "";
                if (calanderType == 2)
                    res = DateTime.Now.AddDays(-1).ToString("yyyy", enCul.DateTimeFormat);
                else
                    res = DateTime.Now.AddDays(-1).ToString("yyyy", arCul.DateTimeFormat);

                return Ok(res);
            }
            catch
            {
                return Ok();
            }
        }

        [AllowAnonymous]
        [HttpPost("TestSetting")]
        public async Task<IActionResult> TestSetting(string empcode)
        {
            try
            {
                return Ok();
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [HttpGet("TestSession")]
        public async Task<IActionResult> TestSession()
        {
            try
            {
                var FacilityId = currentData.FacilityId;
                var FinYear = currentData.FinYear;
                var FinyearGregorian = currentData.FinyearGregorian;
                var UserId = currentData.UserId;
                var GroupId = currentData.GroupId;
                var EmpId = currentData.EmpId;
                var Branches = currentData.Branches;
                var Language = currentData.Language;
                var msg = localization.GetMessagesResource("NoIdInDelete");

                return Ok(new { FacilityId, FinYear, FinyearGregorian, UserId, GroupId, EmpId, Branches, Language, msg });
            }
            catch
            {
                return Ok();
            }
        }
    }
}
