using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Logix.Application.Common
{
    public interface ICurrentData
    {
        long UserId { get; }
        int EmpId { get; }
        long FacilityId { get; }
        long FinYear { get; }
        int GroupId { get; }
        int Language { get; }
        long PeriodId { get; }
        string Branches { get; }
        string OldBaseUrl { get; }
        string CoreBaseUrl { get; }
        string CalendarType { get; }
        long BranchId { get; }
        int FinyearGregorian { get; }
        int DeptId { get; }
        int LocationId { get; }
        int SalesType { get; }
        int IsAzureAuthenticated { get; }
        bool isAgree { get; }
        string DateFomet { get; }

    }

    public class CurrentData : ICurrentData
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ClaimsPrincipal _userClaims;

        public CurrentData(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userClaims = GetUserClaims();
        }

        private ClaimsPrincipal UserClaims => _userClaims ??= GetUserClaims();

        private ClaimsPrincipal GetUserClaims()
        {
            if (_httpContextAccessor.HttpContext?.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase) == true)
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
                //if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                if (string.IsNullOrEmpty(authHeader)) // from swagger tkn not has "Bearer "
                {
                    return new ClaimsPrincipal(new ClaimsIdentity());
                }

                var token = authHeader.Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();

                try
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims));
                }
                catch
                {
                    return new ClaimsPrincipal(new ClaimsIdentity());
                }
            }
            else
            {
                // mean the request isn't api, but for mvc (devexpress report)

                // Request is not API (e.g., MVC for DevExpress reports)
                string token = string.Empty;
                var query = _httpContextAccessor.HttpContext?.Request.Query;

                StringValues values = ""; // Declare values explicitly
                var chk = query?.TryGetValue("jwt", out values) ?? false;
                if (chk == true)
                {
                    token = values.FirstOrDefault() ?? "";

                    var handler2 = new JwtSecurityTokenHandler();

                    try
                    {
                        var jwtToken = handler2.ReadJwtToken(token);
                        return new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims));
                    }
                    catch
                    {
                        return new ClaimsPrincipal(new ClaimsIdentity());
                    }
                }
                else
                {
                    return new ClaimsPrincipal(new ClaimsIdentity());
                }
            }
        }

        private string GetClaimValue(string claimType) => UserClaims.FindFirst(claimType)?.Value ?? "-1";
        private int GetClaimValueInt(string claimType) => int.TryParse(GetClaimValue(claimType), out int value) ? value : -1;
        private long GetClaimValueLong(string claimType) => long.TryParse(GetClaimValue(claimType), out long value) ? value : -1;
        private bool GetClaimValueBool(string claimType) => bool.TryParse(GetClaimValue(claimType), out bool value) && value;

        public long UserId => GetClaimValueLong("UserId");
        public int EmpId => GetClaimValueInt("EmpId");
        public long FacilityId => GetClaimValueLong("FacilityId");
        public long FinYear => GetClaimValueLong("FinYear");
        public int GroupId => GetClaimValueInt("GroupId");
        public int Language => GetClaimValueInt("Language");
        public string OldBaseUrl => GetClaimValue("OldBaseUrl");
        public string CoreBaseUrl => GetClaimValue("CoreBaseUrl");
        public long PeriodId => GetClaimValueLong("PeriodId");
        public string Branches => GetClaimValue("Branches");
        public string CalendarType => GetClaimValue("CalendarType");
        public long BranchId => GetClaimValueLong("BranchId");
        public int FinyearGregorian => GetClaimValueInt("FinyearGregorian");
        public int DeptId => GetClaimValueInt("DeptId");
        public int LocationId => GetClaimValueInt("LocationId");
        public int SalesType => GetClaimValueInt("SalesType");
        public int IsAzureAuthenticated => GetClaimValueInt("IsAzureAuthenticated");
        public bool isAgree => GetClaimValueBool("isAgree");
        public string DateFomet => "yyyy/MM/dd";

    }
}