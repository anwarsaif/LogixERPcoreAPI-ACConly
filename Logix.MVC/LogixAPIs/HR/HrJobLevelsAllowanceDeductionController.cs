using Logix.Application.Common;
using Logix.Application.DTOs.HR;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Wrapper;
using Logix.MVC.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.HR
{
	public class HrJobLevelsAllowanceDeductionController : BaseHrApiController
	{
		private readonly IMainServiceManager mainServiceManager;
		private readonly IHrServiceManager hrServiceManager;
		private readonly IPermissionHelper permission;
		private readonly ICurrentData session;
		private readonly ILocalizationService localization;

		public HrJobLevelsAllowanceDeductionController(IHrServiceManager hrServiceManager, IMainServiceManager mainServiceManager, IPermissionHelper permission, ICurrentData session, ILocalizationService localization)
		{
			this.mainServiceManager = mainServiceManager;
			this.permission = permission;
			this.session = session;
			this.hrServiceManager = hrServiceManager;
			this.localization = localization;
		}
		[HttpPost("Add")]
		public async Task<ActionResult> Add(List<HrJobLevelsAllowanceDeductionDto> obj)
		{
			try
			{
				var chk = await permission.HasPermission(572, PermissionType.Add);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				if (!ModelState.IsValid)
					return Ok(await Result.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

				var add = await hrServiceManager.HrJobLevelsAllowanceDeductionService.Add(obj.FirstOrDefault());
				return Ok(add);
			}
			catch (Exception ex)
			{
				return Ok(await Result.FailAsync($"Exception in Add HrJobAllowanceDeduction Controller: {ex.Message}"));
			}
		}

		[HttpPost("Edit")]
		public async Task<ActionResult> Edit(List<HrJobLevelsAllowanceDeductionEditDto> obj)
		{
			try
			{
				var chk = await permission.HasPermission(572, PermissionType.Edit);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				if (!ModelState.IsValid)
					return Ok(await Result<HrJobLevelsAllowanceDeductionEditDto>.FailAsync($"{localization.GetMessagesResource("dataRequire")}"));

				var update = await hrServiceManager.HrJobLevelsAllowanceDeductionService.Update(obj.FirstOrDefault());
				return Ok(update);
			}
			catch (Exception ex)
			{
				return Ok(await Result<HrJobLevelsAllowanceDeductionDto>.FailAsync($"====== Exp in Edit Hr Job Level Controller, MESSAGE: {ex.Message}"));
			}
		}

		[HttpGet("GetById")]
		public async Task<IActionResult> Edit(long Id)
		{
			try
			{
				//var chk = await permission.HasPermission(572, PermissionType.Edit);
				//if (!chk)
				//	return Ok(await Result.AccessDenied("AccessDenied"));

				if (Id <= 0)
				{
					return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
				}

				var item = await hrServiceManager.HrJobLevelService.GetOneVW(x => x.Id == Id);
				if (item.Succeeded)
				{
					if (item.Data != null)
					{
						var result = new HrJobLevelName
						{
							LevelName = item.Data.LevelName,
						};
						return Ok(await Result<HrJobLevelName>.SuccessAsync(result));
					}
				}
				return Ok(await Result<HrJobLevelName>.FailAsync(item.Status.message));
			}
			catch (Exception ex)
			{
				return Ok(await Result<HrJobLevelName>.FailAsync($"====== Exp in HR job Controller getById, MESSAGE: {ex.Message}"));
			}
		}
		[HttpGet("GetAllowancesByJobId")]
		public async Task<IActionResult> GetAllowancesByJobId(long JobId)
		{
			try
			{
				var chk = await permission.HasPermission(572, PermissionType.Edit);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				if (JobId <= 0)
				{
					return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
				}

				var item = await hrServiceManager.HrJobLevelsAllowanceDeductionService.GetAllVW(x=>x.LevelId==JobId && x.IsDeleted==false);
				return Ok(item);
			}
			catch (Exception ex)
			{
				return Ok(await Result<HrJobLevelsAllowanceDeductionDto>.FailAsync($"====== Exp in Hr Job Grade Controller getById, MESSAGE: {ex.Message}"));
			}
		}

		[HttpGet("GetDeductionsByJobId")]
		public async Task<IActionResult> GetDeductionsByJobId(long JobId)
{
			try
			{
				var chk = await permission.HasPermission(572, PermissionType.Edit);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				if (JobId <= 0)
				{
					return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInUpdate")}"));
				}

				var item = await hrServiceManager.HrJobLevelsAllowanceDeductionService.GetOne(x => x.Id == JobId);
				return Ok(item);
			}
			catch (Exception ex)
			{
				return Ok(await Result<HrJobLevelsAllowanceDeductionDto>.FailAsync($"====== Exp in Hr Job Grade Controller getById, MESSAGE: {ex.Message}"));
			}
		}

		[HttpDelete("Delete")]
		public async Task<IActionResult> Delete(long id)
		{
			try
			{
				var chk = await permission.HasPermission(572, PermissionType.Delete);
				if (!chk)
					return Ok(await Result.AccessDenied("AccessDenied"));

				if (id <= 0)
					return Ok(await Result.FailAsync($"{localization.GetMessagesResource("NoIdInDelete")}"));

				var delete = await hrServiceManager.HrJobLevelsAllowanceDeductionService.Remove(id);
				return Ok(delete);
			}
			catch (Exception ex)
			{
				return Ok(await Result.FailAsync($"====== Exp in Delete Hr Job Level Controller, MESSAGE: {ex.Message}"));
			}
		}
	}
}