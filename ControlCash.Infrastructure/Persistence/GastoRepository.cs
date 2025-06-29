using ControlCash.Domain.Entities;
using ControlCash.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using ControlCash.Infrastructure.Persistence;

namespace ControlCash.Infrastructure.Persistence
{
    public class GastoRepository : IGastoRepository
    {
        private readonly ControlCashDbContext _context;

        public GastoRepository(ControlCashDbContext context)
        {
            _context = context;
        }

        public async Task<List<Gasto>> ObtenerGastosPorUsuarioAsync(int idUsuario)
        {
            return await _context.Gastos
                .Include(g => g.IdCategoriaNavigation)
                .Where(g => g.IdUsuario == idUsuario)
                .ToListAsync();
        }

        public async Task<Gasto?> ObtenerPorIdYUsuarioAsync(int gastoId, int userId)
        {
            return await _context.Gastos
                .FirstOrDefaultAsync(g => g.IdGasto == gastoId && g.IdUsuario == userId);
        }

        public async Task AgregarAsync(Gasto gasto)
        {
            await _context.Gastos.AddAsync(gasto);
        }

        public void Eliminar(Gasto gasto)
        {
            _context.Gastos.Remove(gasto);
        }

        public void EliminarRango(IEnumerable<Gasto> gastos)
        {
            _context.Gastos.RemoveRange(gastos);
        }
    }
}