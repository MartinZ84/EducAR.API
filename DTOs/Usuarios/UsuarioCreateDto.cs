namespace EducAR.API.DTOs.Usuarios;

public class UsuarioCreateDto
{
    public int IdRol { get; set; }
    public int IdEscuela { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public string Contrasena { get; set; } = null!;
}