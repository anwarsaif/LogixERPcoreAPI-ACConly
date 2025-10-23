using AutoMapper;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Helpers;
using Logix.Application.Interfaces.IRepositories;
using Logix.Application.Interfaces.IServices;
using Logix.Application.Interfaces.IServices.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using System.Collections.Generic;





namespace Logix.Application.Services.ACC
{

    public class AccAccountService : GenericQueryService<AccAccount, AccAccountDto, AccAccountsVw>, IAccAccountService
    {


        private readonly IAccRepositoryManager accRepositoryManager;
        private readonly IMainRepositoryManager mainRepositoryManager;
        private readonly IHrRepositoryManager hrRepositoryManager;
        private readonly ISysConfigurationAppHelper sysConfigurationAppHelper;
        private readonly IMapper _mapper;
        private readonly ICurrentData currentData;
        private readonly ILocalizationService localization;

        public AccAccountService(IQueryRepository<AccAccount> queryRepository, IAccRepositoryManager accRepositoryManager, IMapper mapper, ICurrentData currentData, ILocalizationService localization, IMainRepositoryManager mainRepositoryManager, IHrRepositoryManager hrRepositoryManager, ISysConfigurationAppHelper SysConfigurationAppHelper) : base(queryRepository, mapper)
        {
            this.accRepositoryManager = accRepositoryManager;
            this._mapper = mapper;
            this.currentData = currentData;
            this.localization = localization;
            this.mainRepositoryManager = mainRepositoryManager;
            this.hrRepositoryManager = hrRepositoryManager;
            sysConfigurationAppHelper = SysConfigurationAppHelper;
        }

