using System.Security.Claims;
using FTG12_ReviewsApi.Application.Common.Interfaces;

namespace FTG12_ReviewsApi.Services;

/// <summary>
/// Reads the current user's identity from the HTTP context JWT claims.
/// </summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public int? UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Username =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);

    public bool IsAdmin =>
        httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
