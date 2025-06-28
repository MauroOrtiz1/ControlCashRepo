/*
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ControlCash.Models;
using ControlCash.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class CategoriaController : ControllerBase
{
    private readonly ControlCashDbContext _context;
    private readonly ILogger<CategoriaController> _logger;

    public CategoriaController(ControlCashDbContext context, ILogger<CategoriaController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET api/categoria // Obtener todas las categorías del usuario autentificado
    [Authorize]
    [HttpGet]   
    public IActionResult ObtenerCategorias()
    {
        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var categorias = _context.Categoria
            .Where(c => c.IdUsuario == idUsuario)
            .Select(c => new { c.IdCategoria, c.NombreCategoria })
            .ToList();

        return Ok(categorias);
    }
    
    // POST api/categoria   // Crear una nueva categoría
    [Authorize]
    [HttpPost]
    public IActionResult CrearCategoria([FromBody] CategoriaCreateDto dto) 
    {
        if (!ModelState.IsValid) {
            _logger.LogWarning("Modelo inválido al crear categoría");
            return BadRequest(ModelState);
        }

        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        var nuevaCategoria = new Categorium {
            NombreCategoria = dto.NombreCategoria,
            IdUsuario = idUsuario
        };

        _context.Categoria.Add(nuevaCategoria);
        _context.SaveChanges();

        _logger.LogInformation("Categoría creada exitosamente por el usuario {IdUsuario}", idUsuario);
        return Ok(new { mensaje = "Categoría creada exitosamente." });
    }

    // PUT api/categoria/{id}
    [Authorize]
    [HttpPut("{id}")]
    public IActionResult ActualizarCategoria(int id, [FromBody] CategoriaUpdateDto dto)
    {
        if (!ModelState.IsValid) {
            _logger.LogWarning("Modelo inválido al actualizar categoría con ID: {Id}", id);
            return BadRequest(ModelState);
        }

        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var categoria = _context.Categoria.FirstOrDefault(c => c.IdCategoria == id && c.IdUsuario == idUsuario);

        if (categoria == null) {
            _logger.LogWarning("Categoría no encontrada o no pertenece al usuario. ID: {Id}", id);
            return NotFound("Categoría no encontrada.");
        }

        categoria.NombreCategoria = dto.NombreCategoria;
        _context.SaveChanges();

        _logger.LogInformation("Categoría actualizada correctamente. ID: {Id}", id);
        return Ok("Categoría actualizada correctamente.");
    }

    // DELETE api/categoria/{id}  ESTE ELIMINA TANTO LA CATEGORIA COMO USS GASTOS RELACIONADOS
    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult EliminarCategoria(int id)
    {
        var idUsuario = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var categoria = _context.Categoria
            .Include(c => c.Gastos)
            .FirstOrDefault(c => c.IdCategoria == id && c.IdUsuario == idUsuario);

        if (categoria == null)
        {
            _logger.LogWarning("Intento de eliminar categoría no encontrada. ID: {Id}", id);
            return NotFound("Categoría no encontrada.");
        }

        // Eliminar los gastos asociados
        if (categoria.Gastos.Any())
        {
            _logger.LogInformation("Eliminando gastos asociados a la categoría ID: {Id}", id);
            _context.Gastos.RemoveRange(categoria.Gastos);
        }

        _context.Categoria.Remove(categoria);
        _context.SaveChanges();

        _logger.LogInformation("Categoría y gastos asociados eliminados correctamente. ID: {Id}", id);
        return Ok("Categoría y sus gastos asociados eliminados correctamente.");
    }

}
*/