        public async Task<IResult<AccAccountDto>> Add(AccAccountDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccAccountDto>.FailAsync($"{localization.GetMessagesResource("AddNullEntity")}");
            try
            {
                if (entity.Numbring == false && string.IsNullOrEmpty(entity.AccAccountCode))
                {

                    return await Result<AccAccountDto>.FailAsync(localization.GetMessagesResource("Enteraccountnumberfirst"));

                }
                if (!string.IsNullOrEmpty(entity.AccAccountCode) && entity.Numbring == false)
                {
                    var AccAccountCode = await accRepositoryManager.AccAccountRepository.GetAll(s => s.AccAccountCode == entity.AccAccountCode && s.FacilityId == currentData.FacilityId);
                    if (AccAccountCode != null)
                    {
                        if (AccAccountCode.Count() > 0)
                        {
                            return await Result<AccAccountDto>.FailAsync(localization.GetMessagesResource("AccountNumberAlready"));
                        }

                    }

                }
                if (entity.Numbring == false)
                {

                    var parentCode = await accRepositoryManager.AccAccountRepository.GetById(entity.AccAccountParentId ?? 0);

                    if (string.IsNullOrEmpty(parentCode.AccAccountCode)
                        || !entity.AccAccountCode.StartsWith(parentCode.AccAccountCode, StringComparison.Ordinal))
                    {
                        var msg = localization.GetAccResource("AccountCodeStartError") + ": " + parentCode.AccAccountCode;
                        return await Result<AccAccountDto>.FailAsync(msg);
                    }

                    var levelLength = await accRepositoryManager.AccAccountRepository.GetAccLevelDigits(parentCode.AccountLevel + 1 ?? 0);
                    if (levelLength != entity.AccAccountCode.Length)
                    {
                        var msg = localization.GetAccResource("AccountCodeDigitsNoError") + ": " + levelLength;
                        return await Result<AccAccountDto>.FailAsync(msg);
                    }


                }

                var accountRestrictedLevel = await sysConfigurationAppHelper.GetValue(260, currentData.FacilityId);
                var parentAccountLevel = await accRepositoryManager.AccAccountRepository.GetAccountLevel(entity.AccAccountParentId ?? 0, currentData.FacilityId);


                if (accountRestrictedLevel != "1")
                {
                    var subAccountLevelValue = await sysConfigurationAppHelper.GetValue(99, currentData.FacilityId);
                    if (string.IsNullOrEmpty(subAccountLevelValue))
                    {
                        return await Result<AccAccountDto>.FailAsync(localization.GetMessagesResource("levelConfiguration"));
                    }

                    var subAccountLevel = int.Parse(subAccountLevelValue);
                    if (parentAccountLevel >= subAccountLevel)
                    {
                        return await Result<AccAccountDto>.FailAsync(localization.GetMessagesResource("Accountlargerlevel"));
                    }

                    if (!entity.IsSub)
                    {
                        if (subAccountLevel != parentAccountLevel + 1)
                        {
                            return await Result<AccAccountDto>.FailAsync(localization.GetMessagesResource("subAccountLevel"));
                        }
                    }
                }
                else if (!entity.IsSub)
                {
                    if (parentAccountLevel + 1 < 3)
                    {
                        return await Result<AccAccountDto>.FailAsync(localization.GetAccResource("AddSubAccountError"));
                    }
                }





                entity.AccAccountType = 0;
                entity.DeptID = 0;
                entity.FacilityId = currentData.FacilityId;
                // Generate new account code
                var AccAccountcode = await accRepositoryManager.AccAccountRepository.GetAccountCode(currentData.FacilityId, entity.AccAccountParentId ?? 0);
                entity.AccAccountCode = AccAccountcode;
                var item = _mapper.Map<AccAccount>(entity);
                item.SystemId = 2;
                var newEntity = await accRepositoryManager.AccAccountRepository.AddAndReturn(item);

                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                var entityMap = _mapper.Map<AccAccountDto>(newEntity);


                return await Result<AccAccountDto>.SuccessAsync(entityMap, localization.GetMessagesResource("success"));
            }
            catch (Exception exc)
            {

                return await Result<AccAccountDto>.FailAsync($"EXP in {this.GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccAccountRepository.GetById(Id);
            if (item == null) return Result<AccAccountDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");

            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            accRepositoryManager.AccAccountRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccAccountDto>.SuccessAsync(_mapper.Map<AccAccountDto>(item), localization.GetMessagesResource("DeletedSuccess"));

            }
            catch (Exception exp)
            {
                return await Result<AccAccountDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccAccountRepository.GetById(Id);
            if (item == null) return Result<AccAccountDto>.Fail($"{localization.GetMessagesResource("NoIdInDelete")}");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            accRepositoryManager.AccAccountRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccAccountDto>.SuccessAsync(_mapper.Map<AccAccountDto>(item), localization.GetMessagesResource("DeletedSuccess"));
            }
            catch (Exception exp)
            {
                return await Result<AccAccountDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<AccAccountEditDto>> Update(AccAccountEditDto entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<AccAccountEditDto>.FailAsync(localization.GetMessagesResource("UpdateNullEntity"));

            var parent = await accRepositoryManager.AccAccountRepository.GetById(entity.AccAccountParentId ?? 0);

            if (string.IsNullOrEmpty(parent.AccAccountCode)
                || !entity.AccAccountCode.StartsWith(parent.AccAccountCode, StringComparison.Ordinal))
            {
                var msg = localization.GetAccResource("AccountCodeStartError") + ": " + parent.AccAccountCode;
                return await Result<AccAccountEditDto>.FailAsync(msg);
            }

            var AccLevel = entity.AccountLevel;
            if (AccLevel != (parent.AccountLevel + 1))
            {
                var msg = localization.GetAccResource("AccountCodeDigitsNoError") + ": " + (parent.AccountLevel + 1);
                return await Result<AccAccountEditDto>.FailAsync(msg);
            }



            var levelLength = await accRepositoryManager.AccAccountRepository.GetAccLevelDigits(parent.AccountLevel + 1 ?? 0);
            if (levelLength != entity.AccAccountCode.Length)
            {
                var msg = localization.GetAccResource("AccountCodeDigitsNoError") + ": " + levelLength;
                return await Result<AccAccountEditDto>.FailAsync(msg);
            }

            var item = await accRepositoryManager.AccAccountRepository.GetById(entity.AccAccountId);

            if (entity.IsSub != item.IsSub)
            {
                var hasAcc = await accRepositoryManager.AccJournalDetaileRepository.AccountHasTransactions(item.AccAccountId);

                if (entity.IsSub && hasAcc)
                {
                    return await Result<AccAccountEditDto>.FailAsync(localization.GetAccResource("IsSub") + ": " + localization.GetAccResource("IsSubCheckErr"));
                }
                else
                {
                    var isParent = await accRepositoryManager.AccAccountRepository.IsAccAccountParent(parent.AccAccountId, currentData.FacilityId);
                    if (isParent)
                    {
                        return await Result<AccAccountEditDto>.FailAsync(localization.GetAccResource("IsSub") + ": " + localization.GetAccResource("IsSubCheckErr"));

                    }
                }


            }

            if (item == null) return await Result<AccAccountEditDto>.FailAsync(localization.GetMessagesResource("NoIdInUpdate"));

            var accountRestrictedLevel = await sysConfigurationAppHelper.GetValue(260, currentData.FacilityId);
            var parentAccountLevel = await accRepositoryManager.AccAccountRepository.GetAccountLevel(entity.AccAccountParentId ?? 0, currentData.FacilityId);


            if (accountRestrictedLevel != "1")
            {
                var subAccountLevelValue = await sysConfigurationAppHelper.GetValue(99, currentData.FacilityId);
                if (string.IsNullOrEmpty(subAccountLevelValue))
                {
                    return await Result<AccAccountEditDto>.FailAsync(localization.GetMessagesResource("levelConfiguration"));
                }

                var subAccountLevel = int.Parse(subAccountLevelValue);
                if (parentAccountLevel >= subAccountLevel)
                {
                    return await Result<AccAccountEditDto>.FailAsync(localization.GetMessagesResource("Accountlargerlevel"));
                }

                if (!entity.IsSub)
                {
                    if (subAccountLevel != parentAccountLevel + 1)
                    {
                        return await Result<AccAccountEditDto>.FailAsync(localization.GetMessagesResource("subAccountLevel"));
                    }
                }
            }
            else if (!entity.IsSub)
            {
                if (parentAccountLevel + 1 < 3)
                {
                    return await Result<AccAccountEditDto>.FailAsync(localization.GetAccResource("AddSubAccountError"));
                }
            }

            item.IsDeleted = false;
            item.ModifiedOn = DateTime.Now;
            item.ModifiedBy = (int)currentData.UserId;
            _mapper.Map(entity, item);

            accRepositoryManager.AccAccountRepository.Update(item);

            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccAccountEditDto>.SuccessAsync(_mapper.Map<AccAccountEditDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccAccountEditDto>.FailAsync($"EXP in Update at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> UpdateParentId(long Id, CancellationToken cancellationToken = default)
        {
            var item = await accRepositoryManager.AccAccountRepository.GetById(Id);
            if (item == null) return Result<AccAccountDto>.Fail(localization.GetMessagesResource("NoIdInUpdate"));
            item.IsDeleted = false;
            item.AccAccountParentId = item.AccAccountId;
            accRepositoryManager.AccAccountRepository.Update(item);
            try
            {
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

                return await Result<AccAccountDto>.SuccessAsync(_mapper.Map<AccAccountDto>(item), localization.GetMessagesResource("success"));
            }
            catch (Exception exp)
            {
                return await Result<AccAccountDto>.FailAsync($"EXP in Remove at ( {this.GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<List<AccountBalanceSheetDto>>> GetEmployeeAccountTransactionsForAllYears(AccountBalanceSheetFilterDto filter)
        {
            var facilityId = filter.FacilityId;
            var referenceTypeId = filter.referenceTypeId;
            var fromDate = string.IsNullOrEmpty(filter.FromDate) ? (DateTime?)null : DateHelper.StringToDate(filter.FromDate);
            var toDate = string.IsNullOrEmpty(filter.ToDate) ? (DateTime?)null : DateHelper.StringToDate(filter.ToDate);
            var accountId = 0L;
            List<AccountBalanceSheetDto> resultList = new List<AccountBalanceSheetDto>();
            // Validate employee existence
            var employee = await mainRepositoryManager.InvestEmployeeRepository.GetOne(x => x.EmpId == filter.EmpCode);
            if (employee == null)
            {
                return await Result<List<AccountBalanceSheetDto>>.FailAsync(localization.GetResource1("EmployeeNotFound"));
            }
            accountId = employee.Id;

            // Fetch data from repository


            // Apply date filtering in-memory
            if (fromDate.HasValue && toDate.HasValue)
            {
                var allRecords = await accRepositoryManager.AccBalanceSheetRepository.GetAll(x =>
                x.ReferenceDNo == accountId &&
                x.ParentReferenceTypeId == 8 &&
                x.FlagDelete == false &&
                x.FacilityId == facilityId);
                allRecords = allRecords.Where(x =>
                    DateHelper.StringToDate(x.JDateGregorian) < fromDate.Value ||
                    (x.DocTypeId == 4 || x.DocTypeId == 27) &&
                    DateHelper.StringToDate(x.JDateGregorian) >= fromDate.Value &&
                    DateHelper.StringToDate(x.JDateGregorian) <= toDate.Value).ToList();
                if (referenceTypeId > 0)
                {
                    allRecords = allRecords.Where(x => x.ReferenceTypeId == referenceTypeId).ToList();
                }

                // Opening balance query
                var openingBalanceData = allRecords.Where(x => fromDate.HasValue &&
                    (DateHelper.StringToDate(x.JDateGregorian) < fromDate.Value) ||
                    (x.DocTypeId == 4 || x.DocTypeId == 27) &&
                    (DateHelper.StringToDate(x.JDateGregorian) >= fromDate.Value && DateHelper.StringToDate(x.JDateGregorian) <= toDate.Value))
                    .GroupBy(x => x.NatureAccount)
                    .Select(g => new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceCode = string.Empty,
                        ReferenceNo = 0,
                        ReferenceTypeName = null,
                        ReferenceTypeName2 = null,
                        DocTypeName = null,
                        DocTypeName2 = null,
                        JId = 0,
                        JDetailesId = 0,
                        JDateGregorian = "2000/01/01",
                        Description = currentData.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                        Debit = 0,
                        Credit = 0,
                        Balance = 0,
                        CostCenterName = null,
                        JCode = null,
                        NatureAccount = g.Key ?? 1
                    })
                    .ToList();
                resultList.AddRange(openingBalanceData);
                if (!openingBalanceData.Any())
                {
                    resultList.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceCode = string.Empty,
                        ReferenceNo = 0,
                        ReferenceTypeName = null,
                        ReferenceTypeName2 = null,
                        DocTypeName = null,
                        DocTypeName2 = null,
                        JId = 0,
                        JDetailesId = 0,
                        JDateGregorian = "2000/01/01",
                        Description = currentData.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                        Debit = 0,
                        Credit = 0,
                        Balance = 0,
                        CostCenterName = null,
                        JCode = null,
                        NatureAccount = 1
                    });
                }
                ///   رصيد مدور
                var CycleBalanceData = allRecords.Where(x => fromDate.HasValue &&
                   (DateHelper.StringToDate(x.JDateGregorian) < fromDate.Value) ||
                   (x.DocTypeId == 4 || x.DocTypeId == 27) &&
                   (DateHelper.StringToDate(x.JDateGregorian) >= fromDate.Value && DateHelper.StringToDate(x.JDateGregorian) <= toDate.Value))
                   .GroupBy(x => x.NatureAccount)
                   .Select(g => new AccountBalanceSheetDto
                   {
                       DocTypeId = 0,
                       ReferenceCode = string.Empty,
                       ReferenceNo = 0,
                       ReferenceTypeName = null,
                       ReferenceTypeName2 = null,
                       DocTypeName = null,
                       DocTypeName2 = null,
                       JId = 0,
                       JDetailesId = 0,
                       JDateGregorian = "2000/10/10",
                       Description = currentData.Language == 1 ? "رصيد مدور" : "Cycled Balance",
                       Debit = g.Sum(x => x.Debit) ?? 0,
                       Credit = g.Sum(x => x.Credit) ?? 0,
                       Balance = g.Key.HasValue && g.Key.Value > 0
                           ? g.Sum(x => x.Credit) - g.Sum(x => x.Debit)
                           : g.Sum(x => x.Debit) - g.Sum(x => x.Credit),
                       CostCenterName = null,
                       JCode = null,
                       NatureAccount = g.Key ?? 1
                   })
                   .ToList();
                resultList.AddRange(CycleBalanceData);

                if (!CycleBalanceData.Any())
                {
                    resultList.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceCode = string.Empty,
                        ReferenceNo = 0,
                        ReferenceTypeName = null,
                        ReferenceTypeName2 = null,
                        DocTypeName = null,
                        DocTypeName2 = null,
                        JId = 0,
                        JDetailesId = 0,
                        JDateGregorian = "2000/10/10",
                        Description = currentData.Language == 1 ? "رصيد مدور" : "Cycled Balance",
                        Debit = 0,
                        Credit = 0,
                        Balance = 0,
                        CostCenterName = null,
                        JCode = null,
                        NatureAccount = 1
                    });
                }

            }

            var allRecords2 = await accRepositoryManager.AccBalanceSheetRepository.GetAll(x =>
            x.ReferenceDNo == accountId &&
            x.ParentReferenceTypeId == 8 &&
            x.FlagDelete == false &&
            x.FacilityId == facilityId &&
            (x.DocTypeId != 4 && x.DocTypeId != 27)
            );
            if (fromDate.HasValue && toDate.HasValue)
            {
                var allFilteredRecords = allRecords2.Where(x =>
                DateHelper.StringToDate(x.JDateGregorian) >= fromDate.Value && DateHelper.StringToDate(x.JDateGregorian) <= toDate.Value).ToList();
                if (referenceTypeId > 0)
                {
                    allFilteredRecords = allFilteredRecords.Where(x => x.ReferenceTypeId == referenceTypeId).ToList();
                }
            }
            // Transactions query
            var transactions = allRecords2
                .Select(x => new AccountBalanceSheetDto
                {
                    DocTypeId = x.DocTypeId,
                    ReferenceCode = x.ReferenceCode,
                    ReferenceNo = x.ReferenceNo,
                    ReferenceTypeName = x.ReferenceTypeName,
                    ReferenceTypeName2 = x.ReferenceTypeName2,
                    DocTypeName = x.DocTypeName,
                    DocTypeName2 = x.DocTypeName2,
                    JId = x.JId,
                    JDetailesId = x.JDetailesId,
                    JDateGregorian = x.JDateGregorian,
                    Description = x.Description,
                    Debit = x.Debit,
                    Credit = x.Credit,
                    Balance = 0,
                    CostCenterName = x.CostCenterName,
                    CostCenterName2 = x.CostCenterName2,
                    JCode = x.JCode,
                    NatureAccount = x.NatureAccount
                })
                .ToList();
            // Apply date filtering in-memory
            if (fromDate.HasValue && toDate.HasValue)
            {
                transactions = transactions.Where(x =>
                    DateHelper.StringToDate(x.JDateGregorian) >= fromDate.Value &&
                    DateHelper.StringToDate(x.JDateGregorian) <= toDate.Value
                ).ToList();
            }

            resultList.AddRange(transactions);

            if (!fromDate.HasValue)
            {
                resultList = resultList.OrderBy(x => DateHelper.StringToDate(x.JDateGregorian)).ThenBy(x => x.SortNo).ToList();
            }
            else
            {
                resultList = resultList.OrderBy(x => DateHelper.StringToDate(x.JDateGregorian)).ToList();

            }


            decimal? balance = 0;
            foreach (var result in resultList)
            {
                if (result.Description == (currentData.Language == 1 ? "رصيد افتتاحي" : "Opening Balance"))
                {
                    result.JDateGregorian = null;
                    balance = result.Balance;
                }
                else if (result.Description == (currentData.Language == 1 ? "رصيد مدور" : "Cycled Balance"))
                {
                    result.JDateGregorian = null;
                    balance = result.Balance;
                }
                else
                {
                    if (result.NatureAccount.HasValue && result.NatureAccount.Value >= 0)
                    {
                        balance += result.Credit - result.Debit;
                    }
                    else
                    {
                        balance += result.Debit - result.Credit;
                    }

                    result.Balance = balance;
                }
            }
            // Return the result list
            if (resultList.Count > 0)
                return await Result<List<AccountBalanceSheetDto>>.SuccessAsync(resultList);
            return await Result<List<AccountBalanceSheetDto>>.SuccessAsync(localization.GetResource1("NosearchResult"));

        }


        public async Task<IResult<List<AccountBalanceSheetDto>>> GetEmployeeAccountTransactionsForCurrentYear(AccountBalanceSheetFilterDto filter)
        {
            var facilityId = filter.FacilityId;
            var financialYear = filter.FinancialYear;
            var referenceTypeId = filter.referenceTypeId;
            var fromDate = string.IsNullOrEmpty(filter.FromDate) ? (DateTime?)null : DateHelper.StringToDate(filter.FromDate);
            var toDate = string.IsNullOrEmpty(filter.ToDate) ? (DateTime?)null : DateHelper.StringToDate(filter.ToDate);
            var accountId = 0L;
            List<AccountBalanceSheetDto> resultList = new List<AccountBalanceSheetDto>();

            // Validate employee existence
            var employee = await mainRepositoryManager.InvestEmployeeRepository.GetOne(x => x.EmpId == filter.EmpCode);
            if (employee == null)
            {
                return await Result<List<AccountBalanceSheetDto>>.FailAsync(localization.GetResource1("EmployeeNotFound"));
            }
            accountId = employee.Id;

            // Fetch data from repository
            var allRecords = await accRepositoryManager.AccBalanceSheetRepository.GetAll(x =>
                x.ReferenceDNo == accountId &&
                x.ParentReferenceTypeId == 8 &&
                x.FlagDelete == false &&
                x.FacilityId == facilityId &&
                x.FinYear == financialYear);

            if (fromDate.HasValue && toDate.HasValue)
            {
                allRecords = allRecords.Where(x =>
                    DateHelper.StringToDate(x.JDateGregorian) < fromDate.Value ||
                    (x.DocTypeId == 4 || x.DocTypeId == 27) &&
                    DateHelper.StringToDate(x.JDateGregorian) >= fromDate.Value &&
                    DateHelper.StringToDate(x.JDateGregorian) <= toDate.Value).ToList();

                if (referenceTypeId > 0)
                {
                    allRecords = allRecords.Where(x => x.ReferenceTypeId == referenceTypeId).ToList();
                }

                // Opening balance query
                var openingBalanceData = allRecords
                    .Where(x => DateHelper.StringToDate(x.JDateGregorian) < fromDate.Value)
                    .GroupBy(x => x.NatureAccount)
                    .Select(g => new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceCode = string.Empty,
                        ReferenceNo = 0,
                        ReferenceTypeName = null,
                        ReferenceTypeName2 = null,
                        DocTypeName = null,
                        DocTypeName2 = null,
                        JId = 0,
                        JDetailesId = 0,
                        JDateGregorian = "2000/01/01",
                        Description = currentData.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                        Debit = 0,
                        Credit = 0,
                        Balance = g.Key.HasValue && g.Key.Value > 0
                            ? g.Sum(x => x.Credit) - g.Sum(x => x.Debit)
                            : g.Sum(x => x.Debit) - g.Sum(x => x.Credit),
                        CostCenterName = null,
                        JCode = null,
                        NatureAccount = g.Key ?? 1
                    })
                    .ToList();

                if (!openingBalanceData.Any())
                {
                    openingBalanceData.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceCode = string.Empty,
                        ReferenceNo = 0,
                        ReferenceTypeName = null,
                        ReferenceTypeName2 = null,
                        DocTypeName = null,
                        DocTypeName2 = null,
                        JId = 0,
                        JDetailesId = 0,
                        JDateGregorian = "2000/01/01",
                        Description = currentData.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                        Debit = 0,
                        Credit = 0,
                        Balance = 0,
                        CostCenterName = null,
                        JCode = null,
                        NatureAccount = 1
                    });
                }

                resultList.AddRange(openingBalanceData);
            }

            var allRecords2 = await accRepositoryManager.AccBalanceSheetRepository.GetAll(x =>
                x.ReferenceDNo == accountId &&
                x.ParentReferenceTypeId == 8 &&
                x.FlagDelete == false &&
                x.FacilityId == facilityId &&
                x.FinYear == financialYear &&
                (x.DocTypeId != 4 && x.DocTypeId != 27));

            if (fromDate.HasValue && toDate.HasValue)
            {
                allRecords2 = allRecords2.Where(x =>
                    DateHelper.StringToDate(x.JDateGregorian) >= fromDate.Value &&
                    DateHelper.StringToDate(x.JDateGregorian) <= toDate.Value).ToList();

                if (referenceTypeId > 0)
                {
                    allRecords2 = allRecords2.Where(x => x.ReferenceTypeId == referenceTypeId).ToList();
                }
            }

            // Transactions query
            var transactions = allRecords2.Select(x => new AccountBalanceSheetDto
            {
                DocTypeId = x.DocTypeId,
                ReferenceCode = x.ReferenceCode,
                ReferenceNo = x.ReferenceNo,
                ReferenceTypeName = x.ReferenceTypeName,
                ReferenceTypeName2 = x.ReferenceTypeName2,
                DocTypeName = x.DocTypeName,
                DocTypeName2 = x.DocTypeName2,
                JId = x.JId,
                JDetailesId = x.JDetailesId,
                JDateGregorian = x.JDateGregorian,
                Description = x.Description,
                Debit = x.Debit,
                Credit = x.Credit,
                Balance = 0,
                CostCenterName = x.CostCenterName,
                CostCenterName2 = x.CostCenterName2,
                JCode = x.JCode,
                NatureAccount = x.NatureAccount
            }).ToList();

            resultList.AddRange(transactions);

            resultList = fromDate.HasValue
                ? resultList.OrderBy(x => DateHelper.StringToDate(x.JDateGregorian)).ToList()
                : resultList.OrderBy(x => DateHelper.StringToDate(x.JDateGregorian)).ThenBy(x => x.SortNo).ToList();

            decimal? balance = 0;
            foreach (var result in resultList)
            {
                if (result.Description == (currentData.Language == 1 ? "رصيد افتتاحي" : "Opening Balance"))
                {
                    result.JDateGregorian = null;
                    balance = result.Balance;
                }
                else
                {
                    if (result.NatureAccount.HasValue && result.NatureAccount.Value >= 0)
                    {
                        balance += result.Credit - result.Debit;
                    }
                    else
                    {
                        balance += result.Debit - result.Credit;
                    }

                    result.Balance = balance;
                }
            }

            // Return the result list
            if (resultList.Any())
                return await Result<List<AccountBalanceSheetDto>>.SuccessAsync(resultList);

            return await Result<List<AccountBalanceSheetDto>>.FailAsync(localization.GetResource1("NosearchResult"));
        }

        //  كشف حساب الموظفين من الى
        public async Task<IResult<List<AccountFromToFilterDto>>> GetEmployeeAccountTransactionsFromTo(AccountFromToFilterDto filter)
        {
            try
            {
                var facilityId = filter.FacilityId;
                var financialYear = filter.FinancialYear;
                var startDate = DateHelper.StringToDate(filter.FromDate);
                var endDate = DateHelper.StringToDate(filter.ToDate);
                var branchId = filter.BranchId ?? 0;
                var referenceTypeId = filter.referenceTypeId ?? 0;
                var empCode = filter.EmpCode;
                var startCode = Convert.ToInt32(filter.CodeFrom);
                var endCode = Convert.ToInt32(filter.CodeTo);

                // Fetching employees
                var aLLEmployees = await hrRepositoryManager.HrEmployeeRepository.GetAllVw(x =>
                    x.IsDeleted == false &&
                    (facilityId == 0 || x.FacilityId == facilityId) &&
                    (branchId == 0 || x.BranchId == branchId) &&
                    (string.IsNullOrEmpty(empCode) || x.EmpId == empCode)
                );
                var employees = aLLEmployees.Where(x =>
                (Convert.ToInt32(x.EmpId) >= startCode && Convert.ToInt32(x.EmpId) <= endCode));
                var resultList = new List<AccountFromToFilterDto>();
                if (employees.Count() > 0)
                {
                    var GetAllBalanceSheet = await accRepositoryManager.AccBalanceSheetRepository.GetAll(x => x.FlagDelete == false);
                    foreach (var employee in employees)
                    {
                        // Calculate AMOUNTPrev
                        var amountPrev = GetAllBalanceSheet.Where(x =>
                            x.ReferenceDNo == employee.Id &&
                            x.FlagDelete == false &&
                            (facilityId == 0 || x.FacilityId == facilityId) &&
                            (filter.FinancialYear == 0 || x.FinYear == financialYear) &&
                            (DateHelper.StringToDate(x.JDateGregorian) < startDate ||
                            (x.DocTypeId == 4 || x.DocTypeId == 27) &&
                            DateHelper.StringToDate(x.JDateGregorian) >= startDate &&
                            DateHelper.StringToDate(x.JDateGregorian) <= endDate) &&
                            (branchId == 0 || x.BranchId == branchId) &&
                            (referenceTypeId == 0 || x.ReferenceTypeId == referenceTypeId)
                        ).Sum(x => x.Debit - x.Credit);

                        // Calculate Debit
                        var debit = GetAllBalanceSheet.Where(x =>
                            x.ReferenceDNo == employee.Id &&
                            x.FlagDelete == false &&
                            (facilityId == 0 || x.FacilityId == facilityId) &&
                            (filter.FinancialYear == 0 || x.FinYear == financialYear) &&
                            DateHelper.StringToDate(x.JDateGregorian) >= startDate &&
                            DateHelper.StringToDate(x.JDateGregorian) <= endDate &&
                            x.DocTypeId != 4 && x.DocTypeId != 27 &&
                            (branchId == 0 || x.BranchId == branchId) &&
                            (referenceTypeId == 0 || x.ReferenceTypeId == referenceTypeId)
                        ).Sum(x => x.Debit);

                        // Calculate Credit
                        var credit = GetAllBalanceSheet.Where(x =>
                            x.ReferenceDNo == employee.Id &&
                            x.FlagDelete == false &&
                            (facilityId == 0 || x.FacilityId == facilityId) &&
                            (filter.FinancialYear == 0 || x.FinYear == financialYear) &&
                            DateHelper.StringToDate(x.JDateGregorian) >= startDate &&
                            DateHelper.StringToDate(x.JDateGregorian) <= endDate &&
                            x.DocTypeId != 4 && x.DocTypeId != 27 &&
                            (branchId == 0 || x.BranchId == branchId) &&
                            (referenceTypeId == 0 || x.ReferenceTypeId == referenceTypeId)
                        ).Sum(x => x.Credit);

                        // Calculate AMOUNTNext
                        var amountCurrent = GetAllBalanceSheet.Where(x =>
                            x.ReferenceDNo == employee.Id &&
                            x.FlagDelete == false &&
                            (facilityId == 0 || x.FacilityId == facilityId) &&
                            (filter.FinancialYear == 0 || x.FinYear == financialYear) &&
                            DateHelper.StringToDate(x.JDateGregorian) <= endDate &&
                            (branchId == 0 || x.BranchId == branchId) &&
                            (referenceTypeId == 0 || x.ReferenceTypeId == referenceTypeId)
                        ).Sum(x => x.Debit - x.Credit);

                        var employeeAccountBalance = new AccountFromToFilterDto
                        {
                            AccountId = employee.EmpId,
                            EmpCode = employee.EmpId,
                            EmpName = currentData.Language == 1 ? employee.EmpName : employee.EmpName2,
                            AccountName = currentData.Language == 1 ? employee.EmpName : employee.EmpName2,
                            AmountPrevDebit = amountPrev > 0 ? amountPrev.ToString() : "",
                            AmountPrevCredit = amountPrev < 0 ? (amountPrev * -1).ToString() : "",
                            Debit = debit,
                            Credit = credit,
                            AmountCurrentCredit = amountCurrent < 0 ? (amountCurrent * -1).ToString() : "",
                            AmountCurrentDebit = amountCurrent > 0 ? amountCurrent.ToString() : ""
                        };

                        // Apply NoZero filter
                        if (filter.NoZero == false || (amountPrev + debit + credit != 0 || amountPrev != 0))
                        {
                            resultList.Add(employeeAccountBalance);
                        }
                    }

                }

                // Return success result
                if (resultList.Count > 0)
                {
                    return await Result<List<AccountFromToFilterDto>>.SuccessAsync(resultList);
                }
                return await Result<List<AccountFromToFilterDto>>.SuccessAsync(localization.GetResource1("NosearchResult"));
            }
            catch (Exception ex)
            {
                // Return an error result
                return await Result<List<AccountFromToFilterDto>>.FailAsync(ex.Message);
            }
        }
        public async Task<bool> ISHelpAccount(string code, long facilityId)
        {
            var ISHelpAccount = await accRepositoryManager.AccAccountRepository.ISHelpAccount(code, facilityId);
            return ISHelpAccount;
        }
        public async Task<IResult<IEnumerable<AccAccountsVw>>> Search(AccAccountFilterDto filter, CancellationToken cancellationToken = default)
        {
            try
            {
                filter.AccAccountParentId ??= 0;
                filter.AccGroupId ??= 0;
                filter.AccountLevel ??= 0;
                filter.IsActive ??= true;
                var items = await accRepositoryManager.AccAccountRepository.GetAllVw(x =>
                    x.FlagDelete == false &&
                    x.SystemId == 2 &&
                    x.FacilityId == currentData.FacilityId &&
                    (filter.AccAccountParentId == 0 || x.AccAccountParentId == filter.AccAccountParentId) &&
                    (string.IsNullOrEmpty(filter.AccAccountName) || (x.AccAccountName != null && x.AccAccountName == filter.AccAccountName)) &&
                    (string.IsNullOrEmpty(filter.AccAccountCode) || (x.AccAccountCode != null && x.AccAccountCode == filter.AccAccountCode)) &&
                    (filter.AccGroupId == 0 || x.AccGroupId == filter.AccGroupId) &&
                    (filter.IsActive == null || x.IsActive == filter.IsActive) &&
                    (string.IsNullOrEmpty(filter.AccAccountParentCode) || (x.AccAccountCodeParent != null && x.AccAccountCodeParent == filter.AccAccountParentCode)) &&
                    (string.IsNullOrEmpty(filter.AccAccountnameParent) || (x.AccAccountNameParent != null && x.AccAccountNameParent == filter.AccAccountnameParent)) &&
                    (filter.AccountLevel == 0 || x.AccountLevel == filter.AccountLevel) &&
                    (
                        string.IsNullOrEmpty(filter.Code) || string.IsNullOrEmpty(filter.Code2)
                        ? true
                        : string.Compare(x.AccAccountCode, filter.Code) >= 0 && string.Compare(x.AccAccountCode, filter.Code2) <= 0
                    )
                );

                var resultList = items.OrderBy(x => x.AccAccountCode).ToList();

                return await Result<IEnumerable<AccAccountsVw>>.SuccessAsync(resultList, localization.GetResource1("NosearchResult"));
            }
            catch (Exception ex)
            {
                return await Result<IEnumerable<AccAccountsVw>>.FailAsync($"Exception in {GetType().Name}.Search: {ex.Message}");
            }
        }

        public AccAccountFilterDto GetAccAccountFilter(Dictionary<string, string> dictionary)
        {
            var dto = new AccAccountFilterDto();

            if (dictionary.TryGetValue("Code", out var code))
                dto.Code = code;

            if (dictionary.TryGetValue("Code2", out var code2))
                dto.Code2 = code2;

            if (dictionary.TryGetValue("AccAccountCode", out var accAccountCode))
                dto.AccAccountCode = accAccountCode;

            if (dictionary.TryGetValue("AccAccountName", out var accAccountName))
                dto.AccAccountName = accAccountName;

            if (dictionary.TryGetValue("AccAccountParentId", out var parentIdStr) && long.TryParse(parentIdStr, out var parentId))
                dto.AccAccountParentId = parentId;

            if (dictionary.TryGetValue("IsActive", out var isActiveStr) && bool.TryParse(isActiveStr, out var isActive))
                dto.IsActive = isActive;

            if (dictionary.TryGetValue("AccGroupId", out var accGroupIdStr) && long.TryParse(accGroupIdStr, out var accGroupId))
                dto.AccGroupId = accGroupId;

            if (dictionary.TryGetValue("AccAccountParentCode", out var accParentCode))
                dto.AccAccountParentCode = accParentCode;

            if (dictionary.TryGetValue("AccAccountnameParent", out var accParentName))
                dto.AccAccountnameParent = accParentName;

            if (dictionary.TryGetValue("AccountLevel", out var accountLevelStr) && int.TryParse(accountLevelStr, out var accountLevel))
                dto.AccountLevel = accountLevel;

            return dto;
        }

        #region ========================================  Accounts Excel
        public async Task<IResult<List<AccAccountResultExcelDto>>> SaveAccountsExcel(List<AccAccountResultExcelDto> items, CancellationToken cancellationToken = default)
        {
            if (items == null || !items.Any())
                return await Result<List<AccAccountResultExcelDto>>.FailAsync(localization.GetMessagesResource("AddNullEntity"));

            var results = new List<AccAccountResultExcelDto>();

            try
            {
                await accRepositoryManager.UnitOfWork.BeginTransactionAsync(cancellationToken);

                foreach (var rootDto in items)
                {
                    var savedItem = await SaveAccountRecursive(rootDto, null, cancellationToken);
                    results.Add(savedItem);
                }

                await accRepositoryManager.UnitOfWork.CommitTransactionAsync(cancellationToken);

                return await Result<List<AccAccountResultExcelDto>>.SuccessAsync(results, localization.GetMessagesResource("success"));
            }
            catch (Exception ex)
            {
                await accRepositoryManager.UnitOfWork.RollbackTransactionAsync(cancellationToken);
                return await Result<List<AccAccountResultExcelDto>>.FailAsync($"EXP in {GetType().Name}: {ex.Message}");
            }
        }

        private async Task<AccAccountResultExcelDto> SaveAccountRecursive(AccAccountResultExcelDto dto, long? parentId, CancellationToken cancellationToken)
        {
            var CcId = await accRepositoryManager.AccCostCenterRepository
                .GetCostCenterIdByCode(dto.CostCenterCode, currentData.FacilityId);

            var entity = _mapper.Map<AccAccount>(dto);
            entity.SystemId = 2;
            entity.DeptID = 0;
            entity.CcId = CcId;
            entity.AccountCloseTypeId = 0;
            entity.FacilityId = currentData.FacilityId;
            entity.FinYear = 0;
            entity.duration = 0;
            entity.itemType = 0;

            // ربط الحساب الأب إن وجد
            if (parentId.HasValue)
                entity.AccAccountParentId = parentId.Value;

            var savedEntity = await accRepositoryManager.AccAccountRepository.AddAndReturn(entity);
            await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);

            // حفظ الأبناء بشكل متكرر
            if (dto.Children != null && dto.Children.Any())
            {
                foreach (var childDto in dto.Children)
                {
                    await SaveAccountRecursive(childDto, savedEntity.AccAccountId, cancellationToken);
                }
            }

            // التأكد من أن الحساب يشير لنفسه كأب (فقط للأب الأول)
            if (!parentId.HasValue)
            {
                savedEntity.AccAccountParentId = savedEntity.AccAccountId;
                accRepositoryManager.AccAccountRepository.Update(savedEntity);
                await accRepositoryManager.UnitOfWork.CompleteAsync(cancellationToken);
            }

            return _mapper.Map<AccAccountResultExcelDto>(savedEntity);
        }

