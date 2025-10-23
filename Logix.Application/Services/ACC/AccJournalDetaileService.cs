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
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Services.ACC
{
    public class AccJournalDetaileService : GenericQueryService<AccJournalDetaile, AccJournalDetaileDto, AccJournalDetailesVw>, IAccJournalDetaileService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;

        public AccJournalDetaileService(IQueryRepository<AccJournalDetaile> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;

            this.session = session;
        }

        public async Task<IResult<AccJournalDetaileDto>> Add(AccJournalDetaileDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccJournalDetaileDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");

            try
            {

                var item = _mapper.Map<AccJournalDetaile>(entity);
                var newEntity = await accRepositoryManager.AccJournalDetaileRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccJournalDetaileDto>(newEntity);


                return await Result<AccJournalDetaileDto>.SuccessAsync(entityMap, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<AccJournalDetaileDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccJournalDetaileRepository.GetById(Id);
            if (item == null) return Result<AccJournalDetaileDto>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccJournalDetaileRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccJournalDetaileDto>.SuccessAsync(_mapper.Map<AccJournalDetaileDto>(item), " record removed");
            }
            catch (Exception exp)
            {
                return await Result<AccJournalDetaileDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccJournalDetaileRepository.GetById(Id);
            if (item == null) return Result<AccJournalDetaileDto>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccJournalDetaileRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccJournalDetaileDto>.SuccessAsync(_mapper.Map<AccJournalDetaileDto>(item), " record removed");
            }
            catch (Exception exp)
            {
                return await Result<AccJournalDetaileDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccJournalDetailesVw>> SelectACCJournalDetailesDebitor(long JID)
        {
            var result = await accRepositoryManager.AccJournalDetaileRepository.SelectACCJournalDetailesDebitor(JID);

            return result;
        }
        public async Task<IResult<AccJournalDetailesVw>> SelectACCJournalDetailesCredit(long JID)
        {
            var result = await accRepositoryManager.AccJournalDetaileRepository.SelectACCJournalDetailesCredit(JID);

            return result;
        }
        public async Task<IResult<AccJournalDetaileEditDto>> Update(AccJournalDetaileEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccJournalDetaileEditDto>.FailAsync($"Error in {this.GetType()} : the passed entity IS NULL.");

            var item = await accRepositoryManager.AccJournalDetaileRepository.GetById(entity.JDetailesId);

            if (item == null) return await Result<AccJournalDetaileEditDto>.FailAsync($"--- there is no Data with this id: {entity.JDetailesId}---");

            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            _mapper.Map(entity, item);

            accRepositoryManager.AccJournalDetaileRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccJournalDetaileEditDto>.SuccessAsync(_mapper.Map<AccJournalDetaileEditDto>(item), "Item updated successfully");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccJournalDetaileEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

    }
}