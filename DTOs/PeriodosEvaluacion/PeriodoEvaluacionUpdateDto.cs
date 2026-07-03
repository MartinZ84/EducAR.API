namespace EducAR.API.DTOs.PeriodosEvaluacion;

public class PeriodoEvaluacionUpdateDto
{
    public string Nombre { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activo { get; set; }
}