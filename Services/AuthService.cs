using GamingGearBackend.DTOs;
using GamingGearBackend.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth;

namespace GamingGearBackend.Services
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(RegisterDto dto);
        Task<(UserDto user, string refreshToken)> LoginAsync(LoginDto dto);
        Task<GoogleLoginResponseDto> GoogleLoginAsync(string credential);
        Task<(UserDto userDto, string refreshToken)> RefreshTokenAsync(string token);
        Task RevokeTokenAsync(string token);
        Task SendOtpAsync(string email);
        Task<(UserDto user, string refreshToken)> VerifyOtpAsync(string email, string code, string? name = null);
    }

    public class AuthService : IAuthService
    {
        private readonly Data.AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public AuthService(Data.AppDbContext context, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                throw new Exception("Email đã tồn tại trong hệ thống.");
            }

            var newUser = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "customer",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Email = newUser.Email,
                Role = newUser.Role
            };
        }

        public async Task<(UserDto user, string refreshToken)> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                throw new Exception("Email hoặc mật khẩu không chính xác.");
            }

            if (user.Role == "admin_blocked")
            {
                throw new Exception("Tài khoản của bạn đã bị khóa.");
            }

            var refreshToken = GenerateRefreshToken(user.Id);
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = GenerateJwtToken(user)
            };

            return (userDto, refreshToken.Token);
        }

        public async Task<GoogleLoginResponseDto> GoogleLoginAsync(string credential)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings();

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Xác thực token Google thất bại: " + ex.Message);
            }

            // Generate and send OTP
            await SendOtpAsync(payload.Email);

            return new GoogleLoginResponseDto
            {
                Email = payload.Email,
                Name = payload.Name,
                Message = "Mã OTP đã được gửi"
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _config["Jwt:Key"] ?? "SUPER_SECRET_KEY_1234567890_!@#$%";
            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15), // Access token 15 mins
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(int userId)
        {
            var randomBytes = new byte[64];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<(UserDto userDto, string refreshToken)> RefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens
                                        .FirstOrDefaultAsync(r => r.Token == token);
            if (refreshToken == null) throw new Exception("Invalid refresh token");

            var user = await _context.Users.FindAsync(refreshToken.UserId);
            if (user == null) throw new Exception("User not found");

            if (refreshToken.IsRevoked)
            {
                // Reuse attack detected -> revoke all tokens
                var allTokens = _context.RefreshTokens.Where(r => r.UserId == refreshToken.UserId);
                foreach (var t in allTokens) { t.IsRevoked = true; t.RevokedAt = DateTime.UtcNow; t.RevokedReason = "Reuse Attack"; }
                await _context.SaveChangesAsync();
                throw new Exception("Reuse attack detected. All tokens revoked.");
            }

            if (DateTime.UtcNow > refreshToken.ExpiresAt) throw new Exception("Refresh token expired");

            // Rotate
            var newRefreshToken = GenerateRefreshToken(refreshToken.UserId);
            
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            refreshToken.RevokedReason = "Rotation";
            
            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = GenerateJwtToken(user)
            };

            return (userDto, newRefreshToken.Token);
        }

        public async Task RevokeTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
            if (refreshToken == null) return;

            if (!refreshToken.IsRevoked)
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAt = DateTime.UtcNow;
                refreshToken.RevokedReason = "Logout";
                await _context.SaveChangesAsync();
            }
        }

        public async Task SendOtpAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return;
            var cleanEmail = email.Trim().ToLower();

            // Invalidate old OTPs for this email
            var oldOtps = await _context.OtpCodes
                .Where(o => o.Email.ToLower() == cleanEmail && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var otp in oldOtps)
            {
                otp.IsUsed = true;
            }

            // Generate 6-digit code
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            var newOtp = new OtpCode
            {
                Email = cleanEmail,
                Code = code,
                ExpiresAt = DateTime.UtcNow.AddSeconds(30),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.OtpCodes.Add(newOtp);
            await _context.SaveChangesAsync();

            // Send Email
            var subject = "Mã xác nhận đăng nhập OTP";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #4f46e5;'>Mã Xác Nhận Đăng Nhập</h2>
                    <p>Chào bạn,</p>
                    <p>Bạn đã yêu cầu đăng nhập bằng mã OTP. Dưới đây là mã xác nhận của bạn:</p>
                    <div style='background-color: #f3f4f6; padding: 20px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; color: #1f2937; border-radius: 8px; margin: 20px 0;'>
                        {code}
                    </div>
                    <p style='color: #ef4444; font-size: 14px;'>🚫 Mã này sẽ hết hạn sau 30 giây và chỉ được dùng 1 lần.</p>
                    <p style='color: #6b7280; font-size: 12px; margin-top: 30px;'>Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email.</p>
                </div>";

            await _emailService.SendEmailAsync(email, subject, body);
        }

        public async Task<(UserDto user, string refreshToken)> VerifyOtpAsync(string email, string code, string? name = null)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
                throw new Exception("Email hoặc mã OTP không hợp lệ.");

            var cleanEmail = email.Trim().ToLower();
            var cleanCode = code.Trim().ToLower();

            var otpRecord = await _context.OtpCodes
                .Where(o => !o.IsUsed && o.Email.ToLower() == cleanEmail && o.Code.ToLower() == cleanCode)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otpRecord == null)
            {
                throw new Exception("Mã OTP không hợp lệ.");
            }

            var now = DateTime.UtcNow;
            if (otpRecord.ExpiresAt < now)
            {
                otpRecord.IsUsed = true;
                await _context.SaveChangesAsync();
                throw new Exception("Mã OTP đã hết hạn.");
            }

            // Valid -> mark used
            otpRecord.IsUsed = true;
            
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == cleanEmail);
            if (user == null)
            {
                user = new User
                {
                    Name = string.IsNullOrWhiteSpace(name) ? cleanEmail.Split('@')[0] : name.Trim(),
                    Email = cleanEmail,
                    Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), 
                    Role = "customer",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            if (user.Role == "admin_blocked")
                throw new Exception("Tài khoản của bạn đã bị khóa.");

            var refreshToken = GenerateRefreshToken(user.Id);
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = GenerateJwtToken(user)
            };

            return (userDto, refreshToken.Token);
        }
    }
}
