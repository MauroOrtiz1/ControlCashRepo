using ControlCash.Domain.Entities;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Domain.Interfaces.UnitOfWork;
using ControlCash.Application.DTOs;
using Microsoft.Extensions.Logging;
using ControlCash.Application.Common;

namespace ControlCash.Application.UseCases.Auth;

public class RegistrarUsuarioUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly ILogger<RegistrarUsuarioUseCase> _logger;

    public RegistrarUsuarioUseCase(
        IUnitOfWork unitOfWork,
        IAuthService authService,
        ILogger<RegistrarUsuarioUseCase> logger)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
        _logger = logger;
    }

    public async Task<ResultadoOperacion> EjecutarAsync(RegisterRequest request)
    {
        if (await _unitOfWork.UsuarioRepository.EmailExisteAsync(request.Email))
            return ResultadoOperacion.CrearFalla("El email ya está registrado.");

        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Email = request.Email,
            EsPremium = false,
            AnunciosActivos = true,
            FechaRegistro = DateOnly.FromDateTime(DateTime.Now),
            Rol = "user"
        };

        usuario.Password = _authService.HashPassword(usuario, request.Password);
        await _unitOfWork.UsuarioRepository.AgregarAsync(usuario);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Usuario registrado: {Email}", usuario.Email);
        return ResultadoOperacion.CrearExito("Usuario registrado correctamente.");
    }
}