using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.MVC.LogixAPIs.Acc.ViewModels
{
    public class AccountVM
    {

        public long AccAccountId { get; set; }
        public long AccGroupId { get; set; }
        public string? AccAccountName { get; set; }
        public string? AccAccountName2 { get; set; }
        public long? AccAccountParentId { get; set; }
        public List<AccountVM> Children { get; set; }
    }
}
