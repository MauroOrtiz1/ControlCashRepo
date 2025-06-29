using ControlCash.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlCash.Domain.Interfaces.Repositories
{
    public interface ICategoriaRepository
    {
        Task<List<Categorium>> ObtenerPorUsuarioAsync(int userId);
        Task<Categorium?> ObtenerPorIdYUsuarioAsync(int id, int userId);
        Task<Categorium?> ObtenerPorIdAsync(int id);
        Task AgregarAsync(Categorium categoria);
        Task ActualizarAsync(Categorium categoria);
        Task EliminarAsync(Categorium categoria);
        Task<Categorium?> ObtenerConGastosAsync(int idCategoria, int idUsuario);
    }

}