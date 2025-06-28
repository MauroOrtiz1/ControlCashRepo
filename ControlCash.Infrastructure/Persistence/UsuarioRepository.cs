using ControlCash.Domain.Entities;
using ControlCash.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ControlCash.Infrastructure.Persistence;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ControlCashDbContext _context;

    public UsuarioRepository(ControlCashDbContext context)
    {
        _context = context;
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.Email == email);
    }

    public async Task<Usuario?> ObtenerPorEmailAsync(string email)
    {
        return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario?> ObtenerPorIdAsync(int id)
    {
        return await _context.Usuarios.FindAsync(id);
    }

    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task AgregarAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
    }

    public async Task EliminarAsync(Usuario usuario)
    {
        _context.Usuarios.Remove(usuario);
    }
}