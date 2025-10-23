using System.Data;
using System.Globalization;
using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.WF;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Interfaces.IServices.Main;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.Main.ViewModels;
using Logix.MVC.LogixAPIs.ViewModelFilter;
using Logix.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs
{
    [Route($"api/{ApiConfig.ApiVersion}/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IRptServiceManager rptServiceManager;
        private readonly IAuthService authService;
        private readonly ITsServiceManager tsServiceManager;
        private readonly ICurrentData currentData;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly IAccServiceManager accServiceManager;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;
        private readonly ILocalizationService localization;
        private readonly IPermissionHelper permission;
        private readonly IWFServiceManager wFServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IHDServiceManager hDServiceManager;
        private readonly IPurServiceManager purServiceManager;
        private readonly IMainRepositoryManager mainRepositoryManager;

        public MasterController(
            IMainServiceManager mainServiceManager,
          IRptServiceManager rptServiceManager,
            IAuthService authService,
            ITsServiceManager tsServiceManager,
            ICurrentData currentData,
            ISysConfigurationHelper configurationHelper,
            IAccServiceManager accServiceManager,
            IConfiguration configuration,
            IEmailService emailService,
            ILocalizationService localization,
            IPermissionHelper permission,
            IWFServiceManager wFServiceManager,
            IHrServiceManager hrServiceManager,
            IHDServiceManager hDServiceManager,
            IPurServiceManager purServiceManager,
            IMainRepositoryManager mainRepositoryManager
            )
        {
            this.mainServiceManager = mainServiceManager;
            this.rptServiceManager = rptServiceManager;
            this.authService = authService;
            this.tsServiceManager = tsServiceManager;
            this.currentData = currentData;
            this.configurationHelper = configurationHelper;
            this.accServiceManager = accServiceManager;
            this.configuration = configuration;
            this.emailService = emailService;
            this.localization = localization;
            this.permission = permission;
            this.wFServiceManager = wFServiceManager;
            this.hrServiceManager = hrServiceManager;
            this.hDServiceManager = hDServiceManager;
            this.purServiceManager = purServiceManager;
            this.mainRepositoryManager = mainRepositoryManager;

        }


        //#region ======================================/* مجرد تأكيد أن السيرفر شغال*/
        //[AllowAnonymous]
        //[HttpGet("CheckServerHealth")]
        //public IActionResult CheckServerHealth()
        //{
        //    return Ok(new { success = true });
        //}
        //#endregion ====================================== /* مجرد تأكيد أن السيرفر شغال*/



        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var obj = new CurrentUserVM();
                // get calender type
                var calendarType = await configurationHelper.GetValue(19, currentData.FacilityId);
                obj.CalendarType = !string.IsNullOrEmpty(calendarType) ? calendarType : "1";

                var userVw = await mainServiceManager.SysUserService.GetOneVW(x => x.UserId == currentData.UserId);
                if (userVw.Succeeded && userVw.Data != null)
                {
                    var userData = userVw.Data;
                    var pass = await mainRepositoryManager.StoredProceduresRepository.GetDecryptUserPassword(userData.UserId);
                    obj = new CurrentUserVM()
                    {
                        Id = userData.UserId,
                        UserName = userData.UserName,
                        UserFullname = userData.UserFullname,
                        Email = userData.UserEmail,
                        UserTypeId = userData.UserTypeId,
                        UserPkId = userData.UserPkId,
                        BranchsId = userData.BranchsId,
                        GroupsId = userData.GroupsId,
                        FacilityId = userData.FacilityId,
                        EmpId = userData.EmpId,
                        EmpCode = userData.EmpCode,
                        FinYear = currentData.FinYear,
                        FinyearGregorian = currentData.FinyearGregorian,
                        EmpName = userData.EmpName,
                        EmpName2 = userData.EmpName2,
                        CalendarType = calendarType,
                        FacilityName = userData.FacilityName,
                        FacilityName2 = userData.FacilityName2,
                        FacilityLogo = userData.FacilityLogo,
                        FacilityPhone = userData.FacilityPhone,
                        LastLogin = userData.LastLogin != null ? userData.LastLogin.Value.ToString("yyyy/MM/dd h:mm:ss tt", CultureInfo.InvariantCulture) : "",
                        isAgree = userData.IsAgree,
                        pass = pass

                    };
                }
                return Ok(obj);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetEmpUsers")]
        public async Task<IActionResult> GetEmpUsers()
        {
            try
            {
                var getUsers = await authService.GetUsersByEmpId(currentData.EmpId, currentData.UserId);
                if (!getUsers.Succeeded)
                    return Ok(await Result<object>.FailAsync(getUsers.Status.message));

                if (!getUsers.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), ""));
                var result = getUsers.Data.Select(x => new { UserId = x.UserId, Username = x.UserName, FacilityName = x.FacilityName, FacilityName2 = x.FacilityName2, FacilityId = x.FacilityId });
                return Ok(await Result<object>.SuccessAsync(result));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetHomeSystems")]
        public async Task<IActionResult> GetHomeSystems()
        {
            try
            {
                var systems = await mainServiceManager.SysSystemService.GetSystemsToShowInHome();

                return Ok(systems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("SearchScreens")]
        public async Task<IActionResult> SearchScreens(string term)
        {
            try
            {
                //await Task.Delay(TimeSpan.FromMilliseconds(50));
                var screens = new List<ScreenSearchDto>();
                if (string.IsNullOrEmpty(term))
                {
                    return Ok(screens);
                }

                var getScreens = await mainServiceManager.SysSystemService.SearchScreens(term);
                if (getScreens.Succeeded && getScreens.Data != null)
                {
                    if (getScreens.Data.Count() > 0)
                    {
                        foreach (var item in getScreens.Data)
                        {
                            if (!string.IsNullOrEmpty(item.ScreenUrl))
                            {
                                item.ScreenUrl = item.ScreenUrl.StartsWith("/") ? item.ScreenUrl : item.Folder + "/" + item.ScreenUrl;
                                item.ScreenUrl = $"{item.ScreenUrl}";
                            }

                            item.ScreenName = currentData.Language == 1 ? item.ScreenName : item.ScreenName2;
                            screens.Add(item);
                        }
                        return Ok(screens);

                    }
                }

                //return Ok("");
                return Ok(screens);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
        }

        [HttpGet("GetFinYears")]
        public async Task<IActionResult> GetFinYears()
        {
            try
            {
                var allFinYear = new List<AccFinancialYear>();
                var getFinYears = await mainServiceManager.SysSystemService.GetFinancialYearsAll(currentData.FacilityId);
                return Ok(getFinYears);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpGet("GetUserMailBox")]
        public async Task<IActionResult> GetUserMailBox()
        {
            try
            {
                bool loginDMS = false;
                int ReferralsToUserId = 0;
                int ReferralsToDepId = 0;

                var chkEnable = await mainServiceManager.SysUserService.GetOne(u => u.Id == currentData.UserId);
                if (chkEnable.Succeeded)
                {
                    if (chkEnable.Data != null)
                        loginDMS = chkEnable.Data.Isupdate ?? false;

                    if (loginDMS)
                    {
                        var resulDatatable = await mainServiceManager.SysUserService.GetUserMailBox(ReferralsToUserId, ReferralsToDepId);
                        if (resulDatatable.Succeeded)
                        {
                            List<UserMailBoxVM> final = new();

                            DataTable dt = resulDatatable.Data;
                            foreach (DataRow row in dt.Rows)
                            {
                                final.Add(new UserMailBoxVM
                                {
                                    Id = Convert.ToInt64(row["ID"]),
                                    ReferralId = Convert.ToInt64(row["Referral_ID"]),
                                    IsRead = Convert.ToBoolean(row["SCREEN_SHOW"]),
                                    UserFullName = row["USER_FULLNAME"].ToString(),
                                    ModifiedOn = Convert.ToDateTime(row["ModifiedOn"]).ToString("yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture),
                                });
                            }

                            return Ok(await Result<List<UserMailBoxVM>>.SuccessAsync(final));
                        }
                        return Ok(resulDatatable);
                    }
                }
                return Ok(chkEnable);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("GetUserNotifications")]
        public async Task<IActionResult> GetUserNotifications()
        {
            try
            {
                var getNotifs = await mainServiceManager.SysNotificationService.GetTopVw(); 
                if (getNotifs.Succeeded)
                {
                    List<UserNotificationsVM> result = new();
                    foreach (var item in getNotifs.Data)
                    {
                        result.Add(new UserNotificationsVM
                        {
                            Id = item.Id,
                            MsgTxt = item.MsgTxt,
                            Url = item.Url,
                            UserFullname = item.UserFullname,
                            CreatedOn = item.CreatedOn != null ? item.CreatedOn.Value.ToString("yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture) : "",
                        });
                    }


                    return Ok(await Result<List<UserNotificationsVM>>.SuccessAsync(result));
                }
                return Ok(getNotifs);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("ReadNotification")]
        public async Task<IActionResult> ReadNotification(long id)
        {
            try
            {
                var read = await mainServiceManager.SysNotificationService.ReadNotification(id);
                return Ok(await Result.SuccessAsync());
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("ReadAllNotifications")]
        public async Task<IActionResult> ReadAllNotifications(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return Ok(await Result.SuccessAsync());

                var read = await mainServiceManager.SysNotificationService.ReadAllNotifications(id);
                return Ok(await Result.SuccessAsync());
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("GetUserTasks")]
        public async Task<IActionResult> GetUserTasks()
        {
            try
            {
                var getTasks = await tsServiceManager.TsTaskService.GetAllVW(t => t.StatusId != 4 && t.StatusId != 5
                    && t.StatusId != 6
                    && t.AssigneeToUserId != null
                    && t.Isdel == false);

                if (getTasks.Succeeded)
                {
                    var taskData = getTasks.Data.Where(x => !string.IsNullOrEmpty(x.AssigneeToUserId) && x.AssigneeToUserId.Split(',').Contains(currentData.UserId.ToString()));
                    var res = taskData.OrderBy(t => t.Priority).ToList();

                    List<UserTasksVM> final = new();
                    foreach (var task in res)
                    {
                        if (!string.IsNullOrEmpty(task.SendDate) && !string.IsNullOrEmpty(task.DueDate))
                        {
                            var send = DateHelper.StringToDate(task.SendDate); //تاريخ التكليف
                            var due = DateHelper.StringToDate(task.DueDate); //تاريخ الانجاز المفترض

                            int countDays = (due - send).Days; //days count for the task
                            int leftDays = (DateHelper.GetCurrentDateTime() - send).Days; //count left days from assigning date to now

                            double Percent = 0.0;
                            if (countDays > 0)
                            {
                                Percent = Math.Round((double)((leftDays * 100) / countDays), 2); //if leftDays > countDays, the Percent will be > 100,, that means the bar will be completely shaded
                            }

                            final.Add(new UserTasksVM
                            {
                                Id = task.Id,
                                Subject = task.Subject,
                                DueDate = task.DueDate,
                                UserFullname = task.UserFullname,
                                Percent = Percent,
                            });
                        }
                    }
                    return Ok(await Result<List<UserTasksVM>>.SuccessAsync(final));
                }

                return Ok(getTasks);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpGet("GetUserFavorite")]
        public async Task<IActionResult> GetFavorite()
        {
            try
            {
                var getFavs = await mainServiceManager.SysFavMenuService.GetAll(s => s.UserId == currentData.UserId);
                if (getFavs.Succeeded)
                {
                    var res = getFavs.Data.OrderBy(x => x.SortNo).ToList();

                    List<SysFavMenuVM> final = new();
                    foreach (var item in res)
                    {
                        final.Add(new SysFavMenuVM
                        {
                            Title = item.Title,
                            Url = item.Url
                        });
                    }
                    return Ok(await Result<List<SysFavMenuVM>>.SuccessAsync(final));
                }
                return Ok(getFavs);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpPost("AddToFavorite")]
        public async Task<ActionResult> AddToFavorite(string title, string url)
        {
            try
            {
                SysFavMenuDto obj = new()
                {
                    UserId = currentData.UserId,
                    Title = title,
                    Url = url,
                    SortNo = 0
                };

                var update = await mainServiceManager.SysFavMenuService.Add(obj);
                return Ok(update);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }


        [HttpPost("SaveSysUserTracking")]
        public async Task<ActionResult> SaveSysUserTracking(string url)
        {
            try
            {
                var add = await mainServiceManager.SysUserTrackingService.Add(new SysUserTrackingDto { Url = url });
                return Ok(add);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        [HttpGet("ChangeLanguage")]
        public async Task<IActionResult> ChangeLanguage()
        {
            try
            {
                var res = await mainServiceManager.SysUserService.GetOneVW(u => u.UserId == currentData.UserId && u.Enable == 1 && u.Isdel == false && u.IsAgree == true);
                if (res.Succeeded)
                {
                    var user = res.Data;
                    var secret = configuration["IntegrationConfig:IntegrationTokenKey"];
                    var oldBaseUrl = configuration["IntegrationConfig:OldBaseUrl"];
                    var coreBaseUrl = configuration["IntegrationConfig:CoreBaseUrl"];

                    int groupId = 0;

                    if (!string.IsNullOrEmpty(user.GroupsId))
                    {
                        var hasGroup = int.TryParse(user.GroupsId, out groupId);
                    }

                    if (!string.IsNullOrEmpty(secret))
                    {
                        string CalendarType = currentData.CalendarType;
                        long currFinYear = currentData.FinYear;
                        int currFinYearGregorian = currentData.FinyearGregorian;
                        long currPeriodId = currentData.PeriodId;

                        int currLang = currentData.Language;
                        if (currLang == 1)
                        {
                            currLang = 2;
                        }
                        else
                        {
                            currLang = 1;
                        }

                        var token = authService.GetJWTToken(user, secret, currFinYear, currPeriodId, currLang, currFinYearGregorian, CalendarType);
                        return Ok(await Result<string>.SuccessAsync(token, $""));
                    }
                }

                return Ok(await Result<string>.FailAsync($"======= Exp in faild :"));
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"======= Exp in ChangeLanguage: {ex.Message}"));
            }
        }


        [HttpGet("GetSysScreens")]
        public async Task<IActionResult> GetSysScreens(int sysId)
        {
            try
            {
                var getSys = await mainServiceManager.SysSystemService.GetById(sysId);
                if (getSys.Succeeded)
                {
                    var screens = await mainServiceManager.SysSystemService.GetUserScreens(currentData.UserId, sysId);
                    return Ok(await Result<object>.SuccessAsync(new { system = getSys.Data, screens = screens.Data }, ""));
                }
                return Ok(getSys);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCustomReportMenu")]
        public async Task<IActionResult> GetCustomReportMenu(int sysId)
        {
            try
            {


                var sysGroupId = currentData.GroupId;
                var reports = await mainServiceManager.SysSystemService.GetCustomReportMenu(sysId, sysGroupId.ToString());

                return Ok(reports);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("ChangeFinYear")]
        public async Task<IActionResult> ChangeFinYear(long finYear)
        {
            try
            {
                // Validate input
                if (finYear == 0)
                    return Ok(await Result<string>.FailAsync("Invalid financial year provided."));

                // Fetch the user details from the database
                var userResponse = await mainServiceManager.SysUserService.GetOneVW(
                    u => u.UserId == currentData.UserId && u.Enable == 1 && u.Isdel == false && u.IsAgree == true);

                if (!userResponse.Succeeded)
                    return Ok(await Result<AuthModel>.FailAsync($"{userResponse.Status.message}"));

                var user = userResponse.Data;

                // Fetch configuration values for token and base URLs
                var secret = configuration["IntegrationConfig:IntegrationTokenKey"];
                var oldBaseUrl = configuration["IntegrationConfig:OldBaseUrl"];
                var coreBaseUrl = configuration["IntegrationConfig:CoreBaseUrl"];
                if (string.IsNullOrEmpty(secret))
                    return BadRequest("Missing secret key for token generation.");

                // Get the financial year details
                var financialYearResponse = await accServiceManager.AccFinancialYearService.GetOne(f => f.FinYear == finYear);
                if (!financialYearResponse.Succeeded)
                    return Ok(await Result<string>.FailAsync("Financial year not found."));

                var currFinYearGregorian = financialYearResponse.Data.FinYearGregorian;
                var currFinYear = financialYearResponse.Data.FinYear;

                // Get the current period ID
                var periodsResponse = await accServiceManager.AccPeriodsService.GetAll();
                var currPeriodId = periodsResponse.Succeeded
                    ? periodsResponse.Data.FirstOrDefault(x => x.FlagDelete == false && x.PeriodState == 1 && x.FinYear == currFinYear)?.PeriodId ?? 0
                    : 0;

                // Get the calendar type configuration
                var calendarType = await configurationHelper.GetValue(19, currentData.FacilityId) ?? "1";


                // Generate the JWT token
                var currLang = currentData.Language;
                var token = authService.GetJWTToken(user, secret, currFinYear, currPeriodId, currLang, currFinYearGregorian, calendarType);

                // Prepare the auth model
                var auth = new AuthModel
                {
                    EmpId = user.EmpId ?? 0,
                    EmpCode = user.EmpCode,
                    FacilityId = user.FacilityId ?? 0,
                    GroupId = int.Parse(user.GroupsId ?? "0"),
                    UserId = user.UserId,
                    Username = user.UserName,
                    UserEmail = user.UserEmail,
                    UserFullName = user.UserFullname,
                    UserFullName2 = user.EmpName2,
                    CoreBaseUrl = coreBaseUrl,
                    OldBaseUrl = oldBaseUrl,
                    FinYear = currFinYear,
                    PeriodId = currPeriodId,
                    Language = currLang,
                    CalendarType = calendarType,
                    Token = token,
                    FinyearGregorian = currFinYearGregorian,
                    LastLogin = user.LastLogin?.ToString("yyyy/MM/dd h:mm:ss tt", CultureInfo.InvariantCulture) ?? "",
                    SalesType = user.SalesType,
                    Location = user.Location,
                    DeptId = user.DeptId,
                    isAgree = user.IsAgree
                };

                // Get facility details and enrich the auth model
                var facilitiesResponse = await mainServiceManager.SysSystemService.GetFacilities();
                if (facilitiesResponse.Succeeded)
                {
                    var facility = facilitiesResponse.Data.FirstOrDefault(f => f.FacilityId == currentData.FacilityId);
                    if (facility != null)
                    {
                        auth.CompAddress = facility.FacilityAddress;
                        auth.CompLogo = facility.FacilityLogo;
                        auth.CompLogoPrint = facility.LogoPrint;
                        auth.CompPhone = facility.FacilityPhone;
                        auth.CompName = facility.FacilityName;
                        auth.CompName2 = facility.FacilityName2;
                        auth.CompImgFooter = facility.ImgFooter;
                        auth.CompVatNumber = facility.VatNumber;
                    }
                }

                return Ok(await Result<AuthModel>.SuccessAsync(auth, "Financial year updated successfully."));
            }
            catch (Exception ex)
            {
                return Ok(await Result<AuthModel>.FailAsync($"Error while updating financial year: {ex.Message}"));
            }
        }


        [HttpPost("SwitchUserCompany")]
        public async Task<IActionResult> SwitchUserCompany(long userId)
        {
            try
            {
                // Validate input
                if (userId == 0)
                    return Ok(await Result<string>.FailAsync("Invalid user or facility provided."));

                // Fetch the new user details
                var userResponse = await mainServiceManager.SysUserService.GetOneVW(
                    u => u.UserId == userId && u.Enable == 1 && u.Isdel == false && u.IsAgree == true);

                if (!userResponse.Succeeded)
                    return Ok(await Result<AuthModel>.FailAsync($"{userResponse.Status.message}"));

                var user = userResponse.Data;
                var facilityId = user.FacilityId ?? 0;
                // Fetch configuration values for token and base URLs
                var secret = configuration["IntegrationConfig:IntegrationTokenKey"];
                var oldBaseUrl = configuration["IntegrationConfig:OldBaseUrl"];
                var coreBaseUrl = configuration["IntegrationConfig:CoreBaseUrl"];

                if (string.IsNullOrEmpty(secret))
                    return BadRequest("Missing secret key for token generation.");

                // Get the current language and calendar type
                var currLang = currentData.Language;
                var calendarType = await configurationHelper.GetValue(19, facilityId) ?? "1";

                // Generate the JWT token for the new user and company
                var token = authService.GetJWTToken(user, secret, currentData.FinYear, currentData.PeriodId, currLang, currentData.FinyearGregorian, calendarType);


                // Prepare the auth model
                var auth = new AuthModel
                {
                    EmpId = user.EmpId ?? 0,
                    EmpCode = user.EmpCode,
                    FacilityId = facilityId,
                    GroupId = int.Parse(user.GroupsId ?? "0"),
                    UserId = user.UserId,
                    Username = user.UserName,
                    UserEmail = user.UserEmail,
                    UserFullName = user.UserFullname,
                    UserFullName2 = user.EmpName2,
                    CoreBaseUrl = coreBaseUrl,
                    OldBaseUrl = oldBaseUrl,
                    FinYear = currentData.FinYear,
                    PeriodId = currentData.PeriodId,
                    Language = currLang,
                    CalendarType = calendarType,
                    Token = token,
                    FinyearGregorian = currentData.FinyearGregorian,
                    LastLogin = user.LastLogin?.ToString("yyyy/MM/dd h:mm:ss tt", CultureInfo.InvariantCulture) ?? "",
                    SalesType = user.SalesType,
                    Location = user.Location,
                    DeptId = user.DeptId,
                    isAgree = user.IsAgree

                };

                // Get facility details and enrich the auth model
                var facilitiesResponse = await mainServiceManager.SysSystemService.GetFacilities();
                if (facilitiesResponse.Succeeded)
                {
                    var facility = facilitiesResponse.Data.FirstOrDefault(f => f.FacilityId == facilityId);
                    if (facility != null)
                    {
                        auth.CompAddress = facility.FacilityAddress;
                        auth.CompLogo = facility.FacilityLogo;
                        auth.CompLogoPrint = facility.LogoPrint;
                        auth.CompPhone = facility.FacilityPhone;
                        auth.CompName = facility.FacilityName;
                        auth.CompName2 = facility.FacilityName2;
                        auth.CompImgFooter = facility.ImgFooter;
                        auth.CompVatNumber = facility.VatNumber;
                    }
                }

                return Ok(await Result<AuthModel>.SuccessAsync(auth, "User and company switched successfully."));
            }
            catch (Exception ex)
            {
                return Ok(await Result<AuthModel>.FailAsync($"Error while switching user and company: {ex.Message}"));
            }
        }

        [HttpGet("GetPropertyValue")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyValue(long id, long FacilityId = 1)
        {
            string Value = await configurationHelper.GetValue(id, FacilityId);

            return Ok(await Result<string>.SuccessAsync(Value));
        }

        [HttpGet("GetLoginProperties")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoginProperties(long FacilityId = 1)
        {
            List<long> propertyIds = new() { 103, 125, 126, 204, 227, 353, 405, 432 };
            var properties = await mainServiceManager.SysPropertyValueService.GetAll(x => propertyIds.Contains(x.PropertyId ?? 0)
                && x.FacilityId == FacilityId);
            if (properties.Succeeded && properties.Data.Any())
            {
                Dictionary<long, string> propertiesDictionary = properties.Data.ToDictionary(p => p.PropertyId ?? 0, p => p.PropertyValue ?? "");
                return Ok(await Result<Dictionary<long, string>>.SuccessAsync(propertiesDictionary));
            }
            return Ok(await Result.FailAsync());
        }

        #region جلب الاشعارات في القائمة المنبثقة في النظام الرئيسي


        [HttpGet("GetUserPopUpNotifications")]
        public async Task<IActionResult> GetUserPopUpNotifications()
        {
            try
            {

                var result = await mainServiceManager.SysNotificationsMangService.GetNotificationsByUserAndGroupAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(GetUserPopUpNotifications)}: {ex.Message}"));
            }
        }
        [HttpGet("GetAnnouncements")]
        public async Task<IActionResult> GetAnnouncements()
        {
            try
            {
                var FacilityId = currentData.FacilityId;
                var BranchId = currentData.BranchId;
                var DeptId = currentData.DeptId;
                var DeptLocationId = currentData.LocationId;

                var announcements = await mainServiceManager.SysAnnouncementService.GetAllVW(e =>
                    e.IsDeleted == false &&
                    new[] { 1, 3 }.Contains(e.LocationId ?? 0) &&
                    e.IsActive == true &&
                    (FacilityId == 0 || e.FacilityId == FacilityId) &&
                    (BranchId == 0 || e.BranchId == BranchId) &&
                    (DeptId == 0 || e.DeptId == DeptId) &&
                    (DeptLocationId == 0 || e.DeptLocationId == DeptLocationId));

                if (!announcements.Succeeded)
                    return Ok(await Result<object>.FailAsync(announcements.Status.message));

                if (!announcements.Data.Any())
                    return Ok(await Result<object>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                var currentDate = DateHelper.GetCurrentDateTime();
                var filteredResults = announcements.Data
                    .Where(e => DateHelper.StringToDate(e.StartDate) <= currentDate
                             && DateHelper.StringToDate(e.EndDate) >= currentDate)
                    .OrderByDescending(e => DateHelper.StringToDate(e.PublishDate))
                    .ThenByDescending(e => e.Id)
                    .Take(3)
                    .ToList();

                return Ok(await Result<object>.SuccessAsync(filteredResults, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in {this.GetType()}: {ex.Message}"));
            }
        }

        #endregion  

        // جلب البيانات الى صفحة تسجيل الدخول 
        [HttpGet("GetLoginPageData")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLoginPageData(long FacilityId = 1)
        {
            try
            {
                var test = Bahsas.HDateNow3(currentData);
                var CurrentVersion = "";
                IList<object> Slides = new List<object>();
                var SystemName = "";

                // Get the latest version
                var GetVersion = await mainServiceManager.SysVersionService.GetAll();
                if (GetVersion.Succeeded && GetVersion.Data != null && GetVersion.Data.Any())
                {
                    CurrentVersion = GetVersion.Data
                        .OrderByDescending(v => v.Id)
                        .Select(v => v.VersionNo)
                        .FirstOrDefault();
                }

                // Get active slides
                var GetSlides = await mainServiceManager.SysAnnouncementService.GetAllVW(x => x.LocationId == 6 && x.IsDeleted == false && x.IsActive == true && x.FacilityId == FacilityId);
                if (GetSlides.Succeeded && GetSlides.Data != null && GetSlides.Data.Any())
                {
                    var CurrentDate = DateHelper.GetCurrentDateTime();

                    foreach (var slide in GetSlides.Data)
                    {
                        if (!string.IsNullOrEmpty(slide.StartDate) && !string.IsNullOrEmpty(slide.EndDate))
                        {
                            var startDate = DateHelper.StringToDate(slide.StartDate);
                            var endDate = DateHelper.StringToDate(slide.EndDate);

                            if (CurrentDate >= startDate && CurrentDate <= endDate)
                            {
                                Slides.Add(new { slide.Id, AttachFile = slide.AttachFile?.Replace("~", string.Empty), slide.Subject });
                            }
                        }
                    }
                }

                // Get System Name
                var GetSystemName = await configurationHelper.GetValue(213, FacilityId);
                if (!string.IsNullOrEmpty(GetSystemName))
                {
                    SystemName = GetSystemName;
                }
                var result = new { CurrentVersion, Slides, SystemName };
                return Ok(await Result<object>.SuccessAsync(result, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(GetLoginPageData)}: {ex.Message}"));
            }
        }

        // ايقاف تسجيل الدخول على النظام بعد عدد محاولات
        [HttpPost("DisableUserByUserName")]
        [AllowAnonymous]
        public async Task<IActionResult> DisableUserByUserName(string UserName, int language = 1)
        {
            try
            {
                var res = await mainServiceManager.SysUserService.DisableUserByUserName(UserName, language);
                return Ok(res);

            }
            catch (Exception ex)
            {

                return Ok(await Result.FailAsync($"Error in DisableUserByUserName: {ex.Message}"));
            }

        }

        [HttpGet("GetSystemsNotificationCount")]
        public async Task<IActionResult> GetSystemsNotificationCount()
        {
            try
            {
                var result = new List<object>();

                result.Add(await GetWfNotifications());
                // جلب الموظفين تحت الإجراء
                result.Add(await GetHrNotifications());
                // جلب القيود الغير مرحلة
                result.Add(await GetAccNotifications());
                // مهامي
                result.Add(await GetTaskNotifications());

                result.Add(await GetCustomerPortalNotifications());
                //جلب عدد المراسلات الغير مقرؤة  
                result.Add(await GetChatNotifications());
                // جلب عدد التقييم للمنافسات
                result.Add(await GetSupplierPortalNotifications());

                return Ok(await Result<object>.SuccessAsync(result, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"Error in {nameof(GetSystemsNotificationCount)}: {ex.Message}"));
            }
        }



        #region ForgetPassword


        [HttpGet("CheckUserName")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUserName(string userName, int language = 1)
        {
            try
            {
                // Query to find the user
                var userExists = await mainServiceManager.SysUserService.GetOneVW(
                    u => u.UserName == userName
                         && u.IsDeleted == false
                         && u.Isdel == false
                         && u.UserTypeId == 1
                         && u.Enable == 1
                );

                // Handle result
                if (userExists.Data == null)
                {
                    var message = language == 1
                        ? "اسم المستخدم غير صحيح "
                        : "Incorrect username";

                    return Ok(await Result.FailAsync(message));
                }

                return Ok(await Result.SuccessAsync("User exists."));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Error in {nameof(CheckUserName)}: {ex.Message}"));
            }
        }

        [HttpPost("CheckUserEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUserEmail(ResetPasswordByEmail request)
        {
            try
            {
                request.Language ??= 1;
                var CheckEmail = await mainServiceManager.SysUserService.GetOneVW(
                    u => u.UserName == request.UserName
                    && u.UserEmail == request.Email
                    && u.IsDeleted == false
                    && u.Isdel == false
                    && u.Enable == 1
                );

                if (CheckEmail.Data == null)
                {
                    var message = request.Language == 1
                        ? "البريد الإلكتروني غير صحيح "
                        : "Incorrect Email";

                    return Ok(await Result.FailAsync(message));
                }

                return Ok(await Result<object>.SuccessAsync(new
                {
                    CheckEmail.Data.UserEmail,
                }));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Error in {nameof(CheckUserEmail)}: {ex.Message}"));
            }
        }


        [HttpPost("CheckUserMobile")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUserMobile(ResetPasswordByMobile request)
        {
            try
            {
                request.Language ??= 1;
                // Validate input parameters to avoid unnecessary database calls
                if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Mobile))
                {
                    var message = request.Language == 1
                        ? "اسم المستخدم أو رقم الجوال مطلوب"
                        : "Username or mobile number is required";
                    return BadRequest(await Result.FailAsync(message));
                }

                // Step 1: Fetch user data from SYS_USER_VW
                var userResult = await mainServiceManager.SysUserService.GetOneVW(
                    u => u.IsDeleted == false
                         && u.Isdel == false
                         && u.Enable == 1
                         && u.UserName == request.UserName
                );

                if (!userResult.Succeeded || userResult.Data == null)
                {
                    var message = request.Language == 1
                        ? "رقم الجوال او اسم المستخدم غير صحيح "
                        : "Mobile number or username is incorrect";
                    return Ok(await Result.FailAsync(message));
                }

                // Step 2: Fetch employee data from INVEST_Employee
                var empResult = await mainServiceManager.InvestEmployeeService.GetOne(
                    e => e.Mobile == request.Mobile && e.Id == userResult.Data.EmpId
                );

                if (!empResult.Succeeded || empResult.Data == null)
                {
                    var message = request.Language == 1
                        ? "رقم الجوال او اسم المستخدم غير صحيح "
                        : "Mobile number or username is incorrect";
                    return Ok(await Result.FailAsync(message));
                }

                // Step 3: Return user data if match is found
                return Ok(await Result<object>.SuccessAsync(new
                {
                    userResult.Data.UserName,
                    Mobile = empResult.Data.Mobile
                }));
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return Ok(await Result.FailAsync($"Error in {nameof(CheckUserMobile)}: {ex.Message}"));
            }
        }


        [HttpPost("SendCodeByMobile")]
        [AllowAnonymous]
        public async Task<IActionResult> SendCodeByMobile(ResetPasswordByMobile request)
        {
            try
            {
                var res = await mainServiceManager.SysResetPasswordService.SendVerificationCodeByMobile(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Error in SendCodeByMobile: {ex.Message}"));
            }
        }



        [HttpPost("SendCodeByEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> SendCodeByEmail(ResetPasswordByEmail request)
        {
            try
            {
                var res = await mainServiceManager.SysResetPasswordService.SendVerificationCodeByEmail(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Error in SendCodeByEmail: {ex.Message}"));
            }
        }

        [HttpPost("ValidateVerificationCode")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateVerificationCode(ValidateVerificationCodeDto request)
        {
            try
            {
                int Language = 1;

                request.Language ??= 1;
                Language = (int)request.Language;
                if (string.IsNullOrWhiteSpace(request.UserName))
                {
                    var message = request.Language == 1
                        ? "اسم المستخدم مطلوب"
                        : "Username  is required";
                    return Ok(await Result<object>.FailAsync(message));
                }

                if (string.IsNullOrWhiteSpace(request.VerificationCode))
                {
                    var message = request.Language == 1
                        ? "رمز التحقق مطلوب"
                        : "Verification Code  is required";
                    return Ok(await Result<object>.FailAsync(message));
                }
                if (!new[] { 1, 2 }.Contains(request.VerificationType))
                {
                    var message = request.Language == 1
                        ? "نوع التحقق مطلوب"
                        : "Verification Type is required";
                    return Ok(await Result<object>.FailAsync(message));
                }


                var res = await mainServiceManager.SysResetPasswordService.ValidateVerificationCode(request.VerificationCode, request.VerificationType, request.UserName, Language);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Error in ValidateVerificationCode: {ex.Message}"));
            }
        }


        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto request)
        {
            try
            {
                int Language = 1;

                request.Language ??= 1;
                Language = (int)request.Language;
                if (string.IsNullOrWhiteSpace(request.UserName))
                {
                    var message = request.Language == 1
                        ? "اسم المستخدم مطلوب"
                        : "Username  is required";
                    return Ok(await Result<object>.FailAsync(message));
                }

                if (string.IsNullOrWhiteSpace(request.VerificationCode))
                {
                    var message = Language == 1
                        ? "رمز التحقق مطلوب"
                        : "Verification Code  is required";
                    return Ok(await Result<object>.FailAsync(message));
                }
                if (string.IsNullOrWhiteSpace(request.passWord))
                {
                    var message = Language == 1
                        ? "كلمة السر مطلوبة"
                        : "passWord is required";
                    return Ok(await Result<object>.FailAsync(message));
                }

                var res = await mainServiceManager.SysResetPasswordService.ResetPassword(request.VerificationCode, request.passWord, request.UserName, Language);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Error in ResetPassword: {ex.Message}"));
            }
        }

        #endregion

        #region CreateUserRequst




        [HttpPost("GetDataForCreateUserRequst")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDataForCreateUserRequst(GetDataForCreateUserRequstDto request)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(request.IdNo))
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("BirthDate")}"));
                if (string.IsNullOrWhiteSpace(request.BirthDate))
                    return Ok(await Result<object>.FailAsync($"{localization.GetHrResource("PersonID")}"));

                var result = await mainServiceManager.InvestEmployeeService.GetOne(e => e.IdNo == request.IdNo && e.BirthDate == request.BirthDate && e.Isdel == false && e.IsDeleted == false && e.StatusId == 1);
                if (!result.Succeeded)
                    return Ok(await Result<object>.FailAsync($"تاكد من البيانات المدخلة  الهوية و تاريخ الميلاد "));

                var EmpData = new CreateUserRequstDto
                {
                    Email = result.Data.Email,
                    Mobile = result.Data.Mobile,
                    UserName = result.Data.EmpId ?? "",
                    Name = result.Data.EmpName ?? "",
                    BirthDate = result.Data.BirthDate,
                    IdNo = request.IdNo,


                };
                return Ok(await Result<object>.SuccessAsync(EmpData, ""));
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Error in GetDataForCreateUserRequst: {ex.Message}"));
            }
        }


        [HttpPost("CreateUserRequst")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUserRequst(CreateUserRequstDto request)
        {
            try
            {

                if (request.CheckApprove == false)
                    return Ok(await Result<object>.FailAsync(" اوافق على لائحة الاستخدام "));

                if (string.IsNullOrWhiteSpace(request.UserName))
                    return Ok(await Result<object>.FailAsync("اسم المستخدم "));

                if (string.IsNullOrWhiteSpace(request.FileUrl))
                    return Ok(await Result<object>.FailAsync("خطاب مباشرة العمل"));

                if (string.IsNullOrWhiteSpace(request.StringUserPassword))
                    return Ok(await Result<object>.FailAsync("كلمة المرور"));


                var res = await mainServiceManager.SysUserService.CreateUserRequst(request);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync($"Error in CreateUserRequst: {ex.Message}"));
            }
        }


        #endregion

        #region شاشة الاقرار


        [HttpGet("ApproveAgreement")]
        public async Task<IActionResult> ApproveAgreement()
        {
            try
            {
                var result = await mainServiceManager.SysUserService.ApproveAgreement();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result.FailAsync(ex.Message));
            }
        }

        #endregion

        #region دوال مساعدة

        [NonAction]
        private async Task<object> GetWfNotifications()
        {
            var chk1 = await permission.HasPermission(269, PermissionType.Show);

            if (!chk1)
            {
                return new { SystemId = 7, Count = 0 };
            }

            var filter = new WfApplicationFilterDto
            {
                UserId = currentData.UserId,
                DeptId = currentData.DeptId,
                Location = currentData.LocationId,
                SystemId = 7,
                StatusId = 0,
                ApplicationsTypeId = 0
            };

            var countApplications = await wFServiceManager.WfApplicationService.GetApplicationsByUser(filter);
            var countAppCommittees = await wFServiceManager.WfApplicationService.GetApplicationsCommitteesByUser(filter);
            var countAssignes = await wFServiceManager.WfApplicationService.GetApplicationsAssignsByUser(filter);

            return new { SystemId = 7, Count = countApplications.Data.Count() + countAppCommittees.Data.Count() + countAssignes.Data.Count() };
        }

        [NonAction]
        private async Task<object> GetHrNotifications()
        {
            var chk2 = await permission.HasPermission(601, PermissionType.Show);

            if (!chk2)
            {
                return new { SystemId = 3, Count = 0 };
            }

            var getEmployees = await hrServiceManager.HrEmployeeService.GetAllVW(e =>
                e.FacilityId == currentData.FacilityId && e.StatusId == 10 && e.Isdel == false && e.IsDeleted == false);

            return new { SystemId = 3, Count = getEmployees.Data.Count() };
        }

        [NonAction]
        private async Task<object> GetAccNotifications()
        {
            var chk3 = await permission.HasPermission(70, PermissionType.Show);

            if (!chk3)
            {
                return new { SystemId = 2, Count = 0 };
            }

            var getJournal = await accServiceManager.AccJournalMasterService.GetAllVW(e =>
                e.FacilityId == currentData.FacilityId && e.StatusId == 1 && e.FlagDelete == false && e.FinYear == currentData.FinYear);

            return new { SystemId = 2, Count = getJournal.Data.Count() };
        }

        [NonAction]
        private async Task<object> GetTaskNotifications()
        {
            var chk4 = await permission.HasPermission(780, PermissionType.Show);

            if (!chk4)
            {
                return new { SystemId = 28, Count = 0 };
            }

            var excludeStatus = new List<int> { 4, 5, 6 };
            var getTasks = await tsServiceManager.TsTaskService.GetAllVW(t =>
                t.Isdel == false && !excludeStatus.Contains(t.StatusId ?? 0));
            var filteredTasks = getTasks.Data.Where(x =>
                !string.IsNullOrEmpty(x.AssigneeToUserId) && x.AssigneeToUserId.Split(',').Contains(currentData.UserId.ToString()));

            return new { SystemId = 28, Count = filteredTasks.Count() };
        }


        [NonAction]
        private async Task<object> GetChatNotifications()
        {
            var chk5 = await permission.HasPermission(1151, PermissionType.Show);

            if (!chk5)
            {
                return new { SystemId = 41, Count = 0 };
            }

            var getChatCount = await mainServiceManager.ChatMessageService.GetAll(m =>
                m.IsRead == false && m.ReceiverId == currentData.UserId);

            return new { SystemId = 41, Count = getChatCount.Data.Count() };
        }


        [NonAction]
        private async Task<object> GetCustomerPortalNotifications()
        {
            // إدارة بوابة الموردين والمقاولين الرقمية
            int ticketCount = 0;
            int applicationsCount = 0;
            //  تذاكر
            var chkTicket = await permission.HasPermission(1127, PermissionType.Show);
            if (chkTicket)
            {
                var getTicketCount = await hDServiceManager.HdTickectService.GetAll(t =>
                    t.IsDeleted == false && t.StatusId != 4 && t.AssiginTo == currentData.UserId);
                ticketCount = getTicketCount.Data.Count();
            }
            // صندوق البريد الوارد في المشاريع مع العملاء
            var chkApplications = await permission.HasPermission(1222, PermissionType.Show);
            if (chkApplications)
            {
                var filter = new WfApplicationFilterDto
                {
                    UserId = currentData.UserId,
                    DeptId = currentData.DeptId,
                    Location = currentData.LocationId,
                    SystemId = 38,
                    StatusId = 0,
                    ApplicationsTypeId = 0
                };

                var countApplications = await wFServiceManager.WfApplicationService.GetApplicationsByUser(filter);
                applicationsCount = countApplications.Data.Count();
            }

            return new { SystemId = 39, Count = applicationsCount + ticketCount };
        }


        [NonAction]
        private async Task<object> GetSupplierPortalNotifications()
        {
            long FacilityId = currentData.FacilityId;
            int EmpId = currentData.EmpId;

            // تهيئة عدد الاشعارات
            int technicalMemberCount = 0;
            int committeeMemberCount = 0;
            int chairmanCount = 0;
            int decisionMaker1Count = 0;
            int decisionMaker2Count = 0;

            // التحقق من الصلاحيات
            bool hasTechnicalPermission = await permission.HasPermission(1888, PermissionType.Show);
            bool hasCommitteePermission = await permission.HasPermission(1889, PermissionType.Show);
            bool hasChairmanPermission = await permission.HasPermission(1899, PermissionType.Show);
            bool hasDecisionMaker1Permission = await permission.HasPermission(1890, PermissionType.Show);
            bool hasDecisionMaker2Permission = await permission.HasPermission(1891, PermissionType.Show);

            var sharedFilter = await purServiceManager.PurRqfWorkFlowEvaluationService.GetAllVW(x =>
                x.IsDeleted == false &&
                x.FacilityId == FacilityId &&
                x.TransTypeId == 8 &&
                x.RfqStutesId == 4 &&
                x.StatusId != 3
            );

            var evaluations = sharedFilter.Data;
            if (!evaluations.Any())
            {
                return new
                {
                    SystemId = 59,
                    Count = 0
                };
            }

            // عدد أعضاء الفريق الفني
            if (hasTechnicalPermission)
            {
                technicalMemberCount = evaluations.Where(x =>
                    x.CntTechnicalMembers != (x.CntTechnicalMembersEvaluation ?? 0) ||
                    (x.CntCommitteeMembersEvaluation ?? 0) == 0 &&
                    !string.IsNullOrEmpty(x.TechnicalMembersEmpId) &&
                    x.TechnicalMembersEmpId.Split(',').Contains(EmpId.ToString())
                ).Count();
            }

            // عدد أعضاء لجنة الفحص
            if (hasCommitteePermission)
            {
                committeeMemberCount = evaluations.Where(x =>
                    x.CntTechnicalMembers == (x.CntTechnicalMembersEvaluation ?? 0) &&
                    ((x.CntCommitteeMembersEvaluation ?? 0) < x.CntCommitteeMembers || (x.CommitteeChairmanEmpIdEvaluation ?? 0) == 0) &&
                    !string.IsNullOrEmpty(x.CommitteeMembersEmpId) &&
                    x.CommitteeMembersEmpId.Split(',').Contains(EmpId.ToString())
                ).Count();
            }

            // عدد رؤساء اللجان
            if (hasChairmanPermission)
            {
                chairmanCount = evaluations.Where(x =>
                    (x.CntCommitteeMembersEvaluation ?? 0) == x.CntCommitteeMembers &&
                    ((x.CommitteeChairmanEmpIdEvaluation ?? 0) == 0 || (x.DecisionMaker1EmpIdApprove ?? 0) == 0) &&
                    x.CommitteeChairmanEmpId == EmpId
                ).Count();
            }

            // عدد متخذي القرار الأول
            if (hasDecisionMaker1Permission)
            {
                decisionMaker1Count = evaluations.Where(x =>
                    x.CntCommitteeMembers == (x.CntCommitteeMembersEvaluation ?? 0) &&
                    (x.CommitteeChairmanEmpIdEvaluation ?? 0) != 0 &&
                    ((x.DecisionMaker1EmpIdApprove ?? 0) == 0 || (x.DecisionMaker2EmpIdApprove ?? 0) == 0) &&
                    x.DecisionMaker1EmpId == EmpId
                ).Count();
            }

            // عدد متخذي القرار الثاني
            if (hasDecisionMaker2Permission)
            {
                decisionMaker2Count = evaluations.Where(x =>
                    ((x.DecisionMaker1EmpIdApprove ?? 0) != 0 || (x.DecisionMaker1EmpId ?? 0) == 0) &&
                    (x.DecisionMaker2EmpIdApprove ?? 0) == 0 &&
                    (x.CommitteeChairmanEmpIdEvaluation ?? 0) != 0 &&
                    x.DecisionMaker2EmpId == EmpId
                ).Count();
            }

            return new
            {
                SystemId = 59,
                Count = (technicalMemberCount + committeeMemberCount + chairmanCount + decisionMaker1Count + decisionMaker2Count)
            };
        }


        #endregion


        #region ================== Screen Notifications Count =================
        [HttpGet("GetScreensNotificationCount")]
        public async Task<IActionResult> GetScreensNotificationCount(long SystemId)
        {
            try
            {
                // screens that have notification[59, 70, 166, 296, 297, 298, 601, 269, 1280, 1281, 1474, 773, 780, 1087, 1222]
                List<object> result = new();
                switch (SystemId)
                {
                    case 2:
                        result = await GetAccScreensNotification();
                        break;
                    case 3:
                        result = await GetHrScreensNotification();
                        break;
                    case 7:
                        result = await GetWfScreensNotification();
                        break;
                    case 28:
                        result = await GetTaskScreensNotification();
                        break;
                    case 34:
                        result = await GetFollowUpScreensNotification();
                        break;
                    case 39:
                        result = await GetHelpDeskScreensNotification();
                        break;
                    default:
                        break;
                }

                return Ok(await Result<List<object>>.SuccessAsync(result));
            }
            catch
            {
                return Ok(await Result.FailAsync());
            }
        }

        [NonAction]
        private async Task<List<object>> GetAccScreensNotification()
        {
            // screens (59, 70)
            int count = 0;
            List<object> result = new();
            try
            {
                var chk = await permission.HasPermission(70, PermissionType.Show);
                if (chk)
                {
                    var getJournal = await accServiceManager.AccJournalMasterService.GetAllVW(e =>
                    e.FacilityId == currentData.FacilityId && e.StatusId == 1 && e.FlagDelete == false && e.FinYear == currentData.FinYear);
                    count = getJournal.Data.Count();
                }

                result.Add(new { ScreenId = 59, Count = count });
                result.Add(new { ScreenId = 70, Count = count });

                return result;
            }
            catch
            {
                return result;
            }
        }

        [NonAction]
        private async Task<List<object>> GetHrScreensNotification()
        {
            // screens (166, 296, 297, 298, 601)
            bool chk = false;
            List<long> branches = currentData.Branches.Split(',').Select(long.Parse).ToList();
            int count = 0;
            List<object> result = new();
            try
            {
                chk = await permission.HasPermission(601, PermissionType.Show);
                if (chk)
                {
                    var getEmployees = await hrServiceManager.HrEmployeeService.GetAllVW(e =>
                        e.FacilityId == currentData.FacilityId && e.StatusId == 10 && e.Isdel == false && e.IsDeleted == false);
                    count = getEmployees.Data.Count();
                }

                result.Add(new { ScreenId = 166, Count = count });
                result.Add(new { ScreenId = 601, Count = count });

                count = 0;
                chk = await permission.HasPermission(296, PermissionType.Show);
                if (chk)
                {
                    //Get_Payroll_HR_Binding
                    count = await GetPayrollsCount("1,6", branches);
                }
                result.Add(new { ScreenId = 296, Count = count });

                count = 0;
                chk = await permission.HasPermission(297, PermissionType.Show);
                if (chk)
                {
                    //Get_Payroll_Fin_Binding
                    count = await GetPayrollsCount("2,7", branches);
                }
                result.Add(new { ScreenId = 297, Count = count });

                count = 0;
                chk = await permission.HasPermission(298, PermissionType.Show);
                if (chk)
                {
                    //Get_Payroll_GM_Binding
                    count = await GetPayrollsCount("3", branches);
                }
                result.Add(new { ScreenId = 298, Count = count });

                return result;
            }
            catch
            {
                return result;
            }
        }

        [NonAction]
        private async Task<List<object>> GetWfScreensNotification()
        {
            // screens (269, 1280, 1281, 1474)
            bool chk = false;
            int count = 0;
            List<object> result = new();
            try
            {
                var filter = new WfApplicationFilterDto
                {
                    UserId = currentData.UserId,
                    DeptId = currentData.DeptId,
                    Location = currentData.LocationId,
                    SystemId = 7,
                    StatusId = 0,
                    ApplicationsTypeId = 0
                };

                var countApplications = await wFServiceManager.WfApplicationService.GetApplicationsByUser(filter);
                var countAppCommittees = await wFServiceManager.WfApplicationService.GetApplicationsCommitteesByUser(filter);
                var countAssignes = await wFServiceManager.WfApplicationService.GetApplicationsAssignsByUser(filter);
                count = countApplications.Data.Count() + countAppCommittees.Data.Count() + countAssignes.Data.Count();
                result.Add(new { ScreenId = 269, Count = count });

                count = 0;
                chk = await permission.HasPermission(1281, PermissionType.Show);
                if (chk)
                {
                    count = countAssignes.Data.Count();
                }
                result.Add(new { ScreenId = 1280, Count = count });
                result.Add(new { ScreenId = 1281, Count = count });

                count = 0;
                chk = await permission.HasPermission(1474, PermissionType.Show);
                if (chk)
                {
                    var getNotifications = await hrServiceManager.HrNotificationService.GetAllVW(x => x.EmpId == currentData.EmpId
                    && x.IsDeleted == false && x.FacilityId == currentData.FacilityId && x.IsRead == false);

                    count = getNotifications.Data.Count();
                }
                result.Add(new { ScreenId = 1474, Count = count });

                return result;
            }
            catch
            {
                return result;
            }
        }

        [NonAction]
        private async Task<List<object>> GetTaskScreensNotification()
        {
            // screens (773, 780)
            bool chk = false;
            int count = 0;
            List<object> result = new();
            try
            {
                chk = await permission.HasPermission(780, PermissionType.Show);
                if (chk)
                {
                    var excludeStatus = new List<int> { 4, 5, 6 };
                    var getTasks = await tsServiceManager.TsTaskService.GetAllVW(t =>
                        t.Isdel == false && !excludeStatus.Contains(t.StatusId ?? 0));
                    var filteredTasks = getTasks.Data.Where(x =>
                        !string.IsNullOrEmpty(x.AssigneeToUserId) && x.AssigneeToUserId.Split(',').Contains(currentData.UserId.ToString()));
                    count = filteredTasks.Count();
                }
                result.Add(new { ScreenId = 773, Count = count });
                result.Add(new { ScreenId = 780, Count = count });

                return result;
            }
            catch
            {
                return result;
            }
        }

        [NonAction]
        private async Task<List<object>> GetFollowUpScreensNotification()
        {
            // screens (1087)
            bool chk = false;
            int count = 0;
            List<object> result = new();
            try
            {
                chk = await permission.HasPermission(1087, PermissionType.Show);
                if (chk)
                {
                    count = await hrServiceManager.HrVisitScheduleLocationService.GetNewVisitCount(currentData.GroupId, currentData.Branches);
                }
                result.Add(new { ScreenId = 1087, Count = count });

                return result;
            }
            catch
            {
                return result;
            }
        }

        [NonAction]
        private async Task<List<object>> GetHelpDeskScreensNotification()
        {
            // screens (1222)
            bool chk = false;
            int count = 0;
            List<object> result = new();
            try
            {
                chk = await permission.HasPermission(1222, PermissionType.Show);
                if (chk)
                {
                    var filter = new WfApplicationFilterDto
                    {
                        UserId = currentData.UserId,
                        DeptId = currentData.DeptId,
                        Location = currentData.LocationId,
                        SystemId = 38,
                        StatusId = 0,
                        ApplicationsTypeId = 0
                    };

                    var countApplications = await wFServiceManager.WfApplicationService.GetApplicationsByUser(filter);
                    count = countApplications.Data.Count();
                }
                result.Add(new { ScreenId = 1222, Count = count });

                return result;
            }
            catch
            {
                return result;
            }
        }

        [NonAction]
        private async Task<int> GetPayrollsCount(string statusIds, List<long> branches)
        {
            try
            {
                var statusList = statusIds.Split(',').Select(int.Parse).ToList();
                var getPayrolls = await hrServiceManager.HrPayrollService.GetAllVW(x => x.IsDeleted == false
                && statusList.Contains(x.State ?? 0)
                && (x.BranchId == 0 || branches.Contains(x.BranchId ?? 0)));

                int count = getPayrolls.Data.Count();
                return count;
            }
            catch
            {
                return 0;
            }
        }
        #endregion ============== End Screen Notifications Count ==============



    }
}