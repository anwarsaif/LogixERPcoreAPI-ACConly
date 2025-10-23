using Castle.MicroKernel.SubSystems.Conversion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
    public class AccBankChequeBookDto
    {
        
        public int Id { get; set; }
        public long? FromChequeNo { get; set; }
        public long? ToChequeNo { get; set; }
        public long? Count { get; set; }
       
        public long? BankId { get; set; }

        public int? CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }


        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }


        public bool? IsDeleted { get; set; }

    }
    public class AccBankChequeBookEditDto
    {

        public int Id { get; set; }
        public long? FromChequeNo { get; set; }
        public long? ToChequeNo { get; set; }
        public long? Count { get; set; }

        public long? BankId { get; set; }

    }
}
