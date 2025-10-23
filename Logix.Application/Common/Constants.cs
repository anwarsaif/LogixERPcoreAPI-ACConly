namespace Logix.Application.Common
{
    public static class Languages
    {
        public const int Ar = 1;
        public const int Eng = 2;
    }
    public static class RouteHub
    {
        public const string Route = "/notify";

    }

    public static class FilesPath
    {
        public const string TempPath = "TempFiles";
        public const string AllFiles = "AllFiles";
        public static readonly string ZacatQrPath = $"AllFiles{Path.DirectorySeparatorChar}QRCode{Path.DirectorySeparatorChar}ZATCA";
        public static readonly string InvoiceQrPath = $"AllFiles{Path.DirectorySeparatorChar}QRCode{Path.DirectorySeparatorChar}Invoice";
        public static readonly string SaleQrPath = $"AllFiles{Path.DirectorySeparatorChar}QRCode{Path.DirectorySeparatorChar}Sales"; // use in FXA/FxaAdditionsExclusionController
        public static readonly string FixedAssetQrPath = $"AllFiles{Path.DirectorySeparatorChar}QRCode{Path.DirectorySeparatorChar}FixedAssets";
        public static readonly string FixedAssetBarCodePath = $"Files{Path.DirectorySeparatorChar}Barcode{Path.DirectorySeparatorChar}FixedAssets";
        public static readonly string BackupDbPath = $"BackupDB";
    }

    public static class SessionKeys
    {
        public const string AddTempFiles = "AddTempFiles";
        public const string EditTempFiles = "EditTempFiles";
        public const string TempFilesAdd = "TempFilesAdd";
        public const string AddTempCustomerFiles = "AddTempCustomerFiles";
        public const string EditTempCustomerFiles = "EditTempCustomerFiles";
        public const string AddSalTransactionItems = "AddSalTransactionItems";
        public const string EditSalTransactionItems = "EditSalTransactionItems";
        public const string AddSalTransactionLocation = "AddSalTransactionLocation";
        public const string EditSalTransactionLocation = "EditSalTransactionLocation";
        public const string CopiesSalTransactionLocation = " CopiesSalTransactionLocation";


        public const string EditCustomerContacts = "EditCustomerContacts";

        public const string AddOpmContractItem = "AddOpmContractItem";
        public const string AddOpmContractPurchaseItem = "AddOpmContractPurchaseItem";

        public const string AddOpmContractLocation = "AddOpmContractLocation";
        public const string AddOpmContractPurchaseLocation = "AddOpmContractPurchaseLocation";

        public const string AddContractEmployee = "AddContractEmployee";
        public const string AddOPMPayroll = "AddOPMPayroll";
        public const string EditOPMPayroll = "EditOPMPayroll";
        public const string AddOPmPurchasesInvoiceitems = "AddOPmPurchasesInvoiceitems";
        public const string EditOPmPurchasesInvoiceitems = "EditOPmPurchasesInvoiceitems";
        public const string CopiesSalTransactionItems = "CopiesSalTransactionItems";
        public const string InvestEmployee = "InvestEmployee";
        public const string SubitemsDto = "SubitemsDto";
        public const string InitialCreditsAdd = "InitialCreditsAdd";
        public const string InitialCreditsEdit = "InitialCreditsEdit";
        public const string InitialCreditsRepetitiont = "InitialCreditsRepetitiont";
        public const string ReinforcementsAdd = "ReinforcementsAdd";
        public const string ReinforcementsEdit = "ReinforcementsEdit";
        public const string CostsitemsAdd = "CostsitemsAdd";
        public const string CostsitemsEdit = "CostsitemsEdit";
    }

    public enum BranchAccountType
    {
        None,
        Sales,
        Purchases,
        ReSales,
        RePurchases,
        Customer,
        Supplier,
        Sales_Discount,
        Purchases_Discount,
        Cost_Goods_Sold,
        Inventory_Transfer_Account
    }



    public enum DataTypeIdEnum
    {
        String = 1,
        Boolean = 2,
        Numeric = 3,
        Date = 4,
        PickList = 5,
        Longstring = 6,
        Title = 7,
        Time = 8,
        File = 9,
        Table = 10,
        Label = 11,
        Link = 12
    }
    public static class Pagination
    {
        public const int take = 10;

        public const int DDLpageSize = 10;

    }
    public static class SignalNOTIFICATION
    {
        public const string NOTIFICATION_CHANNEL = "new_notification";

        public const string PERMISSION_CHANNEL = "notification_permission";
        public const string SYSTEM_CHANNEL = "notification_system";
        public const string SCREEN_CHANNEL = "notification_screen";

    }

}
