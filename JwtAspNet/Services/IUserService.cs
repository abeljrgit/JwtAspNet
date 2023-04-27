using JwtAspNet.Dto;

namespace JwtAspNet.Services
{
    public interface IUserService
    {
        Task<Tuple<LoginResponseDto?, string>> LoginUser(LoginRequestDto loginDto);
        Task<RegisterResponseDto?> RegisterUser(RegisterRequestDto registerDto);
    }
}