using Logix.Application.DTOs.Main;

namespace Logix.MVC.LogixAPIs.Sales.ViewModel
{
    public class CertificateSearchVm
    {
        public CertifiBasicData BasicData { get; set; }
        public List<object>? CertificateSetting { get; set; }

        // branches list
        public List<CertificateBranchesVm>? Branches { get; set; }

        public CertificateSearchVm()
        {
            BasicData = new CertifiBasicData();
        }
    }

    public class CertificateBranchesVm
    {
        public int BranchId { get; set; }
        public string? BranchCode { get; set; }
        public string? BraName { get; set; }
        public string? BraName2 { get; set; }
        public string? ShortAddress { get; set; }
        public string? CountryCode { get; set; }
        public string? OTPCode { get; set; }
        public bool IsSelected { get; set; }
    }

    public class CertifiBasicData
    {
        public string? CsrTypeValue { get; set; }

        // facility data
        public string? CountryCode { get; set; }
        public string? VatNumber { get; set; }
        public string? FacilityEmail { get; set; }
        public string? InvoiceType { get; set; }
        public string? Category { get; set; }
    }
}
