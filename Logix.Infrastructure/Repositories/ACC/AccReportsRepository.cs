using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Data;
using System.Globalization;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class AccReportsRepository : IAccReportsRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentData session;
        private readonly IAccFunctionRepository accFunctionRepository;

        public AccReportsRepository(ApplicationDbContext context, ICurrentData session, IAccFunctionRepository AccFunctionRepository)
        {
            this.context = context;
            this.session = session;
            this.accFunctionRepository = AccFunctionRepository;
        }
        #region =====================================  كشف حساب
        public async Task<IEnumerable<AccountBalanceSheetDto>> GetAccounttransactionsCurrentYearBalance(AccounttransactionsDto obj)
        {
            try
            {
                decimal balance = 0;
                int rowId = 0;

                DateTime parsedDateFrom = DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // استعلام الأرصدة الافتتاحية
                var initialBalanceQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                                b.FlagDelete == false &&
                                b.FacilityId == obj.facilityId &&
                                b.FinYear == obj.FinYear &&
                                  b.AccAccountId == obj.accountId &&
                  (obj.branchId == 0 || b.MbranchId == obj.branchId))
                    .ToListAsync();

                initialBalanceQuery = initialBalanceQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom ||
                                ((b.DocTypeId == 4 || b.DocTypeId == 27) &&
                                 DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                 DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo))
                    .ToList();

              

                // إنشاء قائمة للرصيد الافتتاحي
                List<AccountBalanceSheetDto> initialBalance = new List<AccountBalanceSheetDto>();

                if (initialBalanceQuery.Any())
                {
                    initialBalance = initialBalanceQuery
                        .GroupBy(b => new { b.AccAccountId, b.FacilityId, b.FinYear })
                        .Select(g => new AccountBalanceSheetDto
                        {
                            DocTypeId = 0,
                            ReferenceNo = 0,
                            ReferenceCode = null,
                            DocTypeName = "رصيد افتتاحي",
                            DocTypeName2 = "Opening Balance",
                            JId = 0,
                            RowID = 0,
                            JDateGregorian = null,
                            Description = session.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                            Debit = 0m,
                            Credit = 0m,
                            Balance = g.Max(b => b.NatureAccount) > 0
                                ? g.Sum(b => b.Credit) - g.Sum(b => b.Debit)
                                : g.Sum(b => b.Debit) - g.Sum(b => b.Credit),
                            CostCenterName = null,
                            JCode = null,
                            NatureAccount = g.Max(b => b.NatureAccount) ?? 1,
                            CostsCenter = 0,
                            ChequNo = null
                        })
                        .ToList();
                }
                else
                {
                    initialBalance.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد افتتاحي",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = 0,
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        CostCenterName = null,
                        JCode = null,
                        NatureAccount = 1,
                        CostsCenter = 0,
                        ChequNo = null
                    });
                }

                // إضافة الرصيد الافتتاحي إلى النتيجة كأول عنصر
                List<AccountBalanceSheetDto> resultList = new List<AccountBalanceSheetDto>(initialBalance);

                // تحميل البيانات للأرصدة الرئيسية
                var mainRecordsQuery = await context.AccBalanceSheets
      .AsNoTracking()
      .Where(b => b.DocTypeId != 4 &&
                  b.DocTypeId != 27 &&
                  b.FlagDelete == false &&
                  b.FacilityId == obj.facilityId &&
                  b.AccAccountId == obj.accountId &&
                  b.FinYear == obj.FinYear &&
                  (obj.branchId == 0 || b.MbranchId == obj.branchId))
      .ToListAsync();

                mainRecordsQuery = mainRecordsQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                    .ToList();


                var mainRecords = mainRecordsQuery
                    .Select(b => new AccountBalanceSheetDto
                    {
                        DocTypeId = b.DocTypeId,
                        ReferenceNo = b.ReferenceNo,
                        ReferenceCode = b.ReferenceCode,
                        DocTypeName = b.DocTypeName,
                        DocTypeName2 = b.DocTypeName2,
                        JId = b.JId,
                        RowID = 0,
                        JDateGregorian = b.JDateGregorian,
                        Description = b.Description,
                        Debit = b.Debit,
                        Credit = b.Credit,
                        Balance = 0m,
                        CostCenterName = b.CostCenterName,
                        JCode = b.JCode,
                        NatureAccount = b.NatureAccount,
                        CostsCenter = context.AccJournalDetailesCostcenter
                            .Where(d => d.JDetailesId == b.JDetailesId && d.FlagDelete == false)
                            .Sum(d => d.Debit - d.Credit) ?? 0,
                        ChequNo = b.ChequNo
                    })
                    .ToList();

                // دمج الأرصدة الافتتاحية والرئيسية وحساب الرصيد التراكمي
                var allRecords = resultList.Union(mainRecords)
                    .OrderBy(r => r.JDateGregorian)
                    .ThenBy(r => r.RowID)
                    .ToList();

                foreach (var record in allRecords)
                {
                    if (rowId == 0 && obj.dateFrom != null && obj.dateTo != null)
                        balance = record.Balance ?? 0m;

                    balance += record.NatureAccount >= 0
                        ? (record.Credit ?? 0m) - (record.Debit ?? 0m)
                        : (record.Debit ?? 0m) - (record.Credit ?? 0m);

                    record.Balance = balance;
                    record.RowID = ++rowId;
                }

                return allRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<AccountBalanceSheetDto>();
            }
        }

        public async Task<IEnumerable<AccountBalanceSheetDto>> GetAccounttransactionsAllYearBalance(AccounttransactionsDto obj)
        {
            try
            {
                decimal balance = 0;
                int rowId = -1;

                DateTime parsedDateFrom = DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // استعلام الرصيد الافتتاحي (Doc_Type_ID = 27)
                var initialBalanceQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId &&
                        b.AccAccountId == obj.accountId &&
                        b.DocTypeId == 27 &&
                  (obj.branchId == 0 || b.MbranchId == obj.branchId)
                        )
                    .ToListAsync();

                // تطبيق فلتر التاريخ في الذاكرة
                var initialBalance = initialBalanceQuery
                    //.Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom)
                    .GroupBy(b => new { b.AccAccountId, b.FacilityId })
                    .Select(g => new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد إفتتاحي أول المدة",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = -1,
                           JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد إفتتاحي أول المدة" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = g.Max(b => b.NatureAccount) > 0
                            ? g.Sum(b => b.Credit) - g.Sum(b => b.Debit)
                            : g.Sum(b => b.Debit) - g.Sum(b => b.Credit),
                        NatureAccount = g.Max(b => b.NatureAccount) ?? 1
                    })
                    .FirstOrDefault();

                List<AccountBalanceSheetDto> resultList = new List<AccountBalanceSheetDto>();
                if (initialBalance != null)
                {
                    resultList.Add(initialBalance);
                }
                else
                {
                    // إضافة سطر فارغ إذا لم توجد بيانات لرصيد المدور
                    resultList.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد إفتتاحي أول المدة",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = 0,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد إفتتاحي أول المدة" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        NatureAccount = 1
                    });
                }

                // استعلام الرصيد المدور (التواريخ قبل التاريخ المحدد Doc_Type_ID not in (4, 27))
                var carriedBalanceQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId &&
                        b.AccAccountId == obj.accountId &&
                        b.DocTypeId != 4 &&
                        b.DocTypeId != 27 &&
                  (obj.branchId == 0 || b.MbranchId == obj.branchId))
                    .ToListAsync();

                var carriedBalance = carriedBalanceQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom)
                    .GroupBy(b => new { b.AccAccountId, b.FacilityId })
                    .Select(g => new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد مدور",
                        DocTypeName2 = "Carried Balance",
                        JId = 0,
                        RowID = 0,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد مدور" : "Carried Balance",
                        Debit = g.Sum(b => b.Debit),
                        Credit = g.Sum(b => b.Credit),
                        Balance = g.Max(b => b.NatureAccount) > 0
                            ? g.Sum(b => b.Credit) - g.Sum(b => b.Debit)
                            : g.Sum(b => b.Debit) - g.Sum(b => b.Credit),
                        NatureAccount = g.Max(b => b.NatureAccount) ?? 1
                    })
                    .FirstOrDefault();

                if (carriedBalance != null)
                {
                    resultList.Add(carriedBalance);
                }
                else
                {
                    // إضافة سطر فارغ إذا لم توجد بيانات لرصيد المدور
                    resultList.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد مدور",
                        DocTypeName2 = "Carried Balance",
                        JId = 0,
                        RowID = 0,
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد مدور" : "Carried Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        NatureAccount = 1
                    });
                }

                // تحميل الأرصدة الرئيسية مع التحقق من التواريخ المطلوبة
                var mainRecordsQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b => b.DocTypeId != 4 &&
                                b.DocTypeId != 27 &&
                                b.FlagDelete == false &&
                                b.FacilityId == obj.facilityId &&
                                b.AccAccountId == obj.accountId &&
                  (obj.branchId == 0 || b.MbranchId == obj.branchId))
                    .ToListAsync();

                var mainRecords = mainRecordsQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                    .Select(b => new AccountBalanceSheetDto
                    {
                        DocTypeId = b.DocTypeId,
                        ReferenceNo = b.ReferenceNo,
                        ReferenceCode = b.ReferenceCode ?? b.ReferenceNo.ToString(),
                        DocTypeName = b.DocTypeName,
                        DocTypeName2 = b.DocTypeName2,
                        JId = b.JId,
                        RowID = 0,
                        JCode=b.JCode,
                        JDateGregorian = b.JDateGregorian,
                        Description = b.Description,
                        Debit = b.Debit,
                        Credit = b.Credit,
                        Balance = 0m,
                        NatureAccount = b.NatureAccount,
                        CostsCenter = context.AccJournalDetailesCostcenter
                            .Where(d => d.JDetailesId == b.JDetailesId && d.FlagDelete == false)
                            .Sum(d => d.Debit - d.Credit) ?? 0,
                        ChequNo = b.ChequNo
                    })
                    .ToList();

                // دمج الأرصدة وحساب الرصيد التراكمي
                var allRecords = resultList.Union(mainRecords)
                    .OrderBy(r => r.JDateGregorian)
                    .ThenBy(r => r.RowID)
                    .ToList();

                foreach (var record in allRecords)
                {
                    if (rowId == -1 && obj.dateFrom != null && obj.dateTo != null)
                        balance = record.Balance ?? 0m;

                    balance += record.NatureAccount >= 0
                        ? (record.Credit ?? 0m) - (record.Debit ?? 0m)
                        : (record.Debit ?? 0m) - (record.Credit ?? 0m);

                    record.Balance = balance;
                    record.RowID = ++rowId;
                }

                return allRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<AccountBalanceSheetDto>();
            }
        }

        public async Task<IEnumerable<AccounttransactionsFromToDto>> GetAccounttransactionsFromTo(AccounttransactionsFromToFilterDto obj)
        {
            try
            {
                // تحويل التواريخ
                DateTime? parsedDateFrom = string.IsNullOrEmpty(obj.dateFrom)
                    ? (DateTime?)null
                    : DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime? parsedDateTo = string.IsNullOrEmpty(obj.dateTo)
                    ? (DateTime?)null
                    : DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // الاستعلام الأساسي (فلترة قابلة للترجمة إلى SQL)
                var query = context.AccBalanceSheetCostCenterVw
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId &&
                        b.FinYear == obj.FinYear  &&
                            string.Compare(b.AccAccountCode, obj.AccountCode) >= 0 && 
                            string.Compare(b.AccAccountCode, obj.AccountCode2) <= 0 &&

                        (string.IsNullOrEmpty(obj.CostCenterCodeFrom) ||
                            (b.CostCenterCode.CompareTo(obj.CostCenterCodeFrom) >= 0 &&
                             b.CostCenterCode.CompareTo(obj.CostCenterCodeTo) <= 0) ||
                            (b.CostCenter2Code.CompareTo(obj.CostCenterCodeFrom) >= 0 &&
                             b.CostCenter2Code.CompareTo(obj.CostCenterCodeTo) <= 0) ||
                            (b.CostCenter3Code.CompareTo(obj.CostCenterCodeFrom) >= 0 &&
                             b.CostCenter3Code.CompareTo(obj.CostCenterCodeTo) <= 0) ||
                            (b.CostCenter4Code.CompareTo(obj.CostCenterCodeFrom) >= 0 &&
                             b.CostCenter4Code.CompareTo(obj.CostCenterCodeTo) <= 0) ||
                            (b.CostCenter5Code.CompareTo(obj.CostCenterCodeFrom) >= 0 &&
                             b.CostCenter5Code.CompareTo(obj.CostCenterCodeTo) <= 0)));

                // نقل الفلترة بالتواريخ إلى الجانب العميل
                var filteredQuery = query.AsEnumerable()
                    .Where(b =>
                        (!parsedDateFrom.HasValue || DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom) &&
                        (!parsedDateTo.HasValue || DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo));

                // الاستعلام الإجمالي
                var totalResult = filteredQuery
     .GroupBy(b => 1) // تجميع الكل في مجموعة واحدة
     .Select(g => new AccounttransactionsFromToDto
     {
         AccAccountName = "الإجمالي",
         AccAccountName2 = "Total",
         AccAccountCode = null,
         Debit = g.Sum(b => b.Debit),
         Credit = g.Sum(b => b.Credit),
         Balance = (g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Max(b => b.NatureAccount)
     })
     .ToList();

                // حساب إجمالي الرصيد
                decimal totalBalance = totalResult.FirstOrDefault()?.Balance ?? 0;

                // تحديث `Rate` للإجمالي
                totalResult.ForEach(r =>
                {
                    r.Rate = totalBalance != 0 ? 100 : 0;
                });


                // استعلام التجميع حسب الحسابات
                var groupedResult = filteredQuery
                    .GroupBy(b => new { b.AccAccountName, b.AccAccountName2, b.AccAccountCode, b.NatureAccount })
                    .Select(g => new AccounttransactionsFromToDto
                    {
                        AccAccountName = g.Key.AccAccountName,
                        AccAccountName2 = g.Key.AccAccountName2,
                        AccAccountCode = g.Key.AccAccountCode,
                        Debit = g.Sum(b => b.Debit),
                        Credit = g.Sum(b => b.Credit),
                        Balance = (g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Key.NatureAccount,
                        Rate = totalBalance != null && totalBalance != 0
    ? Math.Round(((g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Key.NatureAccount * 100) / totalBalance ?? 1.0m, 2)
    : 0,

                    })
                    .ToList();

                // إضافة نسبة الإجمالي
                totalResult.ForEach(r => r.Rate = 100);

                // دمج النتائج وترتيبها حسب كود الحساب
                var finalResult = totalResult
                    .Union(groupedResult)
                    .OrderBy(r => r.AccAccountCode ?? "") // ترتيب مع التعامل مع القيم الفارغة
                    .ToList();

                return finalResult;
            }
            catch (Exception ex)
            {
                // سجل الخطأ
                Console.WriteLine($"Error in GetAccounttransactionsGroup: {ex.Message}");
                return new List<AccounttransactionsFromToDto>(); // إرجاع قائمة فارغة في حالة الخطأ
            }
        }


        #endregion



        #region =====================================  كشف حساب عميل
        public async Task<IEnumerable<AccountBalanceSheetDto>> GetCustomerCurrentYearBalance(AccounttransactionsDto obj)
        {
            try
            {
                decimal balance = 0;
                int rowId = 0;

                DateTime parsedDateFrom = DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // استعلام الأرصدة الافتتاحية
                var initialBalanceQuery = await context.AccBalanceSheetPostOrNot
                    .AsNoTracking()
                    .Where(b =>
                                b.FlagDelete == false &&
                                b.FacilityId == obj.facilityId &&
                                b.FinYear == obj.FinYear &&
                                b.ReferenceDNo == obj.ReferenceDNo &&
                                b.ParentReferenceTypeId == obj.ParentReferenceTypeId &&
                     (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
        (obj.ReferenceTypeId == 0 || b.ReferenceTypeId == obj.ReferenceTypeId)
       && (obj.branchId == 0 || b.MbranchId == obj.branchId))
                .ToListAsync();
                initialBalanceQuery = initialBalanceQuery
                .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom ||
                ((b.DocTypeId == 4 || b.DocTypeId == 27) &&
                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                 DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo))
                    .ToList();

               

                // إنشاء قائمة للرصيد الافتتاحي
                List<AccountBalanceSheetDto> initialBalance = new List<AccountBalanceSheetDto>();

                if (initialBalanceQuery.Any())
                {
                    initialBalance = initialBalanceQuery
                        .GroupBy(b => new { b.AccAccountId, b.FacilityId, b.FinYear })
                        .Select(g => new AccountBalanceSheetDto
                        {
                            DocTypeId = 0,
                            ReferenceNo = 0,
                            ReferenceCode = null,
                            DocTypeName = "رصيد افتتاحي",
                            DocTypeName2 = "Opening Balance",
                            JId = 0,
                            RowID = 0,
                            JDateGregorian = null,
                            Description = session.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                            Debit = 0m,
                            Credit = 0m,
                            Balance = g.Max(b => b.NatureAccount) > 0
                                ? g.Sum(b => b.Credit) - g.Sum(b => b.Debit)
                                : g.Sum(b => b.Debit) - g.Sum(b => b.Credit),
                            CostCenterName = null,
                            JCode = null,
                            NatureAccount = g.Max(b => b.NatureAccount) ?? 1,
                            CostsCenter = 0,
                            ChequNo = null
                        })
                        .ToList();
                }
                else
                {
                    initialBalance.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد افتتاحي",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = 0,
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        CostCenterName = null,
                        JCode = null,
                        NatureAccount = 1,
                        CostsCenter = 0,
                        ChequNo = null
                    });
                }

                // إضافة الرصيد الافتتاحي إلى النتيجة كأول عنصر
                List<AccountBalanceSheetDto> resultList = new List<AccountBalanceSheetDto>(initialBalance);

                // تحميل البيانات للأرصدة الرئيسية
                var mainRecordsQuery = await context.AccBalanceSheetPostOrNot
                    .AsNoTracking()
                    .Where(b => b.DocTypeId != 4 &&
                                b.DocTypeId != 27 &&
                                b.FlagDelete == false &&
                                b.FacilityId == obj.facilityId &&
                                b.FinYear == obj.FinYear &&
                                b.ReferenceDNo == obj.ReferenceDNo &&
                                b.ParentReferenceTypeId == obj.ParentReferenceTypeId &&
                     (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
        (obj.ReferenceTypeId == 0 || b.ReferenceTypeId == obj.ReferenceTypeId)
       && (obj.branchId == 0 || b.MbranchId == obj.branchId) )
                    .ToListAsync();

                mainRecordsQuery = mainRecordsQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                    .ToList();

                if (obj.branchId != 0)
                {
                    mainRecordsQuery = mainRecordsQuery.Where(b => b.MbranchId == obj.branchId).ToList();
                }

                var mainRecords = mainRecordsQuery
                    .Select(b => new AccountBalanceSheetDto
                    {
                        DocTypeId = b.DocTypeId,
                        ReferenceNo = b.ReferenceNo,
                        ReferenceCode = b.ReferenceCode,
                        DocTypeName = b.DocTypeName,
                        DocTypeName2 = b.DocTypeName2,
                        JId = b.JId,
                        RowID = 0,
                        JDateGregorian = b.JDateGregorian,
                        Description = b.Description,
                        Debit = b.Debit,
                        Credit = b.Credit,
                        Balance = 0m,
                        CostCenterName = b.CostCenterName,
                        JCode = b.JCode,
                        NatureAccount = b.NatureAccount,
                        CostsCenter = context.AccJournalDetailesCostcenter
                            .Where(d => d.JDetailesId == b.JDetailesId && d.FlagDelete == false)
                            .Sum(d => d.Debit - d.Credit) ?? 0,
                        ChequNo = b.ChequNo
                    })
                    .ToList();

                // دمج الأرصدة الافتتاحية والرئيسية وحساب الرصيد التراكمي
                var allRecords = resultList.Union(mainRecords)
                    .OrderBy(r => r.JDateGregorian)
                    .ThenBy(r => r.RowID)
                    .ToList();

                foreach (var record in allRecords)
                {
                    if (rowId == 0 && obj.dateFrom != null && obj.dateTo != null)
                        balance = record.Balance ?? 0m;

                    balance += record.NatureAccount >= 0
                        ? (record.Credit ?? 0m) - (record.Debit ?? 0m)
                        : (record.Debit ?? 0m) - (record.Credit ?? 0m);

                    record.Balance = balance;
                    record.RowID = ++rowId;
                }

                return allRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<AccountBalanceSheetDto>();
            }
        }

        public async Task<IEnumerable<AccountBalanceSheetDto>> GetCustomerAllYearBalance(AccounttransactionsDto obj)
        {
            try
            {
                decimal balance = 0;
                int rowId = -1;

                DateTime parsedDateFrom = DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // استعلام الرصيد الافتتاحي (Doc_Type_ID = 27)
                var initialBalanceQuery = await context.AccBalanceSheetPostOrNot
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId &&
                        b.DocTypeId == 27 &&
                          b.ReferenceDNo == obj.ReferenceDNo &&
                          b.ParentReferenceTypeId == obj.ParentReferenceTypeId &&
                    (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
        (obj.ReferenceTypeId == 0 || b.ReferenceTypeId == obj.ReferenceTypeId)&&
        (obj.branchId == 0 || b.MbranchId == obj.branchId)
                        )
                    .ToListAsync();

                // تطبيق فلتر التاريخ في الذاكرة
                var initialBalance = initialBalanceQuery
                    .GroupBy(b => new { b.AccAccountId, b.FacilityId })
                    .Select(g => new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد إفتتاحي أول المدة",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = -1,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد إفتتاحي أول المدة" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = g.Max(b => b.NatureAccount) > 0
                            ? g.Sum(b => b.Credit) - g.Sum(b => b.Debit)
                            : g.Sum(b => b.Debit) - g.Sum(b => b.Credit),
                        NatureAccount = g.Max(b => b.NatureAccount) ?? 1
                    })
                    .FirstOrDefault();

                List<AccountBalanceSheetDto> resultList = new List<AccountBalanceSheetDto>();
                if (initialBalance != null)
                {
                    resultList.Add(initialBalance);
                }
                else
                {
                    // إضافة سطر فارغ إذا لم توجد بيانات لرصيد المدور
                    resultList.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد إفتتاحي أول المدة",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = 0,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد إفتتاحي أول المدة" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        NatureAccount = 1
                    });
                }

                // استعلام الرصيد المدور (التواريخ قبل التاريخ المحدد Doc_Type_ID not in (4, 27))
                var carriedBalanceQuery = await context.AccBalanceSheetPostOrNot
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId &&
                        b.DocTypeId != 4 &&
                        b.DocTypeId != 27 &&
                           b.ReferenceDNo == obj.ReferenceDNo &&
                          b.ParentReferenceTypeId == obj.ParentReferenceTypeId &&
                   (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
        (obj.ReferenceTypeId == 0 || b.ReferenceTypeId == obj.ReferenceTypeId) &&
       (obj.branchId == 0 || b.MbranchId == obj.branchId)

                        )
                    .ToListAsync();

                var carriedBalance = carriedBalanceQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom)
                    .GroupBy(b => new { b.AccAccountId, b.FacilityId })
                    .Select(g => new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد مدور",
                        DocTypeName2 = "Carried Balance",
                        JId = 0,
                        RowID = 0,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد مدور" : "Carried Balance",
                        Debit = g.Sum(b => b.Debit),
                        Credit = g.Sum(b => b.Credit),
                        Balance = g.Max(b => b.NatureAccount) > 0
                            ? g.Sum(b => b.Credit) - g.Sum(b => b.Debit)
                            : g.Sum(b => b.Debit) - g.Sum(b => b.Credit),
                        NatureAccount = g.Max(b => b.NatureAccount) ?? 1
                    })
                    .FirstOrDefault();

                if (carriedBalance != null)
                {
                    resultList.Add(carriedBalance);
                }
                else
                {
                    // إضافة سطر فارغ إذا لم توجد بيانات لرصيد المدور
                    resultList.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد مدور",
                        DocTypeName2 = "Carried Balance",
                        JId = 0,
                        RowID = 0,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد مدور" : "Carried Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        NatureAccount = 1
                    });
                }

                // تحميل الأرصدة الرئيسية مع التحقق من التواريخ المطلوبة
                var mainRecordsQuery = await context.AccBalanceSheetPostOrNot
                    .AsNoTracking()
                    .Where(b => b.DocTypeId != 4 &&
                                b.DocTypeId != 27 &&
                                b.FlagDelete == false &&
                                b.FacilityId == obj.facilityId &&
                                      b.ReferenceDNo == obj.ReferenceDNo &&
       b.ParentReferenceTypeId == obj.ParentReferenceTypeId &&
  (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
        (obj.ReferenceTypeId == 0 || b.ReferenceTypeId == obj.ReferenceTypeId)&&
        (obj.branchId == 0 || b.MbranchId == obj.branchId)
)
                    .ToListAsync();

                var mainRecords = mainRecordsQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                    .Select(b => new AccountBalanceSheetDto
                    {
                        DocTypeId = b.DocTypeId,
                        ReferenceNo = b.ReferenceNo,
                        ReferenceCode = b.ReferenceCode ?? b.ReferenceNo.ToString(),
                        DocTypeName = b.DocTypeName,
                        DocTypeName2 = b.DocTypeName2,
                        JId = b.JId,
                        RowID = 0,
                        JCode = b.JCode,
                        JDateGregorian = b.JDateGregorian,
                        Description = b.Description,
                        Debit = b.Debit,
                        Credit = b.Credit,
                        Balance = 0m,
                        NatureAccount = b.NatureAccount,
                        CostsCenter = context.AccJournalDetailesCostcenter
                            .Where(d => d.JDetailesId == b.JDetailesId && d.FlagDelete == false)
                            .Sum(d => d.Debit - d.Credit) ?? 0,
                        ChequNo = b.ChequNo
                    })
                    .ToList();

                // دمج الأرصدة وحساب الرصيد التراكمي
                var allRecords = resultList.Union(mainRecords)
                    .OrderBy(r => r.JDateGregorian)
                    .ThenBy(r => r.RowID)
                    .ToList();

                foreach (var record in allRecords)
                {
                    if (rowId == -1 && obj.dateFrom != null && obj.dateTo != null)
                        balance = record.Balance ?? 0m;

                    balance += record.NatureAccount >= 0
                        ? (record.Credit ?? 0m) - (record.Debit ?? 0m)
                        : (record.Debit ?? 0m) - (record.Credit ?? 0m);

                    record.Balance = balance;
                    record.RowID = ++rowId;
                }

                return allRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<AccountBalanceSheetDto>();
            }
        }

        #endregion


        #region =====================================  كشف حساب  مورد مقاول 
        public async Task<IEnumerable<AccountBalanceSheetDto>> GetSupplierCurrentYearBalance(AccounttransactionsDto obj)
        {
            try
            {
                decimal balance = 0;
                int rowId = 0;

                DateTime parsedDateFrom = DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // استعلام الأرصدة الافتتاحية
                           
                var initialBalanceQuery = await context.AccBalanceSheets
    .AsNoTracking()
    .Where(b =>
        b.FlagDelete == false &&
        b.FacilityId == obj.facilityId &&
        b.FinYear == obj.FinYear &&
        b.ReferenceDNo == obj.ReferenceDNo &&
        b.ParentReferenceTypeId == obj.ParentReferenceTypeId &&
        (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
        (obj.ReferenceTypeId == 0 || b.ReferenceTypeId == obj.ReferenceTypeId)
       && (obj.branchId == 0 || b.MbranchId == obj.branchId)
    )
    .ToListAsync();

                initialBalanceQuery = initialBalanceQuery
                .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom ||
                ((b.DocTypeId == 4 || b.DocTypeId == 27) &&
                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                 DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo))
                    .ToList();

              
                // إنشاء قائمة للرصيد الافتتاحي
                List<AccountBalanceSheetDto> initialBalance = new List<AccountBalanceSheetDto>();

                if (initialBalanceQuery.Any())
                {
                    initialBalance = initialBalanceQuery
                        .GroupBy(b => new { b.AccAccountId, b.FacilityId, b.FinYear })
                        .Select(g => new AccountBalanceSheetDto
                        {
                            DocTypeId = 0,
                            ReferenceNo = 0,
                            ReferenceCode = null,
                            DocTypeName = "رصيد افتتاحي",
                            DocTypeName2 = "Opening Balance",
                            JId = 0,
                            RowID = 0,
                            JDateGregorian = null,
                            Description = session.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                            Debit = 0m,
                            Credit = 0m,
                            Balance = g.Max(b => b.NatureAccount) > 0
                                ? g.Sum(b => b.Credit) - g.Sum(b => b.Debit)
                                : g.Sum(b => b.Debit) - g.Sum(b => b.Credit),
                            CostCenterName = null,
                            JCode = null,
                            NatureAccount = g.Max(b => b.NatureAccount) ?? 1,
                            CostsCenter = 0,
                            ChequNo = null
                        })
                        .ToList();
                }
                else
                {
                    initialBalance.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد افتتاحي",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = 0,
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        CostCenterName = null,
                        JCode = null,
                        NatureAccount = 1,
                        CostsCenter = 0,
                        ChequNo = null
                    });
                }

                // إضافة الرصيد الافتتاحي إلى النتيجة كأول عنصر
                List<AccountBalanceSheetDto> resultList = new List<AccountBalanceSheetDto>(initialBalance);

                // تحميل البيانات للأرصدة الرئيسية
                var mainRecordsQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b => b.DocTypeId != 4 &&
                                b.DocTypeId != 27 &&
                                b.FlagDelete == false &&
                                b.FacilityId == obj.facilityId &&
                                b.FinYear == obj.FinYear &&
                                b.ReferenceDNo == obj.ReferenceDNo &&
                                b.ParentReferenceTypeId == obj.ParentReferenceTypeId &&
                    (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
                   (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
        (obj.ReferenceTypeId == 0 || b.ReferenceTypeId == obj.ReferenceTypeId)
        &&
        (obj.branchId == 0 || b.MbranchId == obj.branchId)
                                )
                    .ToListAsync();

                mainRecordsQuery = mainRecordsQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                    .ToList();

                if (obj.branchId != 0)
                {
                    mainRecordsQuery = mainRecordsQuery.Where(b => b.MbranchId == obj.branchId).ToList();
                }

                var mainRecords = mainRecordsQuery
                    .Select(b => new AccountBalanceSheetDto
                    {
                        DocTypeId = b.DocTypeId,
                        ReferenceNo = b.ReferenceNo,
                        ReferenceCode = b.ReferenceCode,
                        DocTypeName = b.DocTypeName,
                        DocTypeName2 = b.DocTypeName2,
                        JId = b.JId,
                        RowID = 0,
                        JDateGregorian = b.JDateGregorian,
                        Description = b.Description,
                        Debit = b.Debit,
                        Credit = b.Credit,
                        Balance = 0m,
                        CostCenterName = b.CostCenterName,
                        JCode = b.JCode,
                        NatureAccount = b.NatureAccount,
                        CostsCenter = context.AccJournalDetailesCostcenter
                            .Where(d => d.JDetailesId == b.JDetailesId && d.FlagDelete == false)
                            .Sum(d => d.Debit - d.Credit) ?? 0,
                        ChequNo = b.ChequNo
                    })
                    .ToList();

                // دمج الأرصدة الافتتاحية والرئيسية وحساب الرصيد التراكمي
                var allRecords = resultList.Union(mainRecords)
                    .OrderBy(r => r.JDateGregorian)
                    .ThenBy(r => r.RowID)
                    .ToList();

                foreach (var record in allRecords)
                {
                    if (rowId == 0 && obj.dateFrom != null && obj.dateTo != null)
                        balance = record.Balance ?? 0m;

                    balance += record.NatureAccount >= 0
                        ? (record.Credit ?? 0m) - (record.Debit ?? 0m)
                        : (record.Debit ?? 0m) - (record.Credit ?? 0m);

                    record.Balance = balance;
                    record.RowID = ++rowId;
                }

                return allRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<AccountBalanceSheetDto>();
            }
        }

        public async Task<IEnumerable<AccountBalanceSheetDto>> GetSupplierAllYearBalance(AccounttransactionsDto obj)
        {
            try
            {
                decimal balance = 0;
                int rowId = -1;

                DateTime parsedDateFrom = DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // استعلام الرصيد الافتتاحي (Doc_Type_ID = 27)
                var initialBalanceQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId &&
                        b.DocTypeId == 27 &&
                          b.ReferenceDNo == obj.ReferenceDNo &&
                          b.ParentReferenceTypeId == obj.ParentReferenceTypeId &&
                   (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
        (obj.ReferenceTypeId == 0 || b.ReferenceTypeId == obj.ReferenceTypeId) 
        
        &&
        (obj.branchId == 0 || b.MbranchId == obj.branchId)
                        )
                    .ToListAsync();

                // تطبيق فلتر التاريخ في الذاكرة
                var initialBalance = initialBalanceQuery
                    .GroupBy(b => new { b.AccAccountId, b.FacilityId })
                    .Select(g => new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد إفتتاحي أول المدة",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = -1,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد إفتتاحي أول المدة" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = g.Max(b => b.NatureAccount) > 0
                            ? g.Sum(b => b.Credit) - g.Sum(b => b.Debit)
                            : g.Sum(b => b.Debit) - g.Sum(b => b.Credit),
                        NatureAccount = g.Max(b => b.NatureAccount) ?? 1
                    })
                    .FirstOrDefault();

                List<AccountBalanceSheetDto> resultList = new List<AccountBalanceSheetDto>();
                if (initialBalance != null)
                {
                    resultList.Add(initialBalance);
                }
                else
                {
                    // إضافة سطر فارغ إذا لم توجد بيانات لرصيد المدور
                    resultList.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد إفتتاحي أول المدة",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = 0,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد إفتتاحي أول المدة" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        NatureAccount = 1
                    });
                }

                // استعلام الرصيد المدور (التواريخ قبل التاريخ المحدد Doc_Type_ID not in (4, 27))
                var carriedBalanceQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId &&
                        b.DocTypeId != 4 &&
                        b.DocTypeId != 27 &&
                           b.ReferenceDNo == obj.ReferenceDNo &&
                          b.ParentReferenceTypeId == obj.ParentReferenceTypeId &&
                    (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
        (obj.ReferenceTypeId == 0 || b.ReferenceTypeId == obj.ReferenceTypeId)
        &&(obj.branchId == 0 || b.MbranchId == obj.branchId)

                        )
                    .ToListAsync();

                var carriedBalance = carriedBalanceQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom)
                    .GroupBy(b => new { b.AccAccountId, b.FacilityId })
                    .Select(g => new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد مدور",
                        DocTypeName2 = "Carried Balance",
                        JId = 0,
                        RowID = 0,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد مدور" : "Carried Balance",
                        Debit = g.Sum(b => b.Debit),
                        Credit = g.Sum(b => b.Credit),
                        Balance = g.Max(b => b.NatureAccount) > 0
                            ? g.Sum(b => b.Credit) - g.Sum(b => b.Debit)
                            : g.Sum(b => b.Debit) - g.Sum(b => b.Credit),
                        NatureAccount = g.Max(b => b.NatureAccount) ?? 1
                    })
                    .FirstOrDefault();

                if (carriedBalance != null)
                {
                    resultList.Add(carriedBalance);
                }
                else
                {
                    // إضافة سطر فارغ إذا لم توجد بيانات لرصيد المدور
                    resultList.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد مدور",
                        DocTypeName2 = "Carried Balance",
                        JId = 0,
                        RowID = 0,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد مدور" : "Carried Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        NatureAccount = 1
                    });
                }

                // تحميل الأرصدة الرئيسية مع التحقق من التواريخ المطلوبة
                var mainRecordsQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b => b.DocTypeId != 4 &&
                                b.DocTypeId != 27 &&
                                b.FlagDelete == false &&
                                b.FacilityId == obj.facilityId &&
                                      b.ReferenceDNo == obj.ReferenceDNo &&
       b.ParentReferenceTypeId == obj.ParentReferenceTypeId &&
 (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
        (obj.ReferenceTypeId == 0 || b.ReferenceTypeId == obj.ReferenceTypeId) 
        &&(obj.branchId == 0 || b.MbranchId == obj.branchId)
)
                    .ToListAsync();

                var mainRecords = mainRecordsQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                    .Select(b => new AccountBalanceSheetDto
                    {
                        DocTypeId = b.DocTypeId,
                        ReferenceNo = b.ReferenceNo,
                        ReferenceCode = b.ReferenceCode ?? b.ReferenceNo.ToString(),
                        DocTypeName = b.DocTypeName,
                        DocTypeName2 = b.DocTypeName2,
                        JId = b.JId,
                        RowID = 0,
                        JCode = b.JCode,
                        JDateGregorian = b.JDateGregorian,
                        Description = b.Description,
                        Debit = b.Debit,
                        Credit = b.Credit,
                        Balance = 0m,
                        NatureAccount = b.NatureAccount,
                        CostsCenter = context.AccJournalDetailesCostcenter
                            .Where(d => d.JDetailesId == b.JDetailesId && d.FlagDelete == false)
                            .Sum(d => d.Debit - d.Credit) ?? 0,
                        ChequNo = b.ChequNo
                    })
                    .ToList();

                // دمج الأرصدة وحساب الرصيد التراكمي
                var allRecords = resultList.Union(mainRecords)
                    .OrderBy(r => r.JDateGregorian)
                    .ThenBy(r => r.RowID)
                    .ToList();

                foreach (var record in allRecords)
                {
                    if (rowId == -1 && obj.dateFrom != null && obj.dateTo != null)
                        balance = record.Balance ?? 0m;

                    balance += record.NatureAccount >= 0
                        ? (record.Credit ?? 0m) - (record.Debit ?? 0m)
                        : (record.Debit ?? 0m) - (record.Credit ?? 0m);

                    record.Balance = balance;
                    record.RowID = ++rowId;
                }

                return allRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<AccountBalanceSheetDto>();
            }
        }

        #endregion

        #region =====================================  كشف حساب مجموعة


        

        public async Task<IEnumerable<AccounttransactionsGroupDto>> GetAccounttransactionsGroup(AccounttransactionsFilterGroupDto obj)
        {
            try
            {
                // تحويل التواريخ
                DateTime? parsedDateFrom = string.IsNullOrEmpty(obj.dateFrom)
                    ? (DateTime?)null
                    : DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime? parsedDateTo = string.IsNullOrEmpty(obj.dateTo)
                    ? (DateTime?)null
                    : DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // الاستعلام الأساسي (فلترة قابلة للترجمة إلى SQL)
                var query = context.AccBalanceSheetCostCenterVw
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId &&
                        b.AccAccountCode.StartsWith(obj.AccountCodeParent) &&
                        b.FinYear == obj.FinYear &&
                        (obj.branchId == 0 || b.MbranchId == obj.branchId) &&
                        (string.IsNullOrEmpty(obj.CostCenterCodeFrom) ||
                            (b.CostCenterCode.CompareTo(obj.CostCenterCodeFrom) >= 0 &&
                             b.CostCenterCode.CompareTo(obj.CostCenterCodeTo) <= 0) ||
                            (b.CostCenter2Code.CompareTo(obj.CostCenterCodeFrom) >= 0 &&
                             b.CostCenter2Code.CompareTo(obj.CostCenterCodeTo) <= 0) ||
                            (b.CostCenter3Code.CompareTo(obj.CostCenterCodeFrom) >= 0 &&
                             b.CostCenter3Code.CompareTo(obj.CostCenterCodeTo) <= 0) ||
                            (b.CostCenter4Code.CompareTo(obj.CostCenterCodeFrom) >= 0 &&
                             b.CostCenter4Code.CompareTo(obj.CostCenterCodeTo) <= 0) ||
                            (b.CostCenter5Code.CompareTo(obj.CostCenterCodeFrom) >= 0 &&
                             b.CostCenter5Code.CompareTo(obj.CostCenterCodeTo) <= 0)));

                // نقل الفلترة بالتواريخ إلى الجانب العميل
                var filteredQuery = query.AsEnumerable()
                    .Where(b =>
                        (!parsedDateFrom.HasValue || DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom) &&
                        (!parsedDateTo.HasValue || DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo));

                // الاستعلام الإجمالي
                var totalResult = filteredQuery
     .GroupBy(b => 1) // تجميع الكل في مجموعة واحدة
     .Select(g => new AccounttransactionsGroupDto
     {
         AccAccountName = "الإجمالي",
         AccAccountName2 = "Total",
         AccAccountCode = null,
         Debit = g.Sum(b => b.Debit),
         Credit = g.Sum(b => b.Credit),
         Balance = (g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Max(b => b.NatureAccount)
     })
     .ToList();

                // حساب إجمالي الرصيد
                decimal totalBalance = totalResult.FirstOrDefault()?.Balance ?? 0;

                // تحديث `Rate` للإجمالي
                totalResult.ForEach(r =>
                {
                    r.Rate = totalBalance != 0 ? 100 : 0;
                });


                // استعلام التجميع حسب الحسابات
                var groupedResult = filteredQuery
                    .GroupBy(b => new { b.AccAccountName, b.AccAccountName2, b.AccAccountCode, b.NatureAccount })
                    .Select(g => new AccounttransactionsGroupDto
                    {
                        AccAccountName = g.Key.AccAccountName,
                        AccAccountName2 = g.Key.AccAccountName2,
                        AccAccountCode = g.Key.AccAccountCode,
                        Debit = g.Sum(b => b.Debit),
                        Credit = g.Sum(b => b.Credit),
                        Balance = (g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Key.NatureAccount,
                        Rate = totalBalance != null && totalBalance != 0
    ? Math.Round(((g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Key.NatureAccount * 100) / totalBalance??1.0m, 2)
    : 0,

                    })
                    .ToList();

                // إضافة نسبة الإجمالي
                totalResult.ForEach(r => r.Rate = 100);

                // دمج النتائج وترتيبها حسب كود الحساب
                var finalResult = totalResult
                    .Union(groupedResult)
                    .OrderBy(r => r.AccAccountCode ?? "") // ترتيب مع التعامل مع القيم الفارغة
                    .ToList();

                return finalResult;
            }
            catch (Exception ex)
            {
                // سجل الخطأ
                Console.WriteLine($"Error in GetAccounttransactionsGroup: {ex.Message}");
                return new List<AccounttransactionsGroupDto>(); // إرجاع قائمة فارغة في حالة الخطأ
            }
        }

        #endregion

        #region =====================================  كشف حساب مركز تكلفة
        public async Task<IEnumerable<CostcentertransactionsDto>> GetCostcentertransactions(CostcentertransactionsFilterDto obj)
        {
            try
            {
                decimal balance = 0;
                int rowId = 0;

                DateTime parsedDateFrom = DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

         

    


               

                // إنشاء قائمة للرصيد الافتتاحي
                List<CostcentertransactionsDto> initialBalance = new List<CostcentertransactionsDto>();

                // استعلام الأرصدة الافتتاحية
                decimal openingBalance = (await context.AccBalanceSheets
     .AsNoTracking()
     .Where(b =>
         b.FlagDelete == false &&
         b.FacilityId == obj.facilityId &&
         (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
         (obj.chkAllYear == false || b.FinYear == obj.FinYear))
     .ToListAsync()) // تحميل البيانات في الذاكرة
     .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom)
     .Sum(b => b.Debit - b.Credit) ?? 0;


                // إنشاء إدخال الرصيد الافتتاحي
                List<CostcentertransactionsDto> resultList = new List<CostcentertransactionsDto>
   {
       new CostcentertransactionsDto
       {
           DocTypeId = 0,
           ReferenceNo = 0,
           ReferenceCode = null,
           DocTypeName = "رصيد افتتاحي",
           DocTypeName2 = "Opening Balance",
           AccAccountName = "",
           AccAccountName2 = "",
           JId = 0,
           RowID = 0,
           JDateGregorian = null,
           Description = session.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
           Debit = 0m,
           Credit = 0m,
           Balance = openingBalance,
           CostCenterName = null,
           JCode = null,
           NatureAccount = 1,
           CostsCenter = 0,
           ChequNo = null
       }
   };

                // إضافة الرصيد الافتتاحي إلى النتيجة كأول عنصر
              
                // تحميل البيانات للأرصدة الرئيسية
                var mainRecordsQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId &&
                        (obj.ccId == 0 || b.CcId == obj.ccId || b.Cc2Id == obj.ccId || b.Cc3Id == obj.ccId || b.Cc4Id == obj.ccId || b.Cc5Id == obj.ccId) &&
                        (obj.chkAllYear == false || b.FinYear == obj.FinYear) &&
                        (string.IsNullOrEmpty(obj.AccGrouplist) || obj.AccGrouplist.Contains(b.AccGroupId.ToString())))
                    .ToListAsync();

                mainRecordsQuery = mainRecordsQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                    .ToList();

                var mainRecords = mainRecordsQuery
                    .Select(b => new CostcentertransactionsDto
                    {
                        DocTypeId = b.DocTypeId,
                        ReferenceNo = b.ReferenceNo,
                        ReferenceCode = b.ReferenceCode,
                        DocTypeName = b.DocTypeName,
                        DocTypeName2 = b.DocTypeName2,
                        AccAccountName = b.AccAccountName,
                        AccAccountName2 = b.AccAccountName2,
                        JId = b.JId,
                        RowID = 0,
                        JDateGregorian = b.JDateGregorian,
                        Description = b.Description,
                        Debit = b.Debit,
                        Credit = b.Credit,
                        Balance = 0m,
                        CostCenterName = b.CostCenterName,
                        JCode = b.JCode,
                        NatureAccount = b.NatureAccount,
                        CostsCenter = context.AccJournalDetailesCostcenter
                            .Where(d => d.JDetailesId == b.JDetailesId && d.FlagDelete == false)
                            .Sum(d => d.Debit - d.Credit) ?? 0,
                        ChequNo = b.ChequNo
                    })
                    .ToList();

                // دمج الأرصدة الافتتاحية والرئيسية وحساب الرصيد التراكمي
                var allRecords = resultList.Union(mainRecords)
                   .OrderBy(r => r.JDateGregorian)
                   .ThenBy(r => r.RowID)
                   .ToList();

                foreach (var record in allRecords)
                {
                    if (rowId == 0 && obj.dateFrom != null && obj.dateTo != null)
                        balance = record.Balance.Value;


                    balance += (record.Debit ?? 0m) - (record.Credit ?? 0m);

                    record.Balance = balance;
                    record.RowID = ++rowId;
                }

                return allRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<CostcentertransactionsDto>();
            }
        }

        #endregion

        #region =====================================  كشف حساب بتاريخ العملية

        public async Task<IEnumerable<AccountBalanceSheetDto>> GetAccountStatementTransactionDateYear(AccountTransactionDateFilterDto obj)
        {
            try
            {
                decimal balance = 0;
                int rowId = 0;

                DateTime parsedDateFrom = DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // استعلام الأرصدة الافتتاحية
                var initialBalanceQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                                b.FlagDelete == false &&
                                b.FacilityId == obj.facilityId &&
                                b.FinYear == obj.FinYear &&
                                   b.AccAccountId == obj.accountId
              

                                  )
                    .ToListAsync();

                initialBalanceQuery = initialBalanceQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom ||
                                (b.DocTypeId == 4 &&
                                 DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                 DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo))
                    .ToList();



                // إنشاء قائمة للرصيد الافتتاحي
                List<AccountBalanceSheetDto> initialBalance = new List<AccountBalanceSheetDto>();

                if (initialBalanceQuery.Any())
                {
                    initialBalance = initialBalanceQuery
                        .GroupBy(b => new { b.AccAccountId, b.FacilityId, b.FinYear })
                        .Select(g => new AccountBalanceSheetDto
                        {
                            DocTypeId = 0,
                            ReferenceNo = 0,
                            ReferenceCode = null,
                            DocTypeName = "رصيد افتتاحي",
                            DocTypeName2 = "Opening Balance",
                            JId = 0,
                            RowID = 0,
                            JDateGregorian = null,
                            Description = session.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                            Debit = 0m,
                            Credit = 0m,
                            Balance = g.Max(b => b.NatureAccount) > 0
                                ? g.Sum(b => b.Credit) - g.Sum(b => b.Debit)
                                : g.Sum(b => b.Debit) - g.Sum(b => b.Credit),
                            CostCenterName = null,
                            JCode = null,
                            NatureAccount = g.Max(b => b.NatureAccount) ?? 1,
                            CostsCenter = 0,
                            ChequNo = null
                        })
                        .ToList();
                }
                else
                {
                    initialBalance.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد افتتاحي",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = 0,
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        CostCenterName = null,
                        JCode = null,
                        NatureAccount = 1,
                        CostsCenter = 0,
                        ChequNo = null
                    });
                }

                // إضافة الرصيد الافتتاحي إلى النتيجة كأول عنصر
                List<AccountBalanceSheetDto> resultList = new List<AccountBalanceSheetDto>(initialBalance);

                // تحميل البيانات للأرصدة الرئيسية
                var mainRecordsQuery = await context.AccBalanceSheets
      .AsNoTracking()
      .Where(b => b.DocTypeId != 4 &&
                  b.DocTypeId != 27 &&
                  b.FlagDelete == false &&
                  b.FacilityId == obj.facilityId &&
                  b.AccAccountId == obj.accountId &&
                  b.FinYear == obj.FinYear
                  &&(obj.branchId == 0 || b.MbranchId == obj.branchId)
                  &&(obj.CurrencyId == 0 || b.CurrencyId == obj.CurrencyId))
      .ToListAsync();

                mainRecordsQuery = mainRecordsQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                    .ToList();


                var mainRecords = mainRecordsQuery
                    .Select(b => new AccountBalanceSheetDto
                    {
                        DocTypeId = b.DocTypeId,
                        ReferenceNo = b.ReferenceNo,
                        ReferenceCode = b.ReferenceCode,
                        DocTypeName = b.DocTypeName,
                        DocTypeName2 = b.DocTypeName2,
                        JId = b.JId,
                        RowID = 0,
                        JDateGregorian = b.JDateGregorian,
                        Description = b.Description,
                        Debit = b.Debit,
                        Credit = b.Credit,
                        Balance = 0m,
                        CostCenterName = b.CostCenterName,
                        JCode = b.JCode,
                        NatureAccount = b.NatureAccount,
                        CostsCenter = context.AccJournalDetailesCostcenter
                            .Where(d => d.JDetailesId == b.JDetailesId && d.FlagDelete == false)
                            .Sum(d => d.Debit - d.Credit) ?? 0,
                        ChequNo = b.ChequNo
                    })
                    .ToList();

                // دمج الأرصدة الافتتاحية والرئيسية وحساب الرصيد التراكمي
                var allRecords = resultList.Union(mainRecords)
                    .OrderBy(r => r.JDateGregorian)
                    .ThenBy(r => r.RowID)
                    .ToList();

                foreach (var record in allRecords)
                {
                    if (rowId == 0 && obj.dateFrom != null && obj.dateTo != null)
                        balance = record.Balance ?? 0m;

                    balance += record.NatureAccount >= 0
                        ? (record.Credit ?? 0m) - (record.Debit ?? 0m)
                        : (record.Debit ?? 0m) - (record.Credit ?? 0m);

                    record.Balance = balance;
                    record.RowID = ++rowId;
                }

                return allRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<AccountBalanceSheetDto>();
            }
        }

        public async Task<IEnumerable<AccountBalanceSheetDto>> GetAccountStatementTransactionDateAllYear(AccountTransactionDateFilterDto obj)
        {
            try
            {
                decimal balance = 0;
                int rowId = -1;

                DateTime parsedDateFrom = DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // استعلام الرصيد الافتتاحي (Doc_Type_ID = 27)
                var initialBalanceQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId &&
                        b.AccAccountId == obj.accountId &&
                        b.DocTypeId == 27 

                        )
                    .ToListAsync();

                // تطبيق فلتر التاريخ في الذاكرة
                var initialBalance = initialBalanceQuery
                    //.Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom)
                    .GroupBy(b => new { b.AccAccountId, b.FacilityId })
                    .Select(g => new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد إفتتاحي أول المدة",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = -1,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد إفتتاحي أول المدة" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = g.Max(b => b.NatureAccount) > 0
                            ? g.Sum(b => b.Credit) - g.Sum(b => b.Debit)
                            : g.Sum(b => b.Debit) - g.Sum(b => b.Credit),
                        NatureAccount = g.Max(b => b.NatureAccount) ?? 1
                    })
                    .FirstOrDefault();

                List<AccountBalanceSheetDto> resultList = new List<AccountBalanceSheetDto>();
                if (initialBalance != null)
                {
                    resultList.Add(initialBalance);
                }
                else
                {
                    // إضافة سطر فارغ إذا لم توجد بيانات لرصيد المدور
                    resultList.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد إفتتاحي أول المدة",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = 0,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد إفتتاحي أول المدة" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        NatureAccount = 1
                    });
                }

                // استعلام الرصيد المدور (التواريخ قبل التاريخ المحدد Doc_Type_ID not in (4, 27))
                var carriedBalanceQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId &&
                        b.AccAccountId == obj.accountId &&
                        b.DocTypeId != 4 &&
                        b.DocTypeId != 27 )
                    .ToListAsync();

                var carriedBalance = carriedBalanceQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom)
                    .GroupBy(b => new { b.AccAccountId, b.FacilityId })
                    .Select(g => new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد مدور",
                        DocTypeName2 = "Carried Balance",
                        JId = 0,
                        RowID = 0,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد مدور" : "Carried Balance",
                        Debit = g.Sum(b => b.Debit),
                        Credit = g.Sum(b => b.Credit),
                        Balance = g.Max(b => b.NatureAccount) > 0
                            ? g.Sum(b => b.Credit) - g.Sum(b => b.Debit)
                            : g.Sum(b => b.Debit) - g.Sum(b => b.Credit),
                        NatureAccount = g.Max(b => b.NatureAccount) ?? 1
                    })
                    .FirstOrDefault();

                if (carriedBalance != null)
                {
                    resultList.Add(carriedBalance);
                }
                else
                {
                    // إضافة سطر فارغ إذا لم توجد بيانات لرصيد المدور
                    resultList.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد مدور",
                        DocTypeName2 = "Carried Balance",
                        JId = 0,
                        RowID = 0,
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد مدور" : "Carried Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        NatureAccount = 1
                    });
                }

                // تحميل الأرصدة الرئيسية مع التحقق من التواريخ المطلوبة
                var mainRecordsQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b => b.DocTypeId != 4 &&
                                b.DocTypeId != 27 &&
                                b.FlagDelete == false &&
                                b.FacilityId == obj.facilityId &&
                                b.AccAccountId == obj.accountId
                                &&(obj.branchId == 0 || b.MbranchId == obj.branchId)
                                &&(obj.CurrencyId == 0 || b.CurrencyId == obj.CurrencyId))
                    .ToListAsync();

                var mainRecords = mainRecordsQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                    .Select(b => new AccountBalanceSheetDto
                    {
                        DocTypeId = b.DocTypeId,
                        ReferenceNo = b.ReferenceNo,
                        ReferenceCode = b.ReferenceCode ?? b.ReferenceNo.ToString(),
                        DocTypeName = b.DocTypeName,
                        DocTypeName2 = b.DocTypeName2,
                        JId = b.JId,
                        RowID = 0,
                        JCode = b.JCode,
                        JDateGregorian = b.JDateGregorian,
                        Description = b.Description,
                        Debit = b.Debit,
                        Credit = b.Credit,
                        Balance = 0m,
                        NatureAccount = b.NatureAccount,
                        CostsCenter = context.AccJournalDetailesCostcenter
                            .Where(d => d.JDetailesId == b.JDetailesId && d.FlagDelete == false)
                            .Sum(d => d.Debit - d.Credit) ?? 0,
                        ChequNo = b.ChequNo
                    })
                    .ToList();

                // دمج الأرصدة وحساب الرصيد التراكمي
                var allRecords = resultList.Union(mainRecords)
                    .OrderBy(r => r.JDateGregorian)
                    .ThenBy(r => r.RowID)
                    .ToList();

                foreach (var record in allRecords)
                {
                    if (rowId == -1 && obj.dateFrom != null && obj.dateTo != null)
                        balance = record.Balance ?? 0m;

                    balance += record.NatureAccount >= 0
                        ? (record.Credit ?? 0m) - (record.Debit ?? 0m)
                        : (record.Debit ?? 0m) - (record.Credit ?? 0m);

                    record.Balance = balance;
                    record.RowID = ++rowId;
                }

                return allRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<AccountBalanceSheetDto>();
            }
        }



        #endregion

        #region =====================================  كشف حساب بالعملة الأجنبية

        public async Task<IEnumerable<AccountBalanceSheetDto>> GetCurrencytransactionsCurrentYearBalance(CurrencytransactionsFilterDto obj)
        {
            try
            {
                decimal balance = 0;
                int rowId = 0;

                DateTime parsedDateFrom = DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // استعلام الأرصدة الافتتاحية
                var initialBalanceQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                                b.FlagDelete == false &&
                                b.FacilityId == obj.facilityId &&
                                b.FinYear == obj.FinYear
                              && (obj.ReferenceTypeId == 0 || obj.ReferenceTypeId == 1 ? b.AccAccountId == obj.accountId : b.ReferenceDNo == obj.ReferenceDNo && b.ParentReferenceTypeId == obj.ParentReferenceTypeId) &&
                  (obj.branchId == 0 || b.MbranchId == obj.branchId)
                  &&
                  (obj.CurrencyId == 0 || b.CurrencyId == obj.CurrencyId)

                  )
                    .ToListAsync();

                initialBalanceQuery = initialBalanceQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom ||
                                ((b.DocTypeId == 4) &&
                                 DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                 DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo))
                    .ToList();



                // إنشاء قائمة للرصيد الافتتاحي
                List<AccountBalanceSheetDto> initialBalance = new List<AccountBalanceSheetDto>();

                if (initialBalanceQuery.Any())
                {
                    initialBalance = initialBalanceQuery
                        .GroupBy(b => new { b.AccAccountId, b.FacilityId, b.FinYear })
                        .Select(g => new AccountBalanceSheetDto
                        {
                            DocTypeId = 0,
                            ReferenceNo = 0,
                            ReferenceCode = null,
                            DocTypeName = "رصيد افتتاحي",
                            DocTypeName2 = "Opening Balance",
                            JId = 0,
                            RowID = 0,
                            JDateGregorian = null,
                            Description = session.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                            Debit = 0m,
                            Credit = 0m,
                            Balance = g.Max(b => b.NatureAccount) > 0
                                ? g.Sum(b => b.Credit1) - g.Sum(b => b.Debit1)
                                : g.Sum(b => b.Debit1) - g.Sum(b => b.Credit1),
                            CostCenterName = null,
                            JCode = null,
                            NatureAccount = g.Max(b => b.NatureAccount) ?? 1,
                            CostsCenter = 0,
                            ChequNo = null
                        })
                        .ToList();
                }
                else
                {
                    initialBalance.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد افتتاحي",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = 0,
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد افتتاحي" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        CostCenterName = null,
                        JCode = null,
                        NatureAccount = 1,
                        CostsCenter = 0,
                        ChequNo = null
                    });
                }

                // إضافة الرصيد الافتتاحي إلى النتيجة كأول عنصر
                List<AccountBalanceSheetDto> resultList = new List<AccountBalanceSheetDto>(initialBalance);

                // تحميل البيانات للأرصدة الرئيسية
                var mainRecordsQuery = await context.AccBalanceSheets
      .AsNoTracking()
      .Where(b => b.DocTypeId != 4 &&
                  b.DocTypeId != 27 &&
                  b.FlagDelete == false &&
                  b.FacilityId == obj.facilityId &&
                  b.FinYear == obj.FinYear 
                  && (obj.ReferenceTypeId == 0 || obj.ReferenceTypeId == 1 ? b.AccAccountId == obj.accountId : b.ReferenceDNo == obj.ReferenceDNo && b.ParentReferenceTypeId == obj.ParentReferenceTypeId) &&
                  
                  (obj.branchId == 0 || b.MbranchId == obj.branchId)
                  &&
                  (obj.CurrencyId == 0 || b.CurrencyId == obj.CurrencyId)
                  )
      .ToListAsync();

                mainRecordsQuery = mainRecordsQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                    .ToList();


                var mainRecords = mainRecordsQuery
                    .Select(b => new AccountBalanceSheetDto
                    {
                        DocTypeId = b.DocTypeId,
                        ReferenceNo = b.ReferenceNo,
                        ReferenceCode = b.ReferenceCode,
                        DocTypeName = b.DocTypeName,
                        DocTypeName2 = b.DocTypeName2,
                        JId = b.JId,
                        RowID = 0,
                        JDateGregorian = b.JDateGregorian,
                        Description = b.Description,
                        Debit = b.Debit1,
                        Credit = b.Credit1,
                        Balance = 0m,
                        CostCenterName = b.CostCenterName,
                        JCode = b.JCode,
                        NatureAccount = b.NatureAccount,
                        CostsCenter = 0,
                        ChequNo = b.ChequNo
                    })
                    .ToList();

                // دمج الأرصدة الافتتاحية والرئيسية وحساب الرصيد التراكمي
                var allRecords = resultList.Union(mainRecords)
                    .OrderBy(r => r.JDateGregorian)
                    .ThenBy(r => r.RowID)
                    .ToList();

                foreach (var record in allRecords)
                {
                    if (rowId == 0 && obj.dateFrom != null && obj.dateTo != null)
                        balance = record.Balance ?? 0m;

                    balance += record.NatureAccount >= 0
                        ? (record.Credit ?? 0m) - (record.Debit ?? 0m)
                        : (record.Debit ?? 0m) - (record.Credit ?? 0m);

                    record.Balance = balance;
                    record.RowID = ++rowId;
                }

                return allRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<AccountBalanceSheetDto>();
            }
        }

        public async Task<IEnumerable<AccountBalanceSheetDto>> GetCurrencytransactionsAllYearBalance(CurrencytransactionsFilterDto obj)
        {
            try
            {
                decimal balance = 0;
                int rowId = -1;

                DateTime parsedDateFrom = DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // استعلام الرصيد الافتتاحي (Doc_Type_ID = 27)
                var initialBalanceQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId 
                        && (obj.ReferenceTypeId == 0 || obj.ReferenceTypeId == 1 ? b.AccAccountId == obj.accountId : b.ReferenceDNo == obj.ReferenceDNo && b.ParentReferenceTypeId== obj.ParentReferenceTypeId) &&
                        b.DocTypeId == 27 &&
                  (obj.branchId == 0 || b.MbranchId == obj.branchId)
                  &&
                  (obj.CurrencyId == 0 || b.CurrencyId == obj.CurrencyId)
                        )
                    .ToListAsync();

                // تطبيق فلتر التاريخ في الذاكرة
                var initialBalance = initialBalanceQuery
                    //.Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom)
                    .GroupBy(b => new { b.AccAccountId, b.FacilityId })
                    .Select(g => new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد إفتتاحي أول المدة",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = -1,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد إفتتاحي أول المدة" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = g.Max(b => b.NatureAccount) > 0
                            ? g.Sum(b => b.Credit1) - g.Sum(b => b.Debit1)
                            : g.Sum(b => b.Debit1) - g.Sum(b => b.Credit1),
                        NatureAccount = g.Max(b => b.NatureAccount) ?? 1
                    })
                    .FirstOrDefault();

                List<AccountBalanceSheetDto> resultList = new List<AccountBalanceSheetDto>();
                if (initialBalance != null)
                {
                    resultList.Add(initialBalance);
                }
                else
                {
                    // إضافة سطر فارغ إذا لم توجد بيانات لرصيد المدور
                    resultList.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد إفتتاحي أول المدة",
                        DocTypeName2 = "Opening Balance",
                        JId = 0,
                        RowID = 0,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد إفتتاحي أول المدة" : "Opening Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        NatureAccount = 1
                    });
                }

                // استعلام الرصيد المدور (التواريخ قبل التاريخ المحدد Doc_Type_ID not in (4, 27))
                var carriedBalanceQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId 
                          && (obj.ReferenceTypeId == 0 || obj.ReferenceTypeId == 1 ? b.AccAccountId == obj.accountId : b.ReferenceDNo == obj.ReferenceDNo && b.ParentReferenceTypeId == obj.ParentReferenceTypeId) &&
                        b.DocTypeId != 4 &&
                        b.DocTypeId != 27 &&
                  (obj.branchId == 0 || b.MbranchId == obj.branchId)
                  &&
                  (obj.CurrencyId == 0 || b.CurrencyId == obj.CurrencyId)

                  )
                    .ToListAsync();

                var carriedBalance = carriedBalanceQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom)
                    .GroupBy(b => new { b.AccAccountId, b.FacilityId })
                    .Select(g => new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد مدور",
                        DocTypeName2 = "Carried Balance",
                        JId = 0,
                        RowID = 0,
                        JCode = "",
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد مدور" : "Carried Balance",
                        Debit = g.Sum(b => b.Debit1),
                        Credit = g.Sum(b => b.Credit1),
                        Balance = g.Max(b => b.NatureAccount) > 0
                            ? g.Sum(b => b.Credit1) - g.Sum(b => b.Debit1)
                            : g.Sum(b => b.Debit1) - g.Sum(b => b.Credit1),
                        NatureAccount = g.Max(b => b.NatureAccount) ?? 1
                    })
                    .FirstOrDefault();

                if (carriedBalance != null)
                {
                    resultList.Add(carriedBalance);
                }
                else
                {
                    // إضافة سطر فارغ إذا لم توجد بيانات لرصيد المدور
                    resultList.Add(new AccountBalanceSheetDto
                    {
                        DocTypeId = 0,
                        ReferenceNo = 0,
                        ReferenceCode = null,
                        DocTypeName = "رصيد مدور",
                        DocTypeName2 = "Carried Balance",
                        JId = 0,
                        RowID = 0,
                        JDateGregorian = null,
                        Description = session.Language == 1 ? "رصيد مدور" : "Carried Balance",
                        Debit = 0m,
                        Credit = 0m,
                        Balance = 0m,
                        NatureAccount = 1
                    });
                }

                // تحميل الأرصدة الرئيسية مع التحقق من التواريخ المطلوبة
                var mainRecordsQuery = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b => b.DocTypeId != 4 &&
                                b.DocTypeId != 27 &&
                                b.FlagDelete == false &&
                                b.FacilityId == obj.facilityId 
                                 && (obj.ReferenceTypeId == 0 || obj.ReferenceTypeId == 1 ? b.AccAccountId == obj.accountId : b.ReferenceDNo == obj.ReferenceDNo && b.ParentReferenceTypeId == obj.ParentReferenceTypeId) &&
                  (obj.branchId == 0 || b.MbranchId == obj.branchId)
                  &&
                  (obj.CurrencyId == 0 || b.CurrencyId == obj.CurrencyId)
                  )
                    .ToListAsync();

                var mainRecords = mainRecordsQuery
                    .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                    .Select(b => new AccountBalanceSheetDto
                    {
                        DocTypeId = b.DocTypeId,
                        ReferenceNo = b.ReferenceNo,
                        ReferenceCode = b.ReferenceCode ?? b.ReferenceNo.ToString(),
                        DocTypeName = b.DocTypeName,
                        DocTypeName2 = b.DocTypeName2,
                        JId = b.JId,
                        RowID = 0,
                        JCode = b.JCode,
                        JDateGregorian = b.JDateGregorian,
                        Description = b.Description,
                        Debit = b.Debit1,
                        Credit = b.Credit1,
                        Balance = 0m,
                        NatureAccount = b.NatureAccount,
                        CostsCenter =  0,
                        ChequNo = b.ChequNo
                    })
                    .ToList();

                // دمج الأرصدة وحساب الرصيد التراكمي
                var allRecords = resultList.Union(mainRecords)
                    .OrderBy(r => r.JDateGregorian)
                    .ThenBy(r => r.RowID)
                    .ToList();

                foreach (var record in allRecords)
                {
                    if (rowId == -1 && obj.dateFrom != null && obj.dateTo != null)
                        balance = record.Balance ?? 0m;

                    balance += record.NatureAccount >= 0
                        ? (record.Credit ?? 0m) - (record.Debit ?? 0m)
                        : (record.Debit ?? 0m) - (record.Credit ?? 0m);

                    record.Balance = balance;
                    record.RowID = ++rowId;
                }

                return allRecords;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<AccountBalanceSheetDto>();
            }
        }
        #endregion

        #region =====================================  كشف حساب مجموعة مركز تكلفة

        public async Task<IEnumerable<CostcentertransactionsGroupDto>> GetCostcentertransactionsGroup(CostcentertransactionsGroupFilterDto obj)
        {
            try
            {
                // تحويل التواريخ
                DateTime? parsedDateFrom = string.IsNullOrEmpty(obj.dateFrom)
                    ? (DateTime?)null
                    : DateTime.ParseExact(obj.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime? parsedDateTo = string.IsNullOrEmpty(obj.dateTo)
                    ? (DateTime?)null
                    : DateTime.ParseExact(obj.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // الحصول على قائمة معرفات مراكز التكلفة
                var costCenterIds = (await GetCostCenterIds(obj.ccId ?? 0))
                    .Split(',') // افتراض أن النتيجة سلسلة مفصولة بفواصل
                    .Select(id => id.Trim()) // تنظيف القيم
                    .ToList();

                // تحميل البيانات
                var balanceSheetData = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.facilityId &&
                        b.FinYear == obj.FinYear &&
                        costCenterIds.Contains(b.CcId.ToString())) // تأكد من تطابق النوع
                    .ToListAsync(); // تحميل البيانات إلى الذاكرة

                // تطبيق فلترة التواريخ في الذاكرة
                var filteredData = balanceSheetData
                    .Where(b =>
                        (!parsedDateFrom.HasValue ||
                         DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom) &&
                        (!parsedDateTo.HasValue ||
                         DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo))
                    .ToList();

                // استعلام الإجمالي
                var totalRecord = new CostcentertransactionsGroupDto
                {
                    CostCenterName = "الإجمالي",
                    CostCenterName2 = "Total",
                    CostCenterCode = null,
                    Debit = filteredData.Sum(b => b.Debit ?? 0),
                    Credit = filteredData.Sum(b => b.Credit ?? 0),
                    Balance = filteredData.Sum(b => (b.Credit ?? 0) - (b.Debit ?? 0)),
                    RowID = -1
                };

                // استعلام الأرصدة حسب مركز التكلفة
                var groupedData = filteredData
                    .GroupBy(b => new { b.CostCenterName, b.CostCenterCode, b.NatureAccount })
                    .Select(g => new CostcentertransactionsGroupDto
                    {
                        CostCenterName = g.Key.CostCenterName,
                        CostCenterCode = g.Key.CostCenterCode,
                        Debit = g.Sum(b => b.Debit ?? 0),
                        Credit = g.Sum(b => b.Credit ?? 0),
                        Balance = (g.Sum(b => b.Credit ?? 0) - g.Sum(b => b.Debit ?? 0)) * g.Key.NatureAccount,
                        RowID = 0
                    })
                    .ToList();

                // دمج النتائج
                var result = new List<CostcentertransactionsGroupDto> { totalRecord };
                result.AddRange(groupedData);

                // ترتيب النتائج
                return result
                    .OrderBy(r => r.CostCenterCode ?? "") // ترتيب مع التعامل مع القيم الفارغة
                    .ThenBy(r => r.RowID)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<CostcentertransactionsGroupDto>();
            }
        }

        

        public async Task<string> GetCostCenterIds(long parentId)
        {
            // Asynchronous method to query the database for descendants
            async Task<IEnumerable<long>> RecursiveQueryAsync(long id)
            {
                // Retrieve children asynchronously
                var children = await context.AccCostCenter
                    .Where(cc => cc.IsDeleted == false && cc.CcParentId == id)
                    .Select(cc => cc.CcId)
                    .ToListAsync();

                // Collect results recursively
                var result = new List<long>();
                foreach (var childId in children)
                {
                    result.Add(childId);
                    result.AddRange(await RecursiveQueryAsync(childId));
                }
                return result;
            }

            // Execute the query
            var ids = await RecursiveQueryAsync(parentId);

            // Join results into a comma-separated string
            return string.Join(",", ids);
        }


        #endregion

        #region =====================================   كشف حساب العملاء من رقم الى رقم 

        public async Task<IEnumerable<CustomerTransactionDto>> GetCustomerTransactionsFromTo(CustomerTransactionFilterDto filter)
        {
            try
            {
                filter.BranchId ??= 0;
                filter.GroupId ??= 0;
                filter.EmpId ??= 0;
                // تحويل التواريخ
                DateTime parsedDateFrom = DateTime.ParseExact(filter.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(filter.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                
                // 1. تحميل البيانات من ACC_Balance_Sheet إلى قائمة في الذاكرة (مشابه لإنشاء جدول مؤقت)
                var balanceSheetData = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == filter.FacilityId &&
                        b.FinYear == filter.FinYear &&
                        (string.IsNullOrEmpty(filter.StartCode) || string.Compare(b.AccAccountCode, filter.StartCode) >= 0) &&
                        (string.IsNullOrEmpty(filter.EndCode) || string.Compare(b.AccAccountCode, filter.EndCode) <= 0))
                    .ToListAsync();

                // 2. جلب البيانات من Sys_Customer_VW بناءً على المعرفات المرجعية
                var customerData = await context.SysCustomerVws
                    .AsNoTracking()
                    .Where(c =>
                        c.IsDeleted == false &&
                        c.FacilityId == filter.FacilityId &&
                        (filter.BranchId == 0 || c.BranchId == filter.BranchId) &&
                        (filter.EmpId == 0 || c.EmpId == filter.EmpId) &&
                        (filter.GroupId == 0 || c.GroupId == filter.GroupId) &&
                        c.CusTypeId == 2 &&
                        balanceSheetData.Select(b => b.ReferenceDNo).Contains(c.Id))
                    .Select(c => new CustomerTransactionDataDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Name2 = c.Name2,
                        Code = c.Code,
                        CollectorName = c.CollectorName
                    })
                    .ToListAsync();

                // 3. حساب المعاملات (AMOUNTPrev, Debit, Credit, AMOUNTNext) لكل عميل
                var customerTransactions = customerData.Select(customer =>
                {
                    var transactions = balanceSheetData.Where(b => b.ReferenceDNo == customer.Id);

                    var amountPrev = transactions.Where(b =>
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom ||
                        (b.DocTypeId is 4 or 27 &&
                         DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                         DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo))
                        .Sum(b => (b.Debit ?? 0) - (b.Credit ?? 0));

                    var debit = transactions.Where(b =>
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo &&
                        b.DocTypeId != 4 && b.DocTypeId != 27)
                        .Sum(b => b.Debit ?? 0);

                    var credit = transactions.Where(b =>
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo &&
                        b.DocTypeId != 4 && b.DocTypeId != 27)
                        .Sum(b => b.Credit ?? 0);

                    var amountNext = transactions.Where(b =>
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                        .Sum(b => (b.Debit ?? 0) - (b.Credit ?? 0));

                    return new CustomerTransactionDto
                    {
                        Code = customer.Code,
                        Name = customer.Name,
                        Name2 = customer.Name2,
                        CollectorName1 = customer.CollectorName,
                        AmountPrev = amountPrev,
                        Debit = debit,
                        Credit = credit,
                        AmountNext = amountNext
                    };
                });

                // 4. تصفية النتائج إذا كان `NoZero` موجود
                if (filter.NoZero)
                {
                    customerTransactions = customerTransactions.Where(t => t.AmountNext != 0);
                }

                return customerTransactions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<CustomerTransactionDto>();
            }
        }
        //public async Task<(IEnumerable<CustomerTransactionDto> Transactions, CustomerTransactionSummaryDto Summary)> GetCustomerTransactionsFromTo(CustomerTransactionFilterDto filter)
        //{
        //    try
        //    {
        //        // تحويل التواريخ
        //        DateTime parsedDateFrom = DateTime.ParseExact(filter.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        //        DateTime parsedDateTo = DateTime.ParseExact(filter.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);

        //        // 1. تحميل البيانات من ACC_Balance_Sheet
        //        var balanceSheetData = await context.AccBalanceSheets
        //            .AsNoTracking()
        //            .Where(b =>
        //                b.FlagDelete == false &&
        //                b.FacilityId == filter.FacilityId &&
        //                b.FinYear == filter.FinYear &&
        //                (string.IsNullOrEmpty(filter.StartCode) || string.Compare(b.AccAccountCode, filter.StartCode) >= 0) &&
        //                (string.IsNullOrEmpty(filter.EndCode) || string.Compare(b.AccAccountCode, filter.EndCode) <= 0))
        //            .ToListAsync();

        //        // 2. جلب البيانات من Sys_Customer_VW
        //        var customerData = await context.SysCustomerVws
        //            .AsNoTracking()
        //            .Where(c =>
        //                c.IsDeleted == false &&
        //                c.FacilityId == filter.FacilityId &&
        //                (filter.BranchId == 0 || c.BranchId == filter.BranchId) &&
        //                (filter.EmpId == null || c.EmpId == filter.EmpId) &&
        //                (filter.GroupId == 0 || c.GroupId == filter.GroupId) &&
        //                c.CusTypeId == 2 &&
        //                balanceSheetData.Select(b => b.ReferenceDNo).Contains(c.Id))
        //            .Select(c => new CustomerTransactionDataDto
        //            {
        //                Id = c.Id,
        //                Name = c.Name,
        //                Name2 = c.Name2,
        //                Code = c.Code,
        //                CollectorName = c.CollectorName
        //            })
        //            .ToListAsync();

        //        // 3. حساب المعاملات والمجاميع
        //        decimal sum1 = 0, sum2 = 0, sum3 = 0, sum4 = 0, sum5 = 0, sum6 = 0;

        //        var customerTransactions = customerData.Select(customer =>
        //        {
        //            var transactions = balanceSheetData.Where(b => b.ReferenceDNo == customer.Id);

        //            var amountPrev = transactions.Where(b =>
        //                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom ||
        //                (b.DocTypeId is 4 or 27 &&
        //                 DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
        //                 DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo))
        //                .Sum(b => (b.Debit ?? 0) - (b.Credit ?? 0));

        //            var debit = transactions.Where(b =>
        //                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
        //                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo &&
        //                b.DocTypeId != 4 && b.DocTypeId != 27)
        //                .Sum(b => b.Debit ?? 0);

        //            var credit = transactions.Where(b =>
        //                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
        //                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo &&
        //                b.DocTypeId != 4 && b.DocTypeId != 27)
        //                .Sum(b => b.Credit ?? 0);

        //            var amountNext = transactions.Where(b =>
        //                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
        //                .Sum(b => (b.Debit ?? 0) - (b.Credit ?? 0));

        //            // حساب المجاميع
        //            sum1 += amountPrev > 0 ? amountPrev : 0;
        //            sum2 += amountPrev < 0 ? amountPrev * -1 : 0;
        //            sum3 += debit;
        //            sum4 += credit;
        //            sum5 += amountNext > 0 ? amountNext : 0;
        //            sum6 += amountNext < 0 ? amountNext * -1 : 0;

        //            return new CustomerTransactionDto
        //            {
        //                Code = customer.Code,
        //                Name = customer.Name,
        //                Name2 = customer.Name2,
        //                CollectorName1 = customer.CollectorName,
        //                AmountPrev = amountPrev,
        //                Debit = debit,
        //                Credit = credit,
        //                AmountNext = amountNext
        //            };
        //        });

        //        // 4. تصفية النتائج إذا كان `NoZero` موجود
        //        if (filter.NoZero)
        //        {
        //            customerTransactions = customerTransactions.Where(t => t.AmountNext != 0);
        //        }

        //        // إنشاء كائن المجاميع
        //        var summary = new CustomerTransactionSummaryDto
        //        {
        //            Sum1 = sum1,
        //            Sum2 = sum2,
        //            Sum3 = sum3,
        //            Sum4 = sum4,
        //            Sum5 = sum5,
        //            Sum6 = sum6
        //        };

        //        return (customerTransactions, summary);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //        return (new List<CustomerTransactionDto>(), new CustomerTransactionSummaryDto());
        //    }
        //}
        //public class CustomerTransactionSummaryDto
        //{
        //    public decimal Sum1 { get; set; }
        //    public decimal Sum2 { get; set; }
        //    public decimal Sum3 { get; set; }
        //    public decimal Sum4 { get; set; }
        //    public decimal Sum5 { get; set; }
        //    public decimal Sum6 { get; set; }
        //}

        #endregion

        #region =====================================  كشف حساب مركز تكلفة من الى 

        
        public async Task<IEnumerable<CostcenterTransactionsFromToDto>> GetCostcenterTransactionsFromTo(CostcenterTransactionsFromToFilterDto filter)
        {
            try
            {
                // تحويل التواريخ
                DateTime parsedDateFrom = DateTime.ParseExact(filter.dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(filter.dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // استعلام الرصيد الإجمالي
                var Query = await context.AccBalanceSheetCostCenterVw
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == filter.facilityId &&
                        b.FinYear == filter.FinYear &&
                        (string.IsNullOrEmpty(filter.AccountCode) ||
                         string.IsNullOrEmpty(filter.AccountCode2) ||
                         (b.AccAccountCode != null &&
                          string.Compare(b.AccAccountCode, filter.AccountCode) >= 0 &&
                          string.Compare(b.AccAccountCode, filter.AccountCode2) <= 0))
                          && (string.IsNullOrEmpty(filter.AccGrouplist) || filter.AccGrouplist.Contains(b.AccGroupId.ToString())))
                    .ToListAsync(); 




                var baseQuery = Query
                    .Where(b =>
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                    .ToList();



          



        // استعلام لكل مركز تكلفة
        var results = new List<CostcenterTransactionsFromToDto>();

                var costCenter1 = baseQuery
      .Where(b => b.CcId != 0 &&
          !string.IsNullOrEmpty(b.CostCenterCode) &&
          string.Compare(b.CostCenterCode, filter.CostCenterCodeFrom) >= 0 &&
          string.Compare(b.CostCenterCode, filter.CostCenterCodeTo) <= 0)
      .GroupBy(b => new { b.CostCenterName, b.CostCenterCode })
      .Select(g => new CostcenterTransactionsFromToDto
      {
          CostCenterName = g.Key.CostCenterName,
          CostCenterCode = g.Key.CostCenterCode,
          Debit = g.Sum(b => b.Debit ?? 0),
          Credit = g.Sum(b => b.Credit ?? 0),
          Balance = g.Sum(b => (b.Credit ?? 0) - (b.Debit ?? 0))
      })
      .ToList();
                results.AddRange(costCenter1);

                // CostCenter2
                var costCenter2 = baseQuery
                    .Where(b => b.Cc2Id != 0 &&
                        !string.IsNullOrEmpty(b.CostCenter2Code) &&
                        string.Compare(b.CostCenter2Code, filter.CostCenterCodeFrom) >= 0 &&
                        string.Compare(b.CostCenter2Code, filter.CostCenterCodeTo) <= 0)
                    .GroupBy(b => new { b.CostCenter2Name, b.CostCenter2Code })
                    .Select(g => new CostcenterTransactionsFromToDto
                    {
                        CostCenterName = g.Key.CostCenter2Name,
                        CostCenterCode = g.Key.CostCenter2Code,
                        Debit = g.Sum(b => b.Debit ?? 0),
                        Credit = g.Sum(b => b.Credit ?? 0),
                        Balance = g.Sum(b => (b.Credit ?? 0) - (b.Debit ?? 0))
                    })
                    .ToList();
                results.AddRange(costCenter2);

                // CostCenter3
                var costCenter3 = baseQuery
                    .Where(b => b.Cc3Id != 0 &&
                        !string.IsNullOrEmpty(b.CostCenter3Code) &&
                        string.Compare(b.CostCenter3Code, filter.CostCenterCodeFrom) >= 0 &&
                        string.Compare(b.CostCenter3Code, filter.CostCenterCodeTo) <= 0)
                    .GroupBy(b => new { b.CostCenter3Name, b.CostCenter3Code })
                    .Select(g => new CostcenterTransactionsFromToDto
                    {
                        CostCenterName = g.Key.CostCenter3Name,
                        CostCenterCode = g.Key.CostCenter3Code,
                        Debit = g.Sum(b => b.Debit ?? 0),
                        Credit = g.Sum(b => b.Credit ?? 0),
                        Balance = g.Sum(b => (b.Credit ?? 0) - (b.Debit ?? 0))
                    })
                    .ToList();
                results.AddRange(costCenter3);

                // CostCenter4
                var costCenter4 = baseQuery
                    .Where(b => b.Cc4Id != 0 &&
                        !string.IsNullOrEmpty(b.CostCenter4Code) &&
                        string.Compare(b.CostCenter4Code, filter.CostCenterCodeFrom) >= 0 &&
                        string.Compare(b.CostCenter4Code, filter.CostCenterCodeTo) <= 0)
                    .GroupBy(b => new { b.CostCenter4Name, b.CostCenter4Code })
                    .Select(g => new CostcenterTransactionsFromToDto
                    {
                        CostCenterName = g.Key.CostCenter4Name,
                        CostCenterCode = g.Key.CostCenter4Code,
                        Debit = g.Sum(b => b.Debit ?? 0),
                        Credit = g.Sum(b => b.Credit ?? 0),
                        Balance = g.Sum(b => (b.Credit ?? 0) - (b.Debit ?? 0))
                    })
                    .ToList();
                results.AddRange(costCenter4);

                // CostCenter5
                var costCenter5 = baseQuery
                    .Where(b => b.Cc5Id != 0 &&
                        !string.IsNullOrEmpty(b.CostCenter5Code) &&
                        string.Compare(b.CostCenter5Code, filter.CostCenterCodeFrom) >= 0 &&
                        string.Compare(b.CostCenter5Code, filter.CostCenterCodeTo) <= 0)
                    .GroupBy(b => new { b.CostCenter5Name, b.CostCenter5Code })
                    .Select(g => new CostcenterTransactionsFromToDto
                    {
                        CostCenterName = g.Key.CostCenter5Name,
                        CostCenterCode = g.Key.CostCenter5Code,
                        Debit = g.Sum(b => b.Debit ?? 0),
                        Credit = g.Sum(b => b.Credit ?? 0),
                        Balance = g.Sum(b => (b.Credit ?? 0) - (b.Debit ?? 0))
                    })
                    .ToList();
                results.AddRange(costCenter5);

                // الإجمالي
                var totalResult = baseQuery.Where(b=>
                (
                    (string.Compare(b.CostCenterCode, filter.CostCenterCodeFrom) >= 0 &&
                     string.Compare(b.CostCenterCode, filter.CostCenterCodeTo) <= 0) ||
                    (string.Compare(b.CostCenter2Code, filter.CostCenterCodeFrom) >= 0 &&
                     string.Compare(b.CostCenter2Code, filter.CostCenterCodeTo) <= 0) ||
                    (string.Compare(b.CostCenter3Code, filter.CostCenterCodeFrom) >= 0 &&
                     string.Compare(b.CostCenter3Code, filter.CostCenterCodeTo) <= 0) ||
                    (string.Compare(b.CostCenter4Code, filter.CostCenterCodeFrom) >= 0 &&
                     string.Compare(b.CostCenter4Code, filter.CostCenterCodeTo) <= 0) ||
                    (string.Compare(b.CostCenter5Code, filter.CostCenterCodeFrom) >= 0 &&
                     string.Compare(b.CostCenter5Code, filter.CostCenterCodeTo) <= 0)
                )
                )
                    .GroupBy(_ => 1)
                    .Select(g => new CostcenterTransactionsFromToDto
                    {
                        CostCenterName = "الإجمالي",
                        CostCenterCode = null,
                        Debit = g.Sum(b => b.Debit ?? 0),
                        Credit = g.Sum(b => b.Credit ?? 0),
                        Balance = g.Sum(b => (b.Credit ?? 0) - (b.Debit ?? 0))
                    })
                    .ToList();
                results.AddRange(totalResult);

                // ترتيب النتائج

                return results
           .GroupBy(b => new { b.CostCenterCode, b.CostCenterName })
           .OrderBy(g => g.Key.CostCenterCode)  // ترتيب حسب CostCenterCode
           .Select(g => new CostcenterTransactionsFromToDto
           {
               CostCenterCode = g.Key.CostCenterCode,
               CostCenterName = g.Key.CostCenterName,
               Debit = g.Sum(b => b.Debit ?? 0m),  
               Credit = g.Sum(b => b.Credit ?? 0m),  
               Balance = g.Sum(b => (b.Credit ?? 0m) - (b.Debit ?? 0m)) 
           })
           .ToList();



            }
            catch (Exception ex)
            {
                // التعامل مع الأخطاء
                Console.WriteLine($"Error: {ex.Message}");
                return Enumerable.Empty<CostcenterTransactionsFromToDto>();
            }
        }


        #endregion

        #region ========================================== ميزان المراجعة



        



        public async Task<IEnumerable<TrialBalanceSheetDtoResult>> GetTrialBalanceSheet(TrialBalanceSheetDto obj)
        {
            try
            {
                // تحويل التواريخ إلى DateTime
                var periodStart = DateTime.ParseExact(obj.periodStartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                var periodEnd = DateTime.ParseExact(obj.periodEndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // استعلام الحسابات
                var accountsQuery = from acc in context.AccAccounts
                                    where acc.FacilityId == obj.facilityId
                                          && acc.IsDeleted == false
                                          && (acc.AccountLevel == (obj.showAllLevel == 0 ? obj.accountLevel : acc.AccountLevel))
                                          && (acc.AccountLevel <= (obj.showAllLevel == 1 ? obj.accountLevel : acc.AccountLevel))
                                          && (string.IsNullOrEmpty(obj.accountFrom) || acc.AccAccountCode.CompareTo(obj.accountFrom) >= 0)
                                          && (string.IsNullOrEmpty(obj.accountTo) || acc.AccAccountCode.CompareTo(obj.accountTo) <= 0)
                                    join accGroup in context.AccGroup on acc.AccGroupId equals accGroup.AccGroupId
                                    select new TrialBalanceSheetDtoResult
                                    {
                                        IsSub = acc.IsSub,
                                        AccAccountId = acc.AccAccountId,
                                        AccAccountName = acc.AccAccountName,
                                        AccAccountName2 = acc.AccAccountName2,
                                        AccAccountCode = acc.AccAccountCode,
                                        AccountLevel = acc.AccountLevel,
                                        NatureAccount = accGroup.NatureAccount,
                                        AccGroupId = accGroup.AccGroupId,
                                        Transactions = null // سيتم معالجتها لاحقًا
                                    };

                // جلب النتائج
                var accountResults = await accountsQuery.ToListAsync();

                // معالجة المعاملات على جانب العميل
                foreach (var account in accountResults)
                {
                    var transactions = context.AccBalanceSheets
                        .Where(balance =>
                            balance.AccAccountCode.StartsWith(account.AccAccountCode)
                            && balance.FlagDelete == false
                            && balance.FacilityId == obj.facilityId
                            && balance.FinYear == obj.finYear
                            && (string.IsNullOrEmpty(obj.ccFrom) || string.Compare(balance.CostCenterCode, obj.ccFrom) >= 0)
                            && (string.IsNullOrEmpty(obj.ccTo) || string.Compare(balance.CostCenterCode, obj.ccTo) <= 0))
                        .AsEnumerable() // التحويل إلى جانب العميل
                        .GroupBy(balance => 1)
                        .Select(g => new TrialBalanceTransactionDto
                        {
                            AMOUNTPrev = g.Sum(b =>
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < periodStart ||
                                (b.DocTypeId == 4 || b.DocTypeId == 27) &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= periodStart &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= periodEnd
                                    ? b.Credit - b.Debit
                                    : 0),
                            Debit = g.Sum(b =>
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= periodStart &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= periodEnd &&
                                b.DocTypeId != 4 && b.DocTypeId != 27
                                    ? b.Debit
                                    : 0),
                            Credit = g.Sum(b =>
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= periodStart &&
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= periodEnd &&
                                b.DocTypeId != 4 && b.DocTypeId != 27
                                    ? b.Credit
                                    : 0),
                            AMOUNTNext = g.Sum(b =>
                                DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= periodEnd
                                    ? b.Credit - b.Debit
                                    : 0)
                        }).FirstOrDefault();

                    account.Transactions = transactions;
                }

                // تصفية النتائج إذا كان noZero = true
                if (obj.noZero)
                {
                    accountResults = accountResults
                        .Where(r => r.Transactions != null &&
                                    (r.Transactions.AMOUNTPrev + r.Transactions.Debit + r.Transactions.Credit != 0 ||
                                     r.Transactions.AMOUNTPrev != 0))
                        .ToList();
                }

                return accountResults.OrderBy(r => r.AccAccountCode).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred: {ex.Message}", ex);
            }
        }


        #endregion ================================== ميزان المراجعة

        #region =====================================   كشف حساب الموردين من الى 

        public async Task<IEnumerable<CustomerSupTransactionDto>> GetCustomerSupTransactionsFromTo(CustomerSupTransactionFilterDto filter)
        {
            try
            {
                filter.BranchId ??= 0;
                filter.GroupId ??= 0;
                filter.EmpId ??= 0;
                // تحويل التواريخ
                DateTime parsedDateFrom = DateTime.ParseExact(filter.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(filter.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // 1. تحميل البيانات من ACC_Balance_Sheet إلى قائمة في الذاكرة (مشابه لإنشاء جدول مؤقت)
                var balanceSheetData = await context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == filter.FacilityId &&
                        b.FinYear == filter.FinYear &&
                        (string.IsNullOrEmpty(filter.StartCode) || string.Compare(b.AccAccountCode, filter.StartCode) >= 0) &&
                        (string.IsNullOrEmpty(filter.EndCode) || string.Compare(b.AccAccountCode, filter.EndCode) <= 0))
                    .ToListAsync();

                // 2. جلب البيانات من Sys_Customer_VW بناءً على المعرفات المرجعية
                var customerData = await context.SysCustomerVws
                    .AsNoTracking()
                    .Where(c =>
                        c.IsDeleted == false &&
                        c.FacilityId == filter.FacilityId &&
                        (filter.BranchId == 0 || c.BranchId == filter.BranchId) &&
                        (filter.EmpId == 0 || c.EmpId == filter.EmpId) &&
                        (filter.GroupId == 0 || c.GroupId == filter.GroupId) &&
                        c.CusTypeId == 2 &&
                        balanceSheetData.Select(b => b.ReferenceDNo).Contains(c.Id))
                    .Select(c => new CustomerSupTransactionDataDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Name2 = c.Name2,
                        Code = c.Code,
                        CollectorName = c.CollectorName
                    })
                    .ToListAsync();

                // 3. حساب المعاملات (AMOUNTPrev, Debit, Credit, AMOUNTNext) لكل عميل
                var customerTransactions = customerData.Select(customer =>
                {
                    var transactions = balanceSheetData.Where(b => b.ReferenceDNo == customer.Id);

                    var amountPrev = transactions.Where(b =>
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) < parsedDateFrom ||
                        (b.DocTypeId is 4 or 27 &&
                         DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                         DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo))
                        .Sum(b => (b.Debit ?? 0) - (b.Credit ?? 0));

                    var debit = transactions.Where(b =>
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo &&
                        b.DocTypeId != 4 && b.DocTypeId != 27)
                        .Sum(b => b.Debit ?? 0);

                    var credit = transactions.Where(b =>
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo &&
                        b.DocTypeId != 4 && b.DocTypeId != 27)
                        .Sum(b => b.Credit ?? 0);

                    var amountNext = transactions.Where(b =>
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo)
                        .Sum(b => (b.Debit ?? 0) - (b.Credit ?? 0));

                    return new CustomerSupTransactionDto
                    {
                        Code = customer.Code,
                        Name = customer.Name,
                        Name2 = customer.Name2,
                        CollectorName1 = customer.CollectorName,
                        AmountPrev = amountPrev,
                        Debit = debit,
                        Credit = credit,
                        AmountNext = amountNext
                    };
                });

                // 4. تصفية النتائج إذا كان `NoZero` موجود
                if (filter.NoZero)
                {
                    customerTransactions = customerTransactions.Where(t => t.AmountNext != 0);
                }

                return customerTransactions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<CustomerSupTransactionDto>();
            }
        }



        #endregion


        #region ========================================== الاستاذ العام


        public async Task<IEnumerable<GeneralLedgerDtoResult>> GetGeneralLedger(GeneralLedgerDto obj)
        {
            // تحويل التواريخ من النصوص إلى DateTime
            DateTime parsedDateFrom = DateTime.ParseExact(obj.StartDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime parsedDateTo = DateTime.ParseExact(obj.EndDate, "yyyy/MM/dd", CultureInfo.InvariantCulture);

            // الاستعلام الأساسي بدون فلترة التاريخ في قاعدة البيانات
            var query = context.AccBalanceSheets
                .AsNoTracking()
                .Where(b =>
                    b.FlagDelete == false &&
                    b.FacilityId == obj.facilityId
                );

            // تصفية البيانات باستخدام التواريخ على الجانب العميل بعد جلبها
            var result = await query.ToListAsync(); // جلب البيانات من قاعدة البيانات

            // تصفية البيانات على الجانب العميل
            var filteredResult = result
                .Where(b =>
                    DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                    DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo
                );

            // تجميع النتائج وتحويلها إلى الشكل النهائي
            return filteredResult
                .GroupBy(b => new { b.AccAccountName, b.AccAccountCode, b.AccAccountName2 })
                .Select(g => new GeneralLedgerDtoResult
                {
                    Debit = g.Sum(b => b.Debit),
                    Credit = g.Sum(b => b.Credit),
                    AccAccountName = g.Key.AccAccountName,
                    AccAccountCode = g.Key.AccAccountCode,
                    AccAccountName2 = g.Key.AccAccountName2,
                    Balance=(g.Sum(b => b.Debit)- g.Sum(b => b.Credit))
                })
                .ToList();
        }

        #endregion ================================== الاستاذ العام

        #region ========================================== قائمة الدخل
        public async Task<IEnumerable<IncomeStatementDtoResult>> GetIncomeStatement(IncomeStatementDto obj)
        {
            // تحويل التواريخ من النصوص إلى DateTime
            DateTime parsedDateFrom = DateTime.ParseExact(obj.JDateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime parsedDateTo = DateTime.ParseExact(obj.JDateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

            // الاستعلام الأساسي بدون فلترة التاريخ في قاعدة البيانات
            var query = context.AccAccountsVw
                .AsNoTracking()
                .Where(a =>
                    a.FlagDelete==false &&
                    a.FacilityId == obj.FacilityId &&
                    a.AccountLevel == obj.AccountLevel &&
                    (obj.CCCodeFrom == null || string.Compare(a.CostCenterCode, obj.CCCodeFrom) >= 0) &&
                    (obj.CCCodeTo == null || string.Compare(a.CostCenterCode, obj.CCCodeTo) <= 0)
                );

            // جلب البيانات من قاعدة البيانات
            var accounts = await query.ToListAsync();

            // تصفية البيانات على الجانب العميل
            var incomeAccounts = accounts
                .Where(a => a.AccGroupId == obj.GroupIncome)
                .Select(a => new IncomeStatementDtoResult
                {
                    IsSub = a.IsSub,
                    Value = CalculateNetValue(a.AccAccountCode, obj, parsedDateFrom, parsedDateTo),
                    accountId=a.AccAccountId,
                    AccAccountCode = a.AccAccountCode,
                    AccAccountName = a.AccAccountName,
                    AccAccountName2 = a.AccAccountName2
                });

            var expenseAccounts = accounts
                .Where(a => a.AccGroupId == obj.GroupExpenses)
                .Select(a => new IncomeStatementDtoResult
                {
                    IsSub = a.IsSub,
                    Value = CalculateNetValue(a.AccAccountCode, obj, parsedDateFrom, parsedDateTo),
                    accountId = a.AccAccountId,
                    AccAccountCode = a.AccAccountCode,
                    AccAccountName = a.AccAccountName,
                    AccAccountName2 = a.AccAccountName2
                });

            // حساب إجمالي الدخل والمصاريف
            decimal totalIncome = incomeAccounts.Sum(i => i.Value ?? 0);
            decimal totalExpenses = expenseAccounts.Sum(e => e.Value ?? 0);

            // إضافة إجمالي الدخل والمصاريف وصافي الربح أو الخسارة
            var result = incomeAccounts
                .Concat(expenseAccounts)
                .ToList();

            result.Add(new IncomeStatementDtoResult
            {
                IsSub = false,
                Value = totalIncome,
                accountId =0,
                AccAccountCode = string.Empty,
                AccAccountName = "إجمالي الدخل",
                AccAccountName2 = "Total income"
            });

            result.Add(new IncomeStatementDtoResult
            {
                IsSub = false,
                Value = totalExpenses,
                accountId = 194,
                AccAccountCode = string.Empty,
                AccAccountName = "إجمالي المصاريف",
                AccAccountName2 = "Total expenses"
            });

            result.Add(new IncomeStatementDtoResult
            {
                IsSub = false,
                Value = totalIncome - totalExpenses,
                accountId = 0,
                AccAccountCode = string.Empty,
                AccAccountName = totalIncome >= totalExpenses ? "صافي الربح" : "صافي الخسارة",
                 AccAccountName2 = totalIncome >= totalExpenses ? " net profit" : "net loss"
            });

            return result;
        }

        // دالة حساب القيمة الصافية
        private decimal? CalculateNetValue(string accAccountCode, IncomeStatementDto obj, DateTime fromDate, DateTime toDate)
        {
            // من المفترض أن تكون لديك دالة مشابهة في قاعدة البيانات لحساب القيمة
            return context.AccBalanceSheets
                .Where(b =>
                    b.AccAccountCode == accAccountCode &&
                    b.JDateGregorian.CompareTo(fromDate.ToString("yyyy/MM/dd")) >= 0 &&
                    b.JDateGregorian.CompareTo(toDate.ToString("yyyy/MM/dd")) <= 0 &&
                    b.FacilityId == obj.FacilityId &&
                    b.BranchId == obj.BranchId
                )
                .Sum(b => (decimal?)(b.Debit - b.Credit));
        }
        public async Task<IEnumerable<IncomeStatementDetailsDtoResult>> IncomeStatementDetails(IncomeStatementDetailsDto obj)
        {
            try
            {
                // التحقق من المدخلات
                if (string.IsNullOrEmpty(obj.JDateFrom) || string.IsNullOrEmpty(obj.JDateTo))
                {
                    throw new ArgumentException("Dates cannot be null or empty.");
                }

                // تحويل التواريخ من النصوص إلى DateTime
                DateTime parsedDateFrom = DateTime.ParseExact(obj.JDateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
                DateTime parsedDateTo = DateTime.ParseExact(obj.JDateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

                // جلب البيانات الأساسية
                var query = context.AccBalanceSheets
                    .AsNoTracking()
                    .Where(b =>
                        b.FlagDelete == false &&
                        b.FacilityId == obj.FacilityId &&
                        (b.AccAccountId == obj.accountId || b.AccAccountParentId == obj.accountId))
                    .AsEnumerable() // التصفية على الجانب العميل
                    .Where(b =>
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                        DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo);

                // إضافة تصفية مركز التكلفة إذا كان موجودًا
                if (obj.ccId != 0)
                {
                    query = query.Where(b => b.CcId == obj.ccId);
                }

                // تنفيذ الاستعلام وترتيب النتائج
                var result = query
                    .OrderBy(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture)) // ترتيب التاريخ
                    .ThenBy(b => b.SortNo)
                    .Select(b => new IncomeStatementDetailsDtoResult
                    {
                        RowId = 0,
                        JDateHijri = b.JDateHijri,
                        Description = b.Description,
                        Debit = b.Debit,
                        Credit = b.Credit,
                        Balance = 0, // يمكن حساب الرصيد هنا إذا لزم الأمر
                        CostCenterName = b.CostCenterName,
                        JCode = b.JCode,
                        NatureAccount = b.NatureAccount
                    })
                    .ToList(); // استخدام ToList لأن البيانات على جانب العميل

                return result;
            }
            catch (Exception ex)
            {
                // معالجة الأخطاء
                throw new Exception($"An error occurred while fetching balance sheet data: {ex.Message}", ex);
            }
        }

        #endregion ================================== قائمة الدخل


        #region ==========================================   قائمة المركز المالي

        //public async Task<IEnumerable<FinancialCenterListBindDataDtoResult>> FinancialCenterListBindData(FinancialCenterListBindDataDto obj)
        //{
        //    try
        //    {
        //        if (obj.CMDTYPE == 2)
        //        {
        //            var groupAssets = await context.AccFacilities
        //                .Where(f => f.FacilityId == obj.FacilityId)
        //                .Select(f => f.GroupAssets)
        //                .FirstOrDefaultAsync();

        //            var result = await context.AccAccounts
        //                .Where(a => a.AccGroupId == groupAssets &&
        //                            a.IsDeleted == false &&
        //                            a.FacilityId == obj.FacilityId &&
        //                            a.AccountLevel == obj.AccountLevel)
        //                .Select(async a => new FinancialCenterListBindDataDtoResult
        //                {
        //                    AccAccountId = a.AccAccountId,
        //                    AccAccountName = a.AccAccountName,
        //                    AccAccountName2 = a.AccAccountName2,
        //                    AccAccountCode = a.AccAccountCode,
        //                    Details = await FinancialCenterList(),
        //                    Net = context.AccBalanceSheets
        //                        .Where(b => b.AccAccountCode.StartsWith(a.AccAccountCode) &&
        //                                    b.FlagDelete == false &&
        //                                    b.FacilityId == obj.FacilityId &&
        //                                    string.Compare(b.JDateGregorian, obj.JDateTo) <= 0 &&
        //                                    (obj.BranchId == 0 || b.MbranchId == obj.BranchId) &&
        //                                    b.FinYear == obj.FinYear)
        //                        .GroupBy(b => b.NatureAccount)
        //                        .Select(g => (g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Key)
        //                        .FirstOrDefault()
        //                })
        //                .ToListAsync();

        //            return result;
        //        }
        //        else if (obj.CMDTYPE == 3)
        //        {
        //            var groupLiabilities = await context.AccFacilities
        //                .Where(f => f.FacilityId == obj.FacilityId)
        //                .Select(f => f.GroupLiabilities)
        //                .FirstOrDefaultAsync();

        //            var result = await context.AccAccounts
        //                .Where(a => a.AccGroupId == groupLiabilities &&
        //                            a.IsDeleted == false &&
        //                            a.FacilityId == obj.FacilityId &&
        //                            a.AccountLevel == obj.AccountLevel)
        //                .Select(a => new FinancialCenterListBindDataDtoResult
        //                {
        //                    AccAccountId = a.AccAccountId,
        //                    AccAccountName = a.AccAccountName,
        //                    AccAccountName2 = a.AccAccountName2,
        //                    AccAccountCode = a.AccAccountCode,
        //                    Net = context.AccBalanceSheets
        //                        .Where(b => b.AccAccountCode.StartsWith(a.AccAccountCode) &&
        //                                    b.FlagDelete == false &&
        //                                    b.FacilityId == obj.FacilityId &&
        //                                    string.Compare(b.JDateGregorian, obj.JDateTo) <= 0 &&
        //                                    (obj.BranchId == 0 || b.MbranchId == obj.BranchId) &&
        //                                    b.FinYear == obj.FinYear)
        //                        .GroupBy(b => b.NatureAccount)
        //                        .Select(g => (g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Key)
        //                        .FirstOrDefault()
        //                })
        //                .ToListAsync();

        //            return result;
        //        }
        //        else if (obj.CMDTYPE == 4)
        //        {
        //            var profit = await accFunctionRepository.CalculateProfitLoss(obj.JDateFrom, obj.JDateTo, Convert.ToInt32(obj.FacilityId), 0, (int)obj.FinYear, 0);



        //            var profitLossTitle = "ارباح / خسائر الفترة";

        //            var groupCopyrights = await context.AccFacilities
        //                .Where(f => f.FacilityId == obj.FacilityId)
        //                .Select(f => f.GroupCopyrights)
        //                .FirstOrDefaultAsync();

        //            var accounts = await context.AccAccounts
        //                .Where(a => a.AccGroupId == groupCopyrights &&
        //                            a.IsDeleted == false &&
        //                            a.FacilityId == obj.FacilityId &&
        //                            a.AccountLevel == obj.AccountLevel)
        //                .Select(a => new FinancialCenterListBindDataDtoResult
        //                {
        //                    AccAccountId = a.AccAccountId,
        //                    AccAccountName = a.AccAccountName,
        //                    AccAccountName2 = a.AccAccountName2,
        //                    AccAccountCode = a.AccAccountCode,
        //                    Net = context.AccBalanceSheets
        //                        .Where(b => b.AccAccountCode.StartsWith(a.AccAccountCode) &&
        //                                    b.FlagDelete == false &&
        //                                    b.FacilityId == obj.FacilityId &&
        //                                    string.Compare(b.JDateGregorian, obj.JDateTo) <= 0 &&
        //                                    (obj.BranchId == 0 || b.MbranchId == obj.BranchId) &&
        //                                    b.FinYear == obj.FinYear)
        //                        .GroupBy(b => b.NatureAccount)
        //                        .Select(g => (g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Key)
        //                        .FirstOrDefault()
        //                })
        //                .ToListAsync();

        //            accounts.Add(new FinancialCenterListBindDataDtoResult
        //            {
        //                AccAccountId = 0,
        //                AccAccountName = profitLossTitle,
        //                AccAccountName2 = profitLossTitle,
        //                AccAccountCode = "",
        //                Net = profit
        //            });

        //            return accounts;
        //        }



        //        throw new ArgumentException("Invalid CMDTYPE");
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"An error occurred while fetching Financial Center List: {ex.Message}", ex);
        //    }
        //}

        public async Task<IEnumerable<FinancialCenterListBindDataDtoResult>> FinancialCenterListBindData(FinancialCenterListBindDataDto obj)
        {
            try
            {
                IQueryable<AccAccount> accountsQuery;
                if (obj.CMDTYPE == 2)
                {
                    var groupAssets = await context.AccFacilities
                        .Where(f => f.FacilityId == obj.FacilityId)
                        .Select(f => f.GroupAssets)
                        .FirstOrDefaultAsync();

                    accountsQuery = context.AccAccounts.Where(a => a.AccGroupId == groupAssets &&
                                   a.IsDeleted == false &&
                                    a.FacilityId == obj.FacilityId
                                    //&&
                                    //a.AccountLevel == obj.AccountLevel
                                    );
                }

                else if (obj.CMDTYPE == 3)
                {
                    var groupLiabilities = await context.AccFacilities
                        .Where(f => f.FacilityId == obj.FacilityId)
                        .Select(f => f.GroupLiabilities)
                        .FirstOrDefaultAsync();

                    accountsQuery = context.AccAccounts.Where(a => a.AccGroupId == groupLiabilities &&
                                   a.IsDeleted == false &&
                                    a.FacilityId == obj.FacilityId
                                    //&&
                                    //a.AccountLevel == obj.AccountLevel
                                    );
                }
                else if (obj.CMDTYPE == 4)
                {
                    var profit = await accFunctionRepository.CalculateProfitLoss(obj.JDateFrom, obj.JDateTo, Convert.ToInt32(obj.FacilityId), 0, (int)obj.FinYear, obj.AccountLevel??0);
                    var profitLossTitle = "ارباح / خسائر الفترة";

                    var groupCopyrights = await context.AccFacilities
                        .Where(f => f.FacilityId == obj.FacilityId)
                        .Select(f => f.GroupCopyrights)
                        .FirstOrDefaultAsync();

                    var accounts = await context.AccAccounts
                        .Where(a => a.AccGroupId == groupCopyrights && a.IsDeleted == false && a.FacilityId == obj.FacilityId 
                        //&& a.AccountLevel == obj.AccountLevel
                                    )

                        .ToListAsync();

                    var results = new List<FinancialCenterListBindDataDtoResult>();

                    foreach (var account in accounts)
                    {
                        results.Add(new FinancialCenterListBindDataDtoResult
                        {
                            AccAccountId = account.AccAccountId,
                            AccAccountName = account.AccAccountName,
                            //AccAccountName2 = account.AccAccountName2,
                            AccAccountCode = account.AccAccountCode,
                            Net = (await CalculateNet(account, obj)) ?? 0,
                            Details = (await FinancialCenterList(new FinancialCenterListDto { FacilityId = obj.FacilityId, accountId = account.AccAccountId, JDateFrom = obj.JDateFrom, JDateTo = obj.JDateTo })).ToList()
                        });
                    }

                    results.Add(new FinancialCenterListBindDataDtoResult
                    {
                        AccAccountId = 0,
                        AccAccountName = profitLossTitle,
                        AccAccountName2 = profitLossTitle,
                        AccAccountCode = "",
                        Net = profit
                    });

                    return results;
                }
                else
                {
                    throw new ArgumentException("Invalid CMDTYPE");
                }

                var filteredAccounts = await accountsQuery
                    .Where(a => a.IsDeleted == false && a.FacilityId == obj.FacilityId 
                    //&& a.AccountLevel == obj.AccountLevel
                    )
                    .ToListAsync();

                var finalResult = new List<FinancialCenterListBindDataDtoResult>();

                foreach (var account in filteredAccounts)
                {
                    finalResult.Add(new FinancialCenterListBindDataDtoResult
                    {
                        AccAccountId = account.AccAccountId,
                        AccAccountName = account.AccAccountName,
                        //AccAccountName2 = account.AccAccountName2,
                        AccAccountCode = account.AccAccountCode,
                        Net = (await CalculateNet(account, obj)) ?? 0,
                        Details = (await FinancialCenterList(new FinancialCenterListDto { FacilityId = obj.FacilityId, accountId = account.AccAccountId, JDateFrom = obj.JDateFrom, JDateTo = obj.JDateTo })).ToList()
                    });
                }

                return finalResult;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching Financial Center List: {ex.Message}", ex);
            }
        }

        public async Task<decimal?> CalculateNet(AccAccount account, FinancialCenterListBindDataDto obj)
        {
            return await context.AccBalanceSheets
                .Where(b => b.AccAccountCode.StartsWith(account.AccAccountCode) &&
                            b.FlagDelete == false &&
                            b.FacilityId == obj.FacilityId &&
                            string.Compare(b.JDateGregorian, obj.JDateTo) <= 0 &&
                            (obj.BranchId == 0 || b.MbranchId == obj.BranchId) &&
                            b.FinYear == obj.FinYear)
                .GroupBy(b => b.NatureAccount)
                .Select(g => (g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Max(b => b.NatureAccount))
                .FirstOrDefaultAsync();
        }

        public async Task<List<FinancialCenterListDtoResult>> FinancialCenterList(FinancialCenterListDto obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.JDateFrom) || string.IsNullOrEmpty(obj.JDateTo))
                {
                    throw new ArgumentException("Dates cannot be null or empty.");
                }

                var accountsQuery = context.AccAccountsVw
                    .AsNoTracking()
                    .Where(a => a.FacilityId == obj.FacilityId && a.AccAccountParentId == obj.accountId);

                var result = await accountsQuery
                    .Select(a => new FinancialCenterListDtoResult
                    {
                        AccAccountId = a.AccAccountId,
                        AccAccountName = a.AccAccountName,
                        AccAccountCode = a.AccAccountCode,
                        AccountLevel = a.AccountLevel,
                        NatureAccount = a.NatureAccount,
                        AccGroupId = a.AccGroupId,
                        Net = context.AccBalanceSheets
                            .AsNoTracking()
                            .Where(b => b.FlagDelete == false &&
                                        b.FacilityId == obj.FacilityId &&
                                        b.AccAccountCode.StartsWith(a.AccAccountCode) &&
                                        b.FinYear == obj.FinYear &&
                                        string.Compare(b.JDateGregorian, obj.JDateTo) <= 0 &&
                                        (obj.BranchId == 0 || b.MbranchId == obj.BranchId))
                            .GroupBy(b => b.NatureAccount)
                            .Select(g => (g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Max(b => b.NatureAccount))
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching Financial Center List: {ex.Message}", ex);
            }
        }

        //public async Task<IEnumerable<FinancialCenterListDtoResult>> FinancialCenterList(FinancialCenterListDto obj)
        //{
        //    try
        //    {
        //        // التحقق من المدخلات
        //        if (string.IsNullOrEmpty(obj.JDateFrom) || string.IsNullOrEmpty(obj.JDateTo))
        //        {
        //            throw new ArgumentException("Dates cannot be null or empty.");
        //        }

        //        // تحويل التواريخ من النصوص إلى DateTime
        //        DateTime parsedDateFrom = DateTime.ParseExact(obj.JDateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        //        DateTime parsedDateTo = DateTime.ParseExact(obj.JDateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

        //        // تحويل التواريخ إلى نصوص بصيغة yyyy/MM/dd
        //        string formattedDateFrom = parsedDateFrom.ToString("yyyy/MM/dd");
        //        string formattedDateTo = parsedDateTo.ToString("yyyy/MM/dd");

        //        // الاستعلام الأساسي عن الحسابات
        //        var accountsQuery = context.AccAccountsVw
        //            .AsNoTracking()
        //            .Where(a =>
        //                a.FacilityId == obj.FacilityId &&
        //                a.AccAccountParentId == obj.accountId
        //            );

        //        // الاستعلام للحصول على Net لكل حساب
        //        var result = await accountsQuery
        //            .Select(a => new FinancialCenterListDtoResult
        //            {
        //                AccAccountId = a.AccAccountId,
        //                AccAccountName = a.AccAccountName,
        //                AccAccountCode = a.AccAccountCode,
        //                AccountLevel = a.AccountLevel,
        //                NatureAccount = a.NatureAccount,
        //                AccGroupId = a.AccGroupId,
        //                Net = context.AccBalanceSheets
        //                    .AsNoTracking()
        //                    .Where(b =>
        //                        b.FlagDelete == false &&
        //                        b.FacilityId == obj.FacilityId &&
        //                        (b.AccAccountId == obj.accountId || b.AccAccountParentId == obj.accountId) &&
        //                        string.Compare(b.JDateGregorian, formattedDateFrom) >= 0 &&
        //                        string.Compare(b.JDateGregorian, formattedDateTo) <= 0
        //                    )
        //                    .GroupBy(b => b.NatureAccount)
        //                    .Select(g => (g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Key)
        //                    .FirstOrDefault()
        //            })
        //            .ToListAsync();

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        // معالجة الأخطاء
        //        throw new Exception($"An error occurred while fetching Financial Center List: {ex.Message}", ex);
        //    }
        //}


        #endregion ==================================  قائمة المركز المالي


    }
}
