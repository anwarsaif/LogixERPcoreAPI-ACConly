using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Services.ACC
{
    public class AccBranchAccountService : IAccBranchAccountService
    {
        private readonly IAccRepositoryManager _accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public AccBranchAccountService(IAccRepositoryManager accRepositoryManager, 
            IMapper mapper, 
            ICurrentData session,
            ILocalizationService localization)
        {
            this._accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;
            this.session = session;
            this.localization = localization;
        }
        public async Task<IResult<AccBranchAccountDto>> Add(AccBranchAccountDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccBranchAccountDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");

            try
            {
                entity.CreatedOn = DateTime.Now;
                entity.CreatedBy = session.UserId;
                var newEntity = await _accRepositoryManager.AccBranchAccountRepository.AddAndReturn(_mapper.Map<AccBranchAccount>(entity));

                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccBranchAccountDto>(newEntity);

                return await Result<AccBranchAccountDto>.SuccessAsync(entityMap, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<AccBranchAccountDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public Task<IResult> ChangeActive(AccBranchAccountDto entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<IEnumerable<AccBranchAccountDto>>> GetAll(Expression<Func<AccBranchAccountDto, bool>> expression, CancellationToken cancellationToken = default)
        {
            try
            {
                var mapExpr = _mapper.Map<Expression<Func<AccBranchAccount, bool>>>(expression);
                var items = await _accRepositoryManager.AccBranchAccountRepository.GetAll(mapExpr);
                var itemMap = _mapper.Map<IEnumerable<AccBranchAccountDto>>(items);
                //if (items == null || items.Any() == false) return await Result<IEnumerable<AccBranchAccountDto>>.FailAsync("No Data");
                return await Result<IEnumerable<AccBranchAccountDto>>.SuccessAsync(itemMap);
            }
            catch (Exception exp)
            {
                return await Result<IEnumerable<AccBranchAccountDto>>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}");
            }
        }

        public Task<IResult<DtResult<AccBranchAccountDto>>> GetAll(DtRequest dtRequest, Expression<Func<AccBranchAccountDto, bool>> expression, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<IEnumerable<AccBranchAccountDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            var items = await _accRepositoryManager.AccBranchAccountRepository.GetAll();

            var itemMap = _mapper.Map<IEnumerable<AccBranchAccountDto>>(items);
            if (items == null) return await Result<IEnumerable<AccBranchAccountDto>>.FailAsync("No Data Found");
            return await Result<IEnumerable<AccBranchAccountDto>>.SuccessAsync(itemMap, "records retrieved");
        }

        public async Task<IResult<IEnumerable<AccBranchAccountsVwsDto>>> GetAllVW(CancellationToken cancellationToken = default)
        {
            var items = await _accRepositoryManager.AccBranchAccountRepository.GetAllVW();
            var itemMap = _mapper.Map<IEnumerable<AccBranchAccountsVwsDto>>(items);
            if (items == null) return await Result<IEnumerable<AccBranchAccountsVwsDto>>.FailAsync("No Data Found");
            return await Result<IEnumerable<AccBranchAccountsVwsDto>>.SuccessAsync(itemMap, "records retrieved");
        }

        public Task<IResult<DtResult<AccBranchAccountDto>>> GetAll(DtRequest dtRequest, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<AccBranchAccountDto>> GetById(int Id, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await _accRepositoryManager.AccBranchAccountRepository.GetById(Id);
                if (item == null) return Result<AccBranchAccountDto>.Fail($"--- there is no Data with this id: {Id}---");
                var newEntity = _mapper.Map<AccBranchAccountDto>(item);
                return await Result<AccBranchAccountDto>.SuccessAsync(newEntity, "");

            }
            catch (Exception ex)
            {
                return Result<AccBranchAccountDto>.Fail($"Exp in get data by Id: {ex.Message} --- {(ex.InnerException != null ? "InnerExp: " + ex.InnerException.Message : "no inner")} .");
            }
        }


        public async Task<IResult<AccBranchAccountDto>> GetById(long Id, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await _accRepositoryManager.AccBranchAccountRepository.GetById(Id);
                if (item == null) return Result<AccBranchAccountDto>.Fail($"--- there is no Data with this id: {Id}---");
                var newEntity = _mapper.Map<AccBranchAccountDto>(item);
                return await Result<AccBranchAccountDto>.SuccessAsync(newEntity, "");

            }
            catch (Exception ex)
            {
                return Result<AccBranchAccountDto>.Fail($"Exp in get data by Id: {ex.Message} --- {(ex.InnerException != null ? "InnerExp: " + ex.InnerException.Message : "no inner")} .");
            }
        }
        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await _accRepositoryManager.AccBranchAccountRepository.GetById(Id);
            if (item == null) return Result<AccBranchAccountDto>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = session.UserId;
            _accRepositoryManager.AccBranchAccountRepository.Update(item);
            try
            {
                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccBranchAccountDto>.SuccessAsync(_mapper.Map<AccBranchAccountDto>(item), " record removed");
            }
            catch (Exception exp)
            {
                return await Result<AccBranchAccountDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<AccBranchAccountDto>> Update(AccBranchAccountDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccBranchAccountDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");

            try
            {
                entity.ModifiedOn = DateTime.Now;
                entity.ModifiedBy = session.UserId;
                _accRepositoryManager.AccBranchAccountRepository.Update(_mapper.Map<AccBranchAccount>(entity));

                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccBranchAccountDto>.SuccessAsync(entity, "item updated successfully");
            }
            catch (Exception exc)
            {
                return await Result<AccBranchAccountDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }
        }

        public async Task<IResult<IEnumerable<AccBranchAccountsVwsDto>>> Update(IEnumerable<AccBranchAccountsVwsDto> entities, CancellationToken cancellationToken = default)
        {
            if (entities == null) return await Result<IEnumerable<AccBranchAccountsVwsDto>>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");

            try
            {
                foreach (var item in entities)
                {
                    AccBranchAccountDto accBranchAccountDto = new AccBranchAccountDto();
                    //suppose that.. the user delete any account code, by clear the input field (make it null)
                    if (string.IsNullOrEmpty(item.AccAccountCode))
                    {
                        var branch = await _accRepositoryManager.AccBranchAccountRepository.GetOne(a => a.BranchId == item.BranchId && a.BrAccTypeId == item.BrAccTypeId && a.IsDeleted == false);
                        
                        if(branch != null)
                        {
                            //branch.AccountId = null;
                            branch.AccountId = 0;
                            //branch.IsDeleted = true;
                            branch.ModifiedBy = session.UserId;
                            branch.ModifiedOn = DateTime.Now;
                            _accRepositoryManager.AccBranchAccountRepository.Update(branch);
                            await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        }
                    }
                    else
                    {
                        var account = await _accRepositoryManager.AccAccountRepository.GetOne(a => a.AccAccountCode == item.AccAccountCode && a.IsDeleted == false && a.FacilityId == session.FacilityId);
                        if (account == null)
                            return await Result<IEnumerable<AccBranchAccountsVwsDto>>.FailAsync($"{item.AccAccountCode} {localization.GetResource1("AccountNotExsists")}");

                        var accBranch = await _accRepositoryManager.AccBranchAccountRepository.GetOne(a => a.BranchId == item.BranchId && a.BrAccTypeId == item.BrAccTypeId && a.IsDeleted == false);

                        if (accBranch == null)
                        {
                            //Add
                            accBranchAccountDto.BrAccTypeId = item.BrAccTypeId;
                            accBranchAccountDto.BranchId = item.BranchId;
                            accBranchAccountDto.AccountId = account.AccAccountId;
                            var add = await Add(accBranchAccountDto, cancellationToken);
                        }
                        else
                        {
                            //Update
                            accBranch.AccountId = account.AccAccountId;
                            accBranch.ModifiedBy = session.UserId;
                            accBranch.ModifiedOn = DateTime.Now;
                            _accRepositoryManager.AccBranchAccountRepository.Update(accBranch);
                            await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                        }
                    }
                }
                return await Result<IEnumerable<AccBranchAccountsVwsDto>>.SuccessAsync(entities, "items updated successfully");
            }
            catch (Exception exc)
            {
                return await Result<IEnumerable<AccBranchAccountsVwsDto>>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }
        }

    }
}
