using AutoMapper;
using Logix.Application.DTOs.ACC;
using Logix.Domain.ACC;
using Logix.Domain.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logix.Infrastructure.Mapping.ACC
{
    public class ACCGroupProfile:Profile
    {
        public ACCGroupProfile() {
            CreateMap<AccGroupDto, AccGroup>().ReverseMap();
            CreateMap<AccGroupEditDto, AccGroup>().ReverseMap();
        }


    }
}
