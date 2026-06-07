using Domain.Common;

namespace Domain.Entities;

public record Contacto
{
    public int Id { get; init; }
    public string Nombre { get; init; }
    public string Telefono { get; init; }

    private Contacto(int id, string nombre, string telefono)
    {
        Id = id;
        Nombre = nombre;
        Telefono = telefono;
    }

    // creación funcional con validación
    public static Result<Contacto> Create(int id, string nombre, string telefono)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return Result.Failure<Contacto>("El nombre del contacto no puede ser nulo o estar vacío.");
        }

        if (string.IsNullOrWhiteSpace(telefono))
        {
            return Result.Failure<Contacto>("El teléfono del contacto no puede ser nulo o estar vacío.");
        }

        var contacto = new Contacto(id, nombre, telefono);
        return Result.Success(contacto);
    }
}
