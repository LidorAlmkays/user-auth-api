using Domain.DTOs;
using Domain.Entities;

namespace Application.UserAuthService
{
    public interface IUserAuthService
    {
        User GetUserByEmail(string email);
        bool RegisterUser(UserRegisterDTO user);
        string GenerateToken(GenerateTokenInfoDTO user);

    }
}