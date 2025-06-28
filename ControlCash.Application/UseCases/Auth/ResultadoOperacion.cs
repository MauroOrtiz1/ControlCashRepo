namespace ControlCash.Application.UseCases.Auth;

public class ResultadoOperacion
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = string.Empty;

    public static ResultadoOperacion CrearExito(string mensaje) =>
        new ResultadoOperacion { Exito = true, Mensaje = mensaje };

    public static ResultadoOperacion CrearFalla(string mensaje) =>
        new ResultadoOperacion { Exito = false, Mensaje = mensaje };
}