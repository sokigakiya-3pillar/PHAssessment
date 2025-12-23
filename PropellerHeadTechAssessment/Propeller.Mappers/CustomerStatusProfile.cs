using AutoMapper;
using Propeller.Entities;
using Propeller.Models;
using Propeller.Shared;

namespace Propeller.Mappers
{
    public class CustomerStatusProfile: Profile
    {
        public CustomerStatusProfile()
        {
            CreateMap<CustomerStatus, CustomerStatusDto>()
                .ForMember(d => d.ID, o => o.MapFrom(s => s.ID.Obfuscate()));
        }

    }
}
