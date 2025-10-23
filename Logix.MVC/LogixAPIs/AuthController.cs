using Logix.Application.Common;
using Logix.Application.DTOs.Main;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Interfaces.IServices.Main;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using Logix.MVC.Helpers;
using Logix.MVC.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Novell.Directory.Ldap;

namespace Logix.MVC.LogixAPIs
{
    [Route($"api/{ApiConfig.ApiVersion}/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly IMainServiceManager mainServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IApiDDLHelper ddlHelper;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly ICurrentData session;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILocalizationService localization;

        public AuthController(IAuthService authService,
            IMainServiceManager mainServiceManager,
            IAccServiceManager accServiceManager,
            IApiDDLHelper ddlHelper,
            ISysConfigurationHelper configurationHelper,
            ICurrentData session,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localization)
        {
            this.authService = authService;
            this.mainServiceManager = mainServiceManager;
            this.accServiceManager = accServiceManager;
            this.ddlHelper = ddlHelper;
            this.configurationHelper = configurationHelper;
            this.session = session;
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
            this.localization = localization;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AuthModel>.FailAsync($"يجب ادخال البيانات بشكل صحيح"));
                }
                if (login.Language <= 0)
                {
                    return Ok(await Result<AuthModel>.FailAsync($"يجب ادخال اللغة"));
                }

                var res = await authService.Login(login);
                if (res.Succeeded)
                {
                    var user = res.Data;
                    var secret = configuration["IntegrationConfig:IntegrationTokenKey"];
                    var oldBaseUrl = configuration["IntegrationConfig:OldBaseUrl"];
                    var coreBaseUrl = configuration["IntegrationConfig:CoreBaseUrl"];
                    int SalesType = 0;
                    int groupId = 0;
                    string CalendarType = "1";
                    if (!string.IsNullOrEmpty(user.GroupsId))
                    {
                        var hasGroup = int.TryParse(user.GroupsId, out groupId);
                    }

                    if (!string.IsNullOrEmpty(secret))
                    {
                        long currFinYear = 0;
                        int currFinYearGregorian = 0;
                        var getFinYears = await mainServiceManager.SysSystemService.GetFinancialYears(user.FacilityId ?? 0);
                        if (getFinYears.Succeeded && getFinYears.Data.Any())
                        {
                            currFinYearGregorian = getFinYears.Data.Where(n => n.FacilityId == user.FacilityId && n.IsDeleted == false).Last().FinYearGregorian;
                            currFinYear = getFinYears.Data.Where(n => n.FacilityId == user.FacilityId && n.IsDeleted == false).Last().FinYear;
                        }

                        long currPeriodId = 0;

                        var getPeriodId = await accServiceManager.AccPeriodsService.GetAll();
                        if (getPeriodId.Succeeded)
                        {
                            var Period = getPeriodId.Data.Where(x => x.FlagDelete == false && x.PeriodState == 1 && x.FinYear == currFinYear).FirstOrDefault();
                            if (Period != null)
                            {
                                currPeriodId = Period.PeriodId;
                            }
                        }

                        long FacilityId = 0;
                        if (user.FacilityId > 0)
                        {
                            FacilityId = (long)user.FacilityId;
                        }
                        if (user.SalesType > 0)
                        {
                            SalesType = (int)user.SalesType;
                        }

                        // get Calendat Type
                        var getCalendarType = await configurationHelper.GetValue(19, (long)user.FacilityId);
                        if (!string.IsNullOrEmpty(getCalendarType))
                        {
                            CalendarType = getCalendarType;
                        }
                        var currLang = login.Language;

                        var token = authService.GetJWTToken(user, secret, currFinYear, currPeriodId, currLang, currFinYearGregorian, CalendarType);

                        var auth = new AuthModel
                        {
                            EmpId = user.EmpId ?? 0,
                            EmpCode = user.EmpCode,
                            FacilityId = user.FacilityId ?? 0,
                            GroupId = int.Parse(res.Data.GroupsId ?? "0"),
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
                            CalendarType = CalendarType,
                            Token = token,
                            FinyearGregorian = currFinYearGregorian,
                            LastLogin = user.LastLogin != null ? user.LastLogin.Value.ToString("yyyy/MM/dd h:mm:ss tt", CultureInfo.InvariantCulture) : "",
                            SalesType = user.SalesType,
                            Location = user.Location,
                            DeptId = user.DeptId,
                            isAgree = user.IsAgree
                        };

                        var getFacilities = await mainServiceManager.SysSystemService.GetFacilities();
                        if (getFacilities.Succeeded)
                        {
                            var facility = getFacilities.Data.Where(f => f.FacilityId == login.FacilityId).FirstOrDefault();
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

                        return Ok(await Result<AuthModel>.SuccessAsync(auth, $""));
                    }
                }

                return Ok(await Result<AuthModel>.FailAsync($"{res.Status.message}"));
            }
            catch (Exception ex)
            {
                return Ok(await Result<AuthModel>.FailAsync($"======= Exp in login: {ex.Message}"));
            }
        }


