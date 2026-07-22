namespace EducAR.API.DTOs.Usuarios;

public class CambiarContrasenaDto
{
    public string ContrasenaActual { get; set; } = null!;
    public string NuevaContrasena { get; set; } = null!;
    public string ConfirmarContrasena { get; set; } = null!;
}