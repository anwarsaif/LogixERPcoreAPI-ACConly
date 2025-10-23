using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.DTOs.FXA;
using Logix.Application.DTOs.HR;
using Logix.Application.DTOs.Main;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmProjects;
using Logix.Application.DTOs.PUR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.PM;
using Logix.MVC.LogixAPIs.Main.ViewModels;
using Logix.MVC.LogixAPIs.ViewModelFilter;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.Main
{
    //[Route("api/[controller]")]
    // [ApiController]
    public class PopupApiController : BaseMainApiController
    {
        private readonly IMainServiceManager mainServiceManager;
        private readonly IHrServiceManager hrServiceManager;
        private readonly IAccServiceManager accServiceManager;
        private readonly IPMServiceManager pmServiceManager;
        private readonly IFxaServiceManager fxaServiceManager;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;
        private readonly IPurServiceManager pURServiceManager;
        private readonly ISalServiceManager salServiceManager;

        public PopupApiController(
            IMainServiceManager mainServiceManager,
            IHrServiceManager hrServiceManager,
            IAccServiceManager accServiceManager,
            IPMServiceManager pmServiceManager,
            IFxaServiceManager fxaServiceManager,
            ICurrentData session,
            ILocalizationService localization,
            IPurServiceManager pURServiceManager,
            ISalServiceManager salServiceManager


            )
        {
            this.mainServiceManager = mainServiceManager;
            this.hrServiceManager = hrServiceManager;
            this.accServiceManager = accServiceManager;
            this.pmServiceManager = pmServiceManager;
            this.fxaServiceManager = fxaServiceManager;
            this.session = session;
            this.localization = localization;
            this.pURServiceManager = pURServiceManager;
            this.salServiceManager = salServiceManager;
        }

        #region ============================================================ Employees ============================================================
        [HttpPost("GetEmployeesPopUp")]
        public async Task<IActionResult> GetEmployeesPopUp(EmployeePouUpDto filter)
        {
            try
            {
                List<EmployeePouUpDto> resultList = new List<EmployeePouUpDto>();
                var BranchesList = session.Branches.Split(',');

                var employees = await hrServiceManager.HrEmployeeService.GetAll(x => x.Isdel == false && x.IsDeleted == false && x.StatusId != 2 && BranchesList.Contains(x.BranchId.ToString())
                    && (string.IsNullOrEmpty(filter.EmpName) || x.EmpName.ToLower().Contains(filter.EmpName.ToLower()) || x.EmpName2.ToLower().Contains(filter.EmpName.ToLower()))
                    && (filter.DeptId == null || filter.DeptId == 0 || filter.DeptId == x.DeptId)
                    && (filter.StatusId == null || filter.StatusId == 0 || filter.StatusId == x.StatusId)
                    && (filter.JobCatagoriesId == null || filter.JobCatagoriesId == 0 || filter.JobCatagoriesId == x.JobCatagoriesId)
                    && (string.IsNullOrEmpty(filter.EmpCode) || x.EmpId == filter.EmpCode)
                    && (string.IsNullOrEmpty(filter.IdNo) || x.IdNo == filter.IdNo)
                    );
                if (employees.Succeeded)
                {
                    if (employees.Data.Count() > 0)
                    {
                        var res = employees.Data.AsQueryable();

                        foreach (var item in res)
                        {
                            var newRecord = new EmployeePouUpDto
                            {
                                EmpCode = item.EmpId,
                                EmpName = item.EmpName,
                            };
                            resultList.Add(newRecord);
                        }
                        if (resultList.Count() > 0)
                            return Ok(await Result<List<EmployeePouUpDto>>.SuccessAsync(resultList, ""));
                        return Ok(await Result<List<EmployeePouUpDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<EmployeePouUpDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }

                return Ok(await Result<EmployeePouUpDto>.FailAsync(employees.Status.message));


            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }



        }

        [HttpPost("GetEmployeeAllDataPopUp")]
        public async Task<IActionResult> GetEmployeeAllDataPopUp(EmployeeFilterDto filter)
        {
            var list = new List<HrEmployeeDto>();

            try
            {
                var empData = await hrServiceManager.HrEmployeeService.GetAll(e => e.Isdel == false && e.IsDeleted == false);
                if (empData.Succeeded)
                {
                    if (empData.Data != null)
                    {
                        list.AddRange(empData.Data);
                        if (!string.IsNullOrEmpty(filter.EmpName))
                        {
                            list = list.Where(e => e.EmpName != null && e.EmpName.ToLower().Contains(filter.EmpName.ToLower())).ToList();
                        }
                        if (!string.IsNullOrEmpty(filter.EmpCode))
                        {
                            list = list.Where(e => e.EmpId != null && e.EmpId == filter.EmpCode).ToList();
                        }

                        if (filter.DeptId != null && filter.DeptId > 0)
                        {
                            list = list.Where(e => e.DeptId == filter.DeptId).ToList();
                        }
                        if (list.Any())
                            return Ok(await Result<List<HrEmployeeDto>>.SuccessAsync(list));
                        return Ok(await Result<List<HrEmployeeDto>>.SuccessAsync(list, localization.GetResource1("NosearchResult")));
                    }

                    return Ok(await Result<List<object>>.SuccessAsync(new List<object>(), localization.GetResource1("NosearchResult")));

                }
                return Ok(await Result<List<HrEmployeeDto>>.FailAsync(empData.Status.message));
            }
            catch (Exception exp)
            {
                return Ok(await Result<HrEmployeeDto>.FailAsync($"EXP, Message: {exp.Message}"));
            }



        }

        [HttpGet("GetEmployeeNameByEmpId")]
        public async Task<IActionResult> GetEmployeeNameById(string empId)
        {
            var lang = session.Language;
            try
            {
                if (string.IsNullOrEmpty(empId))
                {
                    return Ok(await Result<object>.FailAsync($"EmpId is null or empty!"));
                }
                var getEmp = await mainServiceManager.InvestEmployeeService.GetOne(a => a.EmpId == empId);
                if (getEmp.Succeeded && getEmp.Data != null)
                {
                    var empName = "";
                    if (lang == 2)
                    {
                        empName = getEmp.Data.EmpName2 ?? getEmp.Data.EmpName ?? "";
                    }
                    else
                    {
                        empName = getEmp.Data.EmpName ?? getEmp.Data.EmpName2 ?? "";
                    }

                    return Ok(await Result<string>.SuccessAsync(empName, ""));
                }

                return Ok(getEmp);
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }

        }

        [HttpGet("GetEmployeeNameByEmpId")]
        public async Task<IActionResult> GetEmployeeNameById(long? empId)
        {
            var lang = session.Language;
            try
            {
                if (empId == null)
                {
                    return Ok(await Result<object>.FailAsync($"EmpId is null or empty!"));
                }
                var getEmp = await mainServiceManager.InvestEmployeeService.GetOne(a => a.Id == empId);
                if (getEmp.Succeeded && getEmp.Data != null)
                {
                    var empName = "";
                    if (lang == 2)
                    {
                        empName = getEmp.Data.EmpName2 ?? getEmp.Data.EmpName ?? "";
                    }
                    else
                    {
                        empName = getEmp.Data.EmpName ?? getEmp.Data.EmpName2 ?? "";
                    }

                    return Ok(await Result<string>.SuccessAsync(empName, ""));
                }

                return Ok(getEmp);
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }

        }
        #endregion


        #region ============================================================ Account ============================================================

        #region ====== Accounts =============

        [HttpPost("GetAccountsPopUp")]
        public async Task<IActionResult> GetAccountsPopUp(AccAccountsSubHelpeVMFilter filter)
        {
            try
            {
                List<AccAccountsSubHelpeVMFilter> resultList = new();
                var getItems = await accServiceManager.AccAccountsSubHelpeVwService.GetAllVW(x => x.Isdel == false && x.FacilityId == session.FacilityId
                        && x.IsActive == true && x.FlagDelete == false && x.SystemId == 2
                        && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(x.AccAccountCode) && x.AccAccountCode.Contains(filter.Code)))
                        && (string.IsNullOrEmpty(filter.Name)
                                || (!string.IsNullOrEmpty(x.AccAccountName) && x.AccAccountName.Contains(filter.Name))
                                || (!string.IsNullOrEmpty(x.AccAccountName2) && x.AccAccountName2.ToLower().Contains(filter.Name.ToLower()))
                            )
                        );

                if (getItems.Succeeded && getItems.Data != null)
                {
                    if (getItems.Data.Any())
                    {
                        var res = getItems.Data.ToList();

                        foreach (var item in res)
                        {
                            var newRecord = new AccAccountsSubHelpeVMFilter
                            {
                                Code = item.AccAccountCode,
                                Name = session.Language == 1 ? item.AccAccountName : item.AccAccountName2
                            };
                            resultList.Add(newRecord);
                        }

                        return Ok(await Result<List<AccAccountsSubHelpeVMFilter>>.SuccessAsync(resultList, ""));
                    }
                    return Ok(await Result<List<AccAccountsSubHelpeVMFilter>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }

                return Ok(await Result<AccAccountsSubHelpeVMFilter>.FailAsync(getItems.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {ex.Message}"));
            }
        }

        [HttpGet("AccountCodeChanged")]
        public async Task<IActionResult> AccountCodeChanged(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return Ok(await Result<AccAccountsSubHelpeVMFilter>.SuccessAsync("there is no id passed"));
            }
            try
            {
                var getAccount = await accServiceManager.AccAccountsSubHelpeVwService.GetOne(x => x.FacilityId == session.FacilityId
                        && x.IsActive == true && x.FlagDelete == false && x.SystemId == 2
                        && !string.IsNullOrEmpty(x.AccAccountCode) && x.AccAccountCode.Equals(Code)
                       );

                if (getAccount.Succeeded)
                {
                    if (getAccount.Data != null)
                    {
                        var item = new AccAccountsSubHelpeVMFilter
                        {
                            Code = getAccount.Data.AccAccountCode,
                            Name = session.Language == 1 ? getAccount.Data.AccAccountName : getAccount.Data.AccAccountName2
                        };
                        return Ok(await Result<AccAccountsSubHelpeVMFilter>.SuccessAsync(item));
                    }
                    else
                    {
                        return Ok(await Result<AccAccountsSubHelpeVMFilter>.SuccessAsync($"There is no data with this No:  {Code}"));
                    }
                }
                return Ok(await Result<AccAccountsSubHelpeVMFilter>.FailAsync($"{getAccount.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<AccAccountsSubHelpeVMFilter>.FailAsync($"{exp.Message}"));
            }
        }


        [HttpPost("GetAccAccountsSubHelpe")]
        public async Task<IActionResult> GetAccAccountsSubHelpe(AccAccountsSubHelpeVMFilter filter)
        {
            try
            {
                var list = new List<AccAccountsSubHelpeVw>();

                var getEmp = await accServiceManager.AccAccountsSubHelpeVwService.GetAllVW(x => x.Isdel == false && x.FacilityId == session.FacilityId && x.IsActive == true && x.SystemId == 2);
                if (getEmp.Succeeded && getEmp.Data != null)
                {
                    list.AddRange(getEmp.Data.ToList());
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    list = list.Where(e => e.AccAccountName != null && e.AccAccountName.ToLower().Contains(filter.Name.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(filter.Code))
                {
                    list = list.Where(e => e.AccAccountCode != null && e.AccAccountCode == filter.Code).ToList();
                }
                list.OrderBy(x => x.AccAccountCode);
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { Id = s.AccAccountId, Code = s.AccAccountCode, AccAccountName = s.AccAccountName, AccAccountName2 = s.AccAccountName2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpPost("GetAccountsGroup")]
        public async Task<IActionResult> GetAccountsGroup(AccAccountFilterDto filter)
        {
            try
            {
                var list = new List<AccAccountDto>();

                if (filter != null)
                {
                    var accounts = await accServiceManager.AccAccountService.GetAll(a => a.IsDeleted == false && a.IsSub == true && a.SystemId == 2);
                    if (accounts.Succeeded && accounts.Data != null)
                    {
                        list.AddRange(accounts.Data.Where(a => a.IsDeleted == false));
                    }

                    if (!string.IsNullOrEmpty(filter.AccAccountName))
                    {
                        list = list.Where(e => e.AccAccountName != null && e.AccAccountName.ToLower().Contains(filter.AccAccountName.ToLower())).ToList();
                    }

                    if (!string.IsNullOrEmpty(filter.AccAccountCode))
                    {
                        list = list.Where(e => e.AccAccountCode != null && e.AccAccountCode.StartsWith(filter.AccAccountCode)).ToList();
                    }

                    return Ok(await Result<List<AccAccountDto>>.SuccessAsync(list));
                }
                else
                {
                    var accounts = await accServiceManager.AccAccountService.GetAll(a => a.IsDeleted == false && a.IsSub == true);
                    if (accounts.Succeeded && accounts.Data != null)
                    {
                        return Ok(await Result<List<AccAccountDto>>.SuccessAsync(accounts.Data.Where(a => a.IsDeleted == false).ToList()));
                    }
                }

                return Ok(await Result<string>.SuccessAsync("No accounts found."));
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"Exception Message: {exp.Message}"));
            }
        }

        #endregion ====== Accounts =============
        #region ====== AccRequest =============
        [HttpPost("GetLORequest")]
        public async Task<IActionResult> GetLORequest(AccRequestPopFilterDto filter)
        {
            try
            {
                var list = new List<AccRequestPopDto>();

                var LORequest = await accServiceManager.AccRequestService.GetAllVW(x =>
                    x.IsDeleted == false &&
                    x.FacilityId == session.FacilityId &&
                    x.TransTypeId == 1 &&
                    x.StatusId == 3 &&
                    (filter.AppCode == null || filter.AppCode == 0 || (x.AppCode != null && x.AppCode.Equals(filter.AppCode)))

                );

                if (LORequest.Succeeded && LORequest.Data != null)
                {
                    foreach (var item in LORequest.Data)
                    {
                        var AccReque = await accServiceManager.AccRequestService.GetAll(a => a.RefranceId == item.Id && a.TransTypeId == 2);
                        if (AccReque.Succeeded && AccReque.Data != null && AccReque.Data.Count() == 0)
                        {
                            var obj = new AccRequestPopDto
                            {
                                AppCode = item.AppCode,
                                AppDate = item.AppDate,
                                //DepName = item.DepName,
                                Amount = item.Amount,
                                Description = item.Description,
                            };
                            list.Add(obj);
                        }

                    }
                }

                return Ok(await Result<List<AccRequestPopDto>>.SuccessAsync(list));
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"Exception Message: {exp.Message}"));
            }
        }

        #endregion ====== AccRequest =============
        #region ====== Bank =============
        [HttpPost("GetBankPopUp")]
        public async Task<IActionResult> GetBankPopUp(AccBankFilterDto filter)
        {
            try
            {

                var list = new List<AccBankDto>();
                var Bank = await accServiceManager.AccBankService.GetAll(c => c.IsDeleted == false
             && (filter.BankName == null || (c.BankName != null && c.BankName.ToLower().Contains(filter.BankName.ToLower())))
             && (filter.BankAccountNo == null || (c.BankAccountNo != null && c.BankAccountNo.ToLower().Contains(filter.BankAccountNo.ToLower())))

                );
                list.AddRange(Bank.Data);
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { Id = s.BankId, Code = s.BankAccountNo, BankName = s.BankName, BankName2 = s.BankName2 }), ""));

            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }

        }
        [HttpGet("GetBankNameByCode")]
        public async Task<IActionResult> GetBankNameByCode(string BankAccountNo)
        {
            try
            {
                if (string.IsNullOrEmpty(BankAccountNo))
                {
                    return Ok(await Result<object>.FailAsync($"BankAccountNo is null or empty!"));
                }
                var getAccBank = await accServiceManager.AccBankService.GetOneVW(a => a.BankAccountNo != null && a.BankAccountNo == BankAccountNo && a.FlagDelete == false);
                if (getAccBank.Succeeded && getAccBank.Data != null)
                {
                    return Ok(await Result<object>.SuccessAsync(new { Id = getAccBank.Data.BankId, Code = getAccBank.Data.BankAccountNo, BankName = getAccBank.Data.BankName, BankName2 = getAccBank.Data.BankName2 }, ""));
                }

                return Ok(getAccBank);
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }

        }
        #endregion ====== Bank =============
        #region ====== Cash =============
        [HttpPost("GetCashPopUp")]
        public async Task<IActionResult> GetCashPopUp(AccCashOnHandFilterDto filter)
        {
            try
            {

                var list = new List<AccCashOnHandDto>();
                var Cash = await accServiceManager.AccCashOnHandService.GetAll(c => c.IsDeleted == false
             && (filter.Name == null || (c.Name != null && c.Name.ToLower().Contains(filter.Name.ToLower())))
             && (filter.Code == null || filter.Code == 0 || (c.Code != null && c.Code.Equals(filter.Code)))

                );
                list.AddRange(Cash.Data);
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { Id = s.Id, Code = s.Code, Name = s.Name, Name2 = s.Name2 }), ""));


            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }



        }

        [HttpGet("GetCashNameByCode")]
        public async Task<IActionResult> GetCashNameByCode(long Code)
        {
            try
            {
                if (Code == 0)
                {
                    return Ok(await Result<object>.FailAsync($"Get Cash Code is null or empty!"));
                }
                var getAccCash = await accServiceManager.AccCashOnHandService.GetOneVW(a => a.Code != null && a.Code == Code && a.IsDeleted == false);
                if (getAccCash.Succeeded && getAccCash.Data != null)
                {
                    return Ok(await Result<object>.SuccessAsync(new { Id = getAccCash.Data.Id, Code = getAccCash.Data.Code, Name = getAccCash.Data.Name, Name2 = getAccCash.Data.Name2 }, ""));
                }

                return Ok(getAccCash);
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }

        }
        #endregion ====== Cash =============
        #endregion


        #region ============================================================ Cost Center ============================================================
        [HttpPost("GetCostCentersPopUp")]
        public async Task<IActionResult> GetCostCentersPopUp(CostCenterPopUpVm filter)
        {
            try
            {
                List<CostCenterPopUpVm> resultList = new();
                var getItems = await accServiceManager.AccCostCenteHelpVwService.GetAll(x => x.Isdel == false && x.FacilityId == session.FacilityId
                        && x.IsActive == true && x.Isdel == false
                        && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(x.CostCenterCode) && x.CostCenterCode.Contains(filter.Code)))
                        && (string.IsNullOrEmpty(filter.Name)
                                || (!string.IsNullOrEmpty(x.CostCenterName) && x.CostCenterName.Contains(filter.Name))
                                || (!string.IsNullOrEmpty(x.CostCenterName2) && x.CostCenterName2.ToLower().Contains(filter.Name.ToLower()))
                            )
                        );

                if (getItems.Succeeded && getItems.Data != null)
                {
                    if (getItems.Data.Any())
                    {
                        var res = getItems.Data.ToList();

                        foreach (var item in res)
                        {
                            var newRecord = new CostCenterPopUpVm
                            {
                                Code = item.CostCenterCode,
                                Name = session.Language == 1 ? item.CostCenterName : item.CostCenterName2
                            };
                            resultList.Add(newRecord);
                        }

                        return Ok(await Result<List<CostCenterPopUpVm>>.SuccessAsync(resultList, ""));
                    }
                    return Ok(await Result<List<CostCenterPopUpVm>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }

                return Ok(await Result<CostCenterPopUpVm>.FailAsync(getItems.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {ex.Message}"));
            }
        }

        [HttpGet("CostCenterCodeChanged")]
        public async Task<IActionResult> CostCenterCodeChanged(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return Ok(await Result<CostCenterPopUpVm>.SuccessAsync("there is no id passed"));
            }
            try
            {
                var getAccount = await accServiceManager.AccCostCenteHelpVwService.GetOne(x => x.Isdel == false && x.FacilityId == session.FacilityId
                        && x.IsActive == true && x.Isdel == false
                        && (!string.IsNullOrEmpty(x.CostCenterCode) && x.CostCenterCode.Equals(Code))
                        );

                if (getAccount.Succeeded)
                {
                    if (getAccount.Data != null)
                    {
                        var item = new CostCenterPopUpVm
                        {
                            Code = getAccount.Data.CostCenterCode,
                            Name = session.Language == 1 ? getAccount.Data.CostCenterName : getAccount.Data.CostCenterName2
                        };
                        return Ok(await Result<CostCenterPopUpVm>.SuccessAsync(item));
                    }
                    else
                    {
                        return Ok(await Result<CostCenterPopUpVm>.SuccessAsync($"There is no data with this No:  {Code}"));
                    }
                }
                return Ok(await Result<CostCenterPopUpVm>.FailAsync($"{getAccount.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<CostCenterPopUpVm>.FailAsync($"{exp.Message}"));
            }
        }


        [HttpPost("GetCostCentersGroupPopUp")]
        public async Task<IActionResult> GetCostCentersGroupPopUp(CostCenterPopUpVm filter)
        {
            try
            {
                List<CostCenterPopUpVm> resultList = new();
                var getItems = await accServiceManager.AccCostCenterService.GetAll(x => x.IsDeleted == false && x.IsParent == true && x.FacilityId == session.FacilityId

                        && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(x.CostCenterCode) && x.CostCenterCode.Contains(filter.Code)))
                        && (string.IsNullOrEmpty(filter.Name)
                                || (!string.IsNullOrEmpty(x.CostCenterName) && x.CostCenterName.Contains(filter.Name))
                                || (!string.IsNullOrEmpty(x.CostCenterName2) && x.CostCenterName2.ToLower().Contains(filter.Name.ToLower()))
                            )
                        );

                if (getItems.Succeeded && getItems.Data != null)
                {
                    if (getItems.Data.Any())
                    {
                        var res = getItems.Data.ToList();

                        foreach (var item in res)
                        {
                            var newRecord = new CostCenterPopUpVm
                            {
                                Code = item.CostCenterCode,
                                Name = session.Language == 1 ? item.CostCenterName : item.CostCenterName2
                            };
                            resultList.Add(newRecord);
                        }

                        return Ok(await Result<List<CostCenterPopUpVm>>.SuccessAsync(resultList, ""));
                    }
                    return Ok(await Result<List<CostCenterPopUpVm>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }

                return Ok(await Result<CostCenterPopUpVm>.FailAsync(getItems.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {ex.Message}"));
            }
        }

        [HttpGet("CostCenterGroupCodeChanged")]
        public async Task<IActionResult> CostCenterGroupCodeChanged(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return Ok(await Result<CostCenterPopUpVm>.SuccessAsync("there is no id passed"));
            }
            try
            {
                var getAccount = await accServiceManager.AccCostCenterService.GetOne(x => x.IsDeleted == false && x.FacilityId == session.FacilityId && x.IsParent == true
                        && (!string.IsNullOrEmpty(x.CostCenterCode) && x.CostCenterCode.Equals(Code))
                        );

                if (getAccount.Succeeded)
                {
                    if (getAccount.Data != null)
                    {
                        var item = new CostCenterPopUpVm
                        {
                            Code = getAccount.Data.CostCenterCode,
                            Name = session.Language == 1 ? getAccount.Data.CostCenterName : getAccount.Data.CostCenterName2
                        };
                        return Ok(await Result<CostCenterPopUpVm>.SuccessAsync(item));
                    }
                    else
                    {
                        return Ok(await Result<CostCenterPopUpVm>.SuccessAsync($"There is no data with this No:  {Code}"));
                    }
                }
                return Ok(await Result<CostCenterPopUpVm>.FailAsync($"{getAccount.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<CostCenterPopUpVm>.FailAsync($"{exp.Message}"));
            }
        }
        #endregion


        #region ================================================ SysCustomer (customers, suppliers, contractors, ...) ===================================================
        [HttpPost("GetCustomersPopUp")]
        public async Task<IActionResult> GetCustomersPopUp(int CustomerType, SysCustomerPopUpDto filter)
        {
            //dynamic action .. get any type of customers (customer, supplier, contractors, ..etc)
            //url must be => "/Main/PopupApi/CustomerCodeChanged?customerType=1&code=" you can modify the value of customerType in url as your popup (supplier, customer,...)

            try
            {
                List<SysCustomerPopUpDto> resultList = new();
                List<int> currentBranch = MethodsHelper.ConvertStringToIntList(session.Branches);

                var getItems = await mainServiceManager.SysCustomerService.GetAll(x => (x.BranchId != null && currentBranch.Contains(x.BranchId ?? 0))
                        && x.IsDeleted == false && x.CusTypeId == CustomerType
                        && (string.IsNullOrEmpty(filter.Name)
                                || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(filter.Name))
                                || (!string.IsNullOrEmpty(x.Name2) && x.Name2.ToLower().Contains(filter.Name.ToLower()))
                            )
                        && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(x.Code) && x.Code.Contains(filter.Code)))
                       );

                if (getItems.Succeeded && getItems.Data != null)
                {
                    if (getItems.Data.Any())
                    {
                        var res = getItems.Data.ToList();

                        foreach (var item in res)
                        {
                            var newRecord = new SysCustomerPopUpDto
                            {
                                Id = item.Id,
                                Code = item.Code,
                                Name = session.Language == 1 ? item.Name : item.Name2,
                                Mobile = item.Mobile,
                                Email = item.Email,
                            };
                            resultList.Add(newRecord);
                        }

                        return Ok(await Result<List<SysCustomerPopUpDto>>.SuccessAsync(resultList, ""));
                    }
                    return Ok(await Result<List<SysCustomerPopUpDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }

                return Ok(await Result<SysCustomerPopUpDto>.FailAsync(getItems.Status.message));
            }
            catch (Exception ex)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {ex.Message}"));
            }
        }

        [HttpGet("CustomerCodeChanged")]
        public async Task<IActionResult> CustomerCodeChanged(int CustomerType, string Code)
        {
            //dynamic action .. get any type of customers (customer, supplier, ..etc)
            //url must be => "/Main/PopupApi/CustomerCodeChanged?customerType=1&code=" you can modify the value of customerType in url as your popup (supplier, customer,...)

            if (string.IsNullOrEmpty(Code))
            {
                return Ok(await Result<SysCustomerPopUpDto>.SuccessAsync("there is no id passed"));
            }
            try
            {
                List<int> currentBranch = MethodsHelper.ConvertStringToIntList(session.Branches);
                var getSupplier = await mainServiceManager.SysCustomerService.GetOne(x => !string.IsNullOrEmpty(x.Code) && x.Code.Equals(Code)
                && (x.BranchId != null && currentBranch.Contains(x.BranchId ?? 0)) && x.IsDeleted == false && x.CusTypeId == CustomerType);

                if (getSupplier.Succeeded)
                {
                    if (getSupplier.Data != null)
                    {
                        var item = new SysCustomerPopUpDto
                        {
                            Code = getSupplier.Data.Code,
                            Name = getSupplier.Data.Name
                        };
                        return Ok(await Result<SysCustomerPopUpDto>.SuccessAsync(item));
                    }
                    else
                    {
                        return Ok(await Result<SysCustomerPopUpDto>.SuccessAsync($"There is no data with this No:  {Code}"));
                    }
                }
                return Ok(await Result<SysCustomerPopUpDto>.FailAsync($"{getSupplier.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<SysCustomerPopUpDto>.FailAsync($"{exp.Message}"));
            }
        }
        #endregion


        #region ================================================ projectManagement ================================================

        [HttpPost("GetProject")]
        public async Task<IActionResult> GetProject(PMProjectVMFilter filter)
        {
            try
            {
                var list = new List<PMProjectsDto>();
                if (filter.Name == null)
                {
                    filter.Name = "";
                }


                var getEmp = await pmServiceManager.PMProjectsService.GetAll(x => (x.Name ?? "").ToLower().Contains(filter.Name) && x.IsDeleted == false && x.IsSubContract == false && x.FacilityId == session.FacilityId);

                if (getEmp.Succeeded && getEmp.Data != null)
                {
                    list.AddRange(getEmp.Data.ToList());
                }




                if (filter.Code != null)
                {
                    list = list.Where(e => e.Code != null && e.Code == filter.Code).ToList();
                }
                list.OrderBy(x => x.Code);
                return Ok(await Result<object>.SuccessAsync(list.Select(s => new { Id = s.Id, Code = s.Code, Name = s.Name, Name2 = s.Name2 }), ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }
        [HttpGet("GetProjectNameByCode")]
        public async Task<IActionResult> GetProjectNameByCode(long code)
        {
            try
            {
                if (code <= 0)
                {
                    return Ok(await Result<string>.FailAsync(localization.GetResource1("ErrorOccurredDuring")));
                }
                var getProject = await pmServiceManager.PMProjectsService.GetOne(a => a.Code == code);
                if (getProject.Succeeded && getProject.Data != null)
                {
                    var ProjectName = "";
                    if (session.Language == 2)
                    {
                        ProjectName = getProject.Data.Name2 ?? getProject.Data.Name ?? "";
                    }
                    else
                    {
                        ProjectName = getProject.Data.Name ?? getProject.Data.Name2 ?? "";
                    }

                    return Ok(await Result<string>.SuccessAsync(ProjectName, ""));
                }

                return Ok(getProject);
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }

        }
        [HttpPost("GetSubContract")]
        public async Task<IActionResult> GetSubContract(PMProjectVMFilter filter)
        {

            try
            {
                var list = new List<PmProjectsEditVw>();
                if (filter.Name == null)
                {
                    filter.Name = "";
                }

                List<long?> projectEmpIds = new List<long?>();
                if (session.SalesType == 2)
                {
                    var projectEmpIdResponse = await pmServiceManager.PMProjectsStaffService.GetAll(x => x.ProjectId, x => x.EmpId == session.EmpId && x.IsDeleted == false);
                    if (projectEmpIdResponse.Succeeded && projectEmpIdResponse.Data != null)
                    {

                        projectEmpIds.AddRange(projectEmpIdResponse.Data.ToList());
                    }
                }
                var getEmp = await pmServiceManager.PMProjectsService.GetPmProjectsEditVw(x => (x.Name ?? "").ToLower().Contains(filter.Name) && x.IsDeleted == false && x.IsSubContract == true && x.FacilityId == session.FacilityId && (projectEmpIds.Count() == 0 || projectEmpIds.Contains(x.Id)));

                if (getEmp.Succeeded && getEmp.Data != null)
                {
                    list.AddRange(getEmp.Data.ToList());
                }

                List<object> result = new List<object>();

                if (filter.Code != null)
                {
                    list = list.Where(e => e.Code != null && e.Code == filter.Code).ToList();
                }
                list.OrderBy(x => x.Code);
                foreach (var project in list)
                {

                    decimal InvAmt = 0;
                    var tempSal = await salServiceManager.SalTransactionService.GetAll(x => x.ProjectId == project.Id && x.IsDeleted == false);
                    if (tempSal.Succeeded && tempSal.Data != null)
                    {
                        InvAmt = tempSal.Data.Sum(x => x.Total);
                    }

                    result.Add(new
                    {
                        Id = project.Id,
                        Code = project.Code,
                        Name = project.Name,
                        Name2 = project.Name2,
                        ProjectValue = project.ProjectValue,
                        InvAmt = InvAmt,
                        RemainingAmt = project.ProjectValue - InvAmt
                    });
                }

                return Ok(await Result<object>.SuccessAsync(result, ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }
        [HttpGet("GetSubContractNameByCode")]
        public async Task<IActionResult> GetSubContractNameByCode(long code)
        {
            try
            {
                if (code <= 0)
                {
                    return Ok(await Result<string>.FailAsync(localization.GetResource1("ErrorOccurredDuring")));
                }
                var getProject = await pmServiceManager.PMProjectsService.GetOne(a => a.Code == code);
                if (getProject.Succeeded && getProject.Data != null)
                {
                    var ProjectName = "";
                    if (session.Language == 2)
                    {
                        ProjectName = getProject.Data.Name2 ?? getProject.Data.Name ?? "";
                    }
                    else
                    {
                        ProjectName = getProject.Data.Name ?? getProject.Data.Name2 ?? "";
                    }

                    return Ok(await Result<string>.SuccessAsync(ProjectName, ""));
                }

                return Ok(getProject);
            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"EXP, Message: {exp.Message}"));
            }

        }
        [HttpPost("GetUnPaidSubEx")]
        public async Task<IActionResult> GetUnPaidSubEx(PmExtractTransactionPoPDto filter)
        {

            try
            {


                var UnPaidPOs = await accServiceManager.AccRequestService.GetUnPaidSubEx(filter.Code, filter.Date1);

                return Ok(await Result<List<TransactionUnPaidResult>>.SuccessAsync(UnPaidPOs.Data));

            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"Exception Message: {exp.Message}"));
            }
        }

        #endregion


        #region ============================================================ FXA System ============================================================
        [HttpPost("GetFixedAssetsPopUp")]
        public async Task<IActionResult> GetFixedAssetsPopUp(FxaFixedAssetPopUpDto filter)
        {
            try
            {
                List<FxaFixedAssetPopUpDto> resultList = new();
                var BranchesList = session.Branches.Split(',');

                var fixedAssets = await fxaServiceManager.FxaFixedAssetService.GetAllVW(x => x.IsDeleted == false && x.FacilityId == session.FacilityId
                    && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(filter.Name)))
                    && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(x.Code) && x.Code.Contains(filter.Code)))
                    );
                if (fixedAssets.Succeeded)
                {
                    if (fixedAssets.Data.Any())
                    {
                        var res = fixedAssets.Data.OrderBy(x => x.Code).AsQueryable();

                        foreach (var item in res)
                        {
                            var newRecord = new FxaFixedAssetPopUpDto
                            {
                                Id = item.Id,
                                No = item.No,
                                Code = item.Code,
                                Name = item.Name
                            };
                            resultList.Add(newRecord);
                        }
                        if (resultList.Count() > 0)
                            return Ok(await Result<List<FxaFixedAssetPopUpDto>>.SuccessAsync(resultList, ""));
                        return Ok(await Result<List<FxaFixedAssetPopUpDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<FxaFixedAssetPopUpDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }

                return Ok(await Result<FxaFixedAssetPopUpDto>.FailAsync(fixedAssets.Status.message));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpGet("FxaNoChanged")]
        public async Task<IActionResult> FxaNoChanged(long No)
        {
            if (No == 0)
            {
                return Ok(await Result<FxaFixedAssetPopUpDto>.SuccessAsync("there is no id passed"));
            }
            try
            {
                var getAsset = await fxaServiceManager.FxaFixedAssetService.GetOneVW(f => f.No == No && f.IsDeleted == false);
                if (getAsset.Succeeded)
                {
                    if (getAsset.Data != null)
                    {
                        var item = new FxaFixedAssetPopUpDto
                        {
                            Id = getAsset.Data.Id,
                            Code = getAsset.Data.Code,
                            No = getAsset.Data.No,
                            Name = getAsset.Data.Name
                        };
                        return Ok(await Result<FxaFixedAssetPopUpDto>.SuccessAsync(item));
                    }
                    else
                    {
                        return Ok(await Result<FxaFixedAssetPopUpDto>.SuccessAsync($"There is no asset with this No:  {No}"));
                    }
                }
                return Ok(await Result<FxaFixedAssetPopUpDto>.FailAsync($"{getAsset.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<FxaFixedAssetPopUpDto>.FailAsync($"{exp.Message}"));
            }
        }


        [HttpPost("GetFixedAssetsTypePopUp")]
        public async Task<IActionResult> GetFixedAssetsTypePopUp(FxaFixedAssetTypePopUpDto filter)
        {
            try
            {
                List<FxaFixedAssetTypePopUpDto> resultList = new List<FxaFixedAssetTypePopUpDto>();
                var BranchesList = session.Branches.Split(',');

                var fixedAssetTypes = await fxaServiceManager.FxaFixedAssetTypeService.GetAllVW(x => x.IsDeleted == false && x.FacilityId == session.FacilityId
                    && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(x.TypeName) && x.TypeName.Contains(filter.Name)))
                    && (string.IsNullOrEmpty(filter.Code) || (!string.IsNullOrEmpty(x.Code) && x.Code.Contains(filter.Code)))
                    );
                if (fixedAssetTypes.Succeeded)
                {
                    if (fixedAssetTypes.Data.Count() > 0)
                    {
                        var res = fixedAssetTypes.Data.OrderBy(x => x.Code).AsQueryable();

                        foreach (var item in res)
                        {
                            var newRecord = new FxaFixedAssetTypePopUpDto
                            {
                                Id = item.Id,
                                Code = item.Code,
                                Name = item.TypeName
                            };

                            var parentName = await fxaServiceManager.FxaFixedAssetTypeService.GetOne(t => t.TypeName, t => t.Id == item.ParentId);
                            newRecord.MainTypeName = parentName.Succeeded ? parentName.Data : "";

                            resultList.Add(newRecord);
                        }
                        if (resultList.Count() > 0)
                            return Ok(await Result<List<FxaFixedAssetTypePopUpDto>>.SuccessAsync(resultList, ""));
                        return Ok(await Result<List<FxaFixedAssetTypePopUpDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                    }
                    return Ok(await Result<List<FxaFixedAssetTypePopUpDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                }

                return Ok(await Result<FxaFixedAssetTypePopUpDto>.FailAsync(fixedAssetTypes.Status.message));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpGet("FxaTypeCodeChanged")]
        public async Task<IActionResult> FxaTypeCodeChanged(string Code)
        {
            if (string.IsNullOrEmpty(Code))
            {
                return Ok(await Result<FxaFixedAssetTypePopUpDto>.SuccessAsync("there is no id passed"));
            }
            try
            {
                var getType = await fxaServiceManager.FxaFixedAssetTypeService.GetOne(f => f.Code == Code && f.IsDeleted == false);
                if (getType.Succeeded)
                {
                    if (getType.Data != null)
                    {
                        var item = new FxaFixedAssetTypePopUpDto
                        {
                            Id = getType.Data.Id,
                            Code = getType.Data.Code,
                            Name = getType.Data.TypeName
                        };
                        return Ok(await Result<FxaFixedAssetTypePopUpDto>.SuccessAsync(item));
                    }
                    else
                    {
                        return Ok(await Result<FxaFixedAssetTypePopUpDto>.SuccessAsync($"There is No type with this code:  {Code}"));
                    }
                }
                return Ok(await Result<FxaFixedAssetTypePopUpDto>.FailAsync($"{getType.Status.message}"));
            }
            catch (Exception exp)
            {
                return Ok(await Result<FxaFixedAssetTypePopUpDto>.FailAsync($"{exp.Message}"));
            }
        }
        #endregion

        #region ================ PUR =====
        [HttpPost("GetUnPaidPO")]
        public async Task<IActionResult> GetUnPaidPO(PurTransactionFilterDto filter)
        {

            try
            {


                string TransTypeId = "1,7,5";
                var UnPaidPOs = await accServiceManager.AccRequestService.GetUnPaidPO(TransTypeId, filter.Code, filter.Date1);

                return Ok(await Result<List<TransactionResult>>.SuccessAsync(UnPaidPOs.Data));

            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"Exception Message: {exp.Message}"));
            }
        }


        [HttpPost("GetApprovedPayroll")]
        public async Task<IActionResult> GetApprovedPayroll(PayrollResultPopupFilter filter)
        {

            try
            {

                var UnPaidPOs = await accServiceManager.AccRequestService.GetApprovedPayroll(filter);

                return Ok(await Result<List<PayrollResultpopup>>.SuccessAsync(UnPaidPOs.Data));

            }
            catch (Exception exp)
            {
                return Ok(await Result<string>.FailAsync($"Exception Message: {exp.Message}"));
            }
        }
        #endregion



        #region ================ Recruitment =====


        [HttpPost("GetApplicantPopUp")]
        public async Task<IActionResult> GetApplicantPopUp(HrRecruitmentApplicationPopUpDto filter)
        {
            try
            {
                filter.ApplicantId ??= 0;
                filter.VacancyId ??= 0;
                List<HrRecruitmentApplicationPopUpDto> resultList = new();

                var allData = await hrServiceManager.HrRecruitmentApplicationService.GetAllVW(x => x.IsDeleted == false
                    && (string.IsNullOrEmpty(filter.Name) || (!string.IsNullOrEmpty(x.Name) && x.Name.Contains(filter.Name)))
                    && (filter.ApplicantId == 0 || filter.ApplicantId == x.ApplicantId)
                    && (filter.VacancyId == 0 || filter.VacancyId == x.VacancyId)
                    );
                if (!allData.Succeeded)
                    return Ok(await Result<FxaFixedAssetPopUpDto>.FailAsync(allData.Status.message));

                if (!allData.Data.Any())
                    return Ok(await Result<List<HrRecruitmentApplicationPopUpDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
                var res = allData.Data.OrderBy(x => x.Id).AsQueryable();
                foreach (var item in res)
                {
                    var newRecord = new HrRecruitmentApplicationPopUpDto
                    {
                        ApplicantId = item.ApplicantId,
                        Id = item.Id,
                        VacancyName = item.VacancyName,
                        Name = item.Name
                    };
                    resultList.Add(newRecord);
                }
                if (resultList.Count() > 0)
                    return Ok(await Result<List<HrRecruitmentApplicationPopUpDto>>.SuccessAsync(resultList, ""));
                return Ok(await Result<List<HrRecruitmentApplicationPopUpDto>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult")));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        [HttpGet("ApplicantIdChanged")]
        public async Task<IActionResult> ApplicantIdChanged(long Id)
        {
            try
            {
                if (Id <= 0)
                    return Ok(await Result<HrRecruitmentApplicationPopUpDto>.FailAsync("There is No ID Passed"));

                var GetRecord = await hrServiceManager.HrRecruitmentApplicationService.GetOneVW(x => x.IsDeleted == false && x.Id == Id);
                if (!GetRecord.Succeeded)
                    return Ok(await Result<HrRecruitmentApplicationPopUpDto>.FailAsync(GetRecord.Status.message));

                if (GetRecord.Data == null)
                    return Ok(await Result<List<HrRecruitmentApplicationPopUpDto>>.SuccessAsync($"There is no Applicant with this Id:  {Id}"));

                var result = new HrRecruitmentApplicationPopUpDto
                {
                    ApplicantId = GetRecord.Data.ApplicantId,
                    Id = GetRecord.Data.Id,
                    VacancyName = GetRecord.Data.VacancyName,
                    Name = GetRecord.Data.Name
                };

                return Ok(await Result<HrRecruitmentApplicationPopUpDto>.SuccessAsync(result, ""));
            }
            catch (Exception exp)
            {
                return Ok(await Result<object>.FailAsync($"EXP, Message: {exp.Message}"));
            }
        }

        #endregion

    }
}