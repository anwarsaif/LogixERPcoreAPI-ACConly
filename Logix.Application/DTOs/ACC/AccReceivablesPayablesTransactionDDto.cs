namespace Logix.Application.DTOs.ACC
{
    public class AccReceivablesPayablesTransactionDDto
    {

        public long? Id { get; set; }
        public long? TId { get; set; }
        public long? RpDId { get; set; }
        public int? CurrStatueId { get; set; }
        public int? NewStatueId { get; set; }
        public long? CrdAccountId { get; set; }
        public long? DbtAccountId { get; set; }
        public bool IsDeleted { get; set; }
        public string? Note { get; set; }
        public string? DueDateOld { get; set; }
        public int? AccBankId { get; set; }
    }
    public class AccReceivablesPayablesTransactionDEditDto
    {

        public long Id { get; set; }
        public long? TId { get; set; }
        public long? RpDId { get; set; }
        public int? CurrStatueId { get; set; }
        public int? NewStatueId { get; set; }
        public long? CrdAccountId { get; set; }
        public long? DbtAccountId { get; set; }
        public string? Note { get; set; }
        public string? DueDateOld { get; set; }
        public int? AccBankId { get; set; }
    }
}
