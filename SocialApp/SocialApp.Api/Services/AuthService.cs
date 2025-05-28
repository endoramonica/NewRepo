using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialApp.Api.Data;
using SocialApp.Api.Data.Entities;
using SocialAppLibrary.Shared.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialApp.Api.Services
{
    public class AuthService
    {
        private readonly DataContext _dataContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly PhotoUploadService _photoUploadService;
        private readonly IConfiguration _configuration;

        // Constructor: Inject các dịch vụ cần thiết
        public AuthService(DataContext dataContext, IPasswordHasher<User> passwordHasher,
            PhotoUploadService photoUploadService, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _passwordHasher = passwordHasher;
            _photoUploadService = photoUploadService;
            _configuration = configuration;
        }

        // Đăng ký tài khoản mới
        public async Task<ApiResult<Guid>> RegisterAsync(RegisterDto dto)
        {
            // Kiểm tra xem email đã tồn tại chưa
            if (await _dataContext.Users.AnyAsync(u => u.Email == dto.Email))
            {
                return ApiResult<Guid>.Fail("User already exists!");
            }

            try
            {
                // Tạo user mới
                var user = new User
                {
                    Email = dto.Email,
                    Name = dto.Name
                };

                // Hash password trước khi lưu
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

                // Lưu vào database
                _dataContext.Users.Add(user);
                await _dataContext.SaveChangesAsync();

                return ApiResult<Guid>.Success(user.Id);
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database Save Error: {ex.InnerException?.Message}");
                throw;
            }
        }

        // Upload ảnh đại diện
        public async Task<ApiResult<string>> UploadPhotoAsync(Guid userId, IFormFile photo)
        {
            try
            {
                // Tìm user theo userId
                var user = await _dataContext.Users.FindAsync(userId);
                if (user == null)
                    return ApiResult<string>.Fail("User not found");

                // Lưu ảnh vào hệ thống
                var (photoPath, photoUrl) = await _photoUploadService.SavePhotoAsync(photo, "uploads", "images", "users");
                user.PhotoPath = photoPath;
                user.PhotoUrl = photoUrl;

                // Cập nhật user với ảnh mới
                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();

                return ApiResult<string>.Success(photoUrl);
            }
            catch (Exception ex)
            {
                return ApiResult<string>.Fail(ex.Message);
            }
        }

        
      

        // Xử lý đăng nhập
        public async Task<ApiResult<LoginResponse>> LoginAsync(LoginDto dto)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            // Nếu user không tồn tại, tạo delay để tránh brute-force
            if (user == null)
            {
                await Task.Delay(Random.Shared.Next(500, 1500));
                return ApiResult<LoginResponse>.Fail("Invalid login attempt");
            }

            // Kiểm tra mật khẩu
            var passwordVerification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (passwordVerification == PasswordVerificationResult.Failed)
            {
                await Task.Delay(Random.Shared.Next(500, 1500));
                return ApiResult<LoginResponse>.Fail("Invalid login attempt");
            }

            // Tạo JWT Token
            var jwt = GenerateJwtToken(user);
            var loggedInUser = new LoggedInUser(user.Id, user.Name, user.Email, user.PhotoUrl);
            var loginResponse = new LoginResponse(loggedInUser, jwt);

            return ApiResult<LoginResponse>.Success(loginResponse);
        }

        // Sinh JWT Token
        private string GenerateJwtToken(User user)
        {
            var secretKey = _configuration.GetValue<string>("Jwt:SecretKey");
            var issuer = _configuration.GetValue<string>("Jwt:Issuer");
            var expiryMinutes = _configuration.GetValue<int>("Jwt:ExpireInMinutes");

            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(issuer) || expiryMinutes <= 0)
            {
                throw new InvalidOperationException("JWT configuration is missing or invalid.");
            }

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserPhotoUrl", user.PhotoUrl ?? "")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }





        public async Task<ApiResult<UserDto>> AuthenticateAsync(string email, string password)
        {
            try
            {
                var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    await Task.Delay(Random.Shared.Next(500, 1500));
                    return ApiResult<UserDto>.Fail("Invalid login attempt");
                }

                var passwordVerification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if (passwordVerification == PasswordVerificationResult.Failed)
                {
                    await Task.Delay(Random.Shared.Next(500, 1500));
                    return ApiResult<UserDto>.Fail("Invalid login attempt");
                }

                var token = GenerateJwtToken(user);
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhotoUrl = user.PhotoUrl,
                    IsOnline = user.IsOnline,
                    IsAway = !user.IsOnline && user.LastLogonTime.HasValue,
                    AwayDuration = !user.IsOnline && user.LastLogonTime.HasValue ? Utilities.CalcAwayDuration(user.LastLogonTime.Value) : null,
                    LastLogonTime = user.LastLogonTime,
                    //Token = token // Add token for client use
                };

                return ApiResult<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                return ApiResult<UserDto>.Fail(ex.Message);
            }
        }

        public async Task<ApiResult<UserDto>> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _dataContext.Users.FindAsync(id);
                if (user == null)
                    return ApiResult<UserDto>.Fail("User not found");

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhotoUrl = user.PhotoUrl,
                    IsOnline = user.IsOnline,
                    IsAway = !user.IsOnline && user.LastLogonTime.HasValue,
                    AwayDuration = !user.IsOnline && user.LastLogonTime.HasValue ? Utilities.CalcAwayDuration(user.LastLogonTime.Value) : null,
                    LastLogonTime = user.LastLogonTime
                };

                return ApiResult<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                return ApiResult<UserDto>.Fail(ex.Message);
            }
        }

        public static class Utilities
        {
            public static string CalcAwayDuration(DateTime? lastLogonTime)
            {
                if (!lastLogonTime.HasValue) return "";
                var duration = DateTime.UtcNow - lastLogonTime.Value;
                return duration.TotalMinutes < 60
                    ? $"{(int)duration.TotalMinutes} minutes"
                    : duration.TotalHours < 24
                        ? $"{(int)duration.TotalHours} hours"
                        : $"{(int)duration.TotalDays} days";
            }
        }
    }
}
