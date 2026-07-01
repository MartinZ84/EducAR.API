namespace EducAR.API.DTOs.Alumnos;

public class AlumnoResponseDto
{
    public int IdAlumno { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public bool Activo { get; set; }
    public List<AlumnoCursoDto> Cursos { get; set; } = new();
    public List<AlumnoTutorDto> Tutores { get; set; } = new();
}