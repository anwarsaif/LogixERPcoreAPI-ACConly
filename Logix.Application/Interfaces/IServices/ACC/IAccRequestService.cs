using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccRequestService : IGenericQueryService<AccRequestDto, AccRequestVw>, IGenericWriteService<AccRequestDto, AccRequestEditDto>

    {
        Task<IResult<AccRequestMultiDto>> AddMulti(AccRequestMultiDto entity, CancellationToken cancellationToken = default);

        Task<IResult<AccRequestPaymentDto>> AddPaymentDecision(AccRequestPaymentDto entity, CancellationToken cancellationToken = default);

        Task<IResult<AccRequestPaymentEditDto>> UpdatePaymentDecision(AccRequestPaymentEditDto entity, CancellationToken cancellationToken = default);
        Task<IResult<List<AccRequestVw>>> Search(AccRequestFilterDto filter, CancellationToken cancellationToken = default);

        Task<IResult<List<TransactionResult>>> GetUnPaidPO(string transTypeIds, string code, string dateText);
        Task<IResult<List<TransactionUnPaidResult>>> GetUnPaidSubEx(string code, string dateText);
        Task<IResult<List<PayrollResultpopup>>> GetApprovedPayroll(PayrollResultPopupFilter filter);

    }

}
