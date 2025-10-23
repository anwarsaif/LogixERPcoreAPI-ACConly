using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
    public class AccCertificateSettingDto
    {
        public long Id { get; set; }
        public string? Csr { get; set; }
        public string? PrivateKey { get; set; }
        public string? Secret { get; set; }
        public string? Certificate { get; set; }
        public string? UserName { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime StartedDate { get; set; }
        public long? FacilityId { get; set; }
        public bool IsDeleted { get; set; }
        public int? BranchId { get; set; }
        public string? Ou { get; set; }
        public string? O { get; set; }
        public string? Cn { get; set; }
        public string? Sn { get; set; }
        public string? Uid { get; set; }
        public string? SystemVersion { get; set; }
        public Guid? Guid { get; set; }
    }
    
    public class AccCertificateSettingEditDto
    {
        public long Id { get; set; }
        public string? Csr { get; set; }
        public string? PrivateKey { get; set; }
        public string? Secret { get; set; }
        public string? Certificate { get; set; }
        public string? UserName { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime StartedDate { get; set; }
        public long? FacilityId { get; set; }
        public bool IsDeleted { get; set; }
        public int? BranchId { get; set; }
        public string? Ou { get; set; }
        public string? O { get; set; }
        public string? Cn { get; set; }
        public string? Sn { get; set; }
        public string? Uid { get; set; }
        public string? SystemVersion { get; set; }
        public Guid? Guid { get; set; }
    }
}
