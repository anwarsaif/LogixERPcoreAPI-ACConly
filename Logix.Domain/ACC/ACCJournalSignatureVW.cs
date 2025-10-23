using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccJournalSignatureVw
    {
        public string? Signature1 { get; set; }
        public string? Signature2 { get; set; }
        public string? Signature3 { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
    }
}
