using Logix.Application.DTOs.ACC;
using Logix.Application.Wrapper;
using Logix.Domain.ACC;
using System.Linq.Expressions;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccFacilityService : IGenericQueryService<AccFacilityDto, AccFacilitiesVw>, IGenericWriteService<AccFacilityDto, AccFacilityEditDto>
    {
        //Task<IResult<IEnumerable<AccFacilitiesVw>>> GetAllVW(CancellationToken cancellationToken = default);

        ////Task<IEnumerable<R>> GetAll<R>(Expression<Func<AccFacilityDto, R>> selector);
        //Task<IResult<IEnumerable<AccFacilityDto>>> GetAll(Expression<Func<AccFacilityDto, bool>> expression);

        Task<IResult> UpdateStamp(long facilityId, string newStampUrl, CancellationToken cancellationToken = default);
        Task<IResult<R>> Find<R>(Expression<Func<AccFacilityDto, R>> selector, Expression<Func<AccFacilityDto, bool>> expression, CancellationToken cancellationToken = default);
        Task<IResult<AccFacilityEditProfileDto>> GetForUpdateProfile(long Id, CancellationToken cancellationToken = default);
        Task<IResult<AccFacilityEditProfileDto>> UpdateProfile(AccFacilityEditProfileDto obj, CancellationToken cancellationToken = default);
        Task<IResult<AccFacilityProfileDto>> UpdateProfileEdit(AccFacilityProfileDto obj, CancellationToken cancellationToken = default);
        Task<IResult> UpdateValue(long ID,long value,long TypeID, CancellationToken cancellationToken = default);
        Task<IResult> UpdateValueVAT(long ID, long value, long value2, bool VATEnable, CancellationToken cancellationToken = default);
        Task<IResult> UpdateLogo(long ID ,string value, long TypeID, CancellationToken cancellationToken = default);
        Task<IResult> UpdatePurchaseAccount(long ID, long value, bool UsingPurchaseAccount, CancellationToken cancellationToken = default);

    }
}
