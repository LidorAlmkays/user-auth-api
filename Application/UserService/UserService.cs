using Application.EncryptingService;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.UserRepository;

namespace Application.UserService
{
    public class UserService(IUserRepositoryService userRepository, IEncryptingService encryptingService) : IUserService
    {
        private readonly IUserRepositoryService userRepository = userRepository;
        private readonly IEncryptingService encryptingService = encryptingService;


        public async Task<User?> GetUserByEmail(string email)
        {
            return await userRepository.GetByEmailAsync(email);
        }

        //Catch for errors, if you cant add a user.
        //For example: email exists it throws an error.
        public async Task RegisterUser(UserRegisterDTO user)
        {
            User userEntity = new()
            {
                Email = user.Email,
                Password = ""

            };
            var hashedPassword = encryptingService.GenerateEncodedPassword(userEntity, user.Password);
            if (string.IsNullOrEmpty(hashedPassword))
            {
                throw new Exception("Failed to generate a hashed password. The resulting hash is null or empty.");
            }
            userEntity.Password = hashedPassword;
            await userRepository.AddAsync(userEntity);
        }

        public async Task<bool> AuthenticateUser(string email, string password)
        {
            User? user = await GetUserByEmail(email);
            if (user == null || user.Salt == null || string.IsNullOrEmpty(user.Password))
                return false;

            return await encryptingService.CheckPasswordAsync(user.Password, password, user.Salt);
        }

    }
}
