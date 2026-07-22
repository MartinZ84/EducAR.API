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
        var curso = await _cursoRepository.ObtenerPorId(dto.IdCurso, idEscuela);
        if (curso is null)
            return (false, "El curso no existe.", null);

        if (!curso.Activo)
            return (false, "No se puede inscribir alumnos en un curso inactivo.", null);

        // Validar inscripción activa duplicada
        if (await _alumnoCursoRepository.ExisteInscripcion(dto.IdAlumno, dto.IdCurso))
            return (false, "El alumno ya está inscripto en este curso.", null);

        // Validar mismo ciclo lectivo
        if (await _alumnoCursoRepository.EstaInscriptoEnCiclo(dto.IdAlumno, curso.IdCicloLectivo, idEscuela))
            return (false, $"El alumno ya está inscripto en otro curso del ciclo lectivo {curso.CicloLectivo.Anio}. " +
                        $"Debe desinscribirlo primero.", null);

        // Reactivar si existe inactiva
        var inactiva = await _alumnoCursoRepository.ObtenerInscripcionInactiva(dto.IdAlumno, dto.IdCurso);
        if (inactiva is not null)
        {
            var reactivada = await _alumnoCursoRepository.Reactivar(inactiva);
            var completaReactivada = await _alumnoCursoRepository.ObtenerPorId(reactivada.IdAlumnoCurso);
            return (true, "Inscripción reactivada correctamente.", MapearAResponseDto(completaReactivada!));
        }

        // Crear nueva inscripción
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