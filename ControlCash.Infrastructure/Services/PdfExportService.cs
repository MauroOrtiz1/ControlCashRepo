using ControlCash.Domain.Entities;
using ControlCash.Domain.Interfaces.Services;
using ControlCash.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Globalization;

namespace ControlCash.Infrastructure.Services;

public class PdfExportService : IPdfExportService
{
    private readonly ControlCashDbContext _context;

    public PdfExportService(ControlCashDbContext context)
    {
        _context = context;
    }

    public async Task<(byte[] contenido, string nombreArchivo, string mensaje)> GenerarReportePdfAsync(int mes, int anio, int userId)
    {
        var startDate = new DateTime(anio, mes, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var gastos = await _context.Gastos
            .Where(g => g.IdUsuario == userId &&
                        g.Fecha.ToDateTime(TimeOnly.MinValue) >= startDate &&
                        g.Fecha.ToDateTime(TimeOnly.MinValue) <= endDate)
            .Include(g => g.IdCategoriaNavigation)
            .OrderBy(g => g.IdCategoriaNavigation.NombreCategoria)
            .ThenBy(g => g.Fecha)
            .ToListAsync();

        if (!gastos.Any())
            return (Array.Empty<byte>(), string.Empty, "No se encontraron gastos para el período seleccionado.");

        using var stream = new MemoryStream();
        using var document = new PdfDocument();

        var page = document.AddPage();
        page.Size = PdfSharpCore.PageSize.A4;
        var gfx = XGraphics.FromPdfPage(page);

        var titleFont = new XFont("Arial", 16, XFontStyle.Bold);
        var headerFont = new XFont("Arial", 12, XFontStyle.Bold);
        var subHeaderFont = new XFont("Arial", 10, XFontStyle.Bold);
        var regularFont = new XFont("Arial", 9, XFontStyle.Regular);
        var smallFont = new XFont("Arial", 8, XFontStyle.Regular);

        double currentY = 40.0;
        double leftMargin = 40.0;
        double rightMargin = page.Width - 40;
        double pageWidth = page.Width - 80;

        DibujarEncabezado(gfx, titleFont, headerFont, regularFont, mes, anio, leftMargin, rightMargin, ref currentY);

        var gastosPorCategoria = gastos.GroupBy(g => g.IdCategoriaNavigation)
            .Select(g => new ResumenCategoria
            {
                Categoria = g.Key.NombreCategoria,
                Total = g.Sum(x => x.Monto),
                Cantidad = g.Count()
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        currentY += 20;
        DibujarResumenCategorias(gfx, subHeaderFont, regularFont, gastosPorCategoria, leftMargin, pageWidth, ref currentY);
        currentY += 30;
        DibujarGraficoBarras(gfx, gastosPorCategoria, leftMargin, pageWidth, ref currentY);
        currentY += 40;
        DibujarDetalleGastos(gfx, subHeaderFont, regularFont, smallFont, gastos, leftMargin, pageWidth, currentY, page);
        DibujarPiePagina(gfx, smallFont, page);

        document.Save(stream, false);
        var fileName = $"reporte_gastos_{mes:D2}_{anio}.pdf";

        // Registro en BD
        var exportacion = new Exportacion
        {
            IdUsuario = userId,
            TipoArchivo = "PDF",
            OrigenGrafico = "Reporte de Gastos por Categoría",
            FechaExportado = DateTime.UtcNow,
            MesExportado = mes,
            AnioExportado = anio
        };

        _context.Exportacions.Add(exportacion);
        await _context.SaveChangesAsync();

        return (stream.ToArray(), fileName, "Reporte generado exitosamente.");
    }

    // ------------------ Métodos auxiliares ----------------------------

    private void DibujarEncabezado(
        XGraphics gfx, XFont titleFont, XFont headerFont, XFont regularFont,
        int mes, int anio, double leftMargin, double rightMargin, ref double currentY)
    {
        var mesNombre = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(mes);
        var titulo = $"REPORTE DE GASTOS - {mesNombre.ToUpper()} {anio}";
        gfx.DrawString(titulo, titleFont, XBrushes.DarkBlue, new XPoint(leftMargin, currentY));
        currentY += 25;

        gfx.DrawLine(new XPen(XColors.DarkBlue, 2), leftMargin, currentY, rightMargin, currentY);
        currentY += 15;

        var fechaGeneracion = $"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}";
        gfx.DrawString(fechaGeneracion, regularFont, XBrushes.Gray, new XPoint(leftMargin, currentY));
        currentY += 20;

        gfx.DrawLine(new XPen(XColors.LightGray, 1), leftMargin, currentY, rightMargin, currentY);
    }

    private void DibujarResumenCategorias(
        XGraphics gfx, XFont subHeaderFont, XFont regularFont, List<ResumenCategoria> gastosPorCategoria,
        double leftMargin, double pageWidth, ref double currentY)
    {
        gfx.DrawString("RESUMEN POR CATEGORÍAS", subHeaderFont, XBrushes.DarkBlue, new XPoint(leftMargin, currentY));
        currentY += 20;

        var col1Width = pageWidth * 0.5;
        var col2Width = pageWidth * 0.25;
        var col3Width = pageWidth * 0.25;

        gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(230, 230, 230)),
            leftMargin, currentY - 15, pageWidth, 18);
        gfx.DrawString("Categoría", regularFont, XBrushes.Black, new XPoint(leftMargin + 5, currentY));
        gfx.DrawString("Cantidad", regularFont, XBrushes.Black, new XPoint(leftMargin + col1Width + 5, currentY));
        gfx.DrawString("Total", regularFont, XBrushes.Black, new XPoint(leftMargin + col1Width + col2Width + 5, currentY));
        currentY += 20;

        decimal totalGeneral = 0m;
        int cantidadTotal = 0;
        int contador = 0;

        foreach (var categoria in gastosPorCategoria)
        {
            if (contador % 2 == 0)
            {
                gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(248, 248, 248)),
                    leftMargin, currentY - 12, pageWidth, 15);
            }

            gfx.DrawString(categoria.Categoria, regularFont, XBrushes.Black, new XPoint(leftMargin + 5, currentY));
            gfx.DrawString(categoria.Cantidad.ToString(), regularFont, XBrushes.Black, new XPoint(leftMargin + col1Width + 5, currentY));
            gfx.DrawString(categoria.Total.ToString("C", CultureInfo.CurrentCulture), regularFont, XBrushes.Black,
                new XPoint(leftMargin + col1Width + col2Width + 5, currentY));

            totalGeneral += categoria.Total;
            cantidadTotal += categoria.Cantidad;
            currentY += 18;
            contador++;
        }

        currentY += 5;
        gfx.DrawLine(new XPen(XColors.Black, 1), leftMargin, currentY, leftMargin + pageWidth, currentY);
        currentY += 15;

        gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(220, 220, 220)),
            leftMargin, currentY - 12, pageWidth, 15);
        gfx.DrawString("TOTAL GENERAL", subHeaderFont, XBrushes.DarkBlue, new XPoint(leftMargin + 5, currentY));
        gfx.DrawString(cantidadTotal.ToString(), subHeaderFont, XBrushes.DarkBlue, new XPoint(leftMargin + col1Width + 5, currentY));
        gfx.DrawString(totalGeneral.ToString("C", CultureInfo.CurrentCulture), subHeaderFont, XBrushes.DarkBlue,
            new XPoint(leftMargin + col1Width + col2Width + 5, currentY));
    }

    private void DibujarGraficoBarras(
        XGraphics gfx, List<ResumenCategoria> gastosPorCategoria, double leftMargin, double pageWidth, ref double currentY)
    {
        if (!gastosPorCategoria.Any()) return;

        var maxMonto = gastosPorCategoria.Max(x => x.Total);
        var chartHeight = 120.0;
        var chartWidth = pageWidth * 0.8;
        var barWidth = Math.Min(chartWidth / gastosPorCategoria.Count * 0.7, 60);
        var spacing = (chartWidth - (barWidth * gastosPorCategoria.Count)) / (gastosPorCategoria.Count + 1);

        gfx.DrawString("DISTRIBUCIÓN POR CATEGORÍAS", new XFont("Arial", 10, XFontStyle.Bold),
            XBrushes.DarkBlue, new XPoint(leftMargin, currentY));
        currentY += 25;

        gfx.DrawRectangle(new XPen(XColors.Gray, 1), leftMargin, currentY, chartWidth, chartHeight);

        var colors = new XColor[] { XColors.SteelBlue, XColors.Orange, XColors.Green, XColors.Red,
                                      XColors.Purple, XColors.Brown, XColors.Pink, XColors.Gray };

        for (int i = 0; i < gastosPorCategoria.Count; i++)
        {
            var categoria = gastosPorCategoria[i];
            var barHeight = maxMonto > 0 ? (double)(categoria.Total / maxMonto) * (chartHeight - 20) : 0;
            var x = leftMargin + spacing + (i * (barWidth + spacing));
            var y = currentY + chartHeight - 10 - barHeight;

            var color = colors[i % colors.Length];
            gfx.DrawRectangle(new XSolidBrush(color), x, y, barWidth, barHeight);

            if (barHeight > 20)
            {
                var montoText = categoria.Total.ToString("C0");
                var textSize = gfx.MeasureString(montoText, new XFont("Arial", 8));
                gfx.DrawString(montoText, new XFont("Arial", 8, XFontStyle.Bold), XBrushes.White,
                    new XPoint(x + (barWidth - textSize.Width) / 2, y + barHeight / 2));
            }
        }

        currentY += chartHeight + 10;

        var legendX = leftMargin;
        var legendY = currentY;
        for (int i = 0; i < gastosPorCategoria.Count && i < 4; i++)
        {
            var categoria = gastosPorCategoria[i];
            var color = colors[i % colors.Length];

            gfx.DrawRectangle(new XSolidBrush(color), legendX, legendY - 8, 12, 12);
            gfx.DrawString(categoria.Categoria, new XFont("Arial", 8), XBrushes.Black,
                new XPoint(legendX + 16, legendY));

            legendX += Math.Min(categoria.Categoria.Length * 6 + 30, 140);

            if (legendX > leftMargin + pageWidth - 100 && i < gastosPorCategoria.Count - 1)
            {
                legendX = leftMargin;
                legendY += 15;
            }
        }
    }

    private void DibujarDetalleGastos(
        XGraphics gfx, XFont subHeaderFont, XFont regularFont, XFont smallFont,
        List<Gasto> gastos, double leftMargin, double pageWidth, double initialY, PdfPage page)
    {
        var currentY = initialY;

        if (currentY > page.Height - 200)
        {
            var newPage = page.Owner.AddPage();
            newPage.Size = PdfSharpCore.PageSize.A4;
            gfx = XGraphics.FromPdfPage(newPage);
            currentY = 40;
        }

        currentY += 20;
        gfx.DrawString("DETALLE DE GASTOS", subHeaderFont, XBrushes.DarkBlue, new XPoint(leftMargin, currentY));
        currentY += 25;

        var gastosPorCategoria = gastos.GroupBy(g => g.IdCategoriaNavigation).OrderBy(g => g.Key.NombreCategoria);

        foreach (var categoriaGrupo in gastosPorCategoria)
        {
            if (currentY > page.Height - 150)
            {
                var newPage = page.Owner.AddPage();
                newPage.Size = PdfSharpCore.PageSize.A4;
                gfx = XGraphics.FromPdfPage(newPage);
                currentY = 40;
            }

            gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(240, 240, 240)),
                leftMargin, currentY - 12, pageWidth, 20);
            gfx.DrawString($"📋 {categoriaGrupo.Key.NombreCategoria}", subHeaderFont, XBrushes.DarkBlue,
                new XPoint(leftMargin + 5, currentY));

            var totalCategoria = categoriaGrupo.Sum(g => g.Monto);
            gfx.DrawString($"Total: {totalCategoria:C}", subHeaderFont, XBrushes.DarkBlue,
                new XPoint(leftMargin + pageWidth - 80, currentY));
            currentY += 25;

            gfx.DrawString("Fecha", regularFont, XBrushes.Black, new XPoint(leftMargin + 5, currentY));
            gfx.DrawString("Descripción", regularFont, XBrushes.Black, new XPoint(leftMargin + 80, currentY));
            gfx.DrawString("Monto", regularFont, XBrushes.Black, new XPoint(leftMargin + pageWidth - 80, currentY));
            currentY += 15;

            gfx.DrawLine(new XPen(XColors.LightGray, 1), leftMargin, currentY, leftMargin + pageWidth, currentY);
            currentY += 5;

            foreach (var gasto in categoriaGrupo.OrderBy(g => g.Fecha))
            {
                if (currentY > page.Height - 50)
                {
                    var newPage = page.Owner.AddPage();
                    newPage.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(newPage);
                    currentY = 40;
                }

                gfx.DrawString(gasto.Fecha.ToString("dd/MM"), smallFont, XBrushes.Black,
                    new XPoint(leftMargin + 5, currentY));

                var descripcion = gasto.Descripcion.Length > 45 ?
                    gasto.Descripcion.Substring(0, 42) + "..." : gasto.Descripcion;
                gfx.DrawString(descripcion, smallFont, XBrushes.Black,
                    new XPoint(leftMargin + 80, currentY));

                gfx.DrawString(gasto.Monto.ToString("C"), smallFont, XBrushes.Black,
                    new XPoint(leftMargin + pageWidth - 80, currentY));

                currentY += 15;
            }

            currentY += 10;
        }
    }

    private void DibujarPiePagina(XGraphics gfx, XFont smallFont, PdfPage page)
    {
        var footerY = page.Height - 30;
        var centerX = page.Width / 2;

        gfx.DrawLine(new XPen(XColors.LightGray, 1), 40, footerY - 10, page.Width - 40, footerY - 10);

        var footerText = $"Reporte generado automáticamente - {DateTime.Now:dd/MM/yyyy HH:mm}";
        var textSize = gfx.MeasureString(footerText, smallFont);
        gfx.DrawString(footerText, smallFont, XBrushes.Gray,
            new XPoint(centerX - textSize.Width / 2, footerY));
    }
}

// Puedes poner esta clase interna si solo la usas aquí.
public class ResumenCategoria
{
    public string Categoria { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Cantidad { get; set; }
}
