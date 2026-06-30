using System.ComponentModel.DataAnnotations;

namespace EducAR.API.Models;

public class AlumnoCurso
{
    [Key]
    public int IdAlumnoCurso { get; set; }
    public int IdAlumno { get; set; }
    public int IdCurso { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCrea { get; set; } = DateTime.Now;
    public DateTime FechaAct { get; set; } = DateTime.Now;

    // Navegación
    public Alumno Alumno { get; set; } = null!;
    public Curso Curso { get; set; } = null!;
}