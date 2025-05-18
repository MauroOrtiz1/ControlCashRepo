using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using ControlCash.Models;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ControlCashDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<Usuario> _passwordHasher;

    public AuthController(ControlCashDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    // POST api/auth/registrarse (registro público, solo rol user)
    [HttpPost("registrarse-usuarios")]
    public IActionResult RegistrarUsuario([FromBody] RegisterRequest register)
    {
        if (_context.Usuarios.Any(u => u.Email == register.Email))
            return BadRequest("El email ya está registrado.");

        var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
        if (!regex.IsMatch(register.Password))
            return BadRequest("La contraseña debe tener mínimo 8 caracteres, una mayúscula, un número y un carácter especial.");

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

        return Ok("Usuario registrado correctamente.");
    }

    // POST api/auth/crear-usuario (solo admin puede crear usuarios con rol arbitrario)
    [Authorize(Roles = "admin")]
    [HttpPost("crear-usuario-admin")]
    public IActionResult CrearUsuarioPorAdmin([FromBody] RegisterRequest register)
    {
        if (_context.Usuarios.Any(u => u.Email == register.Email))
            return BadRequest("El email ya está registrado.");

        var rolesPermitidos = new[] { "user", "admin" };
        if (string.IsNullOrEmpty(register.Rol) || !rolesPermitidos.Contains(register.Rol.ToLower()))
            return BadRequest("Rol inválido. Solo se permiten: user, admin.");

        var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
        if (!regex.IsMatch(register.Password))
            return BadRequest("La contraseña debe tener mínimo 8 caracteres, una mayúscula, un número y un carácter especial.");

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

        return Ok("Usuario creado correctamente.");
    }

    // PUT api/auth/promover-a-admin/{id} (solo admin puede promover usuarios)
    [Authorize(Roles = "admin")]
    [HttpPut("promover-a-admin/{id}")]
    public IActionResult PromoverUsuarioAAdmin(int id)
    {
        var user = _context.Usuarios.Find(id);

        if (user == null)
            return NotFound("Usuario no encontrado.");

        user.Rol = "admin";
        _context.SaveChanges();

        return Ok($"Usuario {user.Email} promovido a administrador.");
    }

    // POST api/auth/login
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest login)
    {
        var user = _context.Usuarios.SingleOrDefault(u => u.Email == login.Email);

        if (user == null)
            return Unauthorized("Usuario o contraseña incorrectos");

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, login.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
            return Unauthorized("Usuario o contraseña incorrectos");

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
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Rol { get; set; }  
}
