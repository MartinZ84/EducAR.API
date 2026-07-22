using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IAlumnoCursoRepository
{
    Task<List<AlumnoCurso>> ObtenerPorCurso(int idCurso);
    Task<List<AlumnoCurso>> ObtenerPorAlumno(int idAlumno, int idEscuela);
    Task<AlumnoCurso?> ObtenerPorId(int idAlumnoCurso);
    Task<bool> ExisteInscripcion(int idAlumno, int idCurso);
    Task<AlumnoCurso> Crear(AlumnoCurso alumnoCurso);
    Task<bool> Eliminar(int idAlumnoCurso, int idEscuela);
    Task<bool> EstaInscriptoEnCiclo(int idAlumno, int idCicloLectivo, int idEscuela);
}