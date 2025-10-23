using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.FXA.ViewModels
{
    public class FxaRevaluationFilterVM
    {
        public long? Id { get; set; } //Id of FXA_Transactions table (in FXATransactionsRevaluationVW)
        public string? Code { get; set; } //Code of FXA_Transactions table (in FXATransactionsRevaluationVW)
        public int? BranchId { get; set; } //BranchID of FXA_Transactions table (in FXATransactionsRevaluationVW)
        public long? FxNo { get; set; }
        public string? FxName { get; set; }
        [StringLength(10)]
        public string? StartDate { get; set; }
        [StringLength(10)]
        public string? EndDate { get; set; }

        // addition property to return
        public string? TransDate { get; set; }
        public decimal? AmountOld { get; set; }
        public decimal? AmountNew { get; set; }
        public decimal? AmountDepreciation { get; set; }
        public decimal? ProfitAndLoss { get; set; }
        public string? JCode { get; set; }
        public string? JDate { get; set; }
    }
}