using Logix.Application.Common;
using Logix.Application.DTOs.WH;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Logix.MVC.LogixAPIs.WH
{
	public class WhItemsBatchController : BaseWHApiController
	{
		private readonly IWhServiceManager whServiceManager;
		private readonly IAccServiceManager accServiceManager;
		private readonly IMainServiceManager mainServiceManager;
		private readonly IPermissionHelper permission;
		private readonly ICurrentData session;
		private readonly IHrServiceManager hrServiceManager;
		private readonly ILocalizationService localization;

		public WhItemsBatchController(
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
				var chk = await permission.HasPermission(880, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}

				var items = await whServiceManager.WhItemsBatchService.GetAll(e => e.IsDeleted == false);
				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemsBatchDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}
		[HttpGet("GetByIdForEdit")]
		public async Task<IActionResult> GetByIdForEdit(long Id)
		{
			try
			{
				var chkView = await permission.HasPermission(880, PermissionType.View);
				var chkEdit = await permission.HasPermission(880, PermissionType.Edit);
				if (!chkView && !chkEdit)
					return Ok(await Result.AccessDenied("AccessDenied"));		

				var items = await whServiceManager.WhItemsBatchService.GetForUpdate<WhItemsBatchEditDto>(Id);
				if (items.Succeeded && items.Data != null)
				{
					// التحقق من وجود ItemId
					if (items.Data.ItemId.HasValue && items.Data.ItemId.Value != 0)
					{
						var getItemId = await whServiceManager.WhItemService.GetById(items.Data.ItemId.Value);

						// التحقق من نجاح العملية ووجود البيانات
						if (getItemId.Succeeded && getItemId.Data != null)
						{
							items.Data.ItemCode = !string.IsNullOrEmpty(getItemId.Data.ItemCode)
								? getItemId.Data.ItemCode
								: "No Item Code";

							items.Data.ItemName = !string.IsNullOrEmpty(getItemId.Data.ItemName)
								? getItemId.Data.ItemName
								: "No Item Name";
						}
						else
						{
							items.Data.ItemCode = "No Item Code";
							items.Data.ItemName = "No Item Name";
						}
					}
					else
					{
						items.Data.ItemCode = "No Item Code";
						items.Data.ItemName = "No Item Name";
					}
				}

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemsBatchDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}

		[HttpGet("GetById")]
		public async Task<IActionResult> GetById(long Id)
		{
			try
			{
				var chk = await permission.HasPermission(880, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				var items = await whServiceManager.WhItemsBatchService.GetById(Id);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemsBatchDto>>.FailAsync($"======= Exp: {ex.Message}"));

			}
		}

		[HttpPost("Search")]
		public async Task<IActionResult> Search(WhItemsBatchFilterDto filter)
		{
			try
			{
				var chk = await permission.HasPermission(880, PermissionType.Show);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}

				var items = await whServiceManager.WhItemsBatchService.GetAllVW(e => e.IsDeleted == false 
					&& (string.IsNullOrEmpty(filter.ItemName) || e.ItemName == filter.ItemName)
					&& (string.IsNullOrEmpty(filter.ItemCode) || e.ItemCode.Contains(filter.ItemCode))
					&& (string.IsNullOrEmpty(filter.BatchNo) || e.BatchNo.Contains(filter.BatchNo))
					&& (string.IsNullOrEmpty(filter.ExpiryDate) ||
                                                       (DateTime.Parse(e.ExpiryDate) >= DateTime.Parse(filter.StartDate) &&
                                                        DateTime.Parse(e.ExpiryDate) <= DateTime.Parse(filter.EndDate)))
				);

				if (items.Succeeded)
				{
					// filter by date
					var res = items.Data;
					if (!string.IsNullOrEmpty(filter.StartDate) && !string.IsNullOrEmpty(filter.EndDate))
					{
						DateTime startDate = DateTime.ParseExact(filter.StartDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);
						DateTime endDate = DateTime.ParseExact(filter.EndDate ?? "", "yyyy/MM/dd", CultureInfo.InvariantCulture);

						res = res.Where(r => !string.IsNullOrEmpty(r.ExpiryDate) && DateTime.ParseExact(r.ExpiryDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= startDate
							&& DateTime.ParseExact(r.ExpiryDate, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= endDate);
					}
				}
					return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<IEnumerable<WhItemsBatchDto>>.FailAsync($"Error: {ex.Message}"));
			}
		}

		[HttpPost("Add")]
		public async Task<IActionResult> Add(WhItemsBatchDto obj)
		{
			try
			{
				var chk = await permission.HasPermission(880, PermissionType.Add);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				if (!ModelState.IsValid)
					return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

				var items = await whServiceManager.WhItemsBatchService.Add(obj);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhItemsBatchDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

			}

		}

		[HttpPost("Edit")]
		public async Task<IActionResult> Edit(WhItemsBatchEditDto entity)
		{
			try
			{
				var chk = await permission.HasPermission(880, PermissionType.Edit);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				var items = await whServiceManager.WhItemsBatchService.Update(entity);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhItemsBatchEditDto>.FailAsync($"======= Exp: {ex.Message}"));

			}

		}

		[HttpDelete("Delete")]
		public async Task<IActionResult> Delete(long Id = 0)
		{
			try
			{
				var chk = await permission.HasPermission(880, PermissionType.Delete);
				if (!chk)
				{
					return Ok(await Result.AccessDenied("AccessDenied"));
				}
				var items = await whServiceManager.WhItemsBatchService.Remove(Id);

				return Ok(items);
			}
			catch (Exception ex)
			{
				return Ok(await Result<WhItemsBatchDto>.FailAsync($"======= Exp : {ex.Message}"));

			}

		}

	}
}
