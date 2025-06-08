using SocialApp.Api.Services;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.Dtos.ChatDto;
using System.Security.Claims;
//#22

namespace SocialApp.Api.Endpoint
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var authGroup = app.MapGroup("/api/auth")
                                .WithTags("Auth");
            authGroup.MapPost("/register", async (RegisterDto dto, AuthService authService) =>
                            Results.Ok(await authService.RegisterAsync(dto)))
                            .Produces<ApiResult<Guid>>()
                           .WithName("Auth-Register");
            /* Đoạn code này tạo một endpoint /register cho phép người dùng đăng ký tài khoản.
             * Khi một request POST được gửi đến endpoint này,
             * nó lấy dữ liệu từ request body (trong đối tượng RegisterDto), 
             * gọi một service (AuthService) để xử lý việc đăng ký,
             * và trả về một response thành công (200 OK) chứa thông tin về người dùng vừa được đăng ký. 
             * ApiResult<Guid> cho biết response sẽ trả về một ID duy nhất cho người dùng vừa tạo trong một format chuẩn. */
            authGroup.MapPost("/login", async (LoginDto dto, AuthService authService) =>
                            Results.Ok(await authService.LoginAsync(dto)))
                            .Produces<ApiResult<LoginResponse>>()
                            .WithName("Auth-Login");

            authGroup.MapPost("/register/{userId:guid}/upload-photo", 
                async (Guid userId, IFormFile photo, AuthService authService) =>
            {
                
                var result = await authService.UploadPhotoAsync(userId, photo);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
                    .DisableAntiforgery()
                    .Produces<ApiResult<string>>()
                    .WithName("User-UploadPhoto");
            // Xác thực người dùng
            authGroup.MapPost("/authenticate", async (LoginDto dto, AuthService authService) =>
            {
                var result = await authService.AuthenticateAsync(dto.Email, dto.Password);
                return result.IsSuccess ? Results.Ok(result) : Results.Unauthorized();
            })
            .Produces<ApiResult<UserDto>>()
            .WithName("Auth-Authenticate")
            .AllowAnonymous();
            /* Tạo endpoint POST /api/auth/authenticate để xác thực người dùng.
             * Nhận LoginDto từ body, gọi IAuthService.AuthenticateAsync,
             * trả về UserDto trong ApiResult<UserDto> nếu thành công, hoặc lỗi (401) nếu thất bại.
             * Cho phép truy cập ẩn danh. */
            // Lấy thông tin người dùng theo ID
            authGroup.MapGet("/{userId:guid}", async (Guid userId, AuthService authService) =>
            {
                var result = await authService.GetUserByIdAsync(userId);
                return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
            })
            .Produces<ApiResult<UserDto>>()
            .WithName("Auth-GetUserById")
            .RequireAuthorization();
            /* Tạo endpoint GET /api/auth/{userId} để lấy thông tin người dùng.
             * Nhận userId từ URL, gọi IAuthService.GetUserByIdAsync,
             * trả về UserDto trong ApiResult<UserDto> nếu thành công, hoặc lỗi (400) nếu thất bại.
             * Yêu cầu xác thực. */

            return app;
        }

        
    }
}
