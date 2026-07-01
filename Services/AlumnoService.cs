using EducAR.API.DTOs.Alumnos;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class AlumnoService : IAlumnoService
{
    private readonly IAlumnoRepository _alumnoRepository;

    public AlumnoService(IAlumnoRepository alumnoRepository)
    {
        _alumnoRepository = alumnoRepository;
    }

    public async Task<List<AlumnoResponseDto>> ObtenerTodos(int idEscuela)
    {
        var alumnos = await _alumnoRepository.ObtenerTodos(idEscuela);
        return alumnos.Select(MapearAResponseDto).ToList();
    }

    public async Task<AlumnoResponseDto?> ObtenerPorId(int idAlumno, int idEscuela)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        return alumno is null ? null : MapearAResponseDto(alumno);
    }

    public async Task<(bool exito, string mensaje, AlumnoResponseDto? alumno)> Crear(AlumnoCreateDto dto, int idEscuela)
    {
        if (await _alumnoRepository.ExisteDni(dto.Dni, idEscuela))
            return (false, "Ya existe un alumno con ese DNI en esta escuela.", null);

        var alumno = new Alumno
        {
            IdEscuela = idEscuela,
            Dni       = dto.Dni,
            Nombre    = dto.Nombre,
            Apellido  = dto.Apellido,
            Activo    = true
        };

        await _alumnoRepository.Crear(alumno);
        return (true, "Alumno creado correctamente.", MapearAResponseDto(alumno));
    }

    public async Task<(bool exito, string mensaje)> Actualizar(int idAlumno, int idEscuela, AlumnoUpdateDto dto)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        if (alumno is null)
            return (false, "Alumno no encontrado.");

        alumno.Nombre   = dto.Nombre;
        alumno.Apellido = dto.Apellido;
        alumno.Activo   = dto.Activo;

        await _alumnoRepository.Actualizar(alumno);
        return (true, "Alumno actualizado correctamente.");
    }

    public async Task<bool> Eliminar(int idAlumno, int idEscuela)
    {
        return await _alumnoRepository.Eliminar(idAlumno, idEscuela);
    }

    // ==========================
    // CURSOS
    // ==========================

    public async Task<(bool exito, string mensaje)> AsignarCurso(int idAlumno, int idEscuela, AsignarCursoDto dto)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        if (alumno is null)
            return (false, "Alumno no encontrado.");

        if (!await _alumnoRepository.ExisteCursoEnEscuela(dto.IdCurso, idEscuela))
            return (false, "El curso no existe o no pertenece a esta escuela.");

        var asignacion = await _alumnoRepository.ObtenerAsignacionCurso(idAlumno, dto.IdCurso);
        if (asignacion is not null)
        {
            if (asignacion.Activo)
                return (false, "El alumno ya está asignado a ese curso.");

            asignacion.Activo = true;
            await _alumnoRepository.GuardarCambios();
            return (true, "Alumno reincorporado al curso correctamente.");
        }

        var nueva = new AlumnoCurso
        {
            IdAlumno = idAlumno,
            IdCurso  = dto.IdCurso,
            Activo   = true
        };

        await _alumnoRepository.AsignarCurso(nueva);
        return (true, "Alumno asignado al curso correctamente.");
    }

    public async Task<(bool exito, string mensaje)> QuitarCurso(int idAlumno, int idEscuela, int idCurso)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        if (alumno is null)
            return (false, "Alumno no encontrado.");

        var exito = await _alumnoRepository.QuitarCurso(idAlumno, idCurso);
        if (!exito)
            return (false, "El alumno no tiene una asignación activa a ese curso.");

        return (true, "Alumno dado de baja del curso correctamente.");
    }

    // ==========================
    // TUTORES
    // ==========================

    public async Task<(bool exito, string mensaje)> AsociarTutor(int idAlumno, int idEscuela, AsociarTutorDto dto)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        if (alumno is null)
            return (false, "Alumno no encontrado.");

        if (!await _alumnoRepository.ExisteTutorEnEscuela(dto.IdTutor, idEscuela))
            return (false, "El tutor no existe o no pertenece a esta escuela.");

        var asociacion = await _alumnoRepository.ObtenerAsociacionTutor(idAlumno, dto.IdTutor);
        if (asociacion is not null)
        {
            if (asociacion.Activo)
                return (false, "El tutor ya está asociado a ese alumno.");

            asociacion.Activo             = true;
            asociacion.Parentesco         = dto.Parentesco;
            asociacion.EsResponsablePrinc = dto.EsResponsablePrinc;
            await _alumnoRepository.GuardarCambios();
            return (true, "Tutor reasociado al alumno correctamente.");
        }

        var nueva = new AlumnoTutor
        {
            IdAlumno            = idAlumno,
            IdTutor             = dto.IdTutor,
            Parentesco          = dto.Parentesco,
            EsResponsablePrinc  = dto.EsResponsablePrinc,
            Activo              = true
        };

        await _alumnoRepository.AsociarTutor(nueva);
        return (true, "Tutor asociado al alumno correctamente.");
    }

    public async Task<(bool exito, string mensaje)> QuitarTutor(int idAlumno, int idEscuela, int idTutor)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        if (alumno is null)
            return (false, "Alumno no encontrado.");

        var exito = await _alumnoRepository.QuitarTutor(idAlumno, idTutor);
        if (!exito)
            return (false, "El tutor no tiene una asociación activa con ese alumno.");

        return (true, "Tutor desasociado del alumno correctamente.");
    }

    private static AlumnoResponseDto MapearAResponseDto(Alumno a) => new()
    {
        IdAlumno = a.IdAlumno,
        Dni      = a.Dni,
        Nombre   = a.Nombre,
        Apellido = a.Apellido,
        Activo   = a.Activo,
        Cursos   = a.AlumnoCursos?.Select(ac => new AlumnoCursoDto
        {
            IdAlumnoCurso = ac.IdAlumnoCurso,
            IdCurso       = ac.IdCurso,
            Grado         = ac.Curso.Grado,
            Division      = ac.Curso.Division,
            Turno         = ac.Curso.Turno,
            Activo        = ac.Activo
        }).ToList() ?? new(),
        Tutores  = a.AlumnoTutores?.Select(at => new AlumnoTutorDto
        {
            IdAlumnoTutor      = at.IdAlumnoTutor,
            IdTutor            = at.IdTutor,
            Nombre             = at.Tutor.Usuario.Nombre,
            Apellido           = at.Tutor.Usuario.Apellido,
            Parentesco         = at.Parentesco,
            EsResponsablePrinc = at.EsResponsablePrinc,
            Activo             = at.Activo
        }).ToList() ?? new()
    };
}