using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Infrastructure.UserRepository;
using System;

namespace RunMigrationsApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Manually configure without file watching or reloading
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                          .AddEnvironmentVariables();
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
                        ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

                    services.AddDbContext<UserRepositoryContext>(options =>
         options.UseNpgsql(connectionString,
             sqlOptions => sqlOptions.MigrationsAssembly("Infrastructure")));

                    services.AddScoped<IUserRepositoryService, UserRepositoryService>();
                })
                .Build();

            var scope = host.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UserRepositoryContext>();

            // Apply migrations
            dbContext.Database.Migrate();
        }
    }
}
