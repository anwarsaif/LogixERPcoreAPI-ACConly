using Logix.Application.Common;
using Logix.Application.DTOs.PM;
using Logix.Application.DTOs.PM.PmExtractAdditionalType;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Services.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.PM;
using Logix.MVC.Helpers;
using Logix.MVC.LogixAPIs.PM.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Logix.MVC.LogixAPIs.PM
{
    public class PMExtractAdditionalTypeApiController : BasePMApiController
    {
        private readonly IPMServiceManager pMServiceManager;
        private readonly IPermissionHelper permission;
        private readonly ICurrentData session;
        private readonly IAccServiceManager accServiceManager;

        public PMExtractAdditionalTypeApiController(IPMServiceManager pMServiceManager, 
            IPermissionHelper permission,
            ICurrentData session,
               IAccServiceManager accServiceManager

            )
        {
            this.pMServiceManager = pMServiceManager;
            this.permission = permission;
            this.session = session;
            this.accServiceManager = accServiceManager;
        }
        /*[HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var chk = await permission.HasPermission(256, PermissionType.Show);
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await pMServiceManager.PMProjectsTypeService.GetAll(e => e.IsDeleted == false);
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<PmProjectsTypeDto>>.SuccessAsync(res.ToList(),items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmProjectsType>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }
        
          */
        [HttpPost("Search")]
        public async Task<IActionResult> Search(PMExtractAdditinalTypeVMFilter filter)
        {
            var chk = await permission.HasPermission(899, PermissionType.Show);
          
            if (!chk )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await pMServiceManager.PMExtractAdditionalTypeService.GetAllVW(
                    e => e.IsDeleted == false && e.FacilityId == session.FacilityId ) ;
                
                if (items.Succeeded)
                {
                    var res = items.Data.AsQueryable();
                    if (filter != null)
                    {
                        if (filter.Code != null && filter.Code !=0)
                        {
                            res = res.Where(x=> x.Code == filter.Code);

                        } 
                        if (filter.CreditOrDebit != null && filter.CreditOrDebit != 0)
                        {
                            res = res.Where(x=> x.CreditOrDebit == filter.CreditOrDebit);

                        }
                        if(filter.TypeId != null && filter.CreditOrDebit != 0)
                        {
                            res = res.Where(res=> res.TypeId == filter.TypeId);
                        }
                        if (filter.AccRefTypeId != null && filter.AccRefTypeId != 0)
                        {
                           res= res.Where(x=> x.AccRefTypeId == filter.AccRefTypeId);
                        }
                        if(filter.RateOrAmount != null && filter.RateOrAmount != 0)
                        {
                            res=res.Where(x=> x.RateOrAmount == filter.RateOrAmount);
                        }
                        if(filter.Name is not null)
                        {
                           res= res.Where(x=> x.Name.Contains( filter.Name));
                        }

                    }
                    res = res.OrderBy(e => e.Id);
                    return Ok(await Result<List<PmExtractAdditionalTypeVw>>.SuccessAsync(res.ToList(), items.Status.message));
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmExtractAdditionalTypeVw>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }

        [HttpGet("GetByIdForEdit")]
        public async Task<IActionResult> GetByIdForEdit(long id)
        {
            var chk = await permission.HasPermission(899, PermissionType.Show);
          
            if (!chk )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }

            try
            {

                var items = await pMServiceManager.PMExtractAdditionalTypeService.GetOneVW(
                    e => e.IsDeleted == false && e.FacilityId == session.FacilityId && e.Id==id) ;
                
            
                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<List<PmExtractAdditionalTypeVw>>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }
        }


        [HttpPost("Add")]
        public async Task<IActionResult> Add(PmExtractAdditionalTypeAddDto entity)
        {

            var chk = await permission.HasPermission(899, PermissionType.Add);
            
            //var chk2 = await permission.HasPermission(410, PermissionType.Add);
            if (!chk )
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }




            try
            {

                entity.FacilityId = (int)session.FacilityId;
                var account = await accServiceManager.AccAccountsSubHelpeVwService.GetAllVW(x => x.AccAccountCode == entity.AccAccountCode);

                long accountId = 0;
                if (account==null || account.Data.Count()==0 || account.Data.Count()>1)
                {

                    return Ok(await Result<PmExtractAdditionalTypeAddDto>.FailAsync("لا يوجد حساب بهذا الرقم"));

                }
                
                accountId = account.Data.FirstOrDefault().AccAccountId;

               entity.AccountId = accountId;
                var items = await pMServiceManager.PMExtractAdditionalTypeService.Add(entity);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmExtractAdditionalTypeAddDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }

        }

        [HttpPost("Edit")]
        public async Task<IActionResult> Edit(PmExtractAdditionalTypeEditDto entity)
        {
            var chk = await permission.HasPermission(899, PermissionType.Edit);

            if (!chk )
            {

                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {

                entity.FacilityId = (int)session.FacilityId;
                var account = await accServiceManager.AccAccountsSubHelpeVwService.GetAllVW(x => x.AccAccountCode == entity.AccAccountCode);

                long accountId = 0;
                if (account == null || account.Data.Count() == 0 || account.Data.Count() > 1)
                {

                    return Ok(await Result<PmExtractAdditionalTypeEditDto>.FailAsync("لا يوجد حساب بهذا الرقم"));

                }
                accountId = account.Data.FirstOrDefault().AccAccountId;

                entity.AccountId = accountId;
                var items = await pMServiceManager.PMExtractAdditionalTypeService.Update(entity); 


                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmExtractAdditionalTypeEditDto>.FailAsync($"======= Exp in Wh whunit : {ex.Message}"));

            }

        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(long id)
        {

            var chk = await permission.HasPermission(899, PermissionType.Delete);
            
            if (!chk)
            {
                return Ok(await Result.AccessDenied("AccessDenied"));
            }
            try
            {
                var items = await pMServiceManager.PMExtractAdditionalTypeService.Remove(id);

                return Ok(items);
            }
            catch (Exception ex)
            {
                return Ok(await Result<PmExtractAdditionalTypeDto>.FailAsync($"======= Exp in Wh whunit delete : {ex.Message}"));
            }

        }


    }


}
