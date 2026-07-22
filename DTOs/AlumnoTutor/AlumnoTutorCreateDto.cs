namespace EducAR.API.DTOs.AlumnoTutor;

public class AlumnoTutorCreateDto
{
    public int IdAlumno { get; set; }
    public int IdTutor { get; set; }
    public string? Parentesco { get; set; }
    public bool EsResponsablePrinc { get; set; }
}