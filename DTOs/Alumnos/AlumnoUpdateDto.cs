namespace EducAR.API.DTOs.Alumnos;

public class AlumnoUpdateDto
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public bool Activo { get; set; }
}