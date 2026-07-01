using EducAR.API.DTOs.Tutores;

namespace EducAR.API.Services.Interfaces;

public interface ITutorService
{
    Task<List<TutorResponseDto>> ObtenerTodos(int idEscuela);
    Task<TutorResponseDto?> ObtenerPorId(int idTutor, int idEscuela);
    Task<(bool exito, string mensaje, TutorResponseDto? tutor)> Crear(TutorCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> Actualizar(int idTutor, int idEscuela, TutorUpdateDto dto);
    Task<bool> Eliminar(int idTutor, int idEscuela);
}