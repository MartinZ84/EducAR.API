using EducAR.API.DTOs.Mensajes;

namespace EducAR.API.Services.Interfaces;

public interface IMensajeService
{
    Task<List<MensajeResumenDto>> ObtenerRecibidos(int idUsuario);
    Task<List<MensajeResumenDto>> ObtenerEnviados(int idUsuario);
    Task<MensajeResponseDto?> ObtenerPorId(int idMensaje, int idUsuario);
    Task<(bool exito, string mensaje)> Enviar(MensajeCreateDto dto, int idUsuarioRemitente, int idEscuela);
    Task<bool> MarcarLeido(int idMensaje, int idUsuario);
    Task<bool> Eliminar(int idMensaje, int idUsuario);
    Task<int> ContarNoLeidos(int idUsuario);
}