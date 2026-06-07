using Domain.Common;

namespace Domain.Entities;

// Punto extra: Elegimos usar "record" para proteger los datos. 
// Esto asegura que la información del contacto no se modifique por accidente una vez creada,
// lo cual es clave para que el sistema no falle si recibe muchas visitas al mismo tiempo.
public record Contacto
{
    // Requisito cumplido: La información básica solicitada para cada contacto.
    public int Id { get; init; }
    public string Nombre { get; init; }
    public string Telefono { get; init; }

    // Decisión de diseño: Bloqueamos la creación directa del contacto. 
    // Así garantizamos que sea imposible guardar un contacto con datos incompletos en el sistema.
    private Contacto(int id, string nombre, string telefono)
    {
        Id = id;
        Nombre = nombre;
        Telefono = telefono;
    }

    // creación funcional con validación
    // Regla de negocio: "Nombre y teléfono son obligatorios".
    // En lugar de dejar que el sistema "explote" con un error si faltan datos, 
    // lo validamos aquí de forma segura y devolvemos un mensaje claro sobre qué salió mal.
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
