using Domain.Entities;

namespace Application.Interfaces;

public interface IContactoRepository
{
    Task<IEnumerable<Contacto>> GetAllAsync();
    Task<Contacto?> GetByIdAsync(int id);
    Task<bool> ExistsByTelefonoAsync(string telefono);
    Task<Contacto> AddAsync(Contacto contacto);
}
