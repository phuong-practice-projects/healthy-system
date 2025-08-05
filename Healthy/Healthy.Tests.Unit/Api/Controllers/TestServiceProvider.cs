using Healthy.Application.Common.Interfaces;

namespace Healthy.Tests.Unit.Api.Controllers;

public class TestServiceProvider : IServiceProvider
{
    private readonly ICurrentUserService _currentUserService;

    public TestServiceProvider(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public object? GetService(Type serviceType)
    {
        if (serviceType == typeof(ICurrentUserService))
            return _currentUserService;
        
        // For any other service type, return null (this would cause GetRequiredService to throw)
        return null;
    }
}
