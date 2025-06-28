namespace ControlCash.Application.DTOs
{
    public class GastoCreateDTO
    {
        public int IdCategoria { get; set; }
        public decimal Monto { get; set; }
        public string? Descripcion { get; set; }
    }
}