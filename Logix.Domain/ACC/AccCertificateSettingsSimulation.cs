using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Logix.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Table("ACC_CertificateSettings_Simulation")]
    public partial class AccCertificateSettingsSimulation : TraceEntity
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Column("CSR")]
        public string? Csr { get; set; }

        public string? PrivateKey { get; set; }

        public string? Secret { get; set; }

        public string? Certificate { get; set; }

        public string? UserName { get; set; }

        public DateTime ExpiredDate { get; set; }

        public DateTime StartedDate { get; set; }

        [Column("Facility_ID")]
        public long? FacilityId { get; set; }

        [Column("Branch_Id")]
        public int? BranchId { get; set; }

        [Column("OU")]
        public string? Ou { get; set; }

        public string? O { get; set; }

        [Column("CN")]
        public string? Cn { get; set; }

        [Column("SN")]
        public string? Sn { get; set; }

        [Column("UID")]
        public string? Uid { get; set; }

        public string? SystemVersion { get; set; }

        public Guid? Guid { get; set; }
    }
}
