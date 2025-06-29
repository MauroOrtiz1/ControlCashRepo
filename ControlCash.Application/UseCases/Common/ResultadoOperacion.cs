namespace ControlCash.Application.Common;

public class ResultadoOperacion
{
    public bool Exito { get; set; }
    public string Mensaje { get; set; }

    public static ResultadoOperacion CrearExito(string mensaje) =>
        new() { Exito = true, Mensaje = mensaje };

    public static ResultadoOperacion CrearFalla(string mensaje) =>
        new() { Exito = false, Mensaje = mensaje };
}