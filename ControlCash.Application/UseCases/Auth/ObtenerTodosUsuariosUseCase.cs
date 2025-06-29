using ControlCash.Domain.Interfaces.UnitOfWork;

namespace ControlCash.Application.UseCases.Auth;

public class ObtenerTodosUsuariosUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public ObtenerTodosUsuariosUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<object> EjecutarAsync()
    {
        var lista = await _unitOfWork.UsuarioRepository.ObtenerTodosAsync();
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