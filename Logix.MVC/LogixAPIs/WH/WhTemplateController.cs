using Logix.Application.Common;
using Logix.Application.DTOs.WH;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WH
{
	public class WhTemplateController : BaseWHApiController
	{
		private readonly IWhServiceManager whServiceManager;
		private readonly IAccServiceManager accServiceManager;
		private readonly IMainServiceManager mainServiceManager;
		private readonly IPermissionHelper permission;
		private readonly ICurrentData session;
		private readonly IHrServiceManager hrServiceManager;
		private readonly ILocalizationService localization;

		public WhTemplateController(
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

				var items = await whServiceManager.WhTemplateService.GetAll(e => e.IsDeleted == false);
				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhTemplateDto>>.FailAsync($"======= Exp: {ex.Message}"));

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
				var items = await whServiceManager.WhTemplateService.GetForUpdate<WhTemplateEditDto>(Id);
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
				return Ok(await Result<IEnumerable<WhTemplateDto>>.FailAsync($"======= Exp: {ex.Message}"));

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
				var items = await whServiceManager.WhTemplateService.GetById(Id);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhTemplateDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}

		[HttpPost("Search")]
		public async Task<IActionResult> Search(WhTemplateFilterDto filter)
		{
			try
			{
				var chk = await permission.HasPermission(806, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}

				var items = await whServiceManager.WhTemplateService.GetAllVW(e => e.IsDeleted == false 
					&& (string.IsNullOrEmpty(filter.Name) || e.Name == filter.Name)
					&& (filter.Code == null || e.Code == filter.Code)
					&& (filter.CatId == null || e.CatId == filter.CatId)
				);


				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhTemplateDto>>.FailAsync($"Error: {ex.Message}"));
			}
		}

		[HttpPost("Add")]
		public async Task<IActionResult> Add(WhTemplateDto obj)
		{
			try
			{
				var chk = await permission.HasPermission(806, PermissionType.Add);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				if (!ModelState.IsValid)
					return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

				var items = await whServiceManager.WhTemplateService.Add(obj);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhTemplateDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

			}

		}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(WhTemplateEditDto entity)
		{
			try
			{
				var chk = await permission.HasPermission(806, PermissionType.Edit);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				var items = await whServiceManager.WhTemplateService.Update(entity);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhTemplateEditDto>.FailAsync($"======= Exp: {ex.Message}"));

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
				var items = await whServiceManager.WhTemplateService.Remove(Id);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhTemplateDto>.FailAsync($"======= Exp : {ex.Message}"));

			}

		}

	}
}
