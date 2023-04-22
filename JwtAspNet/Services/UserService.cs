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
                    return Tuple.Create<UserDto?, string>(_mapper.Map<UserDto>(user[0]), "Success, Your Token: " + CreateToken(user[0]));
                else
                    return Tuple.Create<UserDto?, string>(null, "Please check your Employee Id or Password");
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.Email)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

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
