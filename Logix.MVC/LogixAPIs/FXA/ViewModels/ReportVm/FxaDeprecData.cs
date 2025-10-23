using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.FXA.ViewModels.ReportVm
{
    public class FxaDeprecData
    {
        //used in reports

        public long Id { get; set; }
        public string? PurchaseDate { get; set; }
        public long? No { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? LocationName { get; set; }
        public long? TypeId { get; set; }
        public string? TypeName { get; set; }
        public bool? AdditionType { get; set; }
        public string? MainAssetName { get; set; }
        public string? TypeCode { get; set; }
        public decimal? Amount { get; set; } // amount or 0 + (SUM(Credit) - SUM(Debet) from FXA_Transactions_Assest_VW)
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? DeprecMethodName { get; set; }
        public decimal? DeprecMonthlyRate { get; set; }
        public decimal? InstallmentValue { get; set; }
        public decimal? Additions { get; set; } // SUM(Credit) from FXA_Transactions_Assest_VW
        public decimal? Discards { get; set; } // amount or 0 + SUM(Debet) from FXA_Transactions_Assest_VW
        public decimal? DepreciationOld { get; set; } // SUM(Debet) from FXA_Transactions_Assest_VW
        public decimal? DepreciationNow { get; set; } // SUM(Debet) from FXA_Transactions_Assest_VW
        public decimal? DepreciateDiscardation { get; set; } = 0;
        public decimal? ProfitAndLoss { get; set; } // SUM(Profit_and_loss) from FXA_Transactions_Revaluation_VW

        // in outer apply
        public string? DiscardDate { get; set; } // Trans_Date from FXA_Transactions_Assest_VW
        public decimal? DiscardAmount { get; set; } // Total from FXA_Transactions_Assest_VW

        // use in GetDepreciationReport3
        public string? ClassificationName { get; set; }
        public string? FatherTypeName { get; set; }
        public string? GrandTypeName { get; set; }
    }
}