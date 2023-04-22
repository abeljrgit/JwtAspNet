using AutoMapper;
using JwtAspNet.Dto;
using JwtAspNet.Models;

namespace JwtAspNet.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // CreateMap<Incoming,OutComing>();

            // RegisterUser
            CreateMap<User, UserDto>();
        }
    }
}
