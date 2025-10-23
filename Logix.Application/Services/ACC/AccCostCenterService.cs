using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Services.ACC
{


    public class AccCostCenterService : GenericQueryService<AccCostCenter, AccCostCenterDto, AccCostCenterVws>, IAccCostCenterService
    {
        private readonly IAccRepositoryManager accRepositoryManager;

        private readonly IMapper _mapper;
        private readonly ICurrentData _currentData;
        private readonly ILocalizationService localization;

        public AccCostCenterService(IQueryRepository<AccCostCenter> queryRepository, IAccRepositoryManager AccRepositoryManager, IMapper mapper, ICurrentData currentData, ILocalizationService localization) : base(queryRepository, mapper)
        {
            this.accRepositoryManager = AccRepositoryManager;
            this._mapper = mapper;

            this._currentData = currentData;
            this.localization = localization;
        }
        public async Task<IResult<AccCostCenterDto>> Add(AccCostCenterDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccCostCenterDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {
                if (entity.Numbring == false && string.IsNullOrEmpty(entity.CostCenterCode))
                {

                    return await Result<AccCostCenterDto>.FailAsync(localization.GetMessagesResource("EnterCostCentenumberfirst"));

                }

                if (!string.IsNullOrEmpty(entity.CostCenterCode) && entity.Numbring == false)
                {
                    var CostCenterCode = await accRepositoryManager.AccCostCenterRepository.GetAll(s => s.CostCenterCode == entity.CostCenterCode && s.FacilityId == _currentData.FacilityId && s.IsActive == true && s.IsDeleted == false);
                    if (CostCenterCode != null)
                    {
                        if (CostCenterCode.Count() > 0)
                        {


                            return await Result<AccCostCenterDto>.FailAsync(localization.GetMessagesResource("CostCenterNumberAlready"));
                        }
                    }

                }

                var CostCenterdata = await accRepositoryManager.AccCostCenterRepository.GetCostCenterCode(_currentData.FacilityId, entity.CcParentId ?? 0);

                entity.CostCenterCode = CostCenterdata.CostCenterCode;
                entity.CostCenterLevel = CostCenterdata.CostCenterLevel;
                entity.FacilityId = _currentData.FacilityId;
                entity.FinYear = _currentData.FinYear;
                entity.PeriodId = 0;
                var item = _mapper.Map<AccCostCenter>(entity);
                var newEntity = await accRepositoryManager.AccCostCenterRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccCostCenterDto>(newEntity);


                return await Result<AccCostCenterDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {

                return await Result<AccCostCenterDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult<AccCostCenterEditDto>> Update(AccCostCenterEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccCostCenterEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));

            var item = await accRepositoryManager.AccCostCenterRepository.GetById(entity.CcId);

            if (item == null) return await Result<AccCostCenterEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));

            if (entity.IsParent == true)
            {

                var hasTransactions = await accRepositoryManager.AccJournalMasterRepository.CheckCostCenterHasTransaction(entity.CcId);
                if (hasTransactions > 0)
                {
                    var errorMessage = localization.GetAccResource("CCHasTransaction");
                    return Result<AccCostCenterEditDto>.Fail(errorMessage);
                }
            }

            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)_currentData.UserId;

            _mapper.Map(entity, item);

            accRepositoryManager.AccCostCenterRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccCostCenterEditDto>.SuccessAsync(_mapper.Map<AccCostCenterEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccCostCenterEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var ch = await accRepositoryManager.AccCostCenterRepository.GetAll(x => x.CcParentId == Id);
            if (ch.Count() > 0)
            {

                var errorMessage = localization.GetAccResource("chkCostCenterParentId");
                return Result<AccCostCenterDto>.Fail(errorMessage);

            }

            var hasTransactions = await accRepositoryManager.AccJournalMasterRepository.CheckCostCenterHasTransaction(Id);
            if (hasTransactions > 0)
            {
                var errorMessage = localization.GetAccResource("chkCostCenterhasmovement");
                return Result<AccCostCenterDto>.Fail(errorMessage);
            }
            var item = await accRepositoryManager.AccCostCenterRepository.GetById(Id);
            if (item == null) return Result<AccCostCenterDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)_currentData.UserId;
            accRepositoryManager.AccCostCenterRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccCostCenterDto>.SuccessAsync(_mapper.Map<AccCostCenterDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccCostCenterDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)

        {
            var ch = await accRepositoryManager.AccCostCenterRepository.GetAll(x => x.CcParentId == Id);
            if (ch.Count() > 0)
            {

                var errorMessage = localization.GetAccResource("chkCostCenterParentId");
                return Result<AccCostCenterDto>.Fail(errorMessage);

            }

            var hasTransactions = await accRepositoryManager.AccJournalMasterRepository.CheckCostCenterHasTransaction(Id);
            if (hasTransactions > 0)
            {
                var errorMessage = localization.GetAccResource("chkCostCenterhasmovement");
                return Result<AccCostCenterDto>.Fail(errorMessage);
            }

            var item = await accRepositoryManager.AccCostCenterRepository.GetById(Id);
            if (item == null) return Result<AccCostCenterDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)_currentData.UserId;
            accRepositoryManager.AccCostCenterRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccCostCenterDto>.SuccessAsync(_mapper.Map<AccCostCenterDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccCostCenterDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult> UpdateParentId(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccCostCenterRepository.GetById(Id);
            if (item == null) return Result<AccCostCenterDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));
            item.IsDeleted = false;
            item.CcParentId = item.CcId;
            accRepositoryManager.AccCostCenterRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccCostCenterDto>.SuccessAsync(_mapper.Map<AccCostCenterDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccCostCenterDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult<IEnumerable<AccCostCenterVws>>> Search(AccCostCenterFilterDto filter, CancellationToken cancellationToken = default)
        {
            try
            {

                var items = await accRepositoryManager.AccCostCenterRepository.GetAllVw(x => x.FlagDelete == false && x.FacilityId == _currentData.FacilityId &&
                      (string.IsNullOrEmpty(filter.CostCenterName) || (x.CostCenterName != null && x.CostCenterName.Contains(filter.CostCenterName))) &&
                     (string.IsNullOrEmpty(filter.CostCenterCode) || (x.CostCenterCode != null && x.CostCenterCode.Equals(filter.CostCenterCode))) &&
                     (string.IsNullOrEmpty(filter.CostCenterCodeParent) || (x.CostCenterCodeParent != null && x.CostCenterCodeParent.Contains(filter.CostCenterCodeParent))) &&
                     (string.IsNullOrEmpty(filter.CostCenterNameParent) || (x.CostCenterNameParent != null && x.CostCenterNameParent.Contains(filter.CostCenterNameParent)))
                       && (string.IsNullOrEmpty(filter.Code) && string.IsNullOrEmpty(filter.Code2) || (x.CostCenterCode != null && x.CostCenterCode.CompareTo(filter.Code) >= 0 && x.CostCenterCode != null && x.CostCenterCode.CompareTo(filter.Code2) <= 0))
                     );


                var resultList = items.OrderBy(x => x.CostCenterCode).ToList();

                return await Result<IEnumerable<AccCostCenterVws>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult"));
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<AccCostCenterVws>>.FailAsync($"Exception in {GetType().Name}.Search: {ex.Message}");
            }
        }
        public AccCostCenterFilterDto GetAccCostCenterFilter(Dictionary<string, string> dictionary)
        {
            var dto = new AccCostCenterFilterDto();

            foreach (var prop in typeof(AccCostCenterFilterDto).GetProperties())
            {
                var name = prop.Name;

                switch (name)
                {
                    case nameof(AccCostCenterFilterDto.Code):
                        if (dictionary.TryGetValue("Code", out var code))
                            dto.Code = code;
                        break;

                    case nameof(AccCostCenterFilterDto.Code2):
                        if (dictionary.TryGetValue("Code2", out var code2))
                            dto.Code2 = code2;
                        break;

                    case nameof(AccCostCenterFilterDto.CostCenterCode):
                        if (dictionary.TryGetValue("CostCenterCode", out var costCenterCode))
                            dto.CostCenterCode = costCenterCode;
                        break;

                    case nameof(AccCostCenterFilterDto.CostCenterName):
                        if (dictionary.TryGetValue("CostCenterName", out var costCenterName))
                            dto.CostCenterName = costCenterName;
                        break;

                    case nameof(AccCostCenterFilterDto.CostCenterCodeParent):
                        if (dictionary.TryGetValue("CostCenterCodeParent", out var costCenterCodeParent))
                            dto.CostCenterCodeParent = costCenterCodeParent;
                        break;

                    case nameof(AccCostCenterFilterDto.CostCenterNameParent):
                        if (dictionary.TryGetValue("CostCenterNameParent", out var costCenterNameParent))
                            dto.CostCenterNameParent = costCenterNameParent;
                        break;

                    default:
                        // ❗ هذا يجعل المشروع يتوقف عند التشغيل إذا نسيت خاصية
                        throw new NotImplementedException($"خاصية '{name}' غير معالجة في GetAccCostCenterFilter.");
                }
            }

            return dto;
        }


    }
}
