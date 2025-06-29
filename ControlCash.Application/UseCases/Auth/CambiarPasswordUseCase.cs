using System.Security.Claims;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Domain.Interfaces.UnitOfWork;
using ControlCash.Application.DTOs;
using Microsoft.Extensions.Logging;
using ControlCash.Application.Common;

namespace ControlCash.Application.UseCases.Auth;

public class CambiarPasswordUseCase
{
    private readonly IAuthService _authService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CambiarPasswordUseCase> _logger;

    public CambiarPasswordUseCase(
        IAuthService authService,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ILogger<CambiarPasswordUseCase> logger)
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultadoOperacion> EjecutarAsync(CambiarPasswordRequest request, ClaimsPrincipal user)
    {
        var email = _currentUserService.ObtenerEmailDesdeClaims(user);
        var usuario = await _unitOfWork.UsuarioRepository.ObtenerPorEmailAsync(email);

        if (usuario is null)
            return ResultadoOperacion.CrearFalla("Usuario no encontrado.");

        if (!_authService.VerificarPassword(usuario, request.PasswordActual))
            return ResultadoOperacion.CrearFalla("La contraseña actual es incorrecta.");

        usuario.Password = _authService.HashPassword(usuario, request.NuevaPassword);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Contraseña actualizada: {Email}", usuario.Email);
        return ResultadoOperacion.CrearExito("Contraseña actualizada correctamente.");
    }
}