using Infrastructure.UserRepository;
using Microsoft.EntityFrameworkCore;
using Application.UserService;
using REST.middlewares;
using Application.EncryptingService;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Retrieve the connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                     ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION");



// Configure services
builder.Services.AddDbContext<UserRepositoryContext>(options =>
    options.UseNpgsql(connectionString));  // Use Npgsql for PostgreSQL


builder.Services.Configure<SaltAndPepperSettings>(builder.Configuration.GetSection("SaltAndPepperSettings"));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEncryptingService, SaltAndPepperService>();
builder.Services.AddScoped<IUserRepositoryService, UserRepositoryService>();


builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "User auth api",
        Version = "v1",
        Description = "This is a sample API using Swagger",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Lidor",
            Email = "your-email@example.com",
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();                        // Enable Swagger middleware
    app.UseSwaggerUI();                     // Enable Swagger UI
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
