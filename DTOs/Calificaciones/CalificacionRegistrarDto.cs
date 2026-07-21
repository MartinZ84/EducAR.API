namespace EducAR.API.DTOs.Calificaciones;

public class CalificacionRegistrarDto
{
    public int IdCurso { get; set; }
    public int IdMateria { get; set; }
    public int IdPeriodoEvaluacion { get; set; }
    public List<CalificacionAlumnoDto> Alumnos { get; set; } = new();
}

public class CalificacionAlumnoDto
{
    public int IdAlumno { get; set; }
    public decimal ValorCalificacion { get; set; }
    public string? Observacion { get; set; }
}