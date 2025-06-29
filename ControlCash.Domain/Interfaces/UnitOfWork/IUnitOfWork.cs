using System.Threading.Tasks;
using ControlCash.Domain.Interfaces.Repositories;

namespace ControlCash.Domain.Interfaces.UnitOfWork
{
    public interface IUnitOfWork
    {
        IUsuarioRepository UsuarioRepository { get; }
        IGastoRepository GastoRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }


        Task CommitAsync();
        Task GuardarCambiosAsync(); 
    }
}