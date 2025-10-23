using Logix.Application.Common;
using Logix.Application.DTOs.WH;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WH
{
	public class WhItemTemplateController : BaseWHApiController
	{
		private readonly IWhServiceManager whServiceManager;
		private readonly IAccServiceManager accServiceManager;
		private readonly IMainServiceManager mainServiceManager;
		private readonly IPermissionHelper permission;
		private readonly ICurrentData session;
		private readonly IHrServiceManager hrServiceManager;
		private readonly ILocalizationService localization;

		public WhItemTemplateController(
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
				var chk = await permission.HasPermission(806, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}

				var items = await whServiceManager.WhItemTemplateService.GetAll(e => e.IsDeleted == false);
				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemTemplateDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}
		[HttpGet("GetByIdForEdit")]
		public async Task<IActionResult> GetByIdForEdit(long Id)
		{
			try
			{
				var chkView = await permission.HasPermission(806, PermissionType.View);
				var chkEdit = await permission.HasPermission(806, PermissionType.Edit);
				if (!chkView && !chkEdit)
					return Ok(await Result.AccessDenied("AccessDenied"));
				//long employeeId = 0;
				var items = await whServiceManager.WhItemTemplateService.GetForUpdate<WhItemTemplateEditDto>(Id);
				//if (items.Succeeded)
				//{
				//	 employeeId = items.Data.StorekeeperId??0;
				//	var getEmployee = await mainServiceManager.InvestEmployeeService.GetById(employeeId);
				//	items.Data.StorekeeperCode = getEmployee.Data.EmpId;
				//	//items.Data.StorekeeperName = getEmployee.Data.EmpName;

				//	var getAccount = await accServiceManager.AccAccountService.GetOne(x => x.AccAccountId == items.Data.AccountId);
				//	items.Data.AccountCode = getAccount.Data.AccAccountCode;
				//}

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemTemplateDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}

		[HttpGet("GetById")]
		public async Task<IActionResult> GetById(long Id)
		{
			try
			{
				var chk = await permission.HasPermission(806, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				var items = await whServiceManager.WhItemTemplateService.GetById(Id);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemTemplateDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}

		[HttpPost("Search")]
		public async Task<IActionResult> Search(WhItemTemplateFilterDto filter)
		{
			try
			{
				var chk = await permission.HasPermission(806, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}

				//filter.FacilityId = session.FacilityId; filter.BranchId ??= 0;
				//filter.BranchsId = session.Branches.Split(',').ToString();

				var items = await whServiceManager.WhItemTemplateService.GetAllVW(e => e.IsDeleted == false 
				//&& e.FacilityId == session.FacilityId
					&& (string.IsNullOrEmpty(filter.ItemName) || e.ItemName == filter.ItemName)
					&& (string.IsNullOrEmpty(filter.ItemCode) || e.ItemCode.Contains(filter.ItemCode))
					//&& (filter.StorekeeperId == 0 || e.StorekeeperId == filter.StorekeeperId)
					//&& (string.IsNullOrEmpty(filter.StorekeeperCode) || e.EmpId.Contains(filter.StorekeeperCode))
					//&& (string.IsNullOrEmpty(filter.StorekeeperName) || e.EmpName.Contains(filter.StorekeeperName))
					//&& (filter.BranchId == 0 || e.BranchId == filter.BranchId)
					//&& branchesId.Contains(e.BranchsId)
					//&& (string.IsNullOrEmpty(filter.Location) || e.Location.Contains(filter.Location))
				);


				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemTemplateDto>>.FailAsync($"Error: {ex.Message}"));
			}
		}

		[HttpPost("Add")]
		public async Task<IActionResult> Add(WhItemTemplateDto obj)
		{
			try
			{
				var chk = await permission.HasPermission(806, PermissionType.Add);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				if (!ModelState.IsValid)
					return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

				var items = await whServiceManager.WhItemTemplateService.Add(obj);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhItemTemplateDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

			}

		}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(WhItemTemplateEditDto entity)
		{
			try
			{
				var chk = await permission.HasPermission(806, PermissionType.Edit);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				var items = await whServiceManager.WhItemTemplateService.Update(entity);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhItemTemplateEditDto>.FailAsync($"======= Exp: {ex.Message}"));

			}

		}

		[HttpDelete("Delete")]
		public async Task<IActionResult> Delete(long Id = 0)
		{
			try
			{
				var chk = await permission.HasPermission(806, PermissionType.Delete);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				var items = await whServiceManager.WhItemTemplateService.Remove(Id);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhItemTemplateDto>.FailAsync($"======= Exp : {ex.Message}"));

			}

		}

	}
}
