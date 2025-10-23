using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccJournalCommentService : GenericQueryService<AccJournalComment, AccJournalCommentDto, AccJournalComment>, IAccJournalCommentService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData session;
        private readonly ILocalizationService localization;

        public AccJournalCommentService(IQueryRepository<AccJournalComment> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData session, ILocalizationService localization) : base(queryRepository, mapper)
        {


            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;

            this.session = session;
            this.localization = localization;
        }

        public async Task<IResult<AccJournalCommentDto>> Add(AccJournalCommentDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccJournalCommentDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {

                var item = _mapper.Map<AccJournalComment>(entity);
                var newEntity = await accRepositoryManager.AccJournalCommentRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccJournalCommentDto>(newEntity);


                return await Result<AccJournalCommentDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {

                return await Result<AccJournalCommentDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccJournalCommentRepository.GetById(Id);
            if (item == null) return Result<AccJournalCommentDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");

            accRepositoryManager.AccJournalCommentRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccJournalCommentDto>.SuccessAsync(_mapper.Map<AccJournalCommentDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccJournalCommentDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccJournalCommentRepository.GetById(Id);
            if (item == null) return Result<AccJournalCommentDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");

            accRepositoryManager.AccJournalCommentRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccJournalCommentDto>.SuccessAsync(_mapper.Map<AccJournalCommentDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccJournalCommentDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccJournalCommentDto>> Update(AccJournalCommentDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccJournalCommentDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");

            var item = await accRepositoryManager.AccGroupRepository.GetById(entity.Id);

            if (item == null) return await Result<AccJournalCommentDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)session.UserId;

            _mapper.Map(entity, item);

            accRepositoryManager.AccGroupRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccJournalCommentDto>.SuccessAsync(_mapper.Map<AccJournalCommentDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<AccJournalCommentDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
    }
}
