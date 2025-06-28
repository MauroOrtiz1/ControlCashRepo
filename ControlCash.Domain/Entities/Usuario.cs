using System;
using System.Collections.Generic;

namespace ControlCash.Domain.Entities;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Rol { get; set; } = "user"; 

    public bool? EsPremium { get; set; }

    public bool? AnunciosActivos { get; set; }

    public DateOnly? FechaRegistro { get; set; }

    public virtual ICollection<Exportacion> Exportacions { get; set; } = new List<Exportacion>();

    public virtual ICollection<Gasto> Gastos { get; set; } = new List<Gasto>();

    public virtual ICollection<Categorium> Categorias { get; set; } = new List<Categorium>();
}

