using AutoMapper;
using JwtAspNet.Dto;
using JwtAspNet.Models;
using JwtAspNet.Repository;
using System.Security.Cryptography;

namespace JwtAspNet.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task RegisterUser(RegisterDto registerDto)
        {
            User user = new User();

            CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Name = registerDto.Name;
            user.Email = registerDto.Email;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _userRepository.RegisterUser(user);
        }

        public async Task<Tuple<UserDto?, string>> LoginUser(LoginDto loginDto)
        {
            var user = await _userRepository.LoginUser(loginDto.Email);

            if (user == null || user.Count == 0)
                return Tuple.Create<UserDto?, string>(null, "Please check your Email or Password");
            else
            {
                if (VerifyPasswordHash(loginDto.Password, user[0].PasswordHash, user[0].PasswordSalt))
                    return Tuple.Create<UserDto?, string>(_mapper.Map<UserDto>(user[0]), "Success");
                else
                    return Tuple.Create<UserDto?, string>(null, "Please check your Employee Id or Password");
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
