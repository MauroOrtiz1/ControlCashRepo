using System.Security.Claims;
using ControlCash.Domain.Interfaces.UnitOfWork;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Application.DTOs;

namespace ControlCash.Application.UseCases.Gasto;

public class ObtenerGastosUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ObtenerGastosUseCase(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<List<GastoResponseDTO>> EjecutarAsync(ClaimsPrincipal user)
    {
        // 1. Obtener email desde el token
        var email = _currentUserService.ObtenerEmailDesdeClaims(user);

        // 2. Buscar usuario por email
        var usuario = await _unitOfWork.UsuarioRepository.ObtenerPorEmailAsync(email);
        if (usuario is null)
            throw new Exception("Usuario no encontrado.");

        // 3. Obtener gastos del usuario
        var gastos = await _unitOfWork.GastoRepository.ObtenerGastosPorUsuarioAsync(usuario.IdUsuario);

        // 4. Proyectar a DTO
        return gastos.Select(g => new GastoResponseDTO
        {
            IdGasto = g.IdGasto,
            Monto = g.Monto,
            Categoria = g.IdCategoriaNavigation?.NombreCategoria ?? "",
            Fecha = g.Fecha,
            Descripcion = g.Descripcion
        }).ToList();
    }
}