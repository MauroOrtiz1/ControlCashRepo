using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ControlCash.Application.DTOs;
using ControlCash.Application.UseCases.Exportacion;
using ControlCash.Domain.Interfaces.Services;

namespace ControlCash.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExportacionController : ControllerBase
{
    private readonly GenerarReportePdfUseCase _generarReportePdfUseCase;
    private readonly ICurrentUserService _currentUserService;

    public ExportacionController(
        GenerarReportePdfUseCase generarReportePdfUseCase,
        ICurrentUserService currentUserService)
    {
        _generarReportePdfUseCase = generarReportePdfUseCase;
        _currentUserService = currentUserService;
    }

    [HttpPost("generar-pdf")]
    public async Task<IActionResult> GenerarPdf([FromBody] ExportacionRequest request)
    {
        var userId = _currentUserService.ObtenerId(User);
        var (contenido, nombreArchivo, mensaje) = await _generarReportePdfUseCase.EjecutarAsync(request, userId);

        if (contenido == null || contenido.Length == 0)
            return Ok(new { message = mensaje });

        return File(contenido, "application/pdf", nombreArchivo);
    }
}