using System.Security.Claims;

namespace ControlCash.Domain.Interfaces.Services;

public interface ICurrentUserService
{
    string ObtenerEmailDesdeClaims(ClaimsPrincipal user);
    int ObtenerId(ClaimsPrincipal user);

}