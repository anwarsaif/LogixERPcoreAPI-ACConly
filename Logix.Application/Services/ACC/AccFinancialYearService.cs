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
    public class AccFinancialYearService : GenericQueryService<AccFinancialYear, AccFinancialYearDto, AccFinancialYearVw>, IAccFinancialYearService
    {
        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMainRepositoryManager mainRepositoryManager;
        private readonly IMapper _mapper;
        private readonly ICurrentData currentData;
        private readonly ILocalizationService localization;

        public AccFinancialYearService(IQueryRepository<AccFinancialYear> queryRepository, IAccRepositoryManager AccRepositoryManager, IMainRepositoryManager mainRepositoryManager, IMapper mapper, ICurrentData currentData, ILocalizationService localization) : base(queryRepository, mapper)
        {
            this.accRepositoryManager = AccRepositoryManager;
            this.mainRepositoryManager = mainRepositoryManager;
            this._mapper = mapper;

            this.currentData = currentData;
            this.localization = localization;
        }
        public async Task<IResult<AccFinancialYearDto>> Add(AccFinancialYearDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccFinancialYearDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {
                DateTime startDate = DateTime.ParseExact(entity.StartDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(entity.EndDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                if (startDate > endDate)
                {
                    return await Result<AccFinancialYearDto>.FailAsync(localization.GetMessagesResource("dateFinancialYearStart"));
                }
                //------------------------تشيك التاريخ
                if (Bahsas.IsHijri(entity.StartDateGregorian, currentData))
                {
                    entity.StartDateGregorian = entity.StartDateGregorian;
                }
                else
                {
                    entity.StartDateGregorian = "";
                    return await Result<AccFinancialYearDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }

                //------------------------تشيك التاريخ
                if (Bahsas.IsHijri(entity.EndDateGregorian, currentData))
                {
                    entity.EndDateGregorian = entity.EndDateGregorian;
                }
                else
                {
                    entity.EndDateGregorian = "";
                    return await Result<AccFinancialYearDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

                }



                entity.FacilityId = currentData.FacilityId;
                var item = _mapper.Map<AccFinancialYear>(entity);
                var newEntity = await accRepositoryManager.AccFinancialYearRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccFinancialYearDto>(newEntity);


                return await Result<AccFinancialYearDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {

                return await Result<AccFinancialYearDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult<AccFinancialYearEditDto>> Update(AccFinancialYearEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccFinancialYearEditDto>.FailAsync($"{localization.GetMessagesResource("UpdateNullEntity")}");

            DateTime startDate = DateTime.ParseExact(entity.StartDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(entity.EndDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture);

            if (startDate > endDate)
            {
                return await Result<AccFinancialYearEditDto>.FailAsync(localization.GetMessagesResource("dateFinancialYearStart"));
            }
            //------------------------تشيك التاريخ
            if (Bahsas.IsHijri(entity.StartDateGregorian, currentData))
            {
                entity.StartDateGregorian = entity.StartDateGregorian;
            }
            else
            {
                entity.StartDateGregorian = "";
                return await Result<AccFinancialYearEditDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

            }

            //------------------------تشيك التاريخ
            if (Bahsas.IsHijri(entity.EndDateGregorian, currentData))
            {
                entity.EndDateGregorian = entity.EndDateGregorian;
            }
            else
            {
                entity.EndDateGregorian = "";
                return await Result<AccFinancialYearEditDto>.FailAsync($"{localization.GetResource1("DateIsWrong")}");

            }
            var item = await accRepositoryManager.AccFinancialYearRepository.GetById(entity.FinYear);

            if (item == null) return await Result<AccFinancialYearEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;

            _mapper.Map(entity, item);

            accRepositoryManager.AccFinancialYearRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccFinancialYearEditDto>.SuccessAsync(_mapper.Map<AccFinancialYearEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccFinancialYearEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var hasTransactions = await accRepositoryManager.AccJournalMasterRepository.CheckFinyearHasTransaction(Id);
            if (hasTransactions > 0)
            {
                var errorMessage = localization.GetMessagesResource("NoFinYearDelete");
                return Result<AccFinancialYearDto>.Fail(errorMessage);
            }
            var item = await accRepositoryManager.AccFinancialYearRepository.GetById(Id);
            if (item == null) return Result<AccFinancialYearDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            accRepositoryManager.AccFinancialYearRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccFinancialYearDto>.SuccessAsync(_mapper.Map<AccFinancialYearDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccFinancialYearDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var hasTransactions = await accRepositoryManager.AccJournalMasterRepository.CheckFinyearHasTransaction(Id);
            if (hasTransactions > 0)
            {
                var errorMessage = localization.GetMessagesResource("NoFinYearDelete");
                return Result<AccFinancialYearDto>.Fail(errorMessage);
            }
            var item = await accRepositoryManager.AccFinancialYearRepository.GetById(Id);
            if (item == null) return Result<AccFinancialYearDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            accRepositoryManager.AccFinancialYearRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccFinancialYearDto>.SuccessAsync(_mapper.Map<AccFinancialYearDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccFinancialYearDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<Result<IEnumerable<BalanceSheetFinancialYear>>> GetBalanceSheetData(BalanceSheetFinancialYearFilter entity)
        {
            try
            {
                var balanceSheetResult = await accRepositoryManager.AccFunctionRepository.GetBalanceSheetData(currentData.FacilityId, entity.FinYearFrom, entity.PeriodIdFrom, entity.FinYearTo);

                return Result<IEnumerable<BalanceSheetFinancialYear>>.Success(balanceSheetResult);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BalanceSheetFinancialYear>>.Fail($"An error occurred: {ex.Message}");
            }
        }

        public async Task<IResult<ClosingFinancialYearDto>> CreateJournal(ClosingFinancialYearDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<ClosingFinancialYearDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");

            try
            {
                if (entity.DetailsList.Count == 0)
                {

                    return await Result<ClosingFinancialYearDto>.FailAsync($"{localization.GetMessagesResource("Addrecordsfirst")}");

                }
                if (await accRepositoryManager.AccFinancialYearRepository.checkClosed(entity.FinYearFrom, currentData.FacilityId) > 0)
                {
                    return await Result<ClosingFinancialYearDto>.FailAsync($"{localization.GetMessagesResource("ClosingFinancial")}");

                }

                //====اجمالي الحساب الدائن لايساوي اجمالي الحساب المدين 
                decimal sumCredit = entity.DetailsList.Sum(x => (x.Credit ?? 0) * (x.ExchangeRate ?? 0));
                decimal sumDebit = entity.DetailsList.Sum(x => (x.Debit ?? 0) * (x.ExchangeRate ?? 0));

                if (sumCredit != sumDebit)
                {
                    return await Result<ClosingFinancialYearDto>.FailAsync($"{localization.GetResource1("DebitNotEqualCredit")}");
                }

                int? Status_Id = 0;

                Status_Id = await accRepositoryManager.AccJournalMasterRepository.GetPostingStatuse(currentData.FacilityId);
                //------------------------تشيك الفترة
                if (await accRepositoryManager.AccPeriodsRepository.CheckDateInPeriod((long)entity.PeriodIdTo, entity.dateTo) == false)
                {
                    return await Result<ClosingFinancialYearDto>.FailAsync($"{localization.GetResource1("DateOutOfPERIOD")}");

                }


                AccJournalMasterDto accJournalMaster = new()
                {

                    DocTypeId = 4,
                    ReferenceNo = entity.FinYearFrom,
                    FacilityId = currentData.FacilityId,
                    JDateGregorian = entity.dateTo,
                    JDateHijri = entity.dateTo,
                    StatusId = Status_Id,
                    FinYear = entity.FinYearTo,
                    PaymentTypeId = 0,
                    JDescription = currentData.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                    JBian = currentData.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                    CcId = currentData.BranchId,
                    PeriodId = entity.PeriodIdTo,
                    BankId = 0,
                    CurrencyId = entity.CurrencyId,
                    ExchangeRate = entity.ExchangeRate,
                    ChequNo = "",
                    ChequDateHijri = "",
                };
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);
                var addJournalMaster = await accRepositoryManager.AccJournalMasterRepository.AddACCJournalMaster(accJournalMaster, cancellationToken);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);


                if (!addJournalMaster.Succeeded)
                    return await Result<ClosingFinancialYearDto>.FailAsync(addJournalMaster.Status.message);


                long jId = addJournalMaster.Data.JId;

                foreach (var items in entity.DetailsList)
                {
                    decimal ExchangeRate = 0;
                    int DefCurrency = await mainRepositoryManager.SysCurrencyRepository.GetDefaultCurrency();

                    //---------------------------- الدالة تاتي في العملة الرئسية للنظام  
                    ExchangeRate = await mainRepositoryManager.SysExchangeRateRepository.GetExchangeRateValue((long)DefCurrency);

                    AccJournalDetaileDto accJournalDetail1Debit = new()
                    {
                        JId = jId,
                        CcId = items.CCID ??= 0,
                        Cc2Id = items.CC2ID ??= 0,
                        Cc3Id = items.CC3ID ??= 0,
                        Cc4Id = items.CC4ID ??= 0,
                        Cc5Id = items.CC5ID ??= 0,
                        JDateGregorian = items.JDateGregorian,
                        AccAccountId = items.AccAccountID,
                        Credit = items.Credit,
                        Debit = items.Debit,
                        Description = items.Description,
                        ReferenceTypeId = items.ReferenceTypeID,
                        ReferenceNo = items.ReferenceDNo ??= 0,
                        CurrencyId = DefCurrency,
                        ExchangeRate = ExchangeRate,

                    };
                    var addJournalDetailDebit = await accRepositoryManager.AccJournalDetaileRepository.AddAccJournalDetail(accJournalDetail1Debit, cancellationToken);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                    if (!addJournalDetailDebit.Succeeded)
                        return await Result<ClosingFinancialYearDto>.FailAsync(addJournalDetailDebit.Status.message);
                    await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
                }
                string jCode = addJournalMaster.Data.JCode;
                entity.jCode = jCode;
                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);
                return await Result<ClosingFinancialYearDto>.SuccessAsync(entity, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {
                return await Result<ClosingFinancialYearDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }



        }

    }
}
