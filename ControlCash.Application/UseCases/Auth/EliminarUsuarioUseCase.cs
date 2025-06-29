using ControlCash.Domain.Interfaces.UnitOfWork;
using ControlCash.Application.Common;

namespace ControlCash.Application.UseCases.Auth;

public class EliminarUsuarioUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public EliminarUsuarioUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultadoOperacion> EjecutarAsync(int id)
    {
        var usuario = await _unitOfWork.UsuarioRepository.ObtenerPorIdAsync(id);
        if (usuario is null) return ResultadoOperacion.CrearFalla("Usuario no encontrado.");

        await _unitOfWork.UsuarioRepository.EliminarAsync(usuario);
        await _unitOfWork.CommitAsync();

        return ResultadoOperacion.CrearExito("Usuario eliminado correctamente.");
    }
}