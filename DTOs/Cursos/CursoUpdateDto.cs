namespace EducAR.API.DTOs.Cursos;

public class CursoUpdateDto
{
    public int Grado { get; set; }
    public string Division { get; set; } = null!;
    public string? Turno { get; set; }
    public bool Activo { get; set; }
}