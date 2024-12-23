using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UserRepository
{
    public class UserRepositoryContext(DbContextOptions<UserRepositoryContext> options) : DbContext(options)
    {
        public required DbSet<User> Users { get; set; }
    }
}