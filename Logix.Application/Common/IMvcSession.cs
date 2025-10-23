using DocumentFormat.OpenXml.Drawing.Charts;
using Logix.Application.Extensions;
using Microsoft.AspNetCore.Http;

namespace Logix.Application.Common
{
    /// <summary>
    /// This interface used only for mvc not api.
    /// This interface used in DevExpress report designer.
    /// Do not use this interface for any class that called by an api function.
    /// </summary>
    public interface IMvcSession
    {
        bool ClearSession();
        T GetData<T>(string key);
        void AddData<T>(string key, T value);
        void SetMainData(long userId, int empId, int groupId, long facilityId, long finYear, int language, string branches, string calendartype, long branchId, int FinyearGregorian, int LocationId, int DeptId, int SalesType, long periodId, int IsAzureAuthenticated = 0, bool isAgree = true);

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
    }

    public class MvcSession : IMvcSession
    {
        private readonly ISession _session;

        public long UserId => _session.GetData<long>("UserId");
        public int EmpId => _session.GetData<int>("EmpId");
        public long FacilityId => _session.GetData<long>("FacilityId");
        public long FinYear => _session.GetData<long>("FinYear");
        public int GroupId => _session.GetData<int>("GroupId");
        public int Language => _session.GetData<int>("Language");
        public long PeriodId => _session.GetData<long>("Period");
        public string Branches => _session.GetData<string>("Branches");
        public string OldBaseUrl => _session.GetData<string>("OldBaseUrl");
        public string CoreBaseUrl => _session.GetData<string>("CoreBaseUrl");
        public string CalendarType => _session.GetData<string>("CalendarType");
        public long BranchId => _session.GetData<long>("BranchId");
        public int FinyearGregorian => _session.GetData<int>("FinyearGregorian");
        public int DeptId => _session.GetData<int>("DeptId");
        public int LocationId => _session.GetData<int>("LocationId");
        public int SalesType => _session.GetData<int>("SalesType");
        public int IsAzureAuthenticated => _session.GetData<int>("IsAzureAuthenticated");
        public bool isAgree => _session.GetData<bool>("isAgree");

        public MvcSession(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor != null)
            {
                if (httpContextAccessor.HttpContext != null)
                {
                    _session = httpContextAccessor.HttpContext.Session;
                }
            }
        }

        public T GetData<T>(string key)
        {
            return _session.GetData<T>(key);
        }

        public void AddData<T>(string key, T value)
        {
            _session.AddData<T>(key, value);
        }

        public void SetMainData(long userId, int empId, int groupId, long facilityId, long finYear, int language, string branches, string calendartype, long branchId, int FinyearGregorian, int LocationId, int DeptId, int SalesType, long periodId, int IsAzureAuthenticated = 0, bool isAgree = true)
        {
            _session.AddData<long>("UserId", userId);
            _session.AddData<int>("EmpId", empId);
            _session.AddData<int>("GroupId", groupId);
            _session.AddData<long>("FacilityId", facilityId);
            _session.AddData<long>("FinYear", finYear);
            _session.AddData<int>("Language", language);
            _session.AddData<long>("PeriodId", periodId);
            _session.AddData<string>("Branches", branches);
            _session.AddData<long>("BranchId", branchId);
            _session.AddData<int>("FinyearGregorian", FinyearGregorian);
            _session.AddData<string>("CalendarType", CalendarType);
            _session.AddData<int>("LocationId", LocationId);
            _session.AddData<int>("DeptId", DeptId);
            _session.AddData<int>("SalesType", SalesType);
            _session.AddData<int>("IsAzureAuthenticated", IsAzureAuthenticated);
            _session.AddData<bool>("isAgree", isAgree);

            //_session.AddData<string>("OldBaseUrl", oldBaseUrl);
            //_session.AddData<string>("CoreBaseUrl", coreBaseUrl);
        }

        public bool ClearSession()
        {
            try
            {
                _session.Clear();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
