using AutoMapper;
using StaffManagement.Application.DTOs;
using StaffManagement.Domain.Entities;
using StaffManagement.Domain.Enums;

namespace StaffManagement.Application.Mapping;

public class StaffMappingProfile : Profile
{
    public StaffMappingProfile()
    {
        CreateMap<Staff, StaffDto>()
            .ForMember(dest => dest.GenderName, opt => opt.MapFrom(src => src.Gender.ToString()));

        CreateMap<CreateStaffDto, Staff>()
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => (Gender)src.Gender));

        CreateMap<UpdateStaffDto, Staff>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => (Gender)src.Gender));
    }
}
