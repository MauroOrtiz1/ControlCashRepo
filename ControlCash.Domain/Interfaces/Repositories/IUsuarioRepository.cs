using ControlCash.Domain.Entities;

namespace ControlCash.Domain.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<bool> EmailExisteAsync(string email);
    Task<Usuario?> ObtenerPorEmailAsync(string email);
    Task<Usuario?> ObtenerPorIdAsync(int id);
    Task<List<Usuario>> ObtenerTodosAsync();
    Task AgregarAsync(Usuario usuario);
    Task EliminarAsync(Usuario usuario);
}