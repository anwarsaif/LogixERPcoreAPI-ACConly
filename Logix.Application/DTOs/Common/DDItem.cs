using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.DTOs.Common
{
    public class DDItem
    {
        public long? Code { get; set; }
        public string? Name { get; set; }

        public long Id { get; set; }

        public int? ColorId { get; set; }

        public string? Icon { get; set; }
    }
}
