using Domain.DTOs;
using Domain.Entities;

namespace Application.EncryptingService
{
    public interface IEncryptingService
    {
        string GenerateEncodedPassword(User user, string password);
        Task<bool> CheckPasswordAsync(string hashedPassword, string password, string salt);
    }
}