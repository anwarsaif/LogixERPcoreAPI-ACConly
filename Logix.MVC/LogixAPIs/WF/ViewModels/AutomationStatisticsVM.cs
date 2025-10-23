namespace Logix.MVC.LogixAPIs.WF.ViewModels
{
    public class AutomationStatisticsVM
    {
        public string? StatusName { get; set; } // Application_Type_Name
        public string? StatusName2 { get; set; } // Application_Type_Name2
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public int Count { get; set; }
        //public int ApplicationsTypeId { get; set; }
        public string? Link { get; set; }

        public int StatisticType { get; set; } // 1 mean is the first section of statistics, 2 is the Second section
    }
}
