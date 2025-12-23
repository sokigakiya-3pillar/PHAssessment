using AutoMapper;
using Propeller.Entities;
using Propeller.Models;
using Propeller.Models.Requests;
using Propeller.Shared;

namespace Propeller.Mappers
{
    public class CustomerProfile: Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDto>()
                .ForMember(d => d.ID, o => o.MapFrom(s => s.ID.Obfuscate()) );

            CreateMap<CreateCustomerRequest, Customer>()
                .ForMember(d => d.CustomerStatusID, o => o.MapFrom(s => s.Status.Deobfuscate()))
                ;

            CreateMap<UpdateCustomerRequest, Customer>()
                .ForMember(d => d.CustomerStatusID, o => o.MapFrom(s => s.Status));

            CreateMap<Customer, UpdateCustomerRequest>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.CustomerStatusID));
        }
    }
}