namespace EducAR.API.DTOs.PeriodosEvaluacion;

public class PeriodoEvaluacionCreateDto
{
    public int IdCicloLectivo { get; set; }
    public string Nombre { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
}