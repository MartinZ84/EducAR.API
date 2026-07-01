namespace EducAR.API.DTOs.Tutores;

public class TutorCreateDto
{
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public string Contrasena { get; set; } = null!;
    public bool EsResponsable { get; set; } = true;
}