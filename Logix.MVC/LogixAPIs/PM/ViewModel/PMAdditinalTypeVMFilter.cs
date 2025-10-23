namespace Logix.MVC.LogixAPIs.PM.ViewModel
{
    public class PMExtractAdditinalTypeVMFilter
    {

        public long? Code { get; set; } = 0;
        public int? TypeId { get; set; } = 0;
        public string? Name { get; set; } = null;
        public int? CreditOrDebit { get; set; } = 0;
        public int? RateOrAmount { get; set; }= 0;
        public long? AccRefTypeId { get; set; }=0;
    }   
    
    

}
