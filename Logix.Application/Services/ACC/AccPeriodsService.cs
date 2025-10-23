using System.Globalization;
using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{
    public class AccPeriodsService : GenericQueryService<AccPeriods, AccPeriodsDto, AccPeriodDateVws>, IAccPeriodsService
    {
        private readonly IAccRepositoryManager accRepositoryManager;

        private readonly IMapper _mapper;
        private readonly ICurrentData _currentData;
        private readonly ILocalizationService localization;

        public AccPeriodsService(IQueryRepository<AccPeriods> queryRepository, IAccRepositoryManager AccRepositoryManager, IMapper mapper, ICurrentData currentData, ILocalizationService localization) : base(queryRepository, mapper)
        {
            this.accRepositoryManager = AccRepositoryManager;
            this._mapper = mapper;

            this._currentData = currentData;
            this.localization = localization;
        }
        public async Task<IResult<AccPeriodsDto>> Add(AccPeriodsDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccPeriodsDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {

                DateTime startDate = DateTime.ParseExact(entity.PeriodStartDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(entity.PeriodEndDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                if (startDate > endDate)
                {
                    return await Result<AccPeriodsDto>.FailAsync(localization.GetMessagesResource("datePeriodStart"));
                }
                //------------------------تشيك التاريخ
                if (Bahsas.IsHijri(entity.PeriodStartDateGregorian, _currentData))
                {
                    entity.PeriodStartDateGregorian = entity.PeriodStartDateGregorian;
                }
                else
                {
                    entity.PeriodStartDateGregorian = "";
                    return await Result<AccPeriodsDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }

                //------------------------تشيك التاريخ
                if (Bahsas.IsHijri(entity.PeriodEndDateGregorian, _currentData))
                {
                    entity.PeriodEndDateGregorian = entity.PeriodEndDateGregorian;
                }
                else
                {
                    entity.PeriodEndDateGregorian = "";
                    return await Result<AccPeriodsDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }

                entity.FacilityId = _currentData.FacilityId;
                var item = _mapper.Map<AccPeriods>(entity);
                var newEntity = await accRepositoryManager.AccPeriodsRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccPeriodsDto>(newEntity);


                return await Result<AccPeriodsDto>.SuccessAsync(entityMap, localization.GetMessagesResource("NoIdInUpdate"));
            }
            catch (Exception exc)
            {

                return await Result<AccPeriodsDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult<AccPeriodsEditDto>> Update(AccPeriodsEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccPeriodsEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");
            DateTime startDate = DateTime.ParseExact(entity.PeriodStartDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(entity.PeriodEndDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);

            if (startDate > endDate)
            {
                return await Result<AccPeriodsEditDto>.FailAsync(localization.GetMessagesResource("datePeriodStart"));
            }
            //------------------------تشيك التاريخ
            if (Bahsas.IsHijri(entity.PeriodStartDateGregorian, _currentData))
            {
                entity.PeriodStartDateGregorian = entity.PeriodStartDateGregorian;
            }
            else
            {
                entity.PeriodStartDateGregorian = "";
                return await Result<AccPeriodsEditDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

            }

            //------------------------تشيك التاريخ
            if (Bahsas.IsHijri(entity.PeriodEndDateGregorian, _currentData))
            {
                entity.PeriodEndDateGregorian = entity.PeriodEndDateGregorian;
            }
            else
            {
                entity.PeriodEndDateGregorian = "";
                return await Result<AccPeriodsEditDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

            }

            var item = await accRepositoryManager.AccPeriodsRepository.GetById(entity.PeriodId);

            if (item == null) return await Result<AccPeriodsEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
            item.UpdateDate = DateTime.Now;
            item.UpdateUserId = (int)_currentData.UserId;

            _mapper.Map(entity, item);

            accRepositoryManager.AccPeriodsRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccPeriodsEditDto>.SuccessAsync(_mapper.Map<AccPeriodsEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccPeriodsEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var hasTransactions = await accRepositoryManager.AccJournalMasterRepository.CheckPreiodHasTransaction(Id);
            if (hasTransactions > 0)
            {
                var errorMessage = localization.GetMessagesResource("NoPreiodDelete");
                return Result<AccPeriodsDto>.Fail(errorMessage);
            }
            var item = await accRepositoryManager.AccPeriodsRepository.GetById(Id);
            if (item == null) return Result<AccPeriodsDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.FlagDelete = true;
            item.UpdateDate = DateTime.Now;
            item.UpdateUserId = (int)_currentData.UserId;
            accRepositoryManager.AccPeriodsRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccPeriodsDto>.SuccessAsync(_mapper.Map<AccPeriodsDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccPeriodsDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var hasTransactions = await accRepositoryManager.AccJournalMasterRepository.CheckPreiodHasTransaction(Id);
            if (hasTransactions > 0)
            {
                var errorMessage = localization.GetMessagesResource("NoPreiodDelete");
                return Result<AccPeriodsDto>.Fail(errorMessage);
            }
            var item = await accRepositoryManager.AccPeriodsRepository.GetById(Id);
            if (item == null) return Result<AccPeriodsDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.FlagDelete = true;
            item.UpdateDate = DateTime.Now;
            item.UpdateUserId = (int)_currentData.UserId;
            accRepositoryManager.AccPeriodsRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccPeriodsDto>.SuccessAsync(_mapper.Map<AccPeriodsDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccPeriodsDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<bool> CheckDateInPeriod(long PeriodId, string Date)
        {
            var DateInPeriod = await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod(PeriodId, Date);
            return DateInPeriod;
        }
    }
}
