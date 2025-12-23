using AutoMapper;
using Propeller.Entities;
using Propeller.Models;

namespace Propeller.Mappers
{
    public class NoteProfile: Profile
    {
        public NoteProfile()
        {
            CreateMap<Note, NoteDto>();
        }
    }
}
