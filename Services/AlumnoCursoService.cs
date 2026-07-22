using EducAR.API.DTOs.AlumnoCurso;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class AlumnoCursoService : IAlumnoCursoService
{
    private readonly IAlumnoCursoRepository _alumnoCursoRepository;
    private readonly ICursoRepository _cursoRepository;

    public AlumnoCursoService(
        IAlumnoCursoRepository alumnoCursoRepository,
        ICursoRepository cursoRepository)
    {
        _alumnoCursoRepository = alumnoCursoRepository;
        _cursoRepository = cursoRepository;
    }

    public async Task<List<AlumnoCursoResponseDto>> ObtenerPorCurso(int idCurso, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return new List<AlumnoCursoResponseDto>();

        var inscripciones = await _alumnoCursoRepository.ObtenerPorCurso(idCurso);
        return inscripciones.Select(MapearAResponseDto).ToList();
    }

    public async Task<List<AlumnoCursoResponseDto>> ObtenerPorAlumno(int idAlumno, int idEscuela)
    {
        var inscripciones = await _alumnoCursoRepository.ObtenerPorAlumno(idAlumno, idEscuela);
        return inscripciones.Select(MapearAResponseDto).ToList();
    }
  

    public async Task<(bool exito, string mensaje, AlumnoCursoResponseDto? inscripcion)> Inscribir(AlumnoCursoCreateDto dto, int idEscuela)
    {
        // Validar que el curso existe y pertenece a la escuela
        var curso = await _cursoRepository.ObtenerPorId(dto.IdCurso, idEscuela);
        if (curso is null)
            return (false, "El curso no existe.", null);

        if (!curso.Activo)
            return (false, "No se puede inscribir alumnos en un curso inactivo.", null);

        // Control 1: mismo alumno, mismo curso
        if (await _alumnoCursoRepository.ExisteInscripcion(dto.IdAlumno, dto.IdCurso))
            return (false, "El alumno ya está inscripto en este curso.", null);

        // Control 2: mismo alumno, mismo ciclo lectivo, misma escuela
        if (await _alumnoCursoRepository.EstaInscriptoEnCiclo(dto.IdAlumno, curso.IdCicloLectivo, idEscuela))
            return (false, $"El alumno ya está inscripto en otro curso del ciclo lectivo {curso.CicloLectivo.Anio}. " +
                        $"Debe desinscribirlo primero.", null);

        var inscripcion = new AlumnoCurso
        {
            IdAlumno = dto.IdAlumno,
            IdCurso  = dto.IdCurso,
            Activo   = true
        };

        var creada = await _alumnoCursoRepository.Crear(inscripcion);
        var completa = await _alumnoCursoRepository.ObtenerPorId(creada.IdAlumnoCurso);
        return (true, "Alumno inscripto correctamente.", MapearAResponseDto(completa!));
    }

    public async Task<(bool exito, string mensaje)> Desinscribir(int idAlumnoCurso, int idEscuela)
    {
        var inscripcion = await _alumnoCursoRepository.ObtenerPorId(idAlumnoCurso);
        if (inscripcion is null)
            return (false, "Inscripción no encontrada.");

        if (inscripcion.Curso.IdEscuela != idEscuela)
            return (false, "No tiene permisos para modificar esta inscripción.");

        var exito = await _alumnoCursoRepository.Eliminar(idAlumnoCurso, idEscuela);
        return exito
            ? (true, "Alumno desinscripto correctamente.")
            : (false, "No se pudo desinscribir al alumno.");
    }

    private static AlumnoCursoResponseDto MapearAResponseDto(AlumnoCurso ac) => new()
    {
        IdAlumnoCurso  = ac.IdAlumnoCurso,
        IdAlumno       = ac.IdAlumno,
        NombreAlumno   = ac.Alumno.Nombre,
        ApellidoAlumno = ac.Alumno.Apellido,
        IdCurso        = ac.IdCurso,
        Curso          = $"{ac.Curso.Grado}° {ac.Curso.Division} - {ac.Curso.CicloLectivo.Anio}",
        Activo         = ac.Activo
    };
}