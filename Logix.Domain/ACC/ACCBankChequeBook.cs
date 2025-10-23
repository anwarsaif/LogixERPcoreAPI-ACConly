using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logix.Domain.Base;

namespace Logix.Domain.ACC
{
    [Table("ACC_Bank_Cheque_Book")]
    public partial class AccBankChequeBook:TraceEntity
    {
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        public long? FromChequeNo { get; set; }
        public long? ToChequeNo { get; set; }
        public long? Count { get; set; }
        [Column("Bank_ID")]
        public long? BankId { get; set; }
     
    }
}