        [HttpGet("DDLFacilities")]
        public async Task<IActionResult> DDLFacilities()
        {
            try
            {
                var list = await ddlHelper.GetAnyLis<AccFacility, long>(b => b.IsDeleted == false, "FacilityId", "FacilityName");
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region New Login Logic
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var result = await authService.Logout();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Ok(await Result<object>.FailAsync($"======= Exp in login: {ex.Message}"));
            }
        }

        [HttpPost("login2")]
        public async Task<IActionResult> Login2(LoginDto login)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AuthModel>.FailAsync($"يجب ادخال البيانات بشكل صحيح"));
                }
                if (login.Language <= 0)
                {
                    return Ok(await Result<AuthModel>.FailAsync($"يجب ادخال اللغة"));
                }

                bool IsCheckAD = false;
                bool IsCheckSystem = false;
                bool LoginSuccess = false;
                SysUserVw user = new SysUserVw();
                //الخاص بالعميل    IP  الحصول على عنوان  
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                // الربط مع Active Directory
                var LoginByAD = await configurationHelper.GetValue(168, login.FacilityId);
                if (LoginByAD == "1")
                {
                    string ADPath = await configurationHelper.GetValue(167, login.FacilityId);
                    if (AuthenticateUser(ADPath, login.UserName, login.Password))
                    {
                        string EmpName = login.UserName;
                        string Email = login.UserName;

                        // جلب معلومات المستخدم من Active Directory
                        try
                        {
                            var adInfo = AuthenticateUserGetInfo(ADPath, login.UserName, login.Password);
                            if (adInfo != null)
                            {
                                // استخدام dynamic للوصول إلى الخصائص
                                dynamic info = adInfo;
                                EmpName = string.IsNullOrEmpty(info.Name) ? login.UserName : info.Name;
                                Email = string.IsNullOrEmpty(info.Email) ? login.UserName : info.Email;
                            }

                        }
                        catch (Exception)
                        {
                            // تجاهل الأخطاء هنا لأغراض التبسيط
                        }

                        // تسجيل الدخول بواسطة Active Directory في النظام
                        var checkLoginByUserAD1 = await authService.LoginByUserAD(login.UserName, login.FacilityId);

                        if (checkLoginByUserAD1.Data.Enable == 1)
                        {
                            user = checkLoginByUserAD1.Data;

                            IsCheckSystem = true;
                        }
                        else
                        {
                            string PasswordNew = "P@ss0rd@#";
                            string ADBranch_ID = await configurationHelper.GetValue(170, login.FacilityId);
                            string ADGroup = await configurationHelper.GetValue(169, login.FacilityId);
                            await authService.CreateUser(login.UserName, login.UserName, EmpName, login.FacilityId, int.Parse(ADBranch_ID), Email, PasswordNew, ADGroup);
                            var checkLoginByUserAD2 = await authService.LoginByUserAD(login.UserName, login.FacilityId);
                            if (checkLoginByUserAD2.Data.Enable == 1)
                            {
                                user = checkLoginByUserAD2.Data;
                                IsCheckSystem = true;
                            }
                        }
                        IsCheckAD = true;
                    }
                    if (!IsCheckAD)
                    {
                        var res = await authService.Login2(login);
                        if (res.Succeeded && res.Data != null)
                        {
                            user = res.Data;
                            IsCheckSystem = true;
                        }
                        else
                        {
                            return Ok(await Result<AuthModel>.FailAsync("غير مصرح"));

                        }
                    }
                }
                else
                {
                    var res = await authService.Login2(login);
                    if (res.Succeeded && res.Data != null)
                    {
                        user = res.Data;
                        IsCheckSystem = true;
                    }
                    else
                    {
                        return Ok(await Result<AuthModel>.FailAsync("غير مصرح"));

                    }
                }

                if (IsCheckAD || IsCheckSystem)
                {
                    LoginSuccess = true;
                }

                if (LoginSuccess)
                {
                    if (user.UserTypeId == 1)
                    {
                        if (user.TwoFactor == true)
                        {
                            var DoTwoFactor = await authService.OTP(user);
                            if (!DoTwoFactor.Succeeded)
                                return Ok(await Result<AuthModel>.FailAsync(DoTwoFactor.Status.message));

                            // isTwoFactorActive= true     يحوي  AuthModel بمعنى تم ارسال رمز  لذلك سنرجع اليه 
                            if (DoTwoFactor.Data == true)
                            {
                                var result = new AuthModel();
                                result.isTwoFactorActive = true;
                                return Ok(await Result<AuthModel>.SuccessAsync(result, "تم ارسال رمز التحقق بخطوتين"));

                            }

                        }

                    }
                    // تحديث سجل وقت تسجيل المستخدم
                    var updateLogTimeResult = await authService.UpdateUSERLogTime(user.UserId, ipAddress);
                    if (!updateLogTimeResult.Succeeded)
                    {
                        return Ok(await Result<AuthModel>.FailAsync("فشل في تحديث سجل وقت تسجيل المستخدم"));
                    }

                    // استدعاء دالة InsertTask بعد تحديث سجل وقت تسجيل المستخدم
                    var insertTaskResult = await authService.InsertTask(user.UserId, user.FacilityId ?? 1);
                    if (!insertTaskResult.Succeeded)
                    {
                        return Ok(await Result<AuthModel>.FailAsync("فشل في إدراج المهام المجدولة للمستخدم"));
                    }

                    return await GenerateAuthModel(user, login.FacilityId, login.Language);

                }

                return Ok(await Result<AuthModel>.FailAsync("غير مصرح"));
            }
            catch (Exception ex)
            {
                return Ok(await Result<AuthModel>.FailAsync($"======= Exp in login: {ex.Message}"));
            }
        }

        [HttpPost("loginWithAuthenticationCode")]
        public async Task<IActionResult> LoginWithAuthenticationCode(LoginWithAuthenticationCodeDto login)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AuthModel>.FailAsync("يجب ادخال البيانات بشكل صحيح"));
                }

                if (login.Language <= 0)
                {
                    return Ok(await Result<AuthModel>.FailAsync("يجب ادخال اللغة"));
                }
                //الخاص بالعميل    IP  الحصول على عنوان  
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
                var res = await authService.loginWithAuthenticationCode(login);
                if (!res.Succeeded || res.Data == null)
                {
                    return Ok(await Result<AuthModel>.FailAsync(res.Status.message));
                }

                var user = res.Data;

                // الربط مع Active Directory
                var loginByAD = await configurationHelper.GetValue(168, login.FacilityId);
                if (loginByAD == "1")
                {
                    var checkLoginByUserAD = await authService.LoginByUserAD(login.UserName, login.FacilityId);
                    if (!checkLoginByUserAD.Succeeded)
                    {
                        return Ok(await Result<AuthModel>.FailAsync(checkLoginByUserAD.Status.message));
                    }
                }

                // تحديث سجل وقت تسجيل المستخدم
                var updateLogTimeResult = await authService.UpdateUSERLogTime(user.UserId, ipAddress);
                if (!updateLogTimeResult.Succeeded)
                {
                    return Ok(await Result<AuthModel>.FailAsync("فشل في تحديث سجل وقت تسجيل المستخدم"));
                }

                // استدعاء دالة InsertTask بعد تحديث سجل وقت تسجيل المستخدم
                var insertTaskResult = await authService.InsertTask(user.UserId, user.FacilityId ?? 1);
                if (!insertTaskResult.Succeeded)
                {
                    return Ok(await Result<AuthModel>.FailAsync("فشل في إدراج المهام المجدولة للمستخدم"));
                }

                return await GenerateAuthModel(user, login.FacilityId, login.Language);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AuthModel>.FailAsync($"======= Exp in loginWithAuthenticationCode: {ex.Message}"));
            }
        }
        [HttpPost("ResendCode")]
        public async Task<IActionResult> ResendCode(LoginResendCodeDto login)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return Ok(await Result<AuthModel>.FailAsync("يجب ادخال البيانات بشكل صحيح"));
                }

                if (login.Language <= 0)
                {
                    return Ok(await Result<AuthModel>.FailAsync("يجب ادخال اللغة"));
                }

                var res = await authService.ResendCode(login);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return Ok(await Result<AuthModel>.FailAsync($"======= Exp in ResendCode: {ex.Message}"));
            }
        }

        [NonAction]
        private bool AuthenticateUser(string ldapPath, string user, string pass)
        {
            try
            {
                Uri ldapUri = new Uri(ldapPath);
                string ldapHost = ldapUri.Host;
                int ldapPort = ldapUri.Port == -1 ? LdapConnection.DefaultPort : ldapUri.Port;

                // Construct the DN (Distinguished Name) from the username
                // Adjust to match your LDAP DN structure
                string userDn = $"uid={user},dc=example,dc=com";

                using (var connection = new LdapConnection())
                {
                    // Connect to the LDAP server
                    connection.Connect(ldapHost, ldapPort);

                    // Bind with the constructed DN and provided password
                    connection.Bind(userDn, pass);

                    // Check if connected and bound successfully
                    if (connection.Bound)
                    {
                        Console.WriteLine("Authentication successful.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Authentication failed.");
                        return false;
                    }
                }
            }
            catch (LdapException ex)
            {
                // Log LdapException details
                Console.WriteLine($"LdapException: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Log general exception details
                Console.WriteLine($"Exception: {ex.Message}");
                return false;
            }
        }



        [NonAction]
        private async Task<IActionResult> GenerateAuthModel(SysUserVw user, long facilityId, int currLang, int IsAzureAuthenticated = 0)
        {
            var secret = configuration["IntegrationConfig:IntegrationTokenKey"];
            var oldBaseUrl = configuration["IntegrationConfig:OldBaseUrl"];
            var coreBaseUrl = configuration["IntegrationConfig:CoreBaseUrl"];
            int groupId = 0;
            string CalendarType = "1";
            if (!string.IsNullOrEmpty(user.GroupsId))
            {
                var hasGroup = int.TryParse(user.GroupsId, out groupId);
            }

            if (!string.IsNullOrEmpty(secret))
            {
                long currFinYear = 0;
                int currFinYearGregorian = 0;
                var getFinYears = await mainServiceManager.SysSystemService.GetFinancialYears(user.FacilityId ?? 0);
                if (getFinYears.Succeeded && getFinYears.Data.Any())
                {
                    currFinYearGregorian = getFinYears.Data.Last().FinYearGregorian;
                    currFinYear = getFinYears.Data.Last().FinYear;
                }

                long currPeriodId = 0;
                var getPeriodId = await accServiceManager.AccPeriodsService.GetAll();
                if (getPeriodId.Succeeded)
                {
                    var Period = getPeriodId.Data.FirstOrDefault(x => x.FlagDelete == false && x.PeriodState == 1 && x.FinYear == currFinYear);
                    if (Period != null)
                    {
                        currPeriodId = Period.PeriodId;
                    }
                }

                var getCalendarType = await configurationHelper.GetValue(19, (long)user.FacilityId);
                if (!string.IsNullOrEmpty(getCalendarType))
                {
                    CalendarType = getCalendarType;
                }

                var token = authService.GetJWTToken(user, secret, currFinYear, currPeriodId, currLang, currFinYearGregorian, CalendarType, IsAzureAuthenticated);

                var auth = new AuthModel
                {
                    EmpId = user.EmpId ?? 0,
                    EmpCode = user.EmpCode,
                    FacilityId = user.FacilityId ?? 0,
                    GroupId = groupId,
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
                    CalendarType = CalendarType,
                    Token = token,
                    FinyearGregorian = currFinYearGregorian,
                    LastLogin = user.LastLogin?.ToString("yyyy/MM/dd h:mm:ss tt", CultureInfo.InvariantCulture),
                    SalesType = user.SalesType,
                    Location = user.Location,
                    DeptId = user.DeptId,
                    isTwoFactorActive = false,
                    isAgree = user.IsAgree

                };

                var getFacilities = await mainServiceManager.SysSystemService.GetFacilities();
                if (getFacilities.Succeeded)
                {
                    var facility = getFacilities.Data.FirstOrDefault(f => f.FacilityId == facilityId);
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

                return Ok(await Result<AuthModel>.SuccessAsync(auth, ""));
            }

            return Ok(await Result<AuthModel>.FailAsync("فشل في إنشاء توكن المصادقة"));
        }

        [NonAction]
        private object AuthenticateUserGetInfo(string path, string user, string pass)
        {
#if WINDOWS
    var de = new DirectoryEntry(path, user, pass, AuthenticationTypes.Secure);
    try
    {
        var ds = new DirectorySearcher(de)
        {
            Filter = $"(&((&(objectCategory=Person)(objectClass=User)))(samaccountname={user}))",
            SearchScope = SearchScope.Subtree
        };

        var result = ds.FindOne();
        var name = "";
        var email = "";

        if (result != null)
        {
            if (result.Properties["givenName"].Count > 0)
            {
                name = result.Properties["givenName"][0].ToString();
            }
            if (result.Properties["mail"].Count > 0)
            {
                email = result.Properties["mail"][0].ToString();
            }

            return new
            {
                Name = name,
                Email = email
            };
        }
        else
        {
            return null;
        }
    }
    catch (Exception)
    {
        return null;
    }
#else
            throw new PlatformNotSupportedException("This method is only supported on Windows.");
#endif
        }


        #endregion

        #region login with Azure 
        [HttpGet("loginWithAzure")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithAzure(long facilityId = 1, long languageId = 1)
        {
            try
            {
                // Retrieve Azure AD settings from the database
                string? clientId = await configurationHelper.GetValue(406, facilityId);
                string? tenantId = await configurationHelper.GetValue(408, facilityId);
                string? redirectUri = configuration["FrontEndInfo:AzureRedirectUrl"];
                string scope = "openid email profile User.Read";

                // Validate settings
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(redirectUri))
                {
                    return Ok(await Result<object>.FailAsync("Azure AD configuration is missing."));
                }

                // Create the authorization URL
                var authUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize?" +
                              $"client_id={clientId}&" +
                              $"response_type=code&" +
                              $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                              $"scope={Uri.EscapeDataString(scope)}&" +
                              $"prompt=login";

                return Ok(await Result<object>.SuccessAsync(new { url = authUrl }));
            }
            catch (Exception ex)
            {
                // Log or use the IP address as needed in case of exception
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                Console.WriteLine($"Client IP Address: {ipAddress}");
                Console.WriteLine($"Exception: {ex.Message}");

                return Ok(await Result<object>.FailAsync($"Error: {ex.Message}, Client IP: {ipAddress}"));
            }
        }

        [HttpPost("signin-oidc")]
        [AllowAnonymous]
        public async Task<IActionResult> SignInOidc(AzureCodeDto codeDto)
        {
            if (string.IsNullOrEmpty(codeDto.Code))
            {
                var errorMessage = Request.Query["error"];
                var errorMsg = !string.IsNullOrEmpty(errorMessage) ? $"Unauthorized access: {errorMessage}" : "Unauthorized access";
                return Unauthorized(errorMsg);
            }

            // Retrieve data from session using CurrentData

            var facilityId = codeDto.FacilityId;
            var languageId = codeDto.LanguageId;

            try
            {
                // Retrieve Azure AD settings
                var clientId = await configurationHelper.GetValue(406, facilityId);
                var appSecret = await configurationHelper.GetValue(407, facilityId);
                var redirectUri = "http://localhost:4200/azure-callback";// await configurationHelper.GetValue(415, facilityId);
                var tenantId = await configurationHelper.GetValue(408, facilityId);

                // Validate settings
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(redirectUri) || string.IsNullOrEmpty(appSecret))
                {
                    return BadRequest("Azure AD configuration is missing.");
                }

                // Acquire token
                var confidentialClient = ConfidentialClientApplicationBuilder.Create(clientId)
                    .WithClientSecret(appSecret)
                    .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}/v2.0"))
                    .WithRedirectUri(redirectUri)
                    .Build();

                var result = await confidentialClient.AcquireTokenByAuthorizationCode(new[] { "openid  email User.Read" }, codeDto.Code).ExecuteAsync();

                // Extract user email from Azure AD token
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(result.IdToken) as JwtSecurityToken;
                var emailClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                if (string.IsNullOrEmpty(emailClaim))
                {
                    return BadRequest("Unable to retrieve email from Azure AD.");
                }

                // Get user details directly
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
                var response = await httpClient.GetAsync("https://graph.microsoft.com/v1.0/me");

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("Failed to retrieve user details from Microsoft Graph.");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var details = JObject.Parse(jsonResponse);

                // Call the AuthenticateAsync method
                return await AuthenticateAsync(emailClaim, details, languageId, facilityId, result.IdToken);
            }
            catch (Exception ex)
            {
                httpContextAccessor.HttpContext.Session.SetString("IsAzureAuthenticated", "0");
                var tenantId = await configurationHelper.GetValue(408, facilityId);
                var redirectUri = Regex.Replace(await configurationHelper.GetValue(415, facilityId), "AzureCallback", "Default", RegexOptions.IgnoreCase);
                var logoutUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/logout?" +
                                $"post_logout_redirect_uri={Uri.EscapeDataString(redirectUri)}";
                httpContextAccessor.HttpContext.Session.SetString("ErrorMsg", ex.Message);
                return Redirect(logoutUrl);
            }
        }
        private async Task<IActionResult> AuthenticateAsync(string email, JObject details, long language, long facilityId, string AzureIDToken = "")
        {
            string name = details["displayName"]?.ToString() + " " + details["surname"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return Ok(await Result<AuthModel>.FailAsync("Authentication error."));
            }

            SysUserVw user = new SysUserVw();
            string loginByAd = await configurationHelper.GetValue(405, facilityId);
            if (loginByAd == "1")
            {
                var loginByAzure = await authService.LoginByAzure(email, facilityId, AzureIDToken);
                if (loginByAzure.Succeeded && loginByAzure.Data != null)
                {
                    user = loginByAzure.Data;
                    return await GenerateAuthModel(user, facilityId, (int)language, 1);
                }
                else
                {
                    var createEmpAzure = await authService.CreateEmpAzureAsync(email, name, facilityId, AzureIDToken);
                    if (createEmpAzure.Succeeded)
                    {
                        var loginByAzure2 = await authService.LoginByAzure(email, facilityId, AzureIDToken);
                        if (loginByAzure2.Succeeded)
                        {
                            user = loginByAzure2.Data;
                            return await GenerateAuthModel(user, facilityId, (int)language, 1);
                        }
                        else
                        {
                            return Ok(await Result<AuthModel>.FailAsync(localization.GetCommonResource("UserDisabled")));
                        }
                    }
                    else
                    {
                        return Ok(await Result<AuthModel>.FailAsync(localization.GetCommonResource("CreateUserPermissions")));
                    }
                }
            }
            else
            {
                return Ok(await Result<AuthModel>.FailAsync(localization.GetCommonResource("AzureLoginDisabled")));
            }
        }

        #endregion

    }
}
