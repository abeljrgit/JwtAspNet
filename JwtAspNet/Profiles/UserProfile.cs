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

            // RegisterUser
            CreateMap<Tuple<User, string>, RegisterResponseDto>()
                .ForMember(rrd => rrd.Id, opt => opt.MapFrom(s => s.Item1.Id))
                .ForMember(rrd => rrd.Name, opt => opt.MapFrom(s => s.Item1.Name))
                .ForMember(rrd => rrd.Email, opt => opt.MapFrom(s => s.Item1.Email))
                .ForMember(rrd => rrd.Token, opt => opt.MapFrom(s => s.Item2));

            // LoginUser
            CreateMap<Tuple<User, string>, LoginResponseDto>()
                .ForMember(lrd => lrd.Id, opt => opt.MapFrom(s => s.Item1.Id))
                .ForMember(lrd => lrd.Name, opt => opt.MapFrom(s => s.Item1.Name))
                .ForMember(lrd => lrd.Email, opt => opt.MapFrom(s => s.Item1.Email))
                .ForMember(lrd => lrd.Token, opt => opt.MapFrom(s => s.Item2));
        }
    }
}
