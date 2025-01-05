using Application.EncryptingService;
using Application.TokenService;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.UserRepository;

namespace Application.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepositoryService userRepository;
        private readonly ITokenService tokenService;
        private readonly IEncryptingService encryptingService;
        public UserService(IUserRepositoryService userRepository, IEncryptingService encryptingService, ITokenService tokenService)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.encryptingService = encryptingService ?? throw new ArgumentNullException(nameof(encryptingService));
            this.tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await userRepository.GetByEmailAsync(email);
        }

        public async Task<Role> GetUserRole(string email)
        {
            User? user = await userRepository.GetByEmailAsync(email) ?? throw new Exception("User not found.");
            return user.Role;
        }

        public async Task<TokensGeneratedDTO> Login(UserLoginDTO userInformation)
        {
            User? user = await userRepository.GetByEmailAsync(userInformation.Email) ?? throw new Exception("User not found.");
            if (await encryptingService.CheckPasswordAsync(user.Password, userInformation.Password, user.Salt))
                return await tokenService.GenerateTokens(user.Email, user.Role);
            else
                throw new Exception("Invalid username or password.");

        }

        //Catch for errors, if you cant add a user.
        //For example: email exists it throws an error.
        public async Task RegisterUser(UserRegisterDTO user)
        {
            User userEntity = new()
            {
                Email = user.Email,
                Password = "",
                Role = Role.User

            };
            var hashedPassword = encryptingService.GenerateEncodedPassword(userEntity, user.Password);
            if (string.IsNullOrEmpty(hashedPassword))
            {
                throw new Exception("Failed to generate a hashed password. The resulting hash is null or empty.");
            }
            userEntity.Password = hashedPassword;
            await userRepository.AddAsync(userEntity);
        }

    }
}
