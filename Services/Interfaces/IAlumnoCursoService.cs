using EducAR.API.DTOs.AlumnoCurso;

namespace EducAR.API.Services.Interfaces;

public interface IAlumnoCursoService
{
    Task<List<AlumnoCursoResponseDto>> ObtenerPorCurso(int idCurso, int idEscuela);
    Task<List<AlumnoCursoResponseDto>> ObtenerPorAlumno(int idAlumno, int idEscuela);
    Task<(bool exito, string mensaje, AlumnoCursoResponseDto? inscripcion)> Inscribir(AlumnoCursoCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> Desinscribir(int idAlumnoCurso, int idEscuela);
}