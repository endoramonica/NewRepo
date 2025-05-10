using System.Runtime.CompilerServices;
using System.Security.Claims;
using SocialAppLibrary.Shared.Dtos;

namespace SocialApp.Api;

    public static class ClaimsPrincipalExtension
    {
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    public static LoggedInUser GetUser(this ClaimsPrincipal principal)
        {
            var userId = principal.GetUserId();
            var name = principal.FindFirstValue(ClaimTypes.Name);
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var photoUrl = principal.FindFirstValue("UserPhotoUrl");

            return new LoggedInUser(userId, name, email, photoUrl);
        }
    }