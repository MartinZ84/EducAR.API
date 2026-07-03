namespace EducAR.API.DTOs.Materias;

public class MateriaUpdateDto
{
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
}