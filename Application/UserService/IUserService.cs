using Domain.DTOs;
using Domain.Entities;

namespace Application.UserService
{
    public interface IUserService
    {
        Task<User?> GetUserByEmail(string email);
        Task RegisterUser(UserRegisterDTO user);
        Task<bool> AuthenticateUser(string email, string password);

    }
}