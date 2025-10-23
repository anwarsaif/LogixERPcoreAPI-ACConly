using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccPettyCashDService : GenericQueryService<AccPettyCashD, AccPettyCashDDto, AccPettyCashDVw>, IAccPettyCashDService
    {
        private readonly IAccRepositoryManager accRepositoryManager;

        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public AccPettyCashDService(IQueryRepository<AccPettyCashD> queryRepository, IAccRepositoryManager AccRepositoryManager, IMapper mapper, ICurrentData session, ILocalizationService localization) : base(queryRepository, mapper)
        {
            this.accRepositoryManager = AccRepositoryManager;
            this._mapper = mapper;

            this.session = session;
            this.localization = localization;
        }
        public async Task<IResult<AccPettyCashDDto>> Add(AccPettyCashDDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccPettyCashDDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {

                var item = _mapper.Map<AccPettyCashD>(entity);
                var newEntity = await accRepositoryManager.AccPettyCashDRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccPettyCashDDto>(newEntity);


                return await Result<AccPettyCashDDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {

                return await Result<AccPettyCashDDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult<AccPettyCashDEditDto>> Update(AccPettyCashDEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccPettyCashDEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");

            var item = await accRepositoryManager.AccPettyCashDRepository.GetById(entity.Id);

            if (item == null) return await Result<AccPettyCashDEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
            item.ModifiedOn = DateTime.UtcNow;
            item.ModifiedBy = (int)session.UserId;

            _mapper.Map(entity, item);

            accRepositoryManager.AccPettyCashDRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccPettyCashDEditDto>.SuccessAsync(_mapper.Map<AccPettyCashDEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccPettyCashDEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccPettyCashDRepository.GetById(Id);
            if (item == null) return Result<AccPettyCashDDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.UtcNow;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccPettyCashDRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccPettyCashDDto>.SuccessAsync(_mapper.Map<AccPettyCashDDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccPettyCashDDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccPettyCashDRepository.GetById(Id);
            if (item == null) return Result<AccPettyCashDDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.UtcNow;
            item.ModifiedBy = (int)session.UserId;
            accRepositoryManager.AccPettyCashDRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccPettyCashDDto>.SuccessAsync(_mapper.Map<AccPettyCashDDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccPettyCashDDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
    }

}
