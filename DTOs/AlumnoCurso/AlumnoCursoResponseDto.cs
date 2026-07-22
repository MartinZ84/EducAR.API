namespace EducAR.API.DTOs.AlumnoCurso;

public class AlumnoCursoResponseDto
{
    public int IdAlumnoCurso { get; set; }
    public int IdAlumno { get; set; }
    public string NombreAlumno { get; set; } = null!;
    public string ApellidoAlumno { get; set; } = null!;
    public int IdCurso { get; set; }
    public string Curso { get; set; } = null!;
    public bool Activo { get; set; }
}