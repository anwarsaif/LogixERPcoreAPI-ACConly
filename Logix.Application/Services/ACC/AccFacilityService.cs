using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Services.ACC
{
    public class AccFacilityService : GenericQueryService<AccFacility, AccFacilityDto, AccFacilitiesVw>, IAccFacilityService
    {
        private readonly IAccRepositoryManager _accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public AccFacilityService(IQueryRepository<AccFacility> queryRepository,
            IMapper mapper,
            IAccRepositoryManager accRepositoryManager,
            ICurrentData session,
            ILocalizationService localization) : base(queryRepository, mapper)
        {
            this._accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;
            this.session = session;
            this.localization = localization;
        }

        public async Task<IResult<AccFacilityDto>> Add(AccFacilityDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccFacilityDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {
                var newEntity = await _accRepositoryManager.AccFacilityRepository.AddAndReturn(_mapper.Map<AccFacility>(entity));

                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccFacilityDto>(newEntity);

                return await Result<AccFacilityDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {

                return await Result<AccFacilityDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }
        }

        public async Task<IResult<AccFacilityEditDto>> Update(AccFacilityEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccFacilityEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");
            try
            {
                var item = await _accRepositoryManager.AccFacilityRepository.GetById(entity.FacilityId);

                if (item == null) return await Result<AccFacilityEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
                item.ModifiedOn = DateTime.Now;
                item.ModifiedBy = (int?)session.UserId;

                _mapper.Map(entity, item);

                _accRepositoryManager.AccFacilityRepository.Update(item);

                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccFacilityEditDto>.SuccessAsync(_mapper.Map<AccFacilityEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccFacilityEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        //More function
        public async Task<IResult> UpdateStamp(long facilityId, string newStampUrl, CancellationToken cancellationToken = default)
        {
            var item = await _accRepositoryManager.AccFacilityRepository.GetOne(f => f.FacilityId == facilityId);
            if (item == null) return Result<AccFacilityDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));
            try
            {
                item.Stamp = newStampUrl;
                item.ModifiedBy = session.UserId;
                item.ModifiedOn = DateTime.Now;

                _accRepositoryManager.AccFacilityRepository.Update(item);
                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                return await Result.SuccessAsync("record Update");
            }
            catch (Exception exp)
            {
                return await Result.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccFacilityEditProfileDto>> GetForUpdateProfile(long Id, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await _accRepositoryManager.AccFacilityRepository.GetById(Id);
                if (item == null) return Result<AccFacilityEditProfileDto>.Fail($"--- there is no Data with this id: {Id}---");
                var newEntity = _mapper.Map<AccFacilityEditProfileDto>(item);
                return await Result<AccFacilityEditProfileDto>.SuccessAsync(newEntity, "");
            }
            catch (Exception ex)
            {
                return Result<AccFacilityEditProfileDto>.Fail($"Exp in get data by Id: {ex.Message} --- {(ex.InnerException != null ? "InnerExp: " + ex.InnerException.Message : "no inner")} .");
            }
        }

        public async Task<IResult> UpdateLogo(long ID, string value, long TypeID, CancellationToken cancellationToken = default)
        {
            var item = await _accRepositoryManager.AccFacilityRepository.GetById(ID);
            if (item == null) return Result<AccFacilityDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));
            try
            {
                if (TypeID == 1)
                {
                    item.FacilityLogo = value;
                }
                else if (TypeID == 2)
                {
                    item.LogoPrint = value;
                }
                else if (TypeID == 3)
                {
                    item.ImgFooter = value;
                }

                _accRepositoryManager.AccFacilityRepository.Update(item);
                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                return await Result<AccFacilityDto>.SuccessAsync(_mapper.Map<AccFacilityDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccFacilityDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccFacilityEditProfileDto>> UpdateProfile(AccFacilityEditProfileDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccFacilityEditProfileDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");

            var item = await _accRepositoryManager.AccFacilityRepository.GetById(entity.FacilityId);

            if (item == null) return await Result<AccFacilityEditProfileDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int?)session.UserId;

            _mapper.Map(entity, item);

            _accRepositoryManager.AccFacilityRepository.Update(item);

            try
            {
                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccFacilityEditProfileDto>.SuccessAsync(_mapper.Map<AccFacilityEditProfileDto>(item), "Item updated successfully");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccFacilityEditProfileDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccFacilityProfileDto>> UpdateProfileEdit(AccFacilityProfileDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccFacilityProfileDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");
            try
            {
                var item = await _accRepositoryManager.AccFacilityRepository.GetById(entity.FacilityId);

                if (item == null) return await Result<AccFacilityProfileDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
                item.ModifiedOn = DateTime.Now;
                item.ModifiedBy = (int?)session.UserId;

                _mapper.Map(entity, item);

                _accRepositoryManager.AccFacilityRepository.Update(item);


                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccFacilityProfileDto>.SuccessAsync(_mapper.Map<AccFacilityProfileDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccFacilityProfileDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        
        public async Task<IResult> UpdateValue(long ID, long value, long TypeID, CancellationToken cancellationToken = default)
        {
            var item = await _accRepositoryManager.AccFacilityRepository.GetById(ID);
            if (item == null) return Result<AccFacilityDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));
            if (TypeID == 1)
            {
                item.IdentitiType = (int)value;

            }
            else if (TypeID == 2)
            {
                item.Posting = (int)value;

            }
            else if (TypeID == 3)
            {
                item.SalesAccountType = (int)value;

            }
            
            if (TypeID == 4)
            {
                item.AccountCash = value;

            }
            else if (TypeID == 5)
            {
                item.AccountChequ = value;

            }
            else if (TypeID == 6)
            {
                item.AccountSupplier = value;

            }
            else if (TypeID == 7)
            {
                item.AccountContractors = value;

            }
            else if (TypeID == 8)
            {
                item.AccountChequUnderCollection = value;

            }
            else if (TypeID == 9)
            {
                item.AccountCashSales = value;

            }
            else if (TypeID == 10)
            {
                item.AccountSalesProfits = value;

            }
            else if (TypeID == 11)
            {
                item.AccountSales = value;

            }
            else if (TypeID == 12)
            {
                item.AccountMerchandiseInventory = value;

            }
            else if (TypeID == 13)
            {
                item.AccountCostGoodsSold = value;

            }
            else if (TypeID == 14)
            {
                item.AccountCashSales = value;

            }
            else if (TypeID == 15)
            {
                item.AccountReceivablesSales = value;

            }
            else if (TypeID == 16)
            {
                item.AccountMembers = value;

            }
            else if (TypeID == 17)
            {
                item.AccountInventoryTransit = value;

            }
            else if (TypeID == 18)
            {
                item.AccountBranches = value;

            }
            else if (TypeID == 19)
            {
                item.GroupAssets = value;
                

            }
            else if (TypeID == 20)
            {
                item.GroupLiabilities = value;


            }
            else if (TypeID == 21)
            {
                item.GroupCopyrights = value;
              

            }
            else if (TypeID == 22)
            {
                item.GroupIncame = value;

            }

            else if (TypeID == 23)
            {
                item.GroupExpenses = value;


            }
            
            else if (TypeID == 24)
            {
                item.CcIdProjects = value;

            }
          
            else if (TypeID == 25)
            {
                item.LnkInoviceInventory = (int)value;

            }
            else if (TypeID == 26)
            {
                item.LnkBillInventroy = (int)value;

            }
            else if (TypeID == 27)
            {
                item.DiscountAccountId = value;

            }
            else if (TypeID == 28)
            {
                item.DiscountCreditAccountId = value;

            }
            else if (TypeID == 29)
            {
                item.ProfitAndLossAccountId = (int)value;

            }

            else if (TypeID == 30)
            {
                item.PurchaseAccountId = (int)value;
                //item.UsingPurchaseAccount = (int)value;

            }
          
           
            _accRepositoryManager.AccFacilityRepository.Update(item);
            try
            {
                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccFacilityDto>.SuccessAsync(_mapper.Map<AccFacilityDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccFacilityDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> UpdateValueVAT(long ID, long value, long value2,bool VATEnable, CancellationToken cancellationToken = default)
        {
            var item = await _accRepositoryManager.AccFacilityRepository.GetById(ID);
            if (item == null) return Result<AccFacilityDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));
            item.SalesVatAccountId = value;
            item.PurchasesVatAccountId = value2;
            item.VatEnable= VATEnable;
            _accRepositoryManager.AccFacilityRepository.Update(item);
            try
            {
                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccFacilityDto>.SuccessAsync(_mapper.Map<AccFacilityDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccFacilityDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<R>> Find<R>(Expression<Func<AccFacilityDto, R>> selector, Expression<Func<AccFacilityDto, bool>> expression, CancellationToken cancellationToken = default)
        {
            try
            {
                var mapExpr = _mapper.Map<Expression<Func<AccFacility, bool>>>(expression);
                var mapExpr2 = _mapper.Map<Expression<Func<AccFacility, R>>>(selector);
                var items = await _accRepositoryManager.AccFacilityRepository.GetAll(mapExpr2, mapExpr);
                // var itemMap = _mapper.Map<IEnumerable<AccFacilityDto>>(items);

                return await Result<R>.SuccessAsync(items.FirstOrDefault());
            }
            catch (Exception exp)
            {
                return await Result<R>.FailAsync($"EXP in {this.GetType()} , Message: {exp.Message}");
            }
        }
        public async Task<IResult> UpdatePurchaseAccount(long ID, long value, bool UsingPurchaseAccount, CancellationToken cancellationToken = default)
        {
            var item = await _accRepositoryManager.AccFacilityRepository.GetById(ID);
            if (item == null) return Result<AccFacilityDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));

            item.PurchaseAccountId = (int)value;
            item.UsingPurchaseAccount = UsingPurchaseAccount;

            _accRepositoryManager.AccFacilityRepository.Update(item);
            try
            {
                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccFacilityDto>.SuccessAsync(_mapper.Map<AccFacilityDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccFacilityDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

    }




}
