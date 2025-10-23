using Castle.MicroKernel;
using Logix.Application.Common;
using Logix.Application.DTOs.ACC;
using Logix.Application.Interfaces.IRepositories.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using Logix.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Globalization;

namespace Logix.Infrastructure.Repositories.ACC
{
    public class JoinKey
    {
        public long? ReferenceNo { get; set; }
        public int? RefranceType { get; set; }
    }

    public class AccFunctionRepository : IAccFunctionRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ICurrentData session;

        public AccFunctionRepository(ApplicationDbContext context, ICurrentData session)
        {
            this.context = context;
            this.session = session;
        }
        


        public async Task<decimal> CalculateProfitLoss(string dateFrom, string dateTo, long facilityId, int branchId, int financialYear, int accountLevel)
        {
            // Initialize variables for income and expense group IDs
            long groupIncomeId;
            long groupExpensesId;

            // Retrieve group IDs for income and expenses from ACC_Facilities table
            var facility = await context.AccFacilities
                .Where(f => f.FacilityId == facilityId)
                .Select(f => new { f.GroupIncame, f.GroupExpenses })
                .FirstOrDefaultAsync();

            if (facility == null)
                throw new InvalidOperationException("Facility not found");

            groupIncomeId = facility.GroupIncame ?? 0;
            groupExpensesId = facility.GroupExpenses ?? 0;

            // Initialize cumulative values for income and expenses
            decimal valueIncome = await GetNetValueByGroup(dateFrom, dateTo, facilityId, branchId, groupIncomeId, financialYear);
            decimal valueExpenses = await GetNetValueByGroup(dateFrom, dateTo, facilityId, branchId, groupExpensesId, financialYear);

            // Calculate net profit/loss
            return valueIncome - valueExpenses;
        }

        public async Task<decimal> GetNetValueByGroup(
            string dateFrom, string dateTo, long facilityId, int branchId, long accountGroupId, int financialYear, string ccCodeFrom = null, string ccCodeTo = null)
        {
            DateTime parsedDateFrom = DateTime.ParseExact(dateFrom, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            DateTime parsedDateTo = DateTime.ParseExact(dateTo, "yyyy/MM/dd", CultureInfo.InvariantCulture);

            var data = await context.AccBalanceSheetCostCenterVw
                .Where(b => b.FacilityId == facilityId && b.FlagDelete == false && b.StatusId == 2)
                .Where(b => b.AccGroupId == accountGroupId && b.FinYear == financialYear)
                .Where(b => branchId == 0 || b.MbranchId == branchId)
                .Where(b => ccCodeFrom == null || (
                    (b.CostCenterCode.CompareTo(ccCodeFrom) >= 0 && b.CostCenterCode.CompareTo(ccCodeTo) <= 0) ||
                    (b.CostCenter2Code.CompareTo(ccCodeFrom) >= 0 && b.CostCenter2Code.CompareTo(ccCodeTo) <= 0) ||
                    (b.CostCenter3Code.CompareTo(ccCodeFrom) >= 0 && b.CostCenter3Code.CompareTo(ccCodeTo) <= 0) ||
                    (b.CostCenter4Code.CompareTo(ccCodeFrom) >= 0 && b.CostCenter4Code.CompareTo(ccCodeTo) <= 0) ||
                    (b.CostCenter5Code.CompareTo(ccCodeFrom) >= 0 && b.CostCenter5Code.CompareTo(ccCodeTo) <= 0)
                ))
                .ToListAsync();

            var filteredData = data
                .Where(b => DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) >= parsedDateFrom &&
                            DateTime.ParseExact(b.JDateGregorian, "yyyy/MM/dd", CultureInfo.InvariantCulture) <= parsedDateTo);

            var result = filteredData
                .GroupBy(b => new { b.AccGroupId, b.NatureAccount })
                .Select(g => (g.Sum(b => b.Credit) - g.Sum(b => b.Debit)) * g.Key.NatureAccount)
                .FirstOrDefault();

            return result ?? 0;
        }

        // تعريف فئة المفتاح للربط
        private class JoinKey
        {
            public string ReferenceNo { get; set; }
            public int? RefranceType { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is JoinKey other)
                {
                    return ReferenceNo == other.ReferenceNo && RefranceType == other.RefranceType;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(ReferenceNo, RefranceType);
            }
        }

        public async Task<IEnumerable<BalanceSheetFinancialYear>> GetBalanceSheetData(long facilityId, int currentFinancialYear, int currentPeriodId, int nextFinancialYear)
        {
            try
            {
                // استرجاع بيانات السنة المالية الحالية
                var currentFinancialYearData = await context.AccFinancialYears
                    .Where(f => f.FinYear == currentFinancialYear)
                    .Select(f => new { f.StartDateGregorian, f.EndDateGregorian })
                    .FirstOrDefaultAsync();

                if (currentFinancialYearData == null)
                {
                    throw new Exception("Financial year data not found.");
                }

                var dateFrom = currentFinancialYearData.StartDateGregorian;
                var dateTo = currentFinancialYearData.EndDateGregorian;

                decimal profit = await CalculateProfitLoss(dateFrom, dateTo, facilityId, 0, currentFinancialYear, 0);

                var facilityData = await context.AccFacilities
                    .Where(f => f.FacilityId == facilityId)
                    .Select(f => new
                    {
                        f.GroupCopyrights,
                        f.GroupAssets,
                        f.GroupLiabilities,
                        f.ProfitAndLossAccountId
                    })
                    .FirstOrDefaultAsync();

                if (facilityData == null)
                {
                    throw new Exception("Facility data not found.");
                }

                var profitAccount = await context.AccAccounts
                    .Where(a => a.AccAccountId == facilityData.ProfitAndLossAccountId)
                    .Select(a => new { a.AccAccountCode, a.AccAccountName })
                    .FirstOrDefaultAsync();

                if (profitAccount == null)
                {
                    throw new Exception("Profit and Loss account not found.");
                }

                // استرجاع بيانات ACC_Balance_Sheet و AccAccountsRefrancesVw
                var balanceSheets = await context.AccBalanceSheets
                    .Where(b => b.FinYear == currentFinancialYear &&
                                b.PeriodId == currentPeriodId &&
                                b.FacilityId == facilityId &&
                                new[] { facilityData.GroupCopyrights, facilityData.GroupAssets, facilityData.GroupLiabilities }
                                    .Contains(b.AccGroupId))
                    .ToListAsync();

                var accountReferences = await context.AccAccountsRefrancesVw.ToListAsync();

                // الربط بين البيانات
                var balanceSheetData = balanceSheets
                    .GroupJoin(
                        accountReferences,
                        b => new JoinKey { ReferenceNo = b.ReferenceDNo.ToString(), RefranceType = b.ParentReferenceTypeId },
                        r => new JoinKey { ReferenceNo = r.RefranceNo.ToString(), RefranceType = r.RefranceType },
                        (b, r) => new { AccBalanceSheet = b, AccAccountsRefrancesVw = r.FirstOrDefault() }
                    )
                   .GroupBy(gr => new
                   {
                       gr.AccBalanceSheet.AccAccountId,
                       gr.AccBalanceSheet.CcId,
                       gr.AccBalanceSheet.ReferenceTypeId,
                       gr.AccBalanceSheet.ReferenceTypeName,
                       gr.AccBalanceSheet.ReferenceTypeName2,
                       gr.AccBalanceSheet.ReferenceDNo,
                       gr.AccBalanceSheet.AccAccountCode,
                       gr.AccBalanceSheet.AccAccountName,
                       gr.AccBalanceSheet.AccAccountName2,
                       Code = gr.AccAccountsRefrancesVw?.Code,
                       Name = gr.AccAccountsRefrancesVw?.Name
                   })

                    .Where(g => g.Sum(x => (x.AccBalanceSheet.Credit ?? 0) - (x.AccBalanceSheet.Debit ?? 0)) != 0)
                    .Select(g => new BalanceSheetFinancialYear
                    {
                        ReferenceTypeName = g.Key.ReferenceTypeName,
                        Code = g.Key.Code,
                        Name = g.Key.Name,

                        AccAccountCode = g.Key.ReferenceTypeId == 1 ? g.Key.AccAccountCode : g.Key.Code,
                        AccAccountName = g.Key.ReferenceTypeId == 1 ? g.Key.AccAccountName : g.Key.Name,
                        AccAccountName2 = g.Key.ReferenceTypeId == 1 ? g.Key.AccAccountName2 : g.Key.Name,
                        JDateGregorian = currentFinancialYearData.StartDateGregorian,
                        CCID = (int)(g.Key.CcId.GetValueOrDefault()),
                        AccAccountID = g.Key.AccAccountId ?? 0,
                        Debit = g.Sum(x => (x.AccBalanceSheet.Credit ?? 0) - (x.AccBalanceSheet.Debit ?? 0)) < 0
                            ? -(g.Sum(x => (x.AccBalanceSheet.Credit ?? 0) - (x.AccBalanceSheet.Debit ?? 0)))
                            : 0,
                        Credit = g.Sum(x => (x.AccBalanceSheet.Credit ?? 0) - (x.AccBalanceSheet.Debit ?? 0)) > 0
                            ? g.Sum(x => (x.AccBalanceSheet.Credit ?? 0) - (x.AccBalanceSheet.Debit ?? 0))
                            : 0,
                        ReferenceTypeID = g.Key.ReferenceTypeId ?? 0,
                        ReferenceDNo = (int)g.Key.ReferenceDNo.GetValueOrDefault(),
                        Description = session.Language == 1 ? "رصيد افتتاحي" : "Opening Balance"
                    })
                    .ToList();

                // إضافة إدخال الأرباح والخسائر
                var profitAndLossEntry = new BalanceSheetFinancialYear
                {
                    ReferenceTypeName = null,
                    Code = "0",
                    Name = string.Empty,
                    AccAccountCode = profitAccount.AccAccountCode,
                    AccAccountName = profitAccount.AccAccountName,
                    JDateGregorian = currentFinancialYearData.StartDateGregorian,
                    CCID = 0,
                    AccAccountID = facilityData.ProfitAndLossAccountId ?? 0,
                    Debit = profit < 0 ? -profit : 0,
                    Credit = profit > 0 ? profit : 0,
                    ReferenceTypeID = 1,
                    ReferenceDNo = 0,
                    Description = session.Language == 1 ? "أرباح وخسائر الفترة" : "Profit and Loss for the period"
                };

                balanceSheetData.Add(profitAndLossEntry);

                return balanceSheetData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new List<BalanceSheetFinancialYear>();
            }
        }


        

    }


}
