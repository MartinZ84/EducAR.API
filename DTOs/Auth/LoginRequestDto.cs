namespace EducAR.API.DTOs.Auth;

public class LoginRequestDto
{
    public string NombreUsuario { get; set; } = null!;
    public string Contrasena { get; set; } = null!;
    public int IdEscuela { get; set; }
}