using System.Security.Claims;
using ControlCash.Domain.Entities;
using ControlCash.Application.DTOs;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Domain.Interfaces.UnitOfWork;
using ControlCash.Application.Common;

namespace ControlCash.Application.UseCases.Categoria;

public class CrearCategoriaUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CrearCategoriaUseCase(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ResultadoOperacion> EjecutarAsync(CategoriaCreateDto dto, ClaimsPrincipal user)
    {
        var email = _currentUserService.ObtenerEmailDesdeClaims(user);
        var usuario = await _unitOfWork.UsuarioRepository.ObtenerPorEmailAsync(email);

        var categoria = new Categorium
        {
            NombreCategoria = dto.NombreCategoria,
            IdUsuario = usuario!.IdUsuario
        };

        await _unitOfWork.CategoriaRepository.AgregarAsync(categoria);
        await _unitOfWork.GuardarCambiosAsync();

        return ResultadoOperacion.CrearExito("Categoría creada exitosamente.");
    }
}