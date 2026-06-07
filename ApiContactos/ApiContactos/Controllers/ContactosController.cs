using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiContactos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactosController : ControllerBase
{
    private readonly ContactoService _contactoService;

    public ContactosController(ContactoService contactoService)
    {
        _contactoService = contactoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var contactos = await _contactoService.GetAllAsync();
        return Ok(contactos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _contactoService.GetByIdAsync(id);
        
        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContactoRequest request)
    {
        // Validación básica de DTO antes del dominio
        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Telefono))
        {
            return BadRequest(new { error = "El nombre y el teléfono son obligatorios." });
        }

        // El ID será generado atómicamente por el repositorio, pasamos 0
        var result = await _contactoService.CreateAsync(0, request.Nombre, request.Telefono);

        if (result.IsFailure)
        {
            // Mapeo de errores funcionales a códigos HTTP
            if (result.Error.Contains("Ya existe un contacto con el teléfono"))
            {
                return Conflict(new { error = result.Error });
            }
            
            // Cualquier otro error de dominio (por si se saltó la validación básica)
            return BadRequest(new { error = result.Error });
        }

        return Created($"/api/contactos/{result.Value.Id}", result.Value);
    }
}

// DTO para la petición
public record CreateContactoRequest(string Nombre, string Telefono);
