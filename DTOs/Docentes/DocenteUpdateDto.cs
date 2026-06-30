namespace EducAR.API.DTOs.Docentes;

public class DocenteUpdateDto
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Activo { get; set; }
}