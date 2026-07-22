using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class TutorRepository : ITutorRepository
{
    private readonly AppDbContext _context;

    public TutorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tutor>> ObtenerTodos(int idEscuela)
    {
        return await _context.Tutores
            .Include(t => t.Usuario)
            .Where(t => t.Usuario.IdEscuela == idEscuela)
            .OrderBy(t => t.Usuario.Apellido)
            .ToListAsync();
    }

    public async Task<Tutor?> ObtenerPorId(int idTutor, int idEscuela)
    {
        return await _context.Tutores
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.IdTutor == idTutor && t.Usuario.IdEscuela == idEscuela);
    }

    public async Task<Tutor> Crear(Tutor tutor)
    {
        _context.Tutores.Add(tutor);
        await _context.SaveChangesAsync();
        return tutor;
    }

    public async Task<bool> Actualizar(Tutor tutor)
    {
        tutor.FechaAct = DateTime.Now;
        _context.Tutores.Update(tutor);
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public async Task<bool> Eliminar(int idTutor, int idEscuela)
    {
        var tutor = await ObtenerPorId(idTutor, idEscuela);
        if (tutor is null) return false;
        
        tutor.Usuario.Activo = false;
        tutor.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Tutor?> ObtenerPorUsuarioInactivo(int idUsuario)
    {
        return await _context.Tutores
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.IdUsuario == idUsuario && !t.Usuario.Activo);
    }

    public async Task<Tutor> Reactivar(Tutor tutor)
    {
        tutor.Usuario.Activo   = true;
        tutor.Usuario.FechaAct = DateTime.Now;
        tutor.FechaAct         = DateTime.Now;
        _context.Tutores.Update(tutor);
        await _context.SaveChangesAsync();
        return tutor;
    }
}