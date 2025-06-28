using ControlCash.Domain.Interfaces.Repositories;
using ControlCash.Domain.Interfaces.UnitOfWork;

namespace ControlCash.Application.UseCases.Auth;

public class EliminarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EliminarUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultadoOperacion> EjecutarAsync(int id)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(id);
        if (usuario is null) return ResultadoOperacion.CrearFalla("Usuario no encontrado.");

        await _usuarioRepository.EliminarAsync(usuario);
        await _unitOfWork.CommitAsync();

        return ResultadoOperacion.CrearExito("Usuario eliminado correctamente.");
    }
}