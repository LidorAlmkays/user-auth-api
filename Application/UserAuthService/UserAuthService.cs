using Domain.DTOs;
using Domain.Entities;
using Infrastructure.UserRepository;

namespace Application.UserAuthService
{
    public class UserAuthService(UserRepositoryContext userRepositoryContext) : IUserAuthService
    {
        private readonly UserRepositoryContext userRepositoryContext = userRepositoryContext;

        public string GenerateToken(GenerateTokenInfoDTO user)
        {
            return "";
        }

        public User GetUserByEmail(string email)
        {
            var user = userRepositoryContext.Users.FirstOrDefault(u => u.Email == email) ?? throw new ArgumentException("User not found.");
            return user;
        }

        public bool RegisterUser(UserRegisterDTO user)
        {
            User userEntity = new User
            {
                Email = user.Email,
                Password = user.Password
            };
            userRepositoryContext.Users.Add(userEntity);

            return true;
        }
    }
}
