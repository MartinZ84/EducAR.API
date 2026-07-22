namespace EducAR.API.DTOs.Mensajes;

public class MensajeCreateDto
{
    public int IdUsuarioDestinat { get; set; }
    public string Asunto { get; set; } = null!;
    public string MensajeTexto { get; set; } = null!;
}