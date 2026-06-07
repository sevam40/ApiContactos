using System.Collections.Concurrent;
using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Repositories;

public class InMemoryContactoRepository : IContactoRepository
{
    private readonly ConcurrentDictionary<int, Contacto> _contactos = new();
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
        var exists = _contactos.Values.Any(c => c.Telefono == telefono);
        return Task.FromResult(exists);
    }

    public Task<Contacto> AddAsync(Contacto contacto)
    {
        // Generar ID de forma thread-safe evitando colisiones
        var newId = Interlocked.Increment(ref _currentId);

        // Se usa 'with' del record para clonar el objeto asignando el nuevo ID.
        var contactoAGuardar = contacto with { Id = newId };

        // TryAdd no fallará por colisión de llave gracias al Interlocked.Increment
        _contactos.TryAdd(newId, contactoAGuardar);

        return Task.FromResult(contactoAGuardar);
    }
}
