using EducAR.API.DTOs.Tutores;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TutoresController : ControllerBase
{
    private readonly ITutorService _tutorService;

    public TutoresController(ITutorService tutorService)
    {
        _tutorService = tutorService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    [HttpGet]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerTodos()
    {
        var tutores = await _tutorService.ObtenerTodos(IdEscuelaActual);
        return Ok(tutores);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var tutor = await _tutorService.ObtenerPorId(id, IdEscuelaActual);
        if (tutor is null) return NotFound(new { mensaje = "Tutor no encontrado." });
        return Ok(tutor);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Crear([FromBody] TutorCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, tutor) = await _tutorService.Crear(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return CreatedAtAction(nameof(ObtenerPorId), new { id = tutor!.IdTutor }, tutor);
    }

    // [HttpPut("{id}")]
    // [Authorize(Roles = "Administrador")]
    // public async Task<IActionResult> Actualizar(int id, [FromBody] TutorUpdateDto dto)
    // {
    //     if (!ModelState.IsValid) return BadRequest(ModelState);

    //     var (exito, mensaje) = await _tutorService.Actualizar(id, IdEscuelaActual, dto);
    //     if (!exito) return NotFound(new { mensaje });

    //     return Ok(new { mensaje });
    // }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] TutorUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje) = await _tutorService.Actualizar(id, IdEscuelaActual, dto);
        
        if (!exito)
        {
            // Verificar si el mensaje indica un conflicto de datos duplicados
            if (mensaje.Contains("email") || mensaje.Contains("DNI") || mensaje.Contains("ya está siendo usado"))
            {
                return Conflict(new { mensaje }); // 409 Conflict
            }
            
            // Si no es un conflicto de duplicados, puede ser "Tutor no encontrado"
            if (mensaje.Contains("no encontrado"))
            {
                return NotFound(new { mensaje }); // 404 Not Found
            }
            
            // Otros errores de validación
            return BadRequest(new { mensaje }); // 400 Bad Request
        }

        return Ok(new { mensaje });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var exito = await _tutorService.Eliminar(id, IdEscuelaActual);
        if (!exito) return NotFound(new { mensaje = "Tutor no encontrado." });

        return Ok(new { mensaje = "Tutor dado de baja correctamente." });
    }
}