using ControlCash.Domain.Interfaces.Services;
using ControlCash.Domain.Interfaces.UnitOfWork;
using ControlCash.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace ControlCash.Application.UseCases.Auth;

public class LoginUsuarioUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly ILogger<LoginUsuarioUseCase> _logger;

    public LoginUsuarioUseCase(
        IUnitOfWork unitOfWork,
        IAuthService authService,
        ILogger<LoginUsuarioUseCase> logger)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
        _logger = logger;
    }

    public async Task<string?> EjecutarAsync(LoginRequest request)
    {
        var user = await _unitOfWork.UsuarioRepository.ObtenerPorEmailAsync(request.Email);

        if (user == null || !_authService.VerificarPassword(user, request.Password))
        {
            _logger.LogWarning("Intento de login fallido: {Email}", request.Email);
            return null;
        }

        return _authService.GenerarToken(user);
    }
}