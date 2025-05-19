namespace ControlCash.DTOs
{
    public class GastoResponseDTO
    {
        public int IdGasto { get; set; }
        public decimal Monto { get; set; }
        public string Categoria { get; set; }
        public DateOnly Fecha { get; set; }
        public string? Descripcion { get; set; }
    }
}