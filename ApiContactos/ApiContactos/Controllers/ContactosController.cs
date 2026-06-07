using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiContactos.Controllers;

//Recibimos las peticiones del exterior
[ApiController]
[Route("api/[controller]")]
public class ContactosController : ControllerBase
{
    private readonly ContactoService _contactoService;

    public ContactosController(ContactoService contactoService)
    {
        _contactoService = contactoService;
    }

    // Requisito: "1. Obtener todos los contactos".
    // Responde con un código 200 (Éxito) y la lista. Si no hay contactos, devuelve una lista vacía sin fallar.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var contactos = await _contactoService.GetAllAsync();
        return Ok(contactos);
    }

    // Requisito: "2. Obtener contacto por ID".
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _contactoService.GetByIdAsync(id);

        // Requisito cumplido: "Si no existe, responder con 404".
        if (result.IsFailure)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    // Requisito: "3. Crear un contacto".
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContactoRequest request)
    {
        // Validación temprana: Ahorramos recursos del sistema rechazando de inmediato
        // peticiones incompletas (Código HTTP 400 - Bad Request).
        if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Telefono))
        {
            return BadRequest(new { error = "El nombre y el teléfono son obligatorios." });
        }

        // Se envia la petición a nuestra capa de negocio. El ID se generará automáticamente dentro.
        var result = await _contactoService.CreateAsync(0, request.Nombre, request.Telefono);

        if (result.IsFailure)
        {
            // Requisito cumplido: "Uso correcto de códigos de estado".
            // Si el problema es un teléfono duplicado, respondemos con 409 Conflict, 
            // indicando que hay un choque con una regla de negocio existente.
            if (result.Error.Contains("Ya existe un contacto con el teléfono"))
            {
                return Conflict(new { error = result.Error });
            }
            
            // Cualquier otro error de dominio (por si se saltó la validación básica)
            return BadRequest(new { error = result.Error });
        }

        // Buena práctica REST: Al crear un recurso exitosamente, respondemos con 201 Created
        // e incluimos la ruta donde se puede consultar el nuevo registro.
        return Created($"/api/contactos/{result.Value.Id}", result.Value);
    }
}

// Objeto de Transferencia de Datos (DTO). 
// Define exactamente qué información esperamos recibir del cliente para crear un contacto, 
// protegiendo el resto de nuestra aplicación.
public record CreateContactoRequest(string Nombre, string Telefono);
