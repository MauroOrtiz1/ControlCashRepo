using System.Security.Claims;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Domain.Interfaces.UnitOfWork;
using ControlCash.Application.Common;

namespace ControlCash.Application.UseCases.Categoria;

public class ObtenerCategoriasUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ObtenerCategoriasUseCase(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<object> EjecutarAsync(ClaimsPrincipal user)
    {
        var email = _currentUserService.ObtenerEmailDesdeClaims(user);
        var usuario = await _unitOfWork.UsuarioRepository.ObtenerPorEmailAsync(email);
        var categorias = await _unitOfWork.CategoriaRepository.ObtenerPorUsuarioAsync(usuario!.IdUsuario);

        return categorias.Select(c => new
        {
            c.IdCategoria,
            c.NombreCategoria
        });
    }
}