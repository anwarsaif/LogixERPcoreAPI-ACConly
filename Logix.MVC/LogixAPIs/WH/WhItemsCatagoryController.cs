using DocumentFormat.OpenXml.Office2010.Excel;
using Logix.Application.Common;
using Logix.Application.DTOs.WH;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.Domain.WH;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.WH
{
	public class WhItemsCatagoryController : BaseWHApiController
	{
		private readonly IWhServiceManager whServiceManager;
		private readonly IAccServiceManager accServiceManager;
		private readonly IMainServiceManager mainServiceManager;
		private readonly IPermissionHelper permission;
		private readonly ICurrentData session;
		private readonly IHrServiceManager hrServiceManager;
		private readonly ILocalizationService localization;

		public WhItemsCatagoryController(
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
				var chk = await permission.HasPermission(205, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}

				var items = await whServiceManager.WhItemsCatagoryService.GetAll(e => e.IsDeleted == false);
				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemsCatagoryDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}
		[HttpGet("GetByIdForEdit")]
		public async Task<IActionResult> GetByIdForEdit(long Id)
		{
			try
			{
				var chkView = await permission.HasPermission(205, PermissionType.View);
				var chkEdit = await permission.HasPermission(205, PermissionType.Edit);
				if (!chkView && !chkEdit)
					return Ok(await Result.AccessDenied("AccessDenied"));
				var items = await whServiceManager.WhItemsCatagoryService.GetOneVW(x => x.CatId == Id);
				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemsCatagoryDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}

		[HttpGet("GetById")]
		public async Task<IActionResult> GetById(long Id)
		{
			try
			{
				var chk = await permission.HasPermission(205, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				var items = await whServiceManager.WhItemsCatagoryService.GetById(Id);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemsCatagoryDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}

		[HttpPost("Search")]
		public async Task<IActionResult> Search(WhItemsCatagoryFilterDto filter)
		{
			try
			{
				var chk = await permission.HasPermission(205, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}

				var items = await whServiceManager.WhItemsCatagoryService.GetAllVW(e => e.IsDeleted == false
					&& (string.IsNullOrEmpty(filter.CatName) || e.CatName.Contains(filter.CatName))
					&& (filter.CatId == 0 || filter.CatId == null || e.CatId == filter.CatId || e.ParentId == filter.CatId || e.ParentParentId == filter.CatId)
					&& (filter.FacilityId == 0 || filter.FacilityId == session.FacilityId )
				);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemsCatagoryDto>>.FailAsync($"Error: {ex.Message}"));
			}
		}

		[HttpPost("Add")]
		public async Task<IActionResult> Add(WhItemsCatagoryDto obj)
		{
			try
			{
				var chk = await permission.HasPermission(205, PermissionType.Add);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				if (!ModelState.IsValid)
					return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

				var items = await whServiceManager.WhItemsCatagoryService.Add(obj);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhItemsCatagoryDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

			}

		}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(WhItemsCatagoryEditDto entity)
		{
			try
			{
				var chk = await permission.HasPermission(205, PermissionType.Edit);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				var items = await whServiceManager.WhItemsCatagoryService.Update(entity);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhItemsCatagoryDto>.FailAsync($"======= Exp: {ex.Message}"));

			}

		}

		[HttpDelete("Delete")]
		public async Task<IActionResult> Delete(long Id)
		{
			try
			{
				var chk = await permission.HasPermission(205, PermissionType.Delete);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				var items = await whServiceManager.WhItemsCatagoryService.Remove(Id);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhItemsCatagoryDto>.FailAsync($"======= Exp : {ex.Message}"));

			}

		}

		[HttpGet("GetOn_Items_Catagories")]
		public async Task<IActionResult> GetOn_Items_Catagories(long catId)
		{
			try
			{
				var items = await whServiceManager.WhItemsCatagoryService.GetOneVW(x => x.CatId == catId && x.IsDeleted == false);

				return Ok(items);
			}
			catch(Exception ex)
			{
				return Ok(await Result<WhItemsCatagoryDto>.FailAsync($"======= Exp : {ex.Message}"));
			}
		}

	}
}
