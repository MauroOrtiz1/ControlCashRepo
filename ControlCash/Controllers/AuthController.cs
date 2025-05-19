using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using ControlCash.Models;
using ControlCash.DTOs;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ControlCashDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<Usuario> _passwordHasher;
    private readonly ILogger<AuthController> _logger; // Esto para LOGS
    
    public AuthController(ControlCashDbContext context, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<Usuario>();
        _logger = logger;
    }

    // POST api/auth/registrarse (registro público, solo rol user)
    [HttpPost("registrarse-usuarios")]
    public IActionResult RegistrarUsuario([FromBody] RegisterRequest register)
    {
        if (!ModelState.IsValid) {
            _logger.LogWarning("Registro fallido: modelo inválido para email {Email}", register.Email);
            return BadRequest(ModelState);
        }
        if (_context.Usuarios.Any(u => u.Email == register.Email)) {
            _logger.LogWarning("Intento de registro con email ya existente: {Email}", register.Email);
            return BadRequest("El email ya está registrado.");
        }

        var newUser = new Usuario
        {
            Nombre = register.Nombre,
            Email = register.Email,
            EsPremium = false,
            AnunciosActivos = true,
            FechaRegistro = DateOnly.FromDateTime(DateTime.Now)
        };

        newUser.Password = _passwordHasher.HashPassword(newUser, register.Password);

        _context.Usuarios.Add(newUser);
        _context.SaveChanges();

        _logger.LogInformation("Usuario registrado exitosamente: {Email}", newUser.Email);
        return Ok("Usuario registrado correctamente.");
    }


    // POST api/auth/crear-usuario (solo admin puede crear usuarios con rol arbitrario)
    [Authorize(Roles = "admin")]
    [HttpPost("crear-usuario-admin")]
    public IActionResult CrearUsuarioPorAdmin([FromBody] RegisterAdminRequest register)
    {
        if (!ModelState.IsValid) {
            _logger.LogWarning("Creación de usuario por admin fallida: modelo inválido para email {Email}", register.Email);
            return BadRequest(ModelState);
        }

        if (_context.Usuarios.Any(u => u.Email == register.Email)) {
            _logger.LogWarning("Intento de creación de usuario con email ya existente por admin: {Email}", register.Email);
            return BadRequest("El email ya está registrado.");
        }

        var newUser = new Usuario
        {
            Nombre = register.Nombre,
            Email = register.Email,
            Rol = register.Rol.ToLower(),
            EsPremium = false,
            AnunciosActivos = true,
            FechaRegistro = DateOnly.FromDateTime(DateTime.Now)
        };

        newUser.Password = _passwordHasher.HashPassword(newUser, register.Password);

        _context.Usuarios.Add(newUser);
        _context.SaveChanges();
        
        _logger.LogInformation("Usuario creado por admin exitosamente: {Email} con rol {Rol}", newUser.Email, newUser.Rol);
        return Ok("Usuario creado correctamente.");
    }

    // PUT api/auth/promover-a-admin/{id} (solo admin puede promover usuarios)
    [Authorize(Roles = "admin")]
    [HttpPut("promover-a-admin/{id}")]
    public IActionResult PromoverUsuarioAAdmin(int id)
    {
        var user = _context.Usuarios.Find(id);

        if (user == null) {
            _logger.LogWarning("Intento de promover a admin usuario inexistente con Id {Id}", id);
            return NotFound("Usuario no encontrado.");
        }

        if (user.Rol == "admin") {
            _logger.LogInformation("Intento de promover a admin un usuario que ya es admin: {Email}", user.Email);
            return BadRequest("El usuario ya es administrador.");
        }
        
        user.Rol = "admin";
        _context.SaveChanges();
        
        _logger.LogInformation("Usuario {Email} promovido a administrador.", user.Email);
        return Ok($"Usuario {user.Email} promovido a administrador.");
    }

    // POST api/auth/login
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest login)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Intento de login con modelo inválido para el email: {Email}", login.Email);
            return BadRequest(ModelState); // FluentValidation
        }

        var user = _context.Usuarios.SingleOrDefault(u => u.Email == login.Email);

        if (user == null)
        {
            _logger.LogWarning("Intento de login con email inexistente: {Email}", login.Email);
            return Unauthorized("Usuario o contraseña incorrectos");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Intento de login fallido por contraseña incorrecta para email: {Email}", login.Email);
            return Unauthorized("Usuario o contraseña incorrectos");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Rol)
        };

        var secretKey = _configuration.GetValue<string>("JwtSettings:SecretKey");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpiryMinutes")),
            signingCredentials: creds);

        _logger.LogInformation("Login exitoso para el usuario: {Email}", user.Email);
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
    
    
    //pruebas 

    // GET api/auth/admin-only
    [Authorize(Roles = "admin")]
    [HttpGet("solo-admin")]
    public IActionResult SoloAdmin()
    {
        return Ok("Este endpoint solo es accesible por administradores.");
    }

    // GET api/auth/user-or-admin
    [Authorize(Roles = "user,admin")]
    [HttpGet("usuario-o-admin")]
    public IActionResult UsuarioOAdmin()
    {
        return Ok("Este endpoint es accesible por usuarios y administradores.");
    }

    // GET api/auth/usuarios
    [Authorize(Roles = "admin")]
    [HttpGet("usuarios")]
    public IActionResult ObtenerTodosUsuarios()
    {
        var users = _context.Usuarios
            .Select(u => new
            {
                u.IdUsuario,
                u.Nombre,
                u.Email,
                u.Rol,
                u.EsPremium,
                u.AnunciosActivos,
                u.FechaRegistro
            })
            .ToList();

        return Ok(users);
    }

    // DELETE api/auth/usuarios/{id}
    [Authorize(Roles = "admin")]
    [HttpDelete("usuarios/{id}")]
    public IActionResult EliminarUsuario(int id)
    {
        var user = _context.Usuarios.Find(id);

        if (user == null)
            return NotFound("Usuario no encontrado.");

        _context.Usuarios.Remove(user);
        _context.SaveChanges();

        return Ok("Usuario eliminado correctamente.");
    }
    
    // NUEVOS ENDPOINTS
    
    // Cambiar password
    public class CambiarPasswordRequest {
        public string PasswordActual { get; set; }
        public string NuevaPassword { get; set; }
    }

    [Authorize]
    [HttpPut("cambiar-password")]
    public IActionResult CambiarPassword([FromBody] CambiarPasswordRequest request)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var user = _context.Usuarios.FirstOrDefault(u => u.Email == email);

        if (user == null)
            return NotFound("Usuario no encontrado.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.PasswordActual);
        if (result == PasswordVerificationResult.Failed)
            return BadRequest("La contraseña actual es incorrecta.");

        user.Password = _passwordHasher.HashPassword(user, request.NuevaPassword);
        _context.SaveChanges();

        _logger.LogInformation("Usuario {Email} cambió su contraseña.", user.Email);
        return Ok("Contraseña actualizada correctamente.");
    }


}

