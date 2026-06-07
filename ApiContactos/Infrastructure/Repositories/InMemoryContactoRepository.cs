using System.Collections.Concurrent;
using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Repositories;

// Implementamos el almacenamiento "In-Memory" (sin base de datos).
// Al implementar la interfaz de la capa de Aplicación, mantenemos la arquitectura limpia y desacoplada.
public class InMemoryContactoRepository : IContactoRepository
{
    // Habilidad técnica destacada: "La solución debe ser thread-safe".
    // Usamos ConcurrentDictionary en lugar de un Dictionary normal o una Lista. 
    // Esto asegura que si 100 usuarios intentan guardar o leer contactos al mismo tiempo, 
    // la aplicación no se bloquee ni arroje errores de acceso a memoria.
    private readonly ConcurrentDictionary<int, Contacto> _contactos = new();

    // Variable para controlar el autoincremental del ID.
    private int _currentId = 0;

    public Task<IEnumerable<Contacto>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Contacto>>(_contactos.Values);
    }

    public Task<Contacto?> GetByIdAsync(int id)
    {
        _contactos.TryGetValue(id, out var contacto);
        return Task.FromResult(contacto);
    }

    public Task<bool> ExistsByTelefonoAsync(string telefono)
    {
        // Revisamos si el teléfono ya está registrado para cumplir con la regla de "Evitar duplicados".
        var exists = _contactos.Values.Any(c => c.Telefono == telefono);
        return Task.FromResult(exists);
    }

    public Task<Contacto> AddAsync(Contacto contacto)
    {
        // Generar ID de forma thread-safe evitando colisiones
        // Interlocked.Increment es una operación atómica. Garantiza matemáticamente que
        // dos peticiones simultáneas nunca recibirán el mismo número de ID, sin necesidad de bloquear el sistema.
        var newId = Interlocked.Increment(ref _currentId);

        // Buenas prácticas: Como nuestra entidad es inmutable (record), no podemos simplemente
        // hacer "contacto.Id = newId". Usamos 'with' para crear una copia exacta con el nuevo ID,
        // previniendo cualquier riesgo de alterar la información original en tránsito.
        var contactoAGuardar = contacto with { Id = newId };

        // Guardamos de forma segura. TryAdd no fallará por llaves duplicadas gracias a nuestro generador de IDs .
        _contactos.TryAdd(newId, contactoAGuardar);

        return Task.FromResult(contactoAGuardar);
    }
}
