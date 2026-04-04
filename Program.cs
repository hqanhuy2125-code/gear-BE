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
  options.AddPolicy("CustomerOnly", policy => 
    policy.RequireRole("customer", "user"));
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
builder.Services.AddScoped<IOrderService, OrderService>();
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
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();

    try
    {
        Console.WriteLine("[SEED] --- Database Startup Sequence Starting ---");
        
        // 0. Ensure Migrations
        dbContext.Database.Migrate();

        // 1. Seed Users (Admin & Owner)
        var usersToSeed = new List<User>
        {
            new User { Name = "Huy Admin", Email = "hqanhuy@gear.com", Password = BCrypt.Net.BCrypt.HashPassword("hqanhuy"), Role = "admin", CreatedAt = DateTime.UtcNow },
            new User { Name = "Owner Master", Email = "owner@gear.com", Password = BCrypt.Net.BCrypt.HashPassword("hqanhuy"), Role = "owner", CreatedAt = DateTime.UtcNow }
        };

        foreach (var u in usersToSeed)
        {
            var existing = dbContext.Users.FirstOrDefault(x => x.Email == u.Email);
            if (existing == null)
            {
                dbContext.Users.Add(u);
                Console.WriteLine($"[SEED] User registered: {u.Email} ({u.Role})");
            }
            else
            {
                existing.Password = u.Password; // Always reset password to ensure login works
                existing.Role = u.Role;
            }
        }
        dbContext.SaveChanges(); // Persist users so orders can link to them

        // 2. Seed Vouchers
        var vouchersToSeed = new List<Voucher>
        {
            new Voucher { Code = "SAVE10", Type = "percent", Value = 10, MaxUsages = 100, ExpiryDate = new DateTime(2026, 12, 31).ToUniversalTime(), CreatedAt = DateTime.UtcNow },
            new Voucher { Code = "SAVE20", Type = "percent", Value = 20, MaxUsages = 100, ExpiryDate = new DateTime(2026, 12, 31).ToUniversalTime(), CreatedAt = DateTime.UtcNow },
            new Voucher { Code = "FREESHIP", Type = "fixed", Value = 30000, MaxUsages = 200, ExpiryDate = new DateTime(2026, 12, 31).ToUniversalTime(), CreatedAt = DateTime.UtcNow },
            new Voucher { Code = "NEWUSER", Type = "percent", Value = 15, MaxUsages = 500, ExpiryDate = new DateTime(2026, 12, 31).ToUniversalTime(), CreatedAt = DateTime.UtcNow },
            new Voucher { Code = "VIP50", Type = "percent", Value = 50, MaxUsages = 50, ExpiryDate = new DateTime(2026, 12, 31).ToUniversalTime(), CreatedAt = DateTime.UtcNow },
            new Voucher { Code = "CODE10", Type = "percent", Value = 10, MaxUsages = 100, ExpiryDate = DateTime.UtcNow.AddMonths(1).ToUniversalTime(), CreatedAt = DateTime.UtcNow },
            new Voucher { Code = "CODE50", Type = "fixed", Value = 50000, MinOrderValue = 500000, MaxUsages = 50, ExpiryDate = DateTime.UtcNow.AddMonths(1).ToUniversalTime(), CreatedAt = DateTime.UtcNow }
        };

        foreach (var v in vouchersToSeed)
        {
            if (!dbContext.Vouchers.Any(x => x.Code == v.Code))
            {
                dbContext.Vouchers.Add(v);
                Console.WriteLine($"[SEED] Voucher created: {v.Code}");
            }
        }

        // 3. Seed Flash Sales
        var flashSalesToSeed = new List<FlashSale>
        {
            new FlashSale
            {
                Name = "Black Friday",
                Description = "Siêu sale lớn nhất năm",
                DiscountPercent = 35,
                StartTime = new DateTime(2026, 4, 4, 2, 0, 0).ToUniversalTime(),
                EndTime = new DateTime(2026, 4, 8, 12, 0, 0).ToUniversalTime(),
                ProductIds = "1,2",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new FlashSale
            {
                Name = "Summer Sale",
                Description = "Chào hè rực rỡ với ưu đãi khủng",
                DiscountPercent = 20,
                StartTime = new DateTime(2026, 5, 1, 0, 0, 0).ToUniversalTime(),
                EndTime = new DateTime(2026, 5, 5, 23, 59, 0).ToUniversalTime(),
                ProductIds = "3,4",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var fs in flashSalesToSeed)
        {
            if (!dbContext.FlashSales.Any(f => f.Name == fs.Name))
            {
                dbContext.FlashSales.Add(fs);
                Console.WriteLine($"[SEED] Flash Sale created: {fs.Name}");
            }
        }

        // 4. Seed System Config
        var configs = new List<SystemConfig>
        {
            new SystemConfig { Key = "ShippingFee_Inner", Value = "25000", Description = "Phí ship nội thành" },
            new SystemConfig { Key = "ShippingFee_Outer", Value = "40000", Description = "Phí ship ngoại tỉnh" }
        };

        foreach (var config in configs)
        {
            if (!dbContext.SystemConfigs.Any(c => c.Key == config.Key))
            {
                dbContext.SystemConfigs.Add(config);
            }
        }

        dbContext.SaveChanges();

        // 5. Seed Test Orders (Safe check)
        var testCustomer = dbContext.Users.FirstOrDefault(u => u.Email == "customer@gear.com");
        if (testCustomer != null && !dbContext.Orders.Any(o => o.FullName == "Huy Test Order"))
        {
            var o1 = new Order { UserId = testCustomer.Id, FullName = "Huy Test Order", PhoneNumber = "0123456789", ShippingAddress = "123 Main St", TotalPrice = 3500000, Status = "Pending", OrderCode = "ORD-TEST-001", CreatedAt = DateTime.UtcNow.AddHours(-10) };
            var o2 = new Order { UserId = testCustomer.Id, FullName = "Huy Test Order", PhoneNumber = "0123456789", ShippingAddress = "123 Main St", TotalPrice = 3200000, Status = "Confirmed", OrderCode = "ORD-TEST-002", CreatedAt = DateTime.UtcNow.AddHours(-8), ConfirmedAt = DateTime.UtcNow.AddHours(-7) };
            
            dbContext.Orders.AddRange(o1, o2);
            dbContext.SaveChanges();

            if (dbContext.Products.Any(p => p.Id == 1)) dbContext.OrderItems.Add(new OrderItem { OrderId = o1.Id, ProductId = 1, Quantity = 1, Price = 3500000 });
            if (dbContext.Products.Any(p => p.Id == 2)) dbContext.OrderItems.Add(new OrderItem { OrderId = o2.Id, ProductId = 2, Quantity = 1, Price = 3200000 });
            
            dbContext.SaveChanges();
            Console.WriteLine("[SEED] Sample orders created successfully.");
        }

        Console.WriteLine("[SEED] --- Database Startup Sequence Finished Successfully ---");
    }
    catch (Exception ex)
    {
        Console.WriteLine("==================================================================");
        Console.WriteLine("[FATAL SEED ERROR] The application encountered a database issue during startup.");
        Console.WriteLine($"Message: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        }
        Console.WriteLine("Seeding was aborted to prevent crash, but the application will continue.");
        Console.WriteLine("==================================================================");
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

// ===== ROLE ROUTE MIDDLEWARE =====
// Enforce strict route separation per role after JWT is validated
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower() ?? "";
    var user = context.User;

    if (user.Identity?.IsAuthenticated == true)
    {
        var role = user.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "";

        // Admin cannot access /owner/* routes
        if (path.StartsWith("/owner/") || path == "/owner")
        {
            if (role == "admin" || role == "customer")
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new 
                { 
                    message = $"Role '{role}' không có quyền truy cập vào khu vực Owner.",
                    redirectTo = role == "admin" ? "/admin/dashboard" : "/"
                });
                return;
            }
        }

        // Owner cannot access /admin/* routes
        if (path.StartsWith("/admin/") || path == "/admin")
        {
            if (role == "owner" || role == "customer")
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new 
                { 
                    message = $"Role '{role}' không có quyền truy cập vào khu vực Admin.",
                    redirectTo = role == "owner" ? "/owner/dashboard" : "/"
                });
                return;
            }
        }
    }

    await next();
});
// ===================================

app.MapControllers();
app.MapHub<OrderHub>("/hubs/order");
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
