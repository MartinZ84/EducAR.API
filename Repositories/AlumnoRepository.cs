using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class AlumnoRepository : IAlumnoRepository
{
    private readonly AppDbContext _context;

    public AlumnoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Alumno>> ObtenerTodos(int idEscuela)
    {
        return await _context.Alumnos
            .Where(a => a.IdEscuela == idEscuela)
            .OrderBy(a => a.Apellido)
            .ToListAsync();
    }

    public async Task<Alumno?> ObtenerPorId(int idAlumno, int idEscuela)
    {
        return await _context.Alumnos
            .Include(a => a.AlumnoCursos)
                .ThenInclude(ac => ac.Curso)
            .Include(a => a.AlumnoTutores)
                .ThenInclude(at => at.Tutor)
                    .ThenInclude(t => t.Usuario)
            .FirstOrDefaultAsync(a => a.IdAlumno == idAlumno && a.IdEscuela == idEscuela);
    }

    public async Task<bool> ExisteDni(int dni, int idEscuela, int? excluirIdAlumno = null)
    {
        return await _context.Alumnos
            .AnyAsync(a => a.Dni == dni
                        && a.IdEscuela == idEscuela
                        && (excluirIdAlumno == null || a.IdAlumno != excluirIdAlumno));
    }

    public async Task<Alumno> Crear(Alumno alumno)
    {
        _context.Alumnos.Add(alumno);
        await _context.SaveChangesAsync();
        return alumno;
    }

    public async Task<bool> Actualizar(Alumno alumno)
    {
        alumno.FechaAct = DateTime.Now;
        _context.Alumnos.Update(alumno);
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public async Task<bool> Eliminar(int idAlumno, int idEscuela)
    {
        var alumno = await _context.Alumnos
            .FirstOrDefaultAsync(a => a.IdAlumno == idAlumno && a.IdEscuela == idEscuela);
        if (alumno is null) return false;

        alumno.Activo = false;
        alumno.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    // ==========================
    // CURSOS
    // ==========================

    public async Task<bool> ExisteCursoEnEscuela(int idCurso, int idEscuela)
    {
        return await _context.Cursos
            .AnyAsync(c => c.IdCurso == idCurso && c.IdEscuela == idEscuela && c.Activo);
    }

    public async Task<AlumnoCurso?> ObtenerAsignacionCurso(int idAlumno, int idCurso)
    {
        return await _context.AlumnoCursos
            .Include(ac => ac.Curso)
            .FirstOrDefaultAsync(ac => ac.IdAlumno == idAlumno && ac.IdCurso == idCurso);
    }

    public async Task AsignarCurso(AlumnoCurso alumnoCurso)
    {
        _context.AlumnoCursos.Add(alumnoCurso);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> QuitarCurso(int idAlumno, int idCurso)
    {
        var asignacion = await _context.AlumnoCursos
            .FirstOrDefaultAsync(ac => ac.IdAlumno == idAlumno && ac.IdCurso == idCurso && ac.Activo);
        if (asignacion is null) return false;

        asignacion.Activo = false;
        asignacion.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    // ==========================
    // TUTORES
    // ==========================

    public async Task<bool> ExisteTutorEnEscuela(int idTutor, int idEscuela)
    {
        return await _context.Tutores
            .AnyAsync(t => t.IdTutor == idTutor && t.Usuario.IdEscuela == idEscuela && t.Usuario.Activo);
    }

    public async Task<AlumnoTutor?> ObtenerAsociacionTutor(int idAlumno, int idTutor)
    {
        return await _context.AlumnoTutores
            .Include(at => at.Tutor)
                .ThenInclude(t => t.Usuario)
            .FirstOrDefaultAsync(at => at.IdAlumno == idAlumno && at.IdTutor == idTutor);
    }

    public async Task AsociarTutor(AlumnoTutor alumnoTutor)
    {
        _context.AlumnoTutores.Add(alumnoTutor);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> QuitarTutor(int idAlumno, int idTutor)
    {
        var asociacion = await _context.AlumnoTutores
            .FirstOrDefaultAsync(at => at.IdAlumno == idAlumno && at.IdTutor == idTutor && at.Activo);
        if (asociacion is null) return false;

        asociacion.Activo = false;
        asociacion.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> GuardarCambios()
    {
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public Task<IQueryable<Alumno>> ObtenerQueryable(int idEscuela)
    {
        var query = _context.Alumnos
            .Where(a => a.IdEscuela == idEscuela && a.Activo)
            .OrderBy(a => a.Apellido)
            .AsQueryable();

        return Task.FromResult(query);
    }
}