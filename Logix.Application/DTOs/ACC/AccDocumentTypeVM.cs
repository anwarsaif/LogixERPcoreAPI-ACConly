using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.ACC
{
    public class AccDocumentTypeVM
    {
        public int DocTypeID { get; set; }
        public string DocTypeName { get; set; } = String.Empty;
        public string DocTypeName2 { get; set; } = String.Empty;
        public bool Selected { get; set; } = false;
    }
}
