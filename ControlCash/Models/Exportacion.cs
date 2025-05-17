using System;
using System.Collections.Generic;

namespace ControlCash.Models;

public partial class Exportacion
{
    public int IdExportacion { get; set; }

    public int IdUsuario { get; set; }

    public string TipoArchivo { get; set; } = null!;

    public string? OrigenGrafico { get; set; }

    public DateTime? FechaExportado { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
