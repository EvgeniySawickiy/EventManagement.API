using AutoMapper;
using EventManagement.Application.DTO.Request;
using EventManagement.Application.DTO.Response;
using EventManagement.Core.Entity;


namespace EventManagement.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        { 
            CreateMap<User, UserResponseDTO>();
            CreateMap<UserRequestDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password)); ;

            CreateMap<Participant, ParticipantResponseDTO>();
            CreateMap<RegisterParticipantToEventRequestDTO, Participant>();

            CreateMap<Event, EventResponseDTO>();
            CreateMap<EventRequestDTO, Event>();

            CreateMap<ParticipantRequestDTO, Participant>();

        }
    }
}