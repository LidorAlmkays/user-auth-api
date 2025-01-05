using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;

namespace Application.UserService
{
    public interface IUserService
    {
        Task<Role> GetUserRole(string email);
        public Task<TokensGeneratedDTO> Login(UserLoginDTO userInformation);
        Task<User?> GetUserByEmail(string email);
        Task RegisterUser(UserRegisterDTO user);
    }
}