namespace EducAR.API.DTOs.Tutores;

public class TutorUpdateDto
{
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool EsResponsable { get; set; }
    public bool Activo { get; set; }
}