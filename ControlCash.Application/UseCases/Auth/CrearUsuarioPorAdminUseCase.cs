using ControlCash.Domain.Entities;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Domain.Interfaces.UnitOfWork;
using ControlCash.Application.DTOs;
using Microsoft.Extensions.Logging;
using ControlCash.Application.Common;

namespace ControlCash.Application.UseCases.Auth;

public class CrearUsuarioPorAdminUseCase
{
    private readonly IAuthService _authService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CrearUsuarioPorAdminUseCase> _logger;

    public CrearUsuarioPorAdminUseCase(
        IAuthService authService,
        IUnitOfWork unitOfWork,
        ILogger<CrearUsuarioPorAdminUseCase> logger)
    {
        _authService = authService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultadoOperacion> EjecutarAsync(RegisterAdminRequest request)
    {
        if (await _unitOfWork.UsuarioRepository.EmailExisteAsync(request.Email))
            return ResultadoOperacion.CrearFalla("El email ya está registrado.");

        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Email = request.Email,
            Rol = request.Rol.ToLower(),
            EsPremium = false,
            AnunciosActivos = true,
            FechaRegistro = DateOnly.FromDateTime(DateTime.Now)
        };

        usuario.Password = _authService.HashPassword(usuario, request.Password);
        await _unitOfWork.UsuarioRepository.AgregarAsync(usuario);
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Admin creó un usuario: {Email} con rol {Rol}", usuario.Email, usuario.Rol);
        return ResultadoOperacion.CrearExito("Usuario creado correctamente.");
    }
}