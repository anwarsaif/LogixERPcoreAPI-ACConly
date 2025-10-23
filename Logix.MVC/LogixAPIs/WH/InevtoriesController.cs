using Logix.Application.Common;
using Logix.Application.DTOs.FXA;
using Logix.Application.DTOs.WH;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Services.HR;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.WH.ViewModel;
using Logix.MVC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WH
{
	public class InevtoriesController : BaseWHApiController
	{
		private readonly IWhServiceManager whServiceManager;
		private readonly IAccServiceManager accServiceManager;
		private readonly IMainServiceManager mainServiceManager;
		private readonly IPermissionHelper permission;
		private readonly ICurrentData session;
		private readonly IHrServiceManager hrServiceManager;
		private readonly ILocalizationService localization;

		public InevtoriesController(
			IWhServiceManager whServiceManager, 
			IAccServiceManager accServiceManager, 
			IMainServiceManager mainServiceManager, 
			IPermissionHelper permission,
			ICurrentData session,
			 IHrServiceManager hrServiceManager,
			ILocalizationService localization
			)
		{
			this.whServiceManager = whServiceManager;
			this.accServiceManager = accServiceManager;
			this.mainServiceManager = mainServiceManager;
			this.permission = permission;
			this.session = session;
			this.hrServiceManager = hrServiceManager;
			this.localization = localization;
		}
		[HttpGet("GetAll")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var chk = await permission.HasPermission(345, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}

				var items = await whServiceManager.WhInventoryService.GetAll(e => e.IsDeleted == false);
				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhInventoryDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}
		[HttpGet("GetByIdForEdit")]
		public async Task<IActionResult> GetByIdForEdit(long Id)
		{
			try
			{
				var chkView = await permission.HasPermission(345, PermissionType.View);
				var chkEdit = await permission.HasPermission(345, PermissionType.Edit);
				if (!chkView && !chkEdit)
					return Ok(await Result.AccessDenied("AccessDenied"));
				long employeeId = 0;
				var items = await whServiceManager.WhInventoryService.GetForUpdate<WhInventoryEditDto>(Id);
				if (items.Succeeded)
				{
					 employeeId = items.Data.StorekeeperId??0;
					var getEmployee = await mainServiceManager.InvestEmployeeService.GetById(employeeId);
					items.Data.StorekeeperCode = getEmployee.Data.EmpId;
					//items.Data.StorekeeperName = getEmployee.Data.EmpName;

					var getAccount = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == items.Data.AccountId);
					items.Data.AccountCode = getAccount.Data.AccAccountCode;
				}

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhInventoryDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}

		[HttpGet("GetById")]
		public async Task<IActionResult> GetById(long Id)
		{
			try
			{
				var chk = await permission.HasPermission(345, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				long employeeId = 0;
				var items = await whServiceManager.WhInventoryService.GetById(Id);
				if (items.Succeeded)
				{
					employeeId = items.Data.StorekeeperId ?? 0;
					var getEmployee = await mainServiceManager.InvestEmployeeService.GetById(employeeId);
					items.Data.StorekeeperCode = getEmployee.Data.EmpId;
					items.Data.StorekeeperName = getEmployee.Data.EmpName;

					var getAccount = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == items.Data.AccountId);
					items.Data.AccountCode = getAccount.Data.AccAccountCode;
				}

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhInventoryDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}

		[HttpPost("Search")]
		public async Task<IActionResult> Search(WhInventorySearch filter)
		{
			try
			{
				var chk = await permission.HasPermission(345, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}

				filter.FacilityId = session.FacilityId; filter.BranchId ??= 0;
				filter.BranchsId = session.Branches.Split(',').ToString();

				var items = await whServiceManager.WhInventoryService.GetAllVW(e => e.IsDeleted == false && e.FacilityId == session.FacilityId
					&& (string.IsNullOrEmpty(filter.Code) || e.Code == filter.Code)
					&& (string.IsNullOrEmpty(filter.InventoryName) || e.InventoryName.Contains(filter.InventoryName))
					//&& (filter.StorekeeperId == 0 || e.StorekeeperId == filter.StorekeeperId)
					&& (string.IsNullOrEmpty(filter.StorekeeperCode) || e.EmpId.Contains(filter.StorekeeperCode))
					&& (string.IsNullOrEmpty(filter.StorekeeperName) || e.EmpName.Contains(filter.StorekeeperName))
					&& (filter.BranchId == 0 || e.BranchId == filter.BranchId)
					//&& branchesId.Contains(e.BranchsId)
					&& (string.IsNullOrEmpty(filter.Location) || e.Location.Contains(filter.Location))
				);


				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhInventoryDto>>.FailAsync($"Error: {ex.Message}"));
			}
		}

		[HttpPost("Add")]
		public async Task<IActionResult> Add(WhInventoryDto obj)
		{
			try
			{
				var chk = await permission.HasPermission(345, PermissionType.Add);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				if (!ModelState.IsValid)
					return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

				var items = await whServiceManager.WhInventoryService.Add(obj);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhInventoryDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

			}

		}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(WhInventoryEditDto entity)
		{
			try
			{
				var chk = await permission.HasPermission(345, PermissionType.Edit);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				var items = await whServiceManager.WhInventoryService.Update(entity);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhInventoryEditDto>.FailAsync($"======= Exp: {ex.Message}"));

			}

		}

		[HttpDelete("Delete")]
		public async Task<IActionResult> Delete(long Id = 0)
		{
			try
			{
				var chk = await permission.HasPermission(345, PermissionType.Delete);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				var items = await whServiceManager.WhInventoryService.Remove(Id);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhInventoryDto>.FailAsync($"======= Exp : {ex.Message}"));

			}

		}

	}
}
