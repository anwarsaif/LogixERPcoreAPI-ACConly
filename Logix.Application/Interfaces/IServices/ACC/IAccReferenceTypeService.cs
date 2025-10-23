using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccReferenceTypeService :IGenericQueryService<AccReferenceTypeDto, AccReferenceTypeVw>, IGenericWriteService<AccReferenceTypeDto, AccReferenceTypeEditDto> 
    {

    }
}
