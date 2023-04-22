using JwtAspNet.Dto;

namespace JwtAspNet.Services
{
    public interface IUserService
    {
        Task<Tuple<UserDto?, string>> LoginUser(LoginDto loginDto);
        Task RegisterUser(RegisterDto registerDto);
    }
}