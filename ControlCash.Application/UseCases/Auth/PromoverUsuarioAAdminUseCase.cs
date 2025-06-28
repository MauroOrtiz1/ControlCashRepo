using ControlCash.Domain.Interfaces.Repositories;
using ControlCash.Domain.Interfaces.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace ControlCash.Application.UseCases.Auth;

public class PromoverUsuarioAAdminUseCase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PromoverUsuarioAAdminUseCase> _logger;

    public PromoverUsuarioAAdminUseCase(
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        ILogger<PromoverUsuarioAAdminUseCase> logger)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ResultadoOperacion> EjecutarAsync(int id)
    {
        var user = await _usuarioRepository.ObtenerPorIdAsync(id);
        if (user is null) return ResultadoOperacion.CrearFalla("Usuario no encontrado.");
        if (user.Rol == "admin") return ResultadoOperacion.CrearFalla("El usuario ya es administrador.");

        user.Rol = "admin";
        await _unitOfWork.CommitAsync();

        _logger.LogInformation("Usuario promovido a admin: {Email}", user.Email);
        return ResultadoOperacion.CrearExito($"Usuario {user.Email} promovido a administrador.");
    }
}