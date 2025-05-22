using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialApp.Api.Data;
using SocialApp.Api.Data.Entities;
using SocialApp.Api.Services;

using Microsoft.OpenApi.Models;
using SocialApp.Api.Endpoint;
using SocialAppLibrary.Shared;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
// Thêm Swagger vào DI container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddOpenApi();


var connectionString = builder.Configuration.GetConnectionString("SocialConnection");
builder.Services.AddDbContext<SocialApp.Api.Data.DataContext>(options =>options.UseSqlServer(connectionString));
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection 'SocialConnection' is not found");
}


                
                ;

//#18 
builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<PostService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<PhotoUploadService>();

builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

//#23 Thêm dịch vụ Authentication (Xác thực) vào DI container
builder.Services.AddAuthentication(options =>
{
    // Đặt mặc định phương thức xác thực là JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
//#23 Cấu hình JWT Bearer Authentication
.AddJwtBearer(options =>
{
    // Lấy thông tin Issuer (Nhà phát hành token) từ cấu hình
    var issuer = builder.Configuration.GetValue<string>("Jwt:Issuer");

    // Lấy Secret Key từ cấu hình để dùng trong việc ký token
    var secretKey = builder.Configuration.GetValue<string>("Jwt:SecretKey");

    // Mã hóa Secret Key sang dạng byte
    var securityKey = System.Text.Encoding.UTF8.GetBytes(secretKey);

    // Tạo khóa đối xứng từ secret key
    var symmetricKey = new SymmetricSecurityKey(securityKey);

    // Cấu hình các thông số để kiểm tra tính hợp lệ của token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Xác định Issuer nào được chấp nhận (Chỉ token từ nguồn này mới hợp lệ)
        ValidIssuer = issuer,
        ValidateIssuer = true,

        // Yêu cầu token phải có khóa bí mật hợp lệ để xác thực
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = symmetricKey,
        ValidateAudience = false
    };
});

//#23 Thêm dịch vụ Authorization (Ủy quyền) vào DI container
//#23 Điều này cho phép API kiểm tra quyền của người dùng sau khi đã xác thực
builder.Services.AddAuthorization();

builder.Services.AddSignalR(options =>
{
    // Cấu hình SignalR
    options.EnableDetailedErrors = true;
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddAntiforgery();


builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();



var app = builder.Build();
app.MapControllers();

#if DEBUG

#endif

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.MapOpenApi();

}
app.UseHttpsRedirection();// chatgpt Xoá dòng này để chạy HTTP

app.UseStaticFiles(); // Cho phép lưu file tĩnh 
//app.Use(async (context, next) =>
//{
//    context.Request.Headers.TryGetValue("Authorization", out var value);
//    await next();
//});
//#22 sự khác biệt giữa việc gọi chuỗi (chaining) các phương thức như : Phải trả về (IEndpointRouteBuilder)
//#22 và tách thành từng lệnh riêng lẻ như:Có thể trả về (void) hoặc (IEndpointRouteBuilder)
app.UseAuthentication()
    .UseAuthorization();


app.UseAntiforgery(); // ✅ ở đây

//#22(second tried)
app.MapAuthEndpoints();
app.MapPostsEndpoints();
app.MapUserEndpoints();

app.UseStaticFiles();
app.MapHub<SocialApp.Api.Hubs.SocialHubs>(AppConstants.HubPattern);
//#23
app.Run();
// add autoDBmigrations 


