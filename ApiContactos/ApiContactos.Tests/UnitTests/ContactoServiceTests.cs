using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Moq;

namespace ApiContactos.Tests.UnitTests;

public class ContactoServiceTests
{
	// Prueba unitaria: se valida la regla de negocio "No duplicar teléfonos" 
	// funcione correctamente aislando el repositorio con un "Mock". 
	// Esto garantiza que nuestra lógica de negocio sea independiente de cómo se guardan los datos.
    private readonly Mock<IContactoRepository> _mockRepository;
    private readonly ContactoService _contactoService;

    public ContactoServiceTests()
    {
        _mockRepository = new Mock<IContactoRepository>();
        _contactoService = new ContactoService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenTelefonoYaExiste_ShouldReturnFailure()
    {
        // Arrange
        string nombre = "Juan Pérez";
        string telefono = "555-1234";
        
        // Simulamos que el repositorio ya tiene un contacto con ese teléfono
        _mockRepository.Setup(r => r.ExistsByTelefonoAsync(telefono)).ReturnsAsync(true);

        // Act
        var result = await _contactoService.CreateAsync(0, nombre, telefono);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("Ya existe un contacto con el teléfono", result.Error);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Contacto>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenDataIsValid_ShouldReturnSuccess()
    {
        // Arrange
        string nombre = "Ana Gómez";
        string telefono = "555-9876";

        _mockRepository.Setup(r => r.ExistsByTelefonoAsync(telefono)).ReturnsAsync(false);
        
        // Simulamos la inserción exitosa devolviendo el contacto con ID asignado
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Contacto>()))
                       .ReturnsAsync((Contacto c) => c with { Id = 1 });

        // Act
        var result = await _contactoService.CreateAsync(0, nombre, telefono);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value.Id);
        Assert.Equal(nombre, result.Value.Nombre);
        Assert.Equal(telefono, result.Value.Telefono);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Contacto>()), Times.Once);
    }
}
