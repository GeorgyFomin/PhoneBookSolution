using AutoMapper;
using Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UseCases.API.Dto;

namespace UseCases.API.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Phone, PhoneDto>().ForMember("PhoneNumber", opt => opt.MapFrom(c => c.PhoneNumder == null ? 0 : c.PhoneNumder.NationalNumber));
        }
    }
}
