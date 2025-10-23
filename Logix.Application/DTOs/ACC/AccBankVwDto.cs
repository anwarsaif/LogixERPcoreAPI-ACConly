using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Logix.Application.DTOs.ACC
{
    [Keyless]
    public class AccBankVwDto
    {

        public long BankId { get; set; }

        [StringLength(255)]
        public string? BankName { get; set; }

        [StringLength(255)]
        public string? BankName2 { get; set; }

        public int InsertUserId { get; set; }

        public int? UpdateUserId { get; set; }

        public int? DeleteUserId { get; set; }

        public DateTime InsertDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public DateTime? DeleteDate { get; set; }
        public bool? FlagDelete { get; set; }

        public long? AccAccountId { get; set; }

        public long? FacilityId { get; set; }

        [StringLength(255)]
        public string? AccAccountName { get; set; }

        [StringLength(255)]
        public string? AccAccountName2 { get; set; }

        [StringLength(50)]
        public string? AccAccountCode { get; set; }

        public long? BranchId { get; set; }
        public int? Bank { get; set; }

        [StringLength(250)]
        public string? BranchBank { get; set; }

        public int? AccountType { get; set; }

        public int? CurrencyId { get; set; }

        public int? StatusId { get; set; }

        [StringLength(250)]
        public string? BankAccountNo { get; set; }

        [StringLength(250)]
        public string? Iban { get; set; }
        public string? Note { get; set; }

        public string? UsersPermission { get; set; }
    }
}
