using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Table("ACC_Journal_Master_Files")]
    public partial class AccJournalMasterFile
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }
        [Column("J_ID")]
        public long? JId { get; set; }
        [Column("File_Name")]
        [StringLength(50)]
        public string? FileName { get; set; }
        [Column("File_Description")]
        [StringLength(4000)]
        public string? FileDescription { get; set; }
        [Column("File_Date")]
        [StringLength(10)]
        public string? FileDate { get; set; }
        [Column("File_Type")]
        public int? FileType { get; set; }
        [Column("Source_File")]
        [StringLength(500)]
        public string? SourceFile { get; set; }
        [Column("File_URL")]
        public string? FileUrl { get; set; }
        [Column("File_Ext")]
        [StringLength(50)]
        public string? FileExt { get; set; }
        public long? CreatedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
