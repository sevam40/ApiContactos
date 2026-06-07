using Application.Interfaces;
using Domain.Common;
using Domain.Entities;

namespace Application.Services;

//  Separación de responsabilidades (Clean Architecture).
// Solo conoce las reglas del negocio 
public class ContactoService
{
    private readonly IContactoRepository _repository;

    // Inyección de dependencias: Recibimos la interfaz en lugar de una clase concreta.
    // Esto hace que nuestro código sea altamente mantenible y fácil de someter a pruebas unitarias (Unit Tests).
    public ContactoService(IContactoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Contacto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Result<Contacto>> GetByIdAsync(int id)
    {
        var contacto = await _repository.GetByIdAsync(id);
        
        if (contacto is null)
        {
            // Manejo funcional: Si no existe el registro, devolvemos un mensaje de error claro
            // y controlado en lugar de dejar que el sistema falle inesperadamente (sin lanzar excepciones).
            return Result.Failure<Contacto>($"No se encontró un contacto con el ID {id}.");
        }

        return Result.Success(contacto);
    }

    public async Task<Result<Contacto>> CreateAsync(int id, string nombre, string telefono)
    {
        // Regla de Negocio: No permitir contactos con el mismo teléfono
        var exists = await _repository.ExistsByTelefonoAsync(telefono);
        if (exists)
        {
            return Result.Failure<Contacto>($"Ya existe un contacto con el teléfono {telefono}.");
        }

        // Creación y validación del dominio
        var contactoResult = Contacto.Create(id, nombre, telefono);
        if (contactoResult.IsFailure)
        {
            return contactoResult; // Devuelve el error funcional si el nombre o teléfono son inválidos
        }

        // Si todas las reglas de negocio son aprobadas, procedemos a guardar el contacto de forma segura.
        var contactoCreado = await _repository.AddAsync(contactoResult.Value);

        return Result.Success(contactoCreado);
    }
}
