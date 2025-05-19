namespace ControlCash.DTOs;

public class RegisterAdminRequest
{
    public string Nombre { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Rol { get; set; }
}
