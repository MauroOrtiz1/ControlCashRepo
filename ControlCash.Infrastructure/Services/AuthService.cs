using ControlCash.Domain.Entities;
using ControlCash.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ControlCash.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<Usuario> _passwordHasher;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<Usuario>();
    }

    public string HashPassword(Usuario usuario, string password)
    {
        return _passwordHasher.HashPassword(usuario, password);
    }

    public bool VerificarPassword(Usuario usuario, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Password, password);
        return result != PasswordVerificationResult.Failed;
    }

    public string GenerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(_configuration.GetValue<int>("JwtSettings:ExpiryMinutes")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}