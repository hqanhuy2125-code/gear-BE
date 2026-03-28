using Microsoft.EntityFrameworkCore;
using GamingGearBackend.Data;
using GamingGearBackend.Services;
using GamingGearBackend.Models;
using GamingGearBackend.Hubs;
using BCrypt.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using GamingGearBackend.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddResponseCaching();
builder.Services.AddRateLimiter(options => {
    options.AddFixedWindowLimiter("fixed", o => {
      o.PermitLimit = 100;
      o.Window = TimeSpan.FromMinutes(1);
      o.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
      o.QueueLimit = 0;
    });
    options.RejectionStatusCode = 429;
});

// Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration
        .GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:5174")
            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS") // OPTIONS is generally needed for CORS preflight
            .WithHeaders("Content-Type", "Authorization", "x-requested-with", "x-signalr-user-agent")
            .AllowCredentials(); // Required for SignalR
    });
});

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SUPER_SECRET_KEY_1234567890_!@#$%";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options => {
  options.AddPolicy("OwnerOnly", policy => 
    policy.RequireRole("owner"));
  options.AddPolicy("AdminOrOwner", policy => 
    policy.RequireRole("admin", "owner"));
  options.AddPolicy("AllUsers", policy => 
    policy.RequireAuthenticatedUser());
});

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Scoped Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ScheduledJobs>();

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
            Name = "Huy Admin",
            Email = "hqanhuy@gear.com",
            Password = BCrypt.Net.BCrypt.HashPassword("hqanhuy"),
            Role = "admin",
            CreatedAt = DateTime.UtcNow
        });
    }

    // Seed default owner user
    if (!dbContext.Users.Any(u => u.Email == "owner@gear.com"))
    {
        dbContext.Users.Add(new User
        {
            Name = "Owner Master",
            Email = "owner@gear.com",
            Password = BCrypt.Net.BCrypt.HashPassword("owner123"),
            Role = "owner",
            CreatedAt = DateTime.UtcNow
        });
    }
    dbContext.SaveChanges();

    // Seed sample orders for testing lifecycle
    if (!dbContext.Orders.Any(o => o.FullName == "Huy Test Order"))
    {
        var testUser = dbContext.Users.FirstOrDefault(u => u.Email == "hqanhuy@gear.com");
        if (testUser != null)
        {
            var userId = testUser.Id;

            // 1. Pending Order
            var order1 = new Order
            {
                UserId = userId,
                FullName = "Huy Test Order",
                PhoneNumber = "0123456789",
                ShippingAddress = "123 Main St",
                TotalPrice = 3500000,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow.AddHours(-10)
            };
            dbContext.Orders.Add(order1);

            // 2. Confirmed Order
            var order2 = new Order
            {
                UserId = userId,
                FullName = "Huy Test Order",
                PhoneNumber = "0123456789",
                ShippingAddress = "123 Main St",
                TotalPrice = 3200000,
                Status = "Confirmed",
                CreatedAt = DateTime.UtcNow.AddHours(-8),
                ConfirmedAt = DateTime.UtcNow.AddHours(-7)
            };
            dbContext.Orders.Add(order2);

            // 3. Shipping Order
            var order3 = new Order
            {
                UserId = userId,
                FullName = "Huy Test Order",
                PhoneNumber = "0123456789",
                ShippingAddress = "123 Main St",
                TotalPrice = 4500000,
                Status = "Shipping",
                CreatedAt = DateTime.UtcNow.AddHours(-6),
                ConfirmedAt = DateTime.UtcNow.AddHours(-5),
                ShippingAt = DateTime.UtcNow.AddHours(-4)
            };
            dbContext.Orders.Add(order3);

            dbContext.SaveChanges();

            // Add OrderItems
            dbContext.OrderItems.Add(new OrderItem { OrderId = order1.Id, ProductId = 1, Quantity = 1, Price = 3500000 });
            dbContext.OrderItems.Add(new OrderItem { OrderId = order2.Id, ProductId = 2, Quantity = 1, Price = 3200000 });
            dbContext.OrderItems.Add(new OrderItem { OrderId = order3.Id, ProductId = 3, Quantity = 1, Price = 4500000 });
            
            dbContext.SaveChanges();
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseRouting();
app.UseRateLimiter();
app.UseResponseCaching();

app.UseHangfireDashboard("/hangfire");

// Kiểm tra đơn hàng quá hạn mỗi 5 phút
RecurringJob.AddOrUpdate<ScheduledJobs>(
    "cancel-expired-orders",
    job => job.CancelExpiredOrders(),
    "*/5 * * * *");

// Dọn token hết hạn mỗi ngày
RecurringJob.AddOrUpdate<ScheduledJobs>(
    "clean-expired-tokens", 
    job => job.CleanExpiredTokens(),
    Cron.Daily);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<OrderHub>("/hubs/order");
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
