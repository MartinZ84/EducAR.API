namespace EducAR.API.DTOs.Alumnos;

public class AlumnoCreateDto
{
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
}