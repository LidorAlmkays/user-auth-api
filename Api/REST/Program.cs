using Infrastructure.UserRepository;
using Microsoft.EntityFrameworkCore;
using Application.UserService;
using REST.Middlewares;
using Application.EncryptingService;
using Application.TokenService;

var builder = WebApplication.CreateBuilder(args);

// Configuration setup
ConfigureConfiguration(builder);

// Services setup
ConfigureServices(builder);

// Swagger setup
ConfigureSwagger(builder);

// Build the application
var app = builder.Build();

// Configure HTTP request pipeline
ConfigurePipeline(app);

app.Run();

void ConfigureConfiguration(WebApplicationBuilder builder)
{
    builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();
}

void ConfigureServices(WebApplicationBuilder builder)
{
    // Retrieve the connection string
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                         ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

    // Configure settings
    builder.Services.Configure<SaltAndPepperSettings>(builder.Configuration.GetSection("SaltAndPepperSettings"));
    builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));

    // Configure DbContext
    builder.Services.AddDbContext<UserRepositoryContext>(options =>
        options.UseNpgsql(connectionString));

    // Add scoped services
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITokenService, JwtService>();
    builder.Services.AddScoped<IEncryptingService, SaltAndPepperService>();
    builder.Services.AddScoped<IUserRepositoryService, UserRepositoryService>();

    // Add controllers with validation filter
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    });
}

void ConfigureSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "User auth API",
            Version = "v1",
            Description = "This is a sample API using Swagger",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Lidor",
                Email = "your-email@example.com",
            }
        });
    });
}

void ConfigurePipeline(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();                        // Enable Swagger middleware
        app.UseSwaggerUI();                     // Enable Swagger UI
    }

    app.UseHttpsRedirection();
    app.MapControllers();
}
