using EducAR.API.DTOs.AlumnoCurso;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlumnoCursoController : ControllerBase
{
    private readonly IAlumnoCursoService _alumnoCursoService;

    public AlumnoCursoController(IAlumnoCursoService alumnoCursoService)
    {
        _alumnoCursoService = alumnoCursoService;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    // GET api/alumnocurso/curso/1
    [HttpGet("curso/{idCurso}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorCurso(int idCurso)
    {
        var inscripciones = await _alumnoCursoService.ObtenerPorCurso(idCurso, IdEscuelaActual);
        return Ok(inscripciones);
    }

    // GET api/alumnocurso/alumno/1
    [HttpGet("alumno/{idAlumno}")]
    [Authorize(Roles = "Administrador,Docente,Tutor")]
    public async Task<IActionResult> ObtenerPorAlumno(int idAlumno)
    {
        var inscripciones = await _alumnoCursoService.ObtenerPorAlumno(idAlumno, IdEscuelaActual);
        return Ok(inscripciones);
    }

    // POST api/alumnocurso
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Inscribir([FromBody] AlumnoCursoCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var (exito, mensaje, inscripcion) = await _alumnoCursoService.Inscribir(dto, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(inscripcion);
    }

    // DELETE api/alumnocurso/1
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Desinscribir(int id)
    {
        var (exito, mensaje) = await _alumnoCursoService.Desinscribir(id, IdEscuelaActual);
        if (!exito) return BadRequest(new { mensaje });

        return Ok(new { mensaje });
    }
}