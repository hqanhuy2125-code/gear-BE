using Microsoft.EntityFrameworkCore;
using GamingGearBackend.Data;
using GamingGearBackend.Services;
using GamingGearBackend.Models;
using BCrypt.Net;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Scoped Services
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure VNPay Services
builder.Services.Configure<VnPaySettings>(builder.Configuration.GetSection("VnPay"));
builder.Services.AddScoped<IVnPayService, VnPayService>();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Automatically apply pending migrations and create the database if it doesn't exist.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    // Seed default admin user
    if (!dbContext.Users.Any(u => u.Email == "hqanhuy@gear.com"))
    {
        dbContext.Users.Add(new User
        {
            Name = "Huy",
            Email = "hqanhuy@gear.com",
            Password = BCrypt.Net.BCrypt.HashPassword("hqanhuy"),
            Role = "admin",
            CreatedAt = DateTime.UtcNow
        });
        dbContext.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();

app.Run();
