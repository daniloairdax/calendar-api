using AutoMapper;
using Calendar.Application.Features.Animals.Models;
using Calendar.Application.Features.Appointments.Models;
using Calendar.Domain.Models;

namespace Calendar.Application.Mappings
{
    public class ApplicationMapperProfile : Profile
    {
        public ApplicationMapperProfile()
        {
            CreateMap<Appointment, VetAppointmentDto>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                .ForMember(dest => dest.AnimalName, opt => opt.MapFrom(src => src.Animal != null ? src.Animal.Name : string.Empty))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Animal != null ? src.Animal.OwnerName : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<Animal, AnimalDto>();
            CreateMap<Appointment, AppointmentDto>();
        }
    }
}