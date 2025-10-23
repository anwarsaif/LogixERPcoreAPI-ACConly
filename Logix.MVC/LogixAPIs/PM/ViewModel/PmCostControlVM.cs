namespace Logix.MVC.LogixAPIs.PM.ViewModel
{
    public class PmCostControlVM
    {
        public long Id { get; set; }
        public long? ProjectCode { get; set; }
        public string? ProjectName { get; set; }
        public string? ItemName { get; set; }
        public decimal? Qty { get; set; }

        public decimal? Price
        {
            get; set;
        }
        public decimal? Total { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? TotalCost { get; set; }
    }
     
    
    public class PmProjectPlanVM
    {
       
    }



}
