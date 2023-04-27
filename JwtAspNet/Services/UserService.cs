using AutoMapper;
using JwtAspNet.Dto;
using JwtAspNet.Models;
using JwtAspNet.Repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace JwtAspNet.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<RegisterResponseDto?> RegisterUser(RegisterRequestDto registerDto)
        {
            User user = new User();

            CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Name = registerDto.Name;
            user.Email = registerDto.Email;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            List<User> returnedUserData = await _userRepository.RegisterUser(user);

            if (returnedUserData[0] != null || returnedUserData.Count == 0)
            {
                Tuple<User, string> tupleUserAndString = Tuple.Create(returnedUserData[0], CreateToken(returnedUserData[0]));

                return _mapper.Map<RegisterResponseDto>(tupleUserAndString);
            }
            else
            {
                return null;
            }
        }

        public async Task<Tuple<LoginResponseDto?, string>> LoginUser(LoginRequestDto loginDto)
        {
            List<User> user = await _userRepository.LoginUser(loginDto.Email);

            if (user[0] == null || user.Count == 0)
                return Tuple.Create<LoginResponseDto?, string>(null, "Please check your Email or Password");
            else
            {
                if (VerifyPasswordHash(loginDto.Password, user[0].PasswordHash, user[0].PasswordSalt))
                {
                    Tuple<User, string> tupleUserAndString = Tuple.Create(user[0], CreateToken(user[0]));

                    return Tuple.Create<LoginResponseDto?, string>(_mapper.Map<LoginResponseDto>(tupleUserAndString), "Success!!!");
                }
                else
                    return Tuple.Create<LoginResponseDto?, string>(null, "Please check your Employee Id or Password");
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.Email)
            };

            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(appSettingsToken));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
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
