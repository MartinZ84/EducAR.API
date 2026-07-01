namespace EducAR.API.DTOs.Alumnos;

public class AlumnoCursoDto
{
    public int IdAlumnoCurso { get; set; }
    public int IdCurso { get; set; }
    public int Grado { get; set; }
    public string Division { get; set; } = null!;
    public string? Turno { get; set; }
    public bool Activo { get; set; }
}