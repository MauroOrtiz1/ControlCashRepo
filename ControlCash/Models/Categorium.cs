using System;
using System.Collections.Generic;

namespace ControlCash.Models;

public partial class Categorium
{
    public int IdCategoria { get; set; }

    public string NombreCategoria { get; set; } = null!;

    public int IdUsuario { get; set; }

    public virtual Usuario Usuario { get; set; } = null!;

    public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();
}
