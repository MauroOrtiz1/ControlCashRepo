using ControlCash.Domain.Entities;
using ControlCash.Domain.Interfaces.Repositories;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Domain.Interfaces.UnitOfWork;
using ControlCash.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace ControlCash.Application.UseCases.Auth;

public class RegistrarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegistrarUsuarioUseCase> _logger;

    public RegistrarUsuarioUseCase(
        IUsuarioRepository usuarioRepository,
        IAuthService authService,
        IUnitOfWork unitOfWork,
        ILogger<RegistrarUsuarioUseCase> logger)
    {
        _usuarioRepository = usuarioRepository;
        _authService = authService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultadoOperacion> EjecutarAsync(RegisterRequest request)
    {
        if (await _usuarioRepository.EmailExisteAsync(request.Email))
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
        await _usuarioRepository.AgregarAsync(usuario);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Usuario registrado: {Email}", usuario.Email);
        return ResultadoOperacion.CrearExito("Usuario registrado correctamente.");
    }
}