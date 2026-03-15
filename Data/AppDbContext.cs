using GamingGearBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace GamingGearBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Driver> Drivers { get; set; }

        private static readonly DateTime SeedDate = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Utc);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Decimal precision for SQL Server
            modelBuilder.Entity<Order>().Property(o => o.TotalPrice).HasPrecision(18, 2);
            modelBuilder.Entity<OrderItem>().Property(oi => oi.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

            // Relationships / foreign keys
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Email as unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Admin user is seeded in Program.cs on first run (BCrypt hash is non-deterministic).

            // Seed Products (5 gaming gear)
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Logitech G Pro X Superlight", Description = "Chuột gaming siêu nhẹ", Price = 3500000, ImageUrl = "https://images.unsplash.com/photo-1593640408182-31c70c8268f5?auto=format&fit=crop&q=80&w=600", Category = "Mice", Stock = 50, CreatedAt = SeedDate },
                new Product { Id = 2, Name = "Razer Viper V2 Pro", Description = "Chuột gaming không dây", Price = 3200000, ImageUrl = "https://images.unsplash.com/photo-1615663245857-ac93bb552f41?auto=format&fit=crop&q=80&w=600", Category = "Mice", Stock = 30, CreatedAt = SeedDate },
                new Product { Id = 3, Name = "SteelSeries Apex Pro", Description = "Bàn phím cơ cao cấp switch OmniPoint", Price = 4500000, ImageUrl = "https://images.unsplash.com/photo-1595225476474-87563907a212?auto=format&fit=crop&q=80&w=600", Category = "Keyboards", Stock = 20, CreatedAt = SeedDate },
                new Product { Id = 4, Name = "Logitech G Pro Keyboard", Description = "Bàn phím TKL quốc dân", Price = 2800000, ImageUrl = "https://images.unsplash.com/photo-1618366712010-f4ae9c647dcb?auto=format&fit=crop&q=80&w=600", Category = "Keyboards", Stock = 40, CreatedAt = SeedDate },
                new Product { Id = 5, Name = "HyperX Cloud II", Description = "Tai nghe gaming 7.1", Price = 1800000, ImageUrl = "https://images.unsplash.com/photo-1618366712010-f4ae9c647dcb?auto=format&fit=crop&q=80&w=600", Category = "Headphones", Stock = 60, CreatedAt = SeedDate }
            );

            // Seed Blogs (2)
            modelBuilder.Entity<Blog>().HasData(
                new Blog { Id = 1, Title = "Top 5 Chuột Gaming Tốt Nhất 2025", Content = "Những mẫu chuột gaming được game thủ ưa chuộng nhất hiện nay...", ImageUrl = "https://images.unsplash.com/photo-1615663245857-ac93bb552f41?auto=format&fit=crop&q=80&w=800", Author = "Admin", CreatedAt = SeedDate },
                new Blog { Id = 2, Title = "Cách Chọn Bàn Phím Cơ Phù Hợp Cho Game Thủ", Content = "Hướng dẫn chọn switch, layout và độ bền cho bàn phím cơ...", ImageUrl = "https://images.unsplash.com/photo-1595225476474-87563907a212?auto=format&fit=crop&q=80&w=800", Author = "Admin", CreatedAt = SeedDate }
            );

            // Seed Drivers (2)
            modelBuilder.Entity<Driver>().HasData(
                new Driver { Id = 1, Name = "MADLIONS Gaming Mouse Driver", Description = "Phần mềm tùy chỉnh DPI và Macro", DownloadUrl = "#", Brand = "MADLIONS" },
                new Driver { Id = 2, Name = "MADLIONS Mechanical Keyboard Driver", Description = "Tùy chỉnh LED RGB và phím", DownloadUrl = "#", Brand = "MADLIONS" }
            );
        }
    }
}
