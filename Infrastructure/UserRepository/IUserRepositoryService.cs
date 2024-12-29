using Domain.Entities;

namespace Infrastructure.UserRepository
{
    public interface IUserRepositoryService
    {
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string email);
    }
}