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
    public class AccBranchAccountTypeService : IAccBranchAccountTypeService
    {
        private readonly IAccRepositoryManager _accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;

        public AccBranchAccountTypeService(IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session)
        {
            this._accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;
            this.session = session;
        }
        public async Task<IResult<AccBranchAccountTypeDto>> Add(AccBranchAccountTypeDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccBranchAccountTypeDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");

            try
            {
                entity.CreatedOn = DateTime.Now;
                entity.CreatedBy = session.UserId;
                var newEntity = await _accRepositoryManager.AccBranchAccountTypeRepository.AddAndReturn(_mapper.Map<AccBranchAccountType>(entity));

                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccBranchAccountTypeDto>(newEntity);

                return await Result<AccBranchAccountTypeDto>.SuccessAsync(entityMap, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<AccBranchAccountTypeDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public Task<IResult> ChangeActive(AccBranchAccountTypeDto entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<IEnumerable<AccBranchAccountTypeDto>>> Find(Expression<Func<AccBranchAccountTypeDto, bool>> expression, CancellationToken cancellationToken = default)
        {
            try
            {
                var mapExpr = _mapper.Map<Expression<Func<AccBranchAccountType, bool>>>(expression);
                var items = await _accRepositoryManager.AccBranchAccountTypeRepository.GetAll(mapExpr);
                var itemMap = _mapper.Map<IEnumerable<AccBranchAccountTypeDto>>(items);
                //if (items == null || items.Any() == false) return await Result<IEnumerable<AccBranchAccountTypeDto>>.FailAsync("No Data");
                return await Result<IEnumerable<AccBranchAccountTypeDto>>.SuccessAsync(itemMap);
            }
            catch (Exception exp)
            {
                return await Result<IEnumerable<AccBranchAccountTypeDto>>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}");
            }
        }

        public Task<IResult<DtResult<AccBranchAccountTypeDto>>> Find(DtRequest dtRequest, Expression<Func<AccBranchAccountTypeDto, bool>> expression, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<IEnumerable<AccBranchAccountTypeDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            var items = await _accRepositoryManager.AccBranchAccountTypeRepository.GetAll();

            var itemMap = _mapper.Map<IEnumerable<AccBranchAccountTypeDto>>(items);
            if (items == null) return await Result<IEnumerable<AccBranchAccountTypeDto>>.FailAsync("No Data Found");
            return await Result<IEnumerable<AccBranchAccountTypeDto>>.SuccessAsync(itemMap, "records retrieved");
        }

        public Task<IResult<DtResult<AccBranchAccountTypeDto>>> GetAll(DtRequest dtRequest, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<IEnumerable<AccBranchAccountTypeDto>>> GetAll(Expression<Func<AccBranchAccountTypeDto, bool>> expression, CancellationToken cancellationToken = default)
        {
            try
            {
                var mapExpr = _mapper.Map<Expression<Func<AccBranchAccountType, bool>>>(expression);
                var items = await _accRepositoryManager.AccBranchAccountTypeRepository.GetAll(mapExpr);
                var itemMap = _mapper.Map<IEnumerable<AccBranchAccountTypeDto>>(items);
                //if (items == null || items.Any() == false) return await Result<IEnumerable<AccBranchAccountTypeDto>>.FailAsync("No Data");
                return await Result<IEnumerable<AccBranchAccountTypeDto>>.SuccessAsync(itemMap);
            }
            catch (Exception exp)
            {
                return await Result<IEnumerable<AccBranchAccountTypeDto>>.FailAsync($"EXP in {this.GetType()}, Meesage: {exp.Message}");
            }
        }

        public Task<IResult<DtResult<AccBranchAccountTypeDto>>> GetAll(DtRequest dtRequest, Expression<Func<AccBranchAccountTypeDto, bool>> expression, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<AccBranchAccountTypeDto>> GetById(int Id, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await _accRepositoryManager.AccBranchAccountTypeRepository.GetById(Id);
                if (item == null) return Result<AccBranchAccountTypeDto>.Fail($"--- there is no Data with this id: {Id}---");
                var newEntity = _mapper.Map<AccBranchAccountTypeDto>(item);
                return await Result<AccBranchAccountTypeDto>.SuccessAsync(newEntity, "");

            }
            catch (Exception ex)
            {
                return Result<AccBranchAccountTypeDto>.Fail($"Exp in get data by Id: {ex.Message} --- {(ex.InnerException != null ? "InnerExp: " + ex.InnerException.Message : "no inner")} .");
            }
        }


        public async Task<IResult<AccBranchAccountTypeDto>> GetById(long Id, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await _accRepositoryManager.AccBranchAccountTypeRepository.GetById(Id);
                if (item == null) return Result<AccBranchAccountTypeDto>.Fail($"--- there is no Data with this id: {Id}---");
                var newEntity = _mapper.Map<AccBranchAccountTypeDto>(item);
                return await Result<AccBranchAccountTypeDto>.SuccessAsync(newEntity, "");

            }
            catch (Exception ex)
            {
                return Result<AccBranchAccountTypeDto>.Fail($"Exp in get data by Id: {ex.Message} --- {(ex.InnerException != null ? "InnerExp: " + ex.InnerException.Message : "no inner")} .");
            }
        }
        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await _accRepositoryManager.AccBranchAccountTypeRepository.GetById(Id);
            if (item == null) return Result<AccBranchAccountTypeDto>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = session.UserId;
            _accRepositoryManager.AccBranchAccountTypeRepository.Update(item);
            try
            {
                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccBranchAccountTypeDto>.SuccessAsync(_mapper.Map<AccBranchAccountTypeDto>(item), " record removed");
            }
            catch (Exception exp)
            {
                return await Result<AccBranchAccountTypeDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IResult<AccBranchAccountTypeDto>> Update(AccBranchAccountTypeDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccBranchAccountTypeDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");

            try
            {
                entity.ModifiedOn = DateTime.Now;
                entity.ModifiedBy = session.UserId;
                _accRepositoryManager.AccBranchAccountTypeRepository.Update(_mapper.Map<AccBranchAccountType>(entity));

                await _accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccBranchAccountTypeDto>.SuccessAsync(entity, "item updated successfully");
            }
            catch (Exception exc)
            {
                return await Result<AccBranchAccountTypeDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }
        }

    }
}
