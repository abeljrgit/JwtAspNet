using JwtAspNet.Models;

namespace JwtAspNet.Repository
{
    public interface IUserRepository
    {
        Task<List<User>> LoginUser(string userEmail);
        Task<List<User>> RegisterUser(User user);
    }
}