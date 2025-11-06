using AutoMapper;
using ebay.Dtos;
using ebay.Models;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<UserRegisterDTO, User>()
        .ForMember(vm => vm.PasswordHash, opt => opt.Ignore())
        .ForMember(vm => vm.CreatedAt, opt => opt.MapFrom(dto => DateTime.Now));
    }
}