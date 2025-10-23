using Logix.Application.DTOs.Main;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.MVC.LogixAPIs.Acc.ViewModels
{
    public class ACCCostCentertVM
    {

        public long CcId { get; set; }
        public string? CostCenterName { get; set; }
        public string? CostCenterName2 { get; set; }
        public long? CcParentId { get; set; }
        public List<ACCCostCentertVM> Children { get; set; }
    }
}
