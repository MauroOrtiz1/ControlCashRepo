using ControlCash.Domain.Entities;
using ControlCash.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using ControlCash.Infrastructure.Persistence;

namespace ControlCash.Infrastructure.Persistence;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly ControlCashDbContext _context;

    public CategoriaRepository(ControlCashDbContext context)
    {
        _context = context;
    }

    public async Task<List<Categorium>> ObtenerPorUsuarioAsync(int userId)
    {
        return await _context.Categoria
            .Where(c => c.IdUsuario == userId)
            .ToListAsync();
    }

    public async Task<Categorium?> ObtenerPorIdYUsuarioAsync(int id, int userId)
    {
        return await _context.Categoria
            .FirstOrDefaultAsync(c => c.IdCategoria == id && c.IdUsuario == userId);
    }

    public async Task<Categorium?> ObtenerPorIdAsync(int id)
    {
        return await _context.Categoria
            .FirstOrDefaultAsync(c => c.IdCategoria == id);
    }

    public async Task AgregarAsync(Categorium categoria)
    {
        await _context.Categoria.AddAsync(categoria);
    }

    public Task ActualizarAsync(Categorium categoria)
    {
        _context.Categoria.Update(categoria);
        return Task.CompletedTask;
    }

    public Task EliminarAsync(Categorium categoria)
    {
        _context.Categoria.Remove(categoria);
        return Task.CompletedTask;
    }
    public async Task<Categorium?> ObtenerConGastosAsync(int idCategoria, int idUsuario)
    {
        return await _context.Categoria
            .Include(c => c.Gastos)
            .FirstOrDefaultAsync(c => c.IdCategoria == idCategoria && c.IdUsuario == idUsuario);
    }

}