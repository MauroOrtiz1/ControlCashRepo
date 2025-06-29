
using ControlCash.Application.DTOs;
using ControlCash.Domain.Interfaces.Services;

namespace ControlCash.Application.UseCases.Exportacion;

public class GenerarReportePdfUseCase
{
    private readonly IPdfExportService _pdfExportService;

    public GenerarReportePdfUseCase(IPdfExportService pdfExportService)
    {
        _pdfExportService = pdfExportService;
    }

    public async Task<(byte[] contenido, string nombreArchivo, string mensaje)> EjecutarAsync(ExportacionRequest request, int userId)
    {
        return await _pdfExportService.GenerarReportePdfAsync(request.Mes, request.Anio, userId);
    }
}