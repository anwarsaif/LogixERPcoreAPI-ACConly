using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccBankChequeBookChequeBookService : GenericQueryService<AccBankChequeBook, AccBankChequeBookDto, AccBankChequeBook>, IAccBankChequeBookService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;

        public AccBankChequeBookChequeBookService(IQueryRepository<AccBankChequeBook> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;

            this.session = session;
        }

        public async Task<IResult<AccBankChequeBookDto>> Add(AccBankChequeBookDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccBankChequeBookDto>.FailAsync($"Error in Add of: {this.GetType()}, the passed entity is NULL !!!.");

            try
            {

                var item = _mapper.Map<AccBankChequeBook>(entity);
                var newEntity = await accRepositoryManager.AccBankChequeBookRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccBankChequeBookDto>(newEntity);


                return await Result<AccBankChequeBookDto>.SuccessAsync(entityMap, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<AccBankChequeBookDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccBankChequeBookRepository.GetById(Id);
            if (item == null) return Result<AccBankChequeBookDto>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccBankChequeBookRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccBankChequeBookDto>.SuccessAsync(_mapper.Map<AccBankChequeBookDto>(item), " record removed");
            }
            catch (Exception exp)
            {
                return await Result<AccBankChequeBookDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccBankChequeBookRepository.GetById(Id);
            if (item == null) return Result<AccBankChequeBookDto>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccBankChequeBookRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccBankChequeBookDto>.SuccessAsync(_mapper.Map<AccBankChequeBookDto>(item), " record removed");
            }
            catch (Exception exp)
            {
                return await Result<AccBankChequeBookDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccBankChequeBookEditDto>> Update(AccBankChequeBookEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccBankChequeBookEditDto>.FailAsync($"Error in {this.GetType()} : the passed entity IS NULL.");

            var item = await accRepositoryManager.AccBankChequeBookRepository.GetById(entity.Id);

            if (item == null) return await Result<AccBankChequeBookEditDto>.FailAsync($"--- there is no Data with this id: {entity.Id}---");
            item.IsDeleted = false;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;
            _mapper.Map(entity, item);

            accRepositoryManager.AccBankChequeBookRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccBankChequeBookEditDto>.SuccessAsync(_mapper.Map<AccBankChequeBookEditDto>(item), "Item updated successfully");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccBankChequeBookEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

    }
}
