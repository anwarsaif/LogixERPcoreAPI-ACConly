using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.ViewModelFilter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;

namespace Logix.MVC.LogixAPIs.Main
{
    public class AutoCompleteApiController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPMServiceManager pmServiceManager;
        private readonly IOPMServiceManager opmServiceManager;
        private readonly IPermissionHelper permission;
        private readonly IDDListHelper listHelper;
        private readonly ISysConfigurationHelper configurationHelper;
        private readonly IScreenPropertiesHelper screenPropertiesHelper;
        private readonly IGbServiceManager gbServiceManager;
		private readonly ILocalizationService localization;
		private readonly ICurrentData session;
        public AutoCompleteApiController(
            IMainServiceManager mainServiceManager,
            IHrServiceManager hrServiceManager,
            IAccServiceManager accServiceManager,
            IPMServiceManager pmServiceManager,
            IOPMServiceManager opmServiceManager,
            IPermissionHelper permission,
            ICurrentData session,
            IDDListHelper listHelper,
            ISysConfigurationHelper configurationHelper,
            IScreenPropertiesHelper screenPropertiesHelper,
            IGbServiceManager gbServiceManager,
			  ILocalizationService localization

			)
        {
            this.mainServiceManager = mainServiceManager;
            this.hrServiceManager = hrServiceManager;
            this.accServiceManager = accServiceManager;
            this.pmServiceManager = pmServiceManager;
            this.opmServiceManager = opmServiceManager;
            this.permission = permission;
            this.listHelper = listHelper;
            this.configurationHelper = configurationHelper;
            this.screenPropertiesHelper = screenPropertiesHelper;
            this.gbServiceManager = gbServiceManager;
			this.localization = localization;
			this.session = session;
        }



