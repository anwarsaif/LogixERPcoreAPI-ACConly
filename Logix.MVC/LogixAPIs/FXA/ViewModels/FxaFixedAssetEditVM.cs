using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Logix.MVC.LogixAPIs.FXA.ViewModels
{
    public class FxaFixedAssetEditVM
    {
        public long? HidTransactionId { get; set; }
        //public long? HidTransactionAssestId { get; set; }
        //public long? HidTransactionId2 { get; set; }
        //public long? HidTransactionAssestId2 { get; set; }
        //public bool? HidCreateNewId { get; set; }
        //public int? HidTransTypeId { get; set; }
        public string? JCode { get; set; }

        public List<FxaTransactionsAssetsVwVm> FxaTransactionsAssetsVwVms { get; set; }
        public List<FxaFixedAssetVwVm> FxaFixedAssetVwVms { get; set; }
        public List<FxaFixedAssetTransferVwVm> FxaFixedAssetTransferVwVms { get; set; }
        public List<FxaAdditionsExclusionVwVm> FxaAdditionsExclusionVwVms { get; set; }

        public FxaFixedAssetEditVM()
        {
            this.FxaTransactionsAssetsVwVms = new List<FxaTransactionsAssetsVwVm>();
            this.FxaFixedAssetVwVms = new List<FxaFixedAssetVwVm>();
            this.FxaFixedAssetTransferVwVms = new List<FxaFixedAssetTransferVwVm>();
            this.FxaAdditionsExclusionVwVms = new List<FxaAdditionsExclusionVwVm>();
        }
    }

    public class FxaTransactionsAssetsVwVm
    {
        public long Id { get; set; }
        public string? Code { get; set; }
        public string? TransactionsTypeName { get; set; }
        public string? TransDate { get; set; }
        public string? Description { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debet { get; set; }

        public decimal Balance { get; set; } = 0;
    }

    public class FxaFixedAssetVwVm
    {
        public long Id { get; set; }
        public long? No { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? TypeName { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public decimal? Amount { get; set; }
        public string? PurchaseOrder { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
    }

    public class FxaFixedAssetTransferVwVm
    {
        public long Id { get; set; }
        public string? DateTransfer { get; set; }
        public string? FromLocationName { get; set; }
        public string? FromEmpName { get; set; }
        public string? ToLocationName { get; set; }
        public string? ToEmpName { get; set; }
    }

    public class FxaAdditionsExclusionVwVm
    {
        public decimal? Balance { get; set; }
        public string? AdditionsExclusionTypeName { get; set; }

        public long Id { get; set; }
        public string? Description { get; set; }
        public string? Date1 { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }
    }
}