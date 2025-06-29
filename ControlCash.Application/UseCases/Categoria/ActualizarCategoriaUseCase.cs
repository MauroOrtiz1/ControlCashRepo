using System.Security.Claims;
using ControlCash.Application.DTOs;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Domain.Interfaces.UnitOfWork;
using ControlCash.Application.Common;

namespace ControlCash.Application.UseCases.Categoria;

public class ActualizarCategoriaUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ActualizarCategoriaUseCase(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ResultadoOperacion> EjecutarAsync(int id, CategoriaUpdateDto dto, ClaimsPrincipal user)
    {   
        var email = _currentUserService.ObtenerEmailDesdeClaims(user);
        var usuario = await _unitOfWork.UsuarioRepository.ObtenerPorEmailAsync(email);
        var categoria = await _unitOfWork.CategoriaRepository.ObtenerPorIdYUsuarioAsync(id, usuario!.IdUsuario);

        if (categoria == null)
            return ResultadoOperacion.CrearFalla("Categoría no encontrada.");

        categoria.NombreCategoria = dto.NombreCategoria;
        await _unitOfWork.GuardarCambiosAsync();

        return ResultadoOperacion.CrearExito("Categoría actualizada correctamente.");
    }
}