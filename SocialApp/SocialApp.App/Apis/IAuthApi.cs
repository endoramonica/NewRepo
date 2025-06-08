using Microsoft.AspNetCore.Http;
using Refit;
using SocialAppLibrary.Shared.Dtos;
using SocialAppLibrary.Shared.Dtos.ChatDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialApp.App.Apis;

public interface IAuthApi
{
    [Multipart]
    [Post("/api/auth/register/{userId}/upload-photo")]
    Task<ApiResult<string>> UploadPhotoAsync(Guid userId, StreamPart photo);
    [Post("/api/auth/register")]
    Task<ApiResult<Guid>> RegisterAsync(RegisterDto dto);
    [Post("/api/auth/login")]
    Task<ApiResult<LoginResponse>> LoginAsync(LoginDto dto);


    Task<ApiResult<UserDto>> AuthenticateAsync(string email, string password);
    Task<ApiResult<UserDto>> GetUserByIdAsync(Guid id);

}
