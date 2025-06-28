using ControlCash.Domain.Interfaces.Repositories;

namespace ControlCash.Application.UseCases.Auth;

public class ObtenerTodosUsuariosUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;

    public ObtenerTodosUsuariosUseCase(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<object> EjecutarAsync()
    {
        var lista = await _usuarioRepository.ObtenerTodosAsync();
        return lista.Select(u => new
        {
            u.IdUsuario,
            u.Nombre,
            u.Email,
            u.Rol,
            u.EsPremium,
            u.AnunciosActivos,
            u.FechaRegistro
        });
    }
}