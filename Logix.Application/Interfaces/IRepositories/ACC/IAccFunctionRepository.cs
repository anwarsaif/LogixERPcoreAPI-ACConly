using Logix.Application.DTOs.ACC;

namespace Logix.Application.Interfaces.IRepositories.ACC
{
    public interface IAccFunctionRepository
    {
        Task<IEnumerable<BalanceSheetFinancialYear>> GetBalanceSheetData(long facilityId, int currentFinancialYear, int currentPeriodId, int nextFinancialYear);

        Task<decimal> CalculateProfitLoss(string dateFrom, string dateTo, long facilityId, int branchId, int financialYear, int accountLevel);

        Task<decimal> GetNetValueByGroup(string dateFrom, string dateTo, long facilityId, int branchId, long accountGroupId, int financialYear, string ccCodeFrom = null, string ccCodeTo = null);

    }
}