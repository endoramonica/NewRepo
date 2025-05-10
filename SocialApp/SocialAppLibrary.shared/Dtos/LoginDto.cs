using System;

namespace SocialAppLibrary.Shared.Dtos
{
    //#25 nếu được muốn nâng cấp thành Cách 2: Hỗ trợ cả Email và Username
//public record LoginDto(string EmailOrUsername, string Password);
    public record LoginDto(string Email, string Password);
}
