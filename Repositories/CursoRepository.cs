using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class CursoRepository : ICursoRepository
{
    private readonly AppDbContext _context;

    public CursoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Curso>> ObtenerTodos(int idEscuela)
    {
        return await _context.Cursos
            .Include(c => c.CicloLectivo)
            .Include(c => c.AlumnoCursos)
            .Where(c => c.IdEscuela == idEscuela)
            .OrderBy(c => c.CicloLectivo.Anio)
            .ThenBy(c => c.Grado)
            .ThenBy(c => c.Division)
            .ToListAsync();
    }

    public async Task<List<Curso>> ObtenerPorCicloLectivo(int idCicloLectivo, int idEscuela)
    {
        return await _context.Cursos
            .Include(c => c.CicloLectivo)
            .Include(c => c.AlumnoCursos)
            .Where(c => c.IdCicloLectivo == idCicloLectivo && c.IdEscuela == idEscuela)
            .OrderBy(c => c.Grado)
            .ThenBy(c => c.Division)
            .ToListAsync();
    }

    public async Task<Curso?> ObtenerPorId(int idCurso, int idEscuela)
    {
        return await _context.Cursos
            .Include(c => c.CicloLectivo)
            .Include(c => c.AlumnoCursos)
            .FirstOrDefaultAsync(c => c.IdCurso == idCurso && c.IdEscuela == idEscuela);
    }

    public async Task<bool> ExisteCurso(int grado, string division, string? turno, int idCicloLectivo, int? excluirId = null)
    {
        return await _context.Cursos
            .AnyAsync(c => c.Grado == grado &&
                           c.Division == division &&
                           c.Turno == turno &&
                           c.IdCicloLectivo == idCicloLectivo &&
                           (excluirId == null || c.IdCurso != excluirId));
    }

    public async Task<bool> TieneAlumnosInscriptos(int idCurso)
    {
        return await _context.AlumnoCursos
            .AnyAsync(ac => ac.IdCurso == idCurso && ac.Activo);
    }

    public async Task<bool> TieneAsistencias(int idCurso)
    {
        return await _context.Asistencias
            .AnyAsync(a => a.IdCurso == idCurso);
    }

    public async Task<bool> TieneCalificaciones(int idCurso)
    {
        return await _context.Boletines
            .AnyAsync(b => b.IdCurso == idCurso);
    }

    public async Task<Curso> Crear(Curso curso)
    {
        _context.Cursos.Add(curso);
        await _context.SaveChangesAsync();
        return curso;
    }

    public async Task<bool> Actualizar(Curso curso)
    {
        curso.FechaAct = DateTime.Now;
        _context.Cursos.Update(curso);
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public async Task<bool> Eliminar(int idCurso, int idEscuela)
    {
        var curso = await ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return false;

        curso.Activo = false;
        curso.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }
}