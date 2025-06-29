using ControlCash.Domain.Entities;

namespace ControlCash.Domain.Interfaces.Repositories
{
    public interface IGastoRepository
    {
        Task<List<Gasto>> ObtenerGastosPorUsuarioAsync(int userId);
        Task<Gasto?> ObtenerPorIdYUsuarioAsync(int gastoId, int userId);
        Task AgregarAsync(Gasto gasto);
        void Eliminar(Gasto gasto);
        void EliminarRango(IEnumerable<Gasto> gastos);
    }
}