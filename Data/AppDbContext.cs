using System;
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
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }
        public DbSet<FlashSale> FlashSales { get; set; }
        public DbSet<SystemConfig> SystemConfigs { get; set; }
        public DbSet<PaymentLog> PaymentLogs { get; set; }

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

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Email as unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Admin user is seeded in Program.cs on first run (BCrypt hash is non-deterministic).

            // Seed Products (5 gaming gear)
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Logitech G Pro X Superlight", Description = "Chuột gaming siêu nhẹ", Price = 3500000, ImageUrl = "https://images.unsplash.com/photo-1593640408182-31c70c8268f5?auto=format&fit=crop&q=80&w=600", Category = "Mice", Stock = 50, CreatedAt = SeedDate },
                new Product { Id = 2, Name = "Razer Viper V2 Pro", Description = "Chuột gaming không dây", Price = 3200000, ImageUrl = "https://images.unsplash.com/photo-1615663245857-ac93bb552f41?auto=format&fit=crop&q=80&w=600", Category = "Mice", Stock = 3, CreatedAt = SeedDate },
                new Product { Id = 3, Name = "SteelSeries Apex Pro", Description = "Bàn phím cơ cao cấp switch OmniPoint", Price = 4500000, ImageUrl = "https://images.unsplash.com/photo-1595225476474-87563907a212?auto=format&fit=crop&q=80&w=600", Category = "Keyboards", Stock = 20, CreatedAt = SeedDate, IsPreOrder = true, PreOrderDate = SeedDate.AddMonths(1) },
                new Product { Id = 4, Name = "Logitech G Pro Keyboard", Description = "Bàn phím TKL quốc dân", Price = 2800000, ImageUrl = "https://images.unsplash.com/photo-1618366712010-f4ae9c647dcb?auto=format&fit=crop&q=80&w=600", Category = "Keyboards", Stock = 40, CreatedAt = SeedDate },
                new Product { Id = 5, Name = "HyperX Cloud II", Description = "Tai nghe gaming 7.1", Price = 1800000, ImageUrl = "https://images.unsplash.com/photo-1618366712010-f4ae9c647dcb?auto=format&fit=crop&q=80&w=600", Category = "Headphones", Stock = 0, CreatedAt = SeedDate, IsOrderOnly = true }
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

            // Notifications & Wishlist Relationships
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<WishlistItem>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId);

            modelBuilder.Entity<WishlistItem>()
                .HasOne(w => w.Product)
                .WithMany()
                .HasForeignKey(w => w.ProductId);

            // ChatMessages Relationships
            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Sender)
                .WithMany()
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Receiver)
                .WithMany()
                .HasForeignKey(cm => cm.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Performance Indexes
            modelBuilder.Entity<Product>().HasIndex(p => p.Category);
            modelBuilder.Entity<Order>().HasIndex(o => o.UserId);
            modelBuilder.Entity<Order>().HasIndex(o => o.Status);
            modelBuilder.Entity<Order>().HasIndex(o => o.OrderCode).IsUnique();
            modelBuilder.Entity<CartItem>().HasIndex(ci => ci.UserId);
            
            // Decimal precision for Promotion
            modelBuilder.Entity<Promotion>().Property(p => p.MinOrderValue).HasPrecision(18, 2);
            modelBuilder.Entity<Promotion>().Property(p => p.MaxDiscount).HasPrecision(18, 2);

            // Decimal precision for FlashSale
            modelBuilder.Entity<FlashSale>().Property(f => f.DiscountPercent).HasPrecision(5, 2);

            // Seed default SystemConfig
            modelBuilder.Entity<SystemConfig>().HasData(
                new SystemConfig { Id = 1, Key = "shipping_fee", Value = "30000", Description = "Phí vận chuyển mặc định (VNĐ)", UpdatedAt = SeedDate },
                new SystemConfig { Id = 2, Key = "free_shipping_threshold", Value = "500000", Description = "Đơn hàng từ giá trị này được miễn phí ship (VNĐ)", UpdatedAt = SeedDate },
                new SystemConfig { Id = 3, Key = "return_policy_days", Value = "7", Description = "Số ngày được phép đổi trả hàng", UpdatedAt = SeedDate },
                new SystemConfig { Id = 4, Key = "return_policy_note", Value = "Sản phẩm cần còn nguyên tem, hộp và chưa qua sử dụng", Description = "Điều kiện đổi trả hàng", UpdatedAt = SeedDate },
                new SystemConfig { Id = 5, Key = "site_name", Value = "SCYTOL CLX21", Description = "Tên hiển thị của website", UpdatedAt = SeedDate }
            );
        }
    }
}
