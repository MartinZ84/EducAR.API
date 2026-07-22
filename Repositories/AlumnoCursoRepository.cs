using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class AlumnoCursoRepository : IAlumnoCursoRepository
{
    private readonly AppDbContext _context;

    public AlumnoCursoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AlumnoCurso>> ObtenerPorCurso(int idCurso)
    {
        return await _context.AlumnoCursos
            .Include(ac => ac.Alumno)
            .Include(ac => ac.Curso).ThenInclude(c => c.CicloLectivo)
            .Where(ac => ac.IdCurso == idCurso && ac.Activo)
            .OrderBy(ac => ac.Alumno.Apellido)
            .ToListAsync();
    }

    public async Task<List<AlumnoCurso>> ObtenerPorAlumno(int idAlumno, int idEscuela)
    {
        return await _context.AlumnoCursos
            .Include(ac => ac.Alumno)
            .Include(ac => ac.Curso).ThenInclude(c => c.CicloLectivo)
            .Where(ac => ac.IdAlumno == idAlumno &&
                         ac.Activo &&
                         ac.Curso.IdEscuela == idEscuela)
            .OrderByDescending(ac => ac.Curso.CicloLectivo.Anio)
            .ToListAsync();
    }

    public async Task<AlumnoCurso?> ObtenerPorId(int idAlumnoCurso)
    {
        return await _context.AlumnoCursos
            .Include(ac => ac.Alumno)
            .Include(ac => ac.Curso).ThenInclude(c => c.CicloLectivo)
            .FirstOrDefaultAsync(ac => ac.IdAlumnoCurso == idAlumnoCurso);
    }

    public async Task<bool> ExisteInscripcion(int idAlumno, int idCurso)
    {
        return await _context.AlumnoCursos
            .AnyAsync(ac => ac.IdAlumno == idAlumno &&
                            ac.IdCurso == idCurso &&
                            ac.Activo);
    }

    public async Task<AlumnoCurso> Crear(AlumnoCurso alumnoCurso)
    {
        _context.AlumnoCursos.Add(alumnoCurso);
        await _context.SaveChangesAsync();
        return alumnoCurso;
    }

    public async Task<bool> Eliminar(int idAlumnoCurso, int idEscuela)
    {
        var inscripcion = await ObtenerPorId(idAlumnoCurso);
        if (inscripcion is null) return false;

        // Verificar que pertenece a la escuela
        if (inscripcion.Curso.IdEscuela != idEscuela) return false;

        // Verificar que no tenga asistencias o calificaciones
        inscripcion.Activo = false;
        inscripcion.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EstaInscriptoEnCiclo(int idAlumno, int idCicloLectivo, int idEscuela)
    {
        return await _context.AlumnoCursos
            .AnyAsync(ac => ac.IdAlumno == idAlumno &&
                            ac.Activo &&
                            ac.Curso.IdEscuela == idEscuela &&
                            ac.Curso.IdCicloLectivo == idCicloLectivo);
    }
}