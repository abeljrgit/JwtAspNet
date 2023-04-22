using JwtAspNet.Models;

namespace JwtAspNet.Repository
{
    public interface IUserRepository
    {
        Task<List<User>> LoginUser(string userEmail);
        Task RegisterUser(User user);
    }
}