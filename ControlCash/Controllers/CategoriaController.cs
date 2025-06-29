using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControlCash.Application.DTOs;
using ControlCash.Application.UseCases.Categoria;
using System.Security.Claims;

namespace ControlCash.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly ObtenerCategoriasUseCase _obtenerUseCase;
        private readonly CrearCategoriaUseCase _crearUseCase;
        private readonly ActualizarCategoriaUseCase _actualizarUseCase;
        private readonly EliminarCategoriaUseCase _eliminarUseCase;

        public CategoriaController(
            ObtenerCategoriasUseCase obtenerUseCase,
            CrearCategoriaUseCase crearUseCase,
            ActualizarCategoriaUseCase actualizarUseCase,
            EliminarCategoriaUseCase eliminarUseCase)
        {
            _obtenerUseCase = obtenerUseCase;
            _crearUseCase = crearUseCase;
            _actualizarUseCase = actualizarUseCase;
            _eliminarUseCase = eliminarUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var resultado = await _obtenerUseCase.EjecutarAsync(User);
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CategoriaCreateDto dto)
        {
            var resultado = await _crearUseCase.EjecutarAsync(dto, User);
            return resultado.Exito ? Ok(resultado.Mensaje) : BadRequest(resultado.Mensaje);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] CategoriaUpdateDto dto)
        {
            var resultado = await _actualizarUseCase.EjecutarAsync(id, dto, User);
            return resultado.Exito ? Ok(resultado.Mensaje) : NotFound(resultado.Mensaje);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _eliminarUseCase.EjecutarAsync(id, User);
            return resultado.Exito ? Ok(resultado.Mensaje) : NotFound(resultado.Mensaje);
        }
    }
}
