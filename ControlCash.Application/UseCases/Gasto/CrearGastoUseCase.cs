using ControlCash.Application.DTOs;
using ControlCash.Application.Common;
using ControlCash.Domain.Entities;
using ControlCash.Domain.Interfaces.UnitOfWork;

namespace ControlCash.Application.UseCases.Gasto
{
    public class CrearGastoUseCase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CrearGastoUseCase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResultadoOperacion> EjecutarAsync(GastoCreateDTO dto, int userId)
        {
            var categoria = await _unitOfWork.CategoriaRepository.ObtenerPorIdAsync(dto.IdCategoria);
            if (categoria == null)
                return ResultadoOperacion.CrearFalla("La categoría especificada no existe.");

            var gasto = new Domain.Entities.Gasto
            {
                IdUsuario = userId,
                IdCategoria = dto.IdCategoria,
                Monto = dto.Monto,
                Fecha = DateOnly.FromDateTime(DateTime.Now),
                Descripcion = dto.Descripcion
            };

            await _unitOfWork.GastoRepository.AgregarAsync(gasto);
            await _unitOfWork.GuardarCambiosAsync();

            return ResultadoOperacion.CrearExito("Gasto creado exitosamente.");
        }
    }
}
