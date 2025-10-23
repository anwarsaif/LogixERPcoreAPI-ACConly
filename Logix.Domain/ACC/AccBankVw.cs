using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Logix.Domain.ACC
{
    [Keyless]
    public class AccBankVw
    {
        [Column("Bank_ID")]
        public long BankId { get; set; }
        [Column("Bank_Name")]
        [StringLength(255)]
        public string? BankName { get; set; }
        [Column("Bank_Name2")]
        [StringLength(255)]
        public string? BankName2 { get; set; }
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
        [Column("Acc_Account_ID")]
        public long? AccAccountId { get; set; }
        [Column("Facility_ID")]
        public long? FacilityId { get; set; }
        [Column("Acc_Account_Name")]
        [StringLength(255)]
        public string? AccAccountName { get; set; }
        [Column("Acc_Account_Name2")]
        [StringLength(255)]
        public string? AccAccountName2 { get; set; }
        [Column("Acc_Account_Code")]
        [StringLength(50)]
        public string? AccAccountCode { get; set; }
        [Column("Branch_ID")]
        public long? BranchId { get; set; }
        public int? Bank { get; set; }
        [Column("Branch_Bank")]
        [StringLength(250)]
        public string? BranchBank { get; set; }
        [Column("Account_Type")]
        public int? AccountType { get; set; }
        [Column("Currency_ID")]
        public int? CurrencyId { get; set; }
        [Column("Status_ID")]
        public int? StatusId { get; set; }
        [Column("Bank_Account_NO")]
        [StringLength(250)]
        public string? BankAccountNo { get; set; }
        [Column("IBAN")]
        [StringLength(250)]
        public string? Iban { get; set; }
        public string? Note { get; set; }
        [Column("Users_Permission")]
        public string? UsersPermission { get; set; }
    }
}
