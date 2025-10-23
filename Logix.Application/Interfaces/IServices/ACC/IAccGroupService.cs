using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Application.Interfaces.IServices.ACC
{
    public interface IAccGroupService : IGenericQueryService<AccGroupDto, AccGroup>, IGenericWriteService<AccGroupDto, AccGroupEditDto>
    {

    }
}
