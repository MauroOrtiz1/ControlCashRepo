using System.Security.Claims;
using ControlCash.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace ControlCash.Infrastructure.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int ObtenerUsuarioId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
                throw new UnauthorizedAccessException("No se pudo acceder al usuario.");

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("ID de usuario no encontrado en los claims.");

            return int.Parse(userIdClaim);
        }
    }
}