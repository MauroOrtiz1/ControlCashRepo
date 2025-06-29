using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControlCash.Application.DTOs;
using ControlCash.Application.UseCases.Gasto;
using ControlCash.Application.Common;
using ControlCash.Domain.Interfaces.Services;
using System.Security.Claims;

namespace ControlCash.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GastosController : ControllerBase
    {
        private readonly ObtenerGastosUseCase _obtenerGastosUseCase;
        private readonly CrearGastoUseCase _crearGastoUseCase;
        private readonly EliminarGastoUseCase _eliminarGastoUseCase;
        private readonly ICurrentUserService _currentUserService;

        public GastosController(
            ObtenerGastosUseCase obtenerGastosUseCase,
            CrearGastoUseCase crearGastoUseCase,
            EliminarGastoUseCase eliminarGastoUseCase,
            ICurrentUserService currentUserService)
        {
            _obtenerGastosUseCase = obtenerGastosUseCase;
            _crearGastoUseCase = crearGastoUseCase;
            _eliminarGastoUseCase = eliminarGastoUseCase;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerGastos()
        {
            var resultado = await _obtenerGastosUseCase.EjecutarAsync(User);
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> CrearGasto([FromBody] GastoCreateDTO dto)
        {
            var userId = _currentUserService.ObtenerId(User);
            var resultado = await _crearGastoUseCase.EjecutarAsync(dto, userId);
            return resultado.Exito ? Ok(resultado.Mensaje) : BadRequest(resultado.Mensaje);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarGasto(int id)
        {
            var userId = _currentUserService.ObtenerId(User);
            var resultado = await _eliminarGastoUseCase.EjecutarAsync(id, userId);
            return resultado.Exito ? Ok(resultado.Mensaje) : NotFound(resultado.Mensaje);
        }
    }
}
