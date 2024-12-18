var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "This is a sample API using Swagger",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Your Name",
            Email = "your-email@example.com",
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();                        // Enable Swagger middleware
    app.UseSwaggerUI();                     // Enable Swagger UI
}

app.UseHttpsRedirection();

app.Run();
