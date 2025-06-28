using ControlCash.Domain.Interfaces.Repositories;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace ControlCash.Application.UseCases.Auth;

public class LoginUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuthService _authService;
    private readonly ILogger<LoginUsuarioUseCase> _logger;

    public LoginUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        IAuthService authService,
        ILogger<LoginUsuarioUseCase> logger)
    {
        _usuarioRepository = usuarioRepository;
        _authService = authService;
        _logger = logger;
    }

    public async Task<string?> EjecutarAsync(LoginRequest request)
    {
        var user = await _usuarioRepository.ObtenerPorEmailAsync(request.Email);

        if (user == null || !_authService.VerificarPassword(user, request.Password))
        {
            _logger.LogWarning("Intento de login fallido: {Email}", request.Email);
            return null;
        }

        return _authService.GenerarToken(user);
    }
}