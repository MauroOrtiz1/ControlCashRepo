using ControlCash.Application.Common;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Domain.Interfaces.UnitOfWork;
using System.Security.Claims;

namespace ControlCash.Application.UseCases.Gasto;

public class EliminarGastoUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public EliminarGastoUseCase(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ResultadoOperacion> EjecutarAsync(int gastoId, int userId)
    {
        var gasto = await _unitOfWork.GastoRepository.ObtenerPorIdYUsuarioAsync(gastoId, userId);
        if (gasto == null)
            return ResultadoOperacion.CrearFalla("Gasto no encontrado.");

        _unitOfWork.GastoRepository.Eliminar(gasto);
        await _unitOfWork.GuardarCambiosAsync();

        return ResultadoOperacion.CrearExito("Gasto eliminado correctamente.");
    }

}