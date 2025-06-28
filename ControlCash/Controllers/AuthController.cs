using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControlCash.Application.DTOs;
using ControlCash.Application.UseCases.Auth;

namespace ControlCash.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegistrarUsuarioUseCase _registrarUsuario;
    private readonly LoginUsuarioUseCase _loginUsuario;
    private readonly CrearUsuarioPorAdminUseCase _crearPorAdmin;
    private readonly PromoverUsuarioAAdminUseCase _promoverAdmin;
    private readonly ObtenerTodosUsuariosUseCase _obtenerTodos;
    private readonly EliminarUsuarioUseCase _eliminarUsuario;
    private readonly CambiarPasswordUseCase _cambiarPassword;

    public AuthController(
        RegistrarUsuarioUseCase registrarUsuario,
        LoginUsuarioUseCase loginUsuario,
        CrearUsuarioPorAdminUseCase crearPorAdmin,
        PromoverUsuarioAAdminUseCase promoverAdmin,
        ObtenerTodosUsuariosUseCase obtenerTodos,
        EliminarUsuarioUseCase eliminarUsuario,
        CambiarPasswordUseCase cambiarPassword)
    {
        _registrarUsuario = registrarUsuario;
        _loginUsuario = loginUsuario;
        _crearPorAdmin = crearPorAdmin;
        _promoverAdmin = promoverAdmin;
        _obtenerTodos = obtenerTodos;
        _eliminarUsuario = eliminarUsuario;
        _cambiarPassword = cambiarPassword;
    }

    [HttpPost("registrarse-usuarios")]
    public async Task<IActionResult> RegistrarUsuario([FromBody] RegisterRequest request)
    {
        var resultado = await _registrarUsuario.EjecutarAsync(request);
        return resultado.Exito ? Ok(resultado.Mensaje) : BadRequest(resultado.Mensaje);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _loginUsuario.EjecutarAsync(request);
        return token is null ? Unauthorized("Usuario o contraseña incorrectos.") : Ok(new { token });
    }

    [Authorize(Roles = "admin")]
    [HttpPost("crear-usuario-admin")]
    public async Task<IActionResult> CrearUsuarioAdmin([FromBody] RegisterAdminRequest request)
    {
        var resultado = await _crearPorAdmin.EjecutarAsync(request);
        return resultado.Exito ? Ok(resultado.Mensaje) : BadRequest(resultado.Mensaje);
    }

    [Authorize(Roles = "admin")]
    [HttpPut("promover-a-admin/{id}")]
    public async Task<IActionResult> PromoverAAdmin(int id)
    {
        var resultado = await _promoverAdmin.EjecutarAsync(id);
        return resultado.Exito ? Ok(resultado.Mensaje) : BadRequest(resultado.Mensaje);
    }

    [Authorize(Roles = "admin")]
    [HttpGet("usuarios")]
    public async Task<IActionResult> ObtenerTodosUsuarios()
    {
        var lista = await _obtenerTodos.EjecutarAsync();
        return Ok(lista);
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("usuarios/{id}")]
    public async Task<IActionResult> EliminarUsuario(int id)
    {
        var resultado = await _eliminarUsuario.EjecutarAsync(id);
        return resultado.Exito ? Ok(resultado.Mensaje) : NotFound(resultado.Mensaje);
    }

    [Authorize]
    [HttpPut("cambiar-password")]
    public async Task<IActionResult> CambiarPassword([FromBody] CambiarPasswordRequest request)
    {
        var resultado = await _cambiarPassword.EjecutarAsync(request, User);
        return resultado.Exito ? Ok(resultado.Mensaje) : BadRequest(resultado.Mensaje);
    }
}
