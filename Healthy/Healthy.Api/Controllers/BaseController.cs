using Healthy.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Healthy.Api.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    private ICurrentUserService? _currentUserService;
    protected ICurrentUserService CurrentUser => _currentUserService ??= HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

    protected string? GetCurrentUserId()
    {
        return CurrentUser.UserId;
    }

    protected Guid GetCurrentUserIdAsGuid()
    {
        var userId = GetCurrentUserId();
        return Guid.TryParse(userId, out var guid) ? guid : Guid.Empty;
    }

    protected string? GetCurrentUserName()
    {
        return CurrentUser.UserName;
    }

    protected string? GetCurrentUserEmail()
    {
        return CurrentUser.Email;
    }

    protected bool IsAuthenticated()
    {
        return CurrentUser.IsAuthenticated;
    }

    protected bool HasRole(string role)
    {
        return CurrentUser.HasRole(role);
    }
}
