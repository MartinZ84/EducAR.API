using EducAR.API.DTOs.Alumnos;

namespace EducAR.API.Services.Interfaces;

public interface IAlumnoService
{
    Task<List<AlumnoResponseDto>> ObtenerTodos(int idEscuela);
    Task<AlumnoResponseDto?> ObtenerPorId(int idAlumno, int idEscuela);
    Task<(bool exito, string mensaje, AlumnoResponseDto? alumno)> Crear(AlumnoCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> Actualizar(int idAlumno, int idEscuela, AlumnoUpdateDto dto);
    Task<bool> Eliminar(int idAlumno, int idEscuela);

    Task<(bool exito, string mensaje)> AsignarCurso(int idAlumno, int idEscuela, AsignarCursoDto dto);
    Task<(bool exito, string mensaje)> QuitarCurso(int idAlumno, int idEscuela, int idCurso);

    Task<(bool exito, string mensaje)> AsociarTutor(int idAlumno, int idEscuela, AsociarTutorDto dto);
    Task<(bool exito, string mensaje)> QuitarTutor(int idAlumno, int idEscuela, int idTutor);
}