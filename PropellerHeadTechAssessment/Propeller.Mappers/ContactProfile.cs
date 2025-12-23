using AutoMapper;
using Propeller.Entities;
using Propeller.Models;
using Propeller.Models.Requests;

namespace Propeller.Mappers
{
    public class ContactProfile : Profile
    {
        public ContactProfile()
        {
            CreateMap<UpdateContactRequest, Contact>();
            CreateMap<Contact, ContactDto>();
            CreateMap<CreateContactRequest, Contact>()
                .ForMember(dest => dest.PhoneNumber, m => m.MapFrom(orig => orig.Phone));
        }
    }
}
