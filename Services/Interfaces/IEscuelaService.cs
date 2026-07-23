using EducAR.API.DTOs.Escuelas;

namespace EducAR.API.Services.Interfaces;

public interface IEscuelaService
{
    Task<List<EscuelaResponseDto>> ObtenerTodas();
    Task<EscuelaResponseDto?> ObtenerPorId(int idEscuela);
    Task<(bool exito, string mensaje, EscuelaResponseDto? escuela)> Crear(EscuelaCreateDto dto);
    Task<(bool exito, string mensaje)> Actualizar(int idEscuela, EscuelaUpdateDto dto);
}