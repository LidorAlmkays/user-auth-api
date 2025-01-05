using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UserRepository
{
    public class UserRepositoryService(UserRepositoryContext userRepository) : IUserRepositoryService
    {

        private readonly UserRepositoryContext _context = userRepository;
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(User user)
        {
            var existingUser = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (existingUser)
            {
                throw new InvalidOperationException($"A user with the email '{user.Email}' already exists.");
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email) ?? throw new KeyNotFoundException($"No user found with the email '{user.Email}'.");
            _context.Entry(existingUser).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"No user found with the email '{email}'.");
            }
        }
    }
}