

namespace ControlCash.Domain.Interfaces.Services;

public interface IPdfExportService
{
    Task<(byte[] contenido, string nombreArchivo, string mensaje)> GenerarReportePdfAsync(int mes, int anio, int userId);
}