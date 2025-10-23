using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Common
{
    public class DDLItem<TValue>
    {
        public string Name { get; set; } = string.Empty;
        public TValue Value { get; set; } = default!;
    }
}
