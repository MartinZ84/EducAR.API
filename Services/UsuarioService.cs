using EducAR.API.DTOs.Usuarios;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<UsuarioResponseDto>> ObtenerTodos(int idEscuela)
    {
        var usuarios = await _usuarioRepository.ObtenerTodos(idEscuela);
        return usuarios.Select(MapearAResponseDto).ToList();
    }

    public async Task<UsuarioResponseDto?> ObtenerPorId(int idUsuario, int idEscuela)
    {
        var usuario = await _usuarioRepository.ObtenerPorId(idUsuario, idEscuela);
        return usuario is null ? null : MapearAResponseDto(usuario);
    }

    public async Task<(bool exito, string mensaje, UsuarioResponseDto? usuario)> Crear(UsuarioCreateDto dto)
    {
        // Validar duplicado
        var existe = await _usuarioRepository.ExisteNombreUsuario(dto.NombreUsuario, dto.IdEscuela);
        if (existe)
            return (false, "El nombre de usuario ya existe en esta escuela.", null);

        var usuario = new Usuario
        {
            IdRol           = dto.IdRol,
            IdEscuela       = dto.IdEscuela,
            Dni             = dto.Dni,
            Nombre          = dto.Nombre,
            Apellido        = dto.Apellido,
            Email           = dto.Email,
            NombreUsuario   = dto.NombreUsuario,
            HashContrasena  = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
            Activo          = true
        };

        var creado = await _usuarioRepository.Crear(usuario);

        // Volver a buscarlo para traer el Rol incluido
        var completo = await _usuarioRepository.ObtenerPorId(creado.IdUsuario, creado.IdEscuela);

        return (true, "Usuario creado correctamente.", MapearAResponseDto(completo!));
    }

    public async Task<(bool exito, string mensaje)> Actualizar(int idUsuario, int idEscuela, UsuarioUpdateDto dto)
    {
        var usuario = await _usuarioRepository.ObtenerPorId(idUsuario, idEscuela);
        if (usuario is null)
            return (false, "Usuario no encontrado.");

        usuario.IdRol    = dto.IdRol;
        usuario.Dni      = dto.Dni;
        usuario.Nombre   = dto.Nombre;
        usuario.Apellido = dto.Apellido;
        usuario.Email    = dto.Email;
        usuario.Activo   = dto.Activo;

        await _usuarioRepository.Actualizar(usuario);
        return (true, "Usuario actualizado correctamente.");
    }

    public async Task<bool> Eliminar(int idUsuario, int idEscuela)
    {
        return await _usuarioRepository.Eliminar(idUsuario, idEscuela);
    }

    private static UsuarioResponseDto MapearAResponseDto(Usuario u) => new()
    {
        IdUsuario     = u.IdUsuario,
        IdRol         = u.IdRol,
        Rol           = u.Rol.Nombre,
        IdEscuela     = u.IdEscuela,
        Dni           = u.Dni,
        Nombre        = u.Nombre,
        Apellido      = u.Apellido,
        Email         = u.Email,
        NombreUsuario = u.NombreUsuario,
        Activo        = u.Activo,
        UltimoAcceso  = u.UltimoAcceso
    };
}