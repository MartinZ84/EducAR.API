namespace EducAR.API.DTOs.Cursos;

public class CursoCreateDto
{
    public int IdCicloLectivo { get; set; }
    public int Grado { get; set; }
    public string Division { get; set; } = null!;
    public string? Turno { get; set; }
}