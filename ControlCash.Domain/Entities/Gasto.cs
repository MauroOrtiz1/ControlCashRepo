using System;
using System.Collections.Generic;

namespace ControlCash.Domain.Entities;

public partial class Gasto
{
    public int IdGasto { get; set; }

    public int IdUsuario { get; set; }

    public int IdCategoria { get; set; }

    public decimal Monto { get; set; }

    public DateOnly Fecha { get; set; }

    public string? Descripcion { get; set; }

    public virtual Categorium IdCategoriaNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
