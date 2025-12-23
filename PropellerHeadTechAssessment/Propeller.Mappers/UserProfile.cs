using AutoMapper;
using Propeller.Entities;
using Propeller.Models;

namespace Propeller.Mappers
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<User, PropellerUser>();
        }
    }
}
