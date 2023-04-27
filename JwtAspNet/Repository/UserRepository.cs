using JwtAspNet.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtAspNet.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly JwtAspNetDbContext _jwtAspNetDbcontext;

        public UserRepository(JwtAspNetDbContext jwtAspNetDbcontext)
        {
            _jwtAspNetDbcontext = jwtAspNetDbcontext;
        }

        public async Task<List<User>> RegisterUser(User user)
        {
            await _jwtAspNetDbcontext.Users.AddAsync(user);
            await _jwtAspNetDbcontext.SaveChangesAsync();

            return await _jwtAspNetDbcontext.Users
                .Where(u => u.Email == user.Email)
                .ToListAsync();
        }

        public async Task<List<User>> LoginUser(string userEmail)
        {
            return await _jwtAspNetDbcontext.Users
                .Where(u => u.Email == userEmail)
                .ToListAsync();
        }
    }
}
