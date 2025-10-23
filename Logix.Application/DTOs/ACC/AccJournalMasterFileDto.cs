namespace Logix.Application.DTOs.ACC
{
    public  class AccJournalMasterFileDto
    {
        public long Id { get; set; }
        public long? JId { get; set; }
        public string? FileName { get; set; }
        public string? FileDescription { get; set; }
        public string? FileDate { get; set; }
        public int? FileType { get; set; }
        public string? SourceFile { get; set; }
        public string? FileUrl { get; set; }
        public string? FileExt { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public  class AccJournalMasterFileEditDto
    {
        public long Id { get; set; }
        public long? JId { get; set; }
        public string? FileName { get; set; }
        public string? FileDescription { get; set; }
        public string? FileDate { get; set; }
        public int? FileType { get; set; }
        public string? SourceFile { get; set; }
        public string? FileUrl { get; set; }
        public string? FileExt { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
