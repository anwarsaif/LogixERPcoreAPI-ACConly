
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
    public class AccReferenceTypeDto
    {


        public int ReferenceTypeId { get; set; }
        [Required]
        public string? ReferenceTypeName { get; set; }
        [Required]
        public int? ParentId { get; set; }

        public bool? AllowChangeAccount { get; set; }

        public string? ReferenceTypeName2 { get; set; }
        public bool? FlagDelete { get; set; }
     

    
    }
    public class AccReferenceTypeEditDto
    {

        public int ReferenceTypeId { get; set; }
        [Required]
        public string? ReferenceTypeName { get; set; }
        public bool? FlagDelete { get; set; }
        [Required]
        public int? ParentId { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }
        public bool? AllowChangeAccount { get; set; }

        public string? ReferenceTypeName2 { get; set; }
    }

}