        public async Task<IResult> DeleteAllAccAccounts(CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await accRepositoryManager.AccJournalDetaileRepository.GetCountAccJournalDetailes(currentData.FacilityId, cancellationToken);

                if (count > 0)
                {
                    return await Result.FailAsync(localization.GetResource1("ImportCannotExecutedDuePreviousTransactions"));
                }

                var delete = await accRepositoryManager.AccAccountRepository.DeleteAllAccAccounts(currentData.FacilityId, currentData.UserId, cancellationToken);

                return await Result.SuccessAsync(localization.GetResource1("DeletedSuccess"));
            }
            catch (Exception ex)
            {
                return await Result.FailAsync($"Error occurred while deleting accounts: {ex.Message}");
            }
        }

    public async Task<IResult<List<AccountBalanceSheetDto>>> AccountTransactionsSearch(AccountBalanceSheetFilterDto filter, CancellationToken cancellationToken = default)
    {
      try
      {
        filter.referenceTypeId ??= 0;
        filter.FacilityId ??= 0;
        if (string.IsNullOrEmpty(filter.EmpCode))
        {
          return await Result<List<AccountBalanceSheetDto>>.FailAsync(localization.GetResource1("EmployeeIsNumber"));

        }
        if (filter.FacilityId <= 0)
        {
          filter.FacilityId = currentData.FacilityId;
        }
        if (filter.chkAllYear == true)
        {
          var result = await GetEmployeeAccountTransactionsForAllYears(filter);
          return  result;

        }
        else
        {
          var result = await GetEmployeeAccountTransactionsForCurrentYear(filter);
          return result;


        }
      }
      catch (Exception ex)
      {
        return await Result<List<AccountBalanceSheetDto>>.FailAsync(ex.Message);
      }
    }

    #endregion ========================================== Accounts Excel
  }
}
