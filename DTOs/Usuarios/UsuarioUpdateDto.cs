namespace EducAR.API.DTOs.Usuarios;

public class UsuarioUpdateDto
{
    public int IdRol { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Activo { get; set; }
}