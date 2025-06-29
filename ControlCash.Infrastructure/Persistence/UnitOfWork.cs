using System.Threading.Tasks;
using ControlCash.Domain.Interfaces.Repositories;
using ControlCash.Domain.Interfaces.UnitOfWork;

namespace ControlCash.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ControlCashDbContext _context;

        public IUsuarioRepository UsuarioRepository { get; }
        public IGastoRepository GastoRepository { get; }
        public ICategoriaRepository CategoriaRepository { get; }

        public UnitOfWork(ControlCashDbContext context)
        {
            _context = context;
            UsuarioRepository = new UsuarioRepository(_context);
            GastoRepository = new GastoRepository(_context);
            CategoriaRepository = new CategoriaRepository(_context); 
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync() // 👈 Agrega este método
        {
            await _context.SaveChangesAsync();
        }
    }
}