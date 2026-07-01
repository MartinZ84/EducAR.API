using EducAR.API.Data;
using EducAR.API.DTOs.Tutores;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class TutorService : ITutorService
{
    private readonly ITutorRepository _tutorRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly AppDbContext _context;

    private const int ID_ROL_TUTOR = 3;

    public TutorService(
        ITutorRepository tutorRepository,
        IUsuarioRepository usuarioRepository,
        AppDbContext context)
    {
        _tutorRepository = tutorRepository;
        _usuarioRepository = usuarioRepository;
        _context = context;
    }

    public async Task<List<TutorResponseDto>> ObtenerTodos(int idEscuela)
    {
        var tutores = await _tutorRepository.ObtenerTodos(idEscuela);
        return tutores.Select(MapearAResponseDto).ToList();
    }

    public async Task<TutorResponseDto?> ObtenerPorId(int idTutor, int idEscuela)
    {
        var tutor = await _tutorRepository.ObtenerPorId(idTutor, idEscuela);
        return tutor is null ? null : MapearAResponseDto(tutor);
    }

    public async Task<(bool exito, string mensaje, TutorResponseDto? tutor)> Crear(TutorCreateDto dto, int idEscuela)
    {
        if (await _usuarioRepository.ExisteNombreUsuario(dto.NombreUsuario, idEscuela))
            return (false, "El nombre de usuario ya existe en esta escuela.", null);

        if (await _usuarioRepository.ExisteDni(dto.Dni, idEscuela))
            return (false, "Ya existe un usuario con ese DNI en esta escuela.", null);

        if (await _usuarioRepository.ExisteEmail(dto.Email, idEscuela))
            return (false, "Ya existe un usuario con ese email en esta escuela.", null);   

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var usuario = new Usuario
            {
                IdRol          = ID_ROL_TUTOR,
                IdEscuela      = idEscuela,
                Dni            = dto.Dni,
                Nombre         = dto.Nombre,
                Apellido       = dto.Apellido,
                Email          = dto.Email,
                NombreUsuario  = dto.NombreUsuario,
                HashContrasena = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
                Activo         = true
            };
            await _usuarioRepository.Crear(usuario);

            var tutor = new Tutor
            {
                IdUsuario    = usuario.IdUsuario,
                EsResponsable = dto.EsResponsable                
            };
            await _tutorRepository.Crear(tutor);

            await transaction.CommitAsync();

            var completo = await _tutorRepository.ObtenerPorId(tutor.IdTutor, idEscuela);
            return (true, "Tutor creado correctamente.", MapearAResponseDto(completo!));
        }
        catch
        {
            await transaction.RollbackAsync();
            return (false, "Ocurrió un error al crear el tutor.", null);
        }
    }

    public async Task<(bool exito, string mensaje)> Actualizar(int idTutor, int idEscuela, TutorUpdateDto dto)
    {
        var tutor = await _tutorRepository.ObtenerPorId(idTutor, idEscuela);
        if (tutor is null)
            return (false, "Tutor no encontrado.");

        // VALIDACIÓN: Verificar si el email cambió y ya existe para otro usuario
        if (!string.Equals(tutor.Usuario.Email, dto.Email, StringComparison.OrdinalIgnoreCase))
        {
            var emailExiste = await _usuarioRepository.ExisteEmail(dto.Email, idEscuela, tutor.IdUsuario);
            if (emailExiste)
            {
                return (false, $"El email '{dto.Email}' ya está siendo usado por otro usuario en esta escuela.");
            }
        }

        tutor.Usuario.Nombre    = dto.Nombre;
        tutor.Usuario.Apellido  = dto.Apellido;
        tutor.Usuario.Email     = dto.Email;
        tutor.Usuario.Activo    = dto.Activo;
        tutor.EsResponsable     = dto.EsResponsable;
     
        await _tutorRepository.Actualizar(tutor);
        return (true, "Tutor actualizado correctamente.");
    }

    public async Task<bool> Eliminar(int idTutor, int idEscuela)
    {
        return await _tutorRepository.Eliminar(idTutor, idEscuela);
    }

    private static TutorResponseDto MapearAResponseDto(Tutor t) => new()
    {
        IdTutor       = t.IdTutor,
        IdUsuario     = t.IdUsuario,
        Dni           = t.Usuario.Dni,
        Nombre        = t.Usuario.Nombre,
        Apellido      = t.Usuario.Apellido,
        Email         = t.Usuario.Email,
        NombreUsuario = t.Usuario.NombreUsuario,
        EsResponsable = t.EsResponsable,
        
    };
}