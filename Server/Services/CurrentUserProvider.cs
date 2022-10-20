using Tradibit.Common.Entities;
using Tradibit.Common.Interfaces;

namespace Tradibit.Api.Services;

public class CurrentUserProvider : ICurrentUserProvider
{
    private readonly IHttpContextAccessor _httpAccessor;

    public CurrentUserProvider(IHttpContextAccessor httpAccessor)
    {
        _httpAccessor = httpAccessor;
    }

    public User CurrentUser => new(_httpAccessor.HttpContext?.User);
}