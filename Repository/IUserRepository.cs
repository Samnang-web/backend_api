using Backend.Models;

namespace Backend.Repository
{
    public interface IUserRepository
    {
        Task<User> GetByUsername(string username);
        Task<int> Create(User user);
    }
}
