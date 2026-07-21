namespace EducAR.API.DTOs.Asistencia;

public class AsistenciaRegistrarDto
{
    public int IdCurso { get; set; }
    public DateTime Fecha { get; set; }
    public List<AsistenciaAlumnoDto> Alumnos { get; set; } = new();
}

public class AsistenciaAlumnoDto
{
    public int IdAlumno { get; set; }
    public bool Presente { get; set; }
}