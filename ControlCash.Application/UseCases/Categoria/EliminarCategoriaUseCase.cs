using System.Security.Claims;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Domain.Interfaces.UnitOfWork;
using ControlCash.Application.Common;

namespace ControlCash.Application.UseCases.Categoria;

public class EliminarCategoriaUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public EliminarCategoriaUseCase(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ResultadoOperacion> EjecutarAsync(int id, ClaimsPrincipal user)
    {
        var email = _currentUserService.ObtenerEmailDesdeClaims(user);
        var usuario = await _unitOfWork.UsuarioRepository.ObtenerPorEmailAsync(email);

        var categoria = await _unitOfWork.CategoriaRepository.ObtenerConGastosAsync(id, usuario!.IdUsuario);
        if (categoria == null)
            return ResultadoOperacion.CrearFalla("Categoría no encontrada.");

        if (categoria.Gastos.Any())
        {
            _unitOfWork.GastoRepository.EliminarRango(categoria.Gastos);
        }

        await _unitOfWork.CategoriaRepository.EliminarAsync(categoria);
        await _unitOfWork.GuardarCambiosAsync();

        return ResultadoOperacion.CrearExito("Categoría y sus gastos asociados eliminados correctamente.");
    }
}