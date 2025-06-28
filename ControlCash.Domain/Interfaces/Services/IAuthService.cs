using ControlCash.Domain.Entities;

namespace ControlCash.Domain.Interfaces.Services;

public interface IAuthService
{
    string HashPassword(Usuario usuario, string password);
    bool VerificarPassword(Usuario usuario, string password);
    string GenerarToken(Usuario usuario);
}