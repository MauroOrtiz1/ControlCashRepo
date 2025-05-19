using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControlCash.Models;
using ControlCash.DTOs;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GastosController : ControllerBase
{
    private readonly ControlCashDbContext _context;
    private readonly ILogger<GastosController> _logger;

    public GastosController(ControlCashDbContext context, ILogger<GastosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/gastos
    [HttpGet]
    public IActionResult ObtenerGastos()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var gastos = _context.Gastos
            .Include(g => g.IdCategoriaNavigation) 
            .Where(g => g.IdUsuario == userId)
            .Select(g => new GastoResponseDTO
            {
                IdGasto = g.IdGasto,
                Monto = g.Monto,
                Categoria = g.IdCategoriaNavigation.NombreCategoria, // corregido aquí
                Fecha = g.Fecha,
                Descripcion = g.Descripcion
            })
            .ToList();

        _logger.LogInformation("Usuario {UserId} obtuvo sus gastos.", userId);
        return Ok(gastos);
    }

    // POST: api/gastos
    [HttpPost]
    public IActionResult CrearGasto([FromBody] GastoCreateDTO dto)
    {
        if (!ModelState.IsValid) {
            _logger.LogWarning("Modelo inválido al crear gasto.");
            return BadRequest(ModelState);
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var categoria = _context.Categoria.FirstOrDefault(c => c.IdCategoria == dto.IdCategoria);
        if (categoria == null)
        {
            _logger.LogWarning("Categoría inválida al crear gasto.");
            return BadRequest("La categoría especificada no existe.");
        }

        var gasto = new Gasto
        {
            IdUsuario = userId,
            IdCategoria = dto.IdCategoria,
            Monto = dto.Monto,
            Fecha = DateOnly.FromDateTime(DateTime.Now),
            Descripcion = dto.Descripcion
        };

        _context.Gastos.Add(gasto);
        _context.SaveChanges();

        _logger.LogInformation("Usuario {UserId} creó un nuevo gasto.", userId);
        return Ok("Gasto creado exitosamente.");
    }

    // DELETE: api/gastos/{id}
    [HttpDelete("{id}")]
    public IActionResult EliminarGasto(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var gasto = _context.Gastos.FirstOrDefault(g => g.IdGasto == id && g.IdUsuario == userId);

        if (gasto == null) {
            _logger.LogWarning("Intento de eliminar gasto no encontrado o no autorizado. Id: {Id}", id);
            return NotFound("Gasto no encontrado.");
        }

        _context.Gastos.Remove(gasto);
        _context.SaveChanges();

        _logger.LogInformation("Usuario {UserId} eliminó el gasto con Id {IdGasto}.", userId, id);
        return Ok("Gasto eliminado correctamente.");
    }
}
