namespace EducAR.API.DTOs.Escuelas;

public class EscuelaCreateDto
{
    public string Nombre { get; set; } = null!;
    public string Direccion { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
}