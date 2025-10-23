using System.ComponentModel.DataAnnotations;

namespace Logix.MVC.LogixAPIs.PM.ViewModel
{
    public class PmExtractTransactionVM
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public string? Date1 { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public long? ProjectCode { get; set; }
        public string? ProjectName { get; set; }
        public string? BraName { get; set; }
        public decimal? Total { get; set; }
        public decimal? Vat { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? Subtotal { get; set; }
        public string? CurrencyName { get; set; }
        public decimal? ExchangeRate { get; set; }

        //this use in followUp extract gridview
        public string? LastStatusName { get; set; }
        public string? DateChange { get; set; }
        public string? DateRemind { get; set; }
        public string? LastNote { get; set; }

        //this use in reports => RpExtract
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

        //this use in  SubContractExtractSearch
        public decimal? ValueInLocalCurrency { get; set; }
        public string? DeliveryDate { get; set; }
    }
}