using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace Logix.Domain.ACC
{
    [Keyless]
    public partial class AccReferenceTypeVw
    {
        [Column("Reference_Type_ID")]
        public int ReferenceTypeId { get; set; }
        [Column("Reference_Type_Name")]
        [StringLength(50)]
        public string? ReferenceTypeName { get; set; }
        [Column("Reference_Type_Name2")]
        [StringLength(50)]
        public string? ReferenceTypeName2 { get; set; }
        [Column("Parent_Name")]
        [StringLength(50)]
        public string? ParentName { get; set; }
        public bool? FlagDelete { get; set; }
        [Column("Parent_ID")]
        public int? ParentId { get; set; }
        public long? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public bool? AllowChangeAccount { get; set; }
    }
}
