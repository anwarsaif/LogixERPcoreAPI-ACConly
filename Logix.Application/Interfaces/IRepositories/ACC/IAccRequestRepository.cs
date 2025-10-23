using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IRepositories.ACC
{
    public interface IAccRequestRepository : IGenericRepository<AccRequest, AccRequestVw>
    {

        Task<long> GetAccRequestCode(string AppDate);
        Task<List<AccRequestVw>> GetRequestWaitingTransfer(long facilityId, int transTypeId, long statusId, long appCode);
        Task<List<TransactionResult>> GetUnPaidPO(string transTypeIds, string code, string dateText);
        Task<List<TransactionUnPaidResult>> GetUnPaidSubEx(string code, string dateText, int paymentTermsId = 0);
        Task<List<PayrollResultpopup>> GetApprovedPayroll(PayrollResultPopupFilter filter);
    }

}
