using System.Security.Cryptography;
using System.Text;
using Application.UserService;
using Domain.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Options;

namespace Application.EncryptingService
{
    public class SaltAndPepperService(IOptions<SaltAndPepperSettings> options) : IEncryptingService
    {
        private readonly SaltAndPepperSettings _settings = options.Value;
        private bool _found = false; // Shared flag to indicate if a match is found
        private readonly object _lock = new object(); // Lock to synchronize access to _found



        public string GenerateEncodedPassword(User user, string password)
        {
            // Step 1: Generate a random salt using SaltLettersLength
            var salt = GenerateRandomSalt(_settings.SaltLettersLength);
            user.Salt = salt;
            // Step 2: Generate a random salt using SaltLettersLength
            var pepper = GenerateRandomPepper();

            // Step 3: Create the hash password using the salt and pepper
            var hashedPassword = CreateHashPassword(password, salt, pepper);
            return hashedPassword;
        }

        public async Task<bool> CheckPasswordAsync(string hashedPassword, string password, string salt)
        {
            int totalCombinations = CalculateTotalCombinations();
            int combinationsPerTask = totalCombinations / _settings.PepperLength;

            var tasks = new Task[_settings.PepperLength];

            for (int i = 0; i < _settings.PepperLength; i++)
            {
                int start = i * combinationsPerTask;
                int end = (i == _settings.PepperLength - 1) ? totalCombinations : start + combinationsPerTask;

                tasks[i] = Task.Run(() => ProcessRange(start, end, hashedPassword, password, salt));
            }

            await Task.WhenAll(tasks);

            return _found;
        }


        // Method to generate a random salt of the specified length
        private string GenerateRandomSalt(int length)
        {
            var random = new Random();
            var salt = new string(Enumerable.Range(0, length)
                .Select(_ => _settings.SaltLetters[random.Next(_settings.SaltLetters.Length)])
                .ToArray());
            return salt;
        }

        // Method to generate a pepper string of the specified length
        private string GenerateRandomPepper()
        {
            var random = new Random();
            var pepper = new string(Enumerable.Range(0, _settings.PepperLength)
                .Select(_ => _settings.PepperLetters[random.Next(_settings.PepperLetters.Length)])
                .ToArray());
            return pepper;
        }

        // Method to hash the final salted and peppered password using SHA-256
        private string HashPassword(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashedBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashedBytes);
        }

        private string CreateHashPassword(string password, string salt, string pepper)
        {
            // Step 3: Add the salt and pepper to the password
            var saltedPepperedPassword = $"{salt}{password}{pepper}";
            return HashPassword(saltedPepperedPassword);
        }

        private int CalculateTotalCombinations()
        {
            return (int)Math.Pow(_settings.PepperLetters.Length, _settings.PepperLength);
        }

        private void ProcessRange(int start, int end, string hashedPassword, string password, string salt)
        {
            for (int i = start; i < end; i++)
            {
                if (_found)
                    return;

                string pepper = GeneratePepperFromIndex(i);
                if (CheckMatch(pepper, password, hashedPassword, salt))
                    return;
            }
        }

        private string GeneratePepperFromIndex(int index)
        {
            char[] combination = new char[_settings.PepperLength];
            int n = _settings.PepperLetters.Length;
            int temp = index;

            for (int j = _settings.PepperLength - 1; j >= 0; j--)
            {
                combination[j] = _settings.PepperLetters[temp % n];
                temp /= n;
            }

            return new string(combination);
        }

        private bool CheckMatch(string pepper, string password, string hashedPassword, string salt)
        {
            var combinedWord = CreateHashPassword(password, salt, pepper);

            if (combinedWord == hashedPassword)
            {
                MarkAsFound();
                return true;
            }

            return false;
        }

        private void MarkAsFound()
        {
            lock (_lock)
            {
                _found = true;
            }
        }

    }
}