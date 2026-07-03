namespace EducAR.API.DTOs.CiclosLectivos;

public class CicloLectivoUpdateDto
{
    public int Anio { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool Activo { get; set; }
}