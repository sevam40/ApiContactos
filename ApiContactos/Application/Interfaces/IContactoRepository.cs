using Domain.Entities;

namespace Application.Interfaces;

// Requisito cumplido: Uso de interfaces para lograr un código desacoplado y testeable.
// Al definir un "contrato" de comunicación en lugar de acceder directamente a los datos, 
// podemos probar esta capa de forma aislada y cambiar la forma de almacenamiento en el futuro sin romper el sistema.
public interface IContactoRepository
{
    Task<IEnumerable<Contacto>> GetAllAsync();
    Task<Contacto?> GetByIdAsync(int id);

    // Regla de negocio: Método para revisar  si un teléfono ya está en uso 
    // para evitar duplicados antes de guardar .
    Task<bool> ExistsByTelefonoAsync(string telefono);
    Task<Contacto> AddAsync(Contacto contacto);
}
