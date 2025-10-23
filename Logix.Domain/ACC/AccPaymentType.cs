using Logix.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Domain.ACC
{

    [Table("ACC_Payment_Type")]
    public partial class AccPaymentType
    {
        [Key]
        [Column("Payment_Type_ID")]
        public long PaymentTypeId { get; set; }
        [Column("Payment_Type_Name")]
        [StringLength(255)]
        public string? PaymentTypeName { get; set; }
        [Column("Payment_Type_Name2")]
        [StringLength(255)]
        public string? PaymentTypeName2 { get; set; }
        [Column("Insert_User_ID")]
        public int InsertUserId { get; set; }
        [Column("Update_User_ID")]
        public int? UpdateUserId { get; set; }
        [Column("Delete_User_ID")]
        public int? DeleteUserId { get; set; }
        [Column("Insert_Date", TypeName = "datetime")]
        public DateTime InsertDate { get; set; }
        [Column("Update_Date", TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [Column("Delete_Date", TypeName = "datetime")]
        public DateTime? DeleteDate { get; set; }
        public bool? FlagDelete { get; set; }
    }
}