        #region ================ main ==================
        [HttpGet("GetCustomersPopUp")]
        public async Task<IActionResult> GetCustomersPopUp(string? code)
        {
            try
            {
          
                var list = new List<SysCustomerDto>();
                List<int> currentBranch = MethodsHelper.ConvertStringToIntList(session.Branches);

                var accounts = await mainServiceManager.SysCustomerService.GetAll(x => x.Code == code & (x.BranchId != null && currentBranch.Contains(x.BranchId ?? 0)) && x.IsDeleted == false && x.CusTypeId == 2);


                if (accounts.Succeeded && accounts.Data != null)
                {
                    list.AddRange(accounts.Data.ToList());
                }





                list.OrderBy(x => x.Code);
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { Id = s.Id, Code = s.Code, Name = s.Name, Name2 = s.Name2}), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpGet("GetCustomersName")]
        public async Task<IActionResult> GetCustomersName(string Name)
        {
            try
            {

                var list = new List<SysCustomerDto>();
                List<int> currentBranch = MethodsHelper.ConvertStringToIntList(session.Branches);

                var accounts = await mainServiceManager.SysCustomerService.GetAll(x => x.Name.Contains(Name) & (x.BranchId != null && currentBranch.Contains(x.BranchId ?? 0)) && x.IsDeleted == false && x.CusTypeId == 2);


                if (accounts.Succeeded && accounts.Data != null)
                {
                    list.AddRange(accounts.Data.ToList());
                }





                list.OrderBy(x => x.Code);
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { Id = s.Id, Code = s.Code, Name = s.Name, Name2 = s.Name2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        #endregion
        #region ================ Accounts ==================
        [HttpGet("GetAccountsByNameOrCode")]
        public async Task<IActionResult> GetAccountsByNameOrCode(string? nameOrCode)
        {
            try
            {
                var list = new List<AccAccountDto>();
                var accounts = await accServiceManager.AccAccountService.GetAll();
                if (accounts.Succeeded && accounts.Data != null)
                {
                    list.AddRange(accounts.Data.Where(a => a.IsDeleted == false));
                }

                if (!string.IsNullOrEmpty(nameOrCode))
                {
                    list = list.Where(e => (e.AccAccountCode != null && e.AccAccountCode.ToLower().Contains(nameOrCode.ToLower())) || (e.AccAccountName != null && e.AccAccountName.ToLower().Contains(nameOrCode.ToLower()))).ToList();
                }
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { AccAccountId = s.AccAccountId, AccAccountCode = s.AccAccountCode, AccAccountName = s.AccAccountName, AccAccountName2 = s.AccAccountName2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }

        }

        [HttpGet("GetAccountsByCode")]
        public async Task<IActionResult> GetAccountsByCode(string? code)
        {
            try
            {
                var list = new List<AccAccountDto>();
                var accounts = await accServiceManager.AccAccountService.GetAll(a => a.IsDeleted == false);
                if (accounts.Succeeded && accounts.Data != null)
                {
                    list.AddRange(accounts.Data.Where(a => a.IsDeleted == false));
                }

                if (!string.IsNullOrEmpty(code))
                {
                    list = list.Where(e => e.AccAccountCode != null && e.AccAccountCode==code).ToList();
                }
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { AccAccountId = s.AccAccountId, AccAccountCode = s.AccAccountCode, AccAccountName = s.AccAccountName, AccAccountName2 = s.AccAccountName2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpGet("GetAccountsByName")]
        public async Task<IActionResult> GetAccountsByName(string? name)
        {
            try
            {
                var list = new List<AccAccountDto>();
                var accounts = await accServiceManager.AccAccountService.GetAll();
                if (accounts.Succeeded && accounts.Data != null)
                {
                    list.AddRange(accounts.Data.Where(a => a.IsDeleted == false));
                }

                if (!string.IsNullOrEmpty(name))
                {
                    list = list.Where(e => e.AccAccountName != null && e.AccAccountName.ToLower().Contains(name.ToLower())).ToList();
                }
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { AccAccountId = s.AccAccountId, AccAccountCode = s.AccAccountCode, AccAccountName = s.AccAccountName, AccAccountName2 = s.AccAccountName2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }

        }
      
		[HttpGet("GetAccAccountsSubHelpeByCode")]
		public async Task<IActionResult> GetAccAccountsSubHelpeByCode(string? code)
		{
			try
			{
				var obj = new AccAccountsSubHelpeVMFilter();
				if (string.IsNullOrEmpty(code))
				{
					return Ok(await Result<AccAccountsSubHelpeVMFilter>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
				}
				var getAcc = await accServiceManager.AccAccountsSubHelpeVwService.GetOne(a => a.FlagDelete == false  && a.AccAccountCode == code);
				if (getAcc.Succeeded && getAcc.Data != null)
				{
					if (session.Language == 1)
					{
						obj.Name = getAcc.Data.AccAccountName;
					}
					else
					{
						obj.Name = getAcc.Data.AccAccountName2;
					}
					obj.Code = getAcc.Data.AccAccountCode;
					return Ok(await Result<AccAccountsSubHelpeVMFilter>.SuccessAsync(obj, $""));


				}
				else
				{
					return Ok(getAcc.Data);
				}

			}
			catch (Exception exp)
			{
				return Ok(await Result<AccAccountsSubHelpeVMFilter>.FailAsync($"EXP, Message: {exp.Message}"));

			}
		}
		[HttpGet("AccountsGroupByCode")]
        public async Task<IActionResult> AccountsGroupByCode(string? code)
        {
            try
            {
                var obj = new AccAccountPopDto();
				if (string.IsNullOrEmpty(code))
				{
					return Ok(await Result<AccAccountPopDto>.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
				}
				var getAcc= await accServiceManager.AccAccountService.GetOne(a => a.IsDeleted == false && a.IsSub == true && a.AccAccountCode== code) ;
				if (getAcc.Succeeded && getAcc.Data != null)
				{
					if (session.Language == 1)
					{
						obj.AccAccountName= getAcc.Data.AccAccountName;
					}
					else
					{
						obj.AccAccountName = getAcc.Data.AccAccountName2;
					}
					obj.AccAccountCode = getAcc.Data.AccAccountCode;
					return Ok(await Result<AccAccountPopDto>.SuccessAsync(obj, $""));


				}
				else
				{
					return Ok(getAcc.Data);
				}
			
            }
            catch (Exception exp)
            {
				return Ok(await Result<AccAccountPopDto>.FailAsync($"EXP, Message: {exp.Message}"));

			}
		}
        #endregion


        #region  ============= Employees ===============
        [HttpGet("GetEmployeesByName")]
        public async Task<IActionResult> GetEmployeesByName(string? name)
        {
            try
            {
                var list = new List<HrEmployeeDto>();
                var screens = await hrServiceManager.HrEmployeeService.GetAll();
                if (screens.Succeeded && screens.Data != null)
                {
                    list.AddRange(screens.Data);
                }

                if (!string.IsNullOrEmpty(name))
                {
                    list = list.Where(e => e.EmpName != null && e.EmpName.ToLower().Contains(name.ToLower())).ToList();
                }
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { Id = s.Id, EmpId = s.EmpId, EmpName = s.EmpName, EmpName2 = s.EmpName2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpGet("GetEmployeesByEmpId")]
        public async Task<IActionResult> GetEmployeesByEmpId(string? empId)
        {
            try
            {
                var list = new List<HrEmployeeDto>();
                var screens = await hrServiceManager.HrEmployeeService.GetAll();
                if (screens.Succeeded && screens.Data != null)
                {
                    list.AddRange(screens.Data);
                }

                if (!string.IsNullOrEmpty(empId))
                {
                    list = list.Where(e => e.EmpId != null && e.EmpId.ToLower().Contains(empId.ToLower())).ToList();
                }
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { Id = s.Id, EmpId = s.EmpId, EmpName = s.EmpName, EmpName2 = s.EmpName2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpGet("GetEmployeesByNameOrEmpId")]
        public async Task<IActionResult> GetEmployeesByNameOrEmpId(string? nameOrEmpId)
        {
            try
            {
                var list = new List<HrEmployeeDto>();
                var screens = await hrServiceManager.HrEmployeeService.GetAll();
                if (screens.Succeeded && screens.Data != null)
                {
                    list.AddRange(screens.Data);
                }

                if (!string.IsNullOrEmpty(nameOrEmpId))
                {
                    list = list.Where(e => (e.EmpId != null && e.EmpId.ToLower().Contains(nameOrEmpId.ToLower())) || (e.EmpName != null && e.EmpName.ToLower().Contains(nameOrEmpId.ToLower()))).ToList();
                }
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { Id = s.Id, EmpId = s.EmpId, EmpName = s.EmpName, EmpName2 = s.EmpName2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }
        #endregion


        #region ============= CostCenters ===============
        [HttpGet("GetCostCentersByName")]
        public async Task<IActionResult> GetCostCentersByName(string? name)
        {
            try
            {
                var list = new List<AccCostCenterDto>();
                var costCenters = await accServiceManager.AccCostCenterService.GetAll(c => c.IsDeleted == false);
                if (costCenters.Succeeded && costCenters.Data != null)
                {
                    list.AddRange(costCenters.Data.ToList());
                }

                if (!string.IsNullOrEmpty(name))
                {
                    list = list.Where(e => e.CostCenterName != null && e.CostCenterName.ToLower().Contains(name.ToLower())).ToList();
                }

                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { CcId = s.CcId, CostCenterCode = s.CostCenterCode, CostCenterName = s.CostCenterName, CostCenterName2 = s.CostCenterName2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpGet("GetCostCentersByCode")]
        public async Task<IActionResult> GetCostCentersByCode(string? code)
        {
            try
            {
                var list = new List<AccCostCenterDto>();
                var costCenters = await accServiceManager.AccCostCenterService.GetAll(c => c.IsDeleted == false);
                if (costCenters.Succeeded && costCenters.Data != null)
                {
                    list.AddRange(costCenters.Data.ToList());
                }

                if (!string.IsNullOrEmpty(code))
                {
                    list = list.Where(e => e.CostCenterCode != null && e.CostCenterCode.ToLower().Contains(code.ToLower())).ToList();
                }

                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { CcId = s.CcId, CostCenterCode = s.CostCenterCode, CostCenterName = s.CostCenterName, CostCenterName2 = s.CostCenterName2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpGet("GetCostCentersByNameOrCode")]
        public async Task<IActionResult> GetCostCentersByNameOrCode(string? nameOrCode)
        {
            try
            {
                var list = new List<AccCostCenterDto>();
                var costCenters = await accServiceManager.AccCostCenterService.GetAll(c => c.IsDeleted == false);
                if (costCenters.Succeeded && costCenters.Data != null)
                {
                    list.AddRange(costCenters.Data.ToList());
                }

                if (!string.IsNullOrEmpty(nameOrCode))
                {
                    list = list.Where(e => (e.CostCenterCode != null && e.CostCenterCode.ToLower().Contains(nameOrCode.ToLower())) || (e.CostCenterName != null && e.CostCenterName.ToLower().Contains(nameOrCode.ToLower()))).ToList();
                }

                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { CcId = s.CcId, CostCenterCode = s.CostCenterCode, CostCenterName = s.CostCenterName, CostCenterName2 = s.CostCenterName2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }
        #endregion


        #region ============== Projects ===============
        [HttpGet("GetProjectsByNameOrCode")]
        public async Task<IActionResult> GetProjectsByNameOrCode(string? nameOrCode)
        {
            try
            {
                var list = new List<PMProjectsDto>();
                var getItems = await pmServiceManager.PMProjectsService.GetAll();
                if (getItems.Succeeded && getItems.Data != null)
                {
                    list.AddRange(getItems.Data);
                }

                if (!string.IsNullOrEmpty(nameOrCode))
                {
                    list = list.Where(e => (e.Code != null && e.Code.Value.ToString().ToLower().Contains(nameOrCode.ToLower())) || (e.Name != null && e.Name.ToLower().Contains(nameOrCode.ToLower()))).ToList();
                }
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { Id = s.Id, Code = s.Code, Name = s.Name, Name2 = s.Name2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpGet("GetProjectsByName")]
        public async Task<IActionResult> GetProjectsByName(string? name)
        {
            try
            {
                var list = new List<PMProjectsDto>();
                var getItems = await pmServiceManager.PMProjectsService.GetAll();
                if (getItems.Succeeded && getItems.Data != null)
                {
                    list.AddRange(getItems.Data);
                }

                if (!string.IsNullOrEmpty(name))
                {
                    list = list.Where(e => e.Name != null && e.Name.ToLower().Contains(name.ToLower())).ToList();
                }
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { Id = s.Id, Code = s.Code, Name = s.Name, Name2 = s.Name2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpGet("GetProjectsByCode")]
        public async Task<IActionResult> GetProjectsByCode(long? code)
        {
            try
            {
                var list = new List<PMProjectsDto>();
             

                var getEmp = await pmServiceManager.PMProjectsService.GetAll(x => x.Code==code &&x.IsDeleted == false && x.FacilityId == session.FacilityId);
                if (getEmp.Succeeded && getEmp.Data != null)
                {
                    list.AddRange(getEmp.Data.ToList());
                }




               
                list.OrderBy(x => x.Code);
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { Id = s.Id, Code = s.Code, Name = s.Name, Name2 = s.Name2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }
        #endregion
    }
}
