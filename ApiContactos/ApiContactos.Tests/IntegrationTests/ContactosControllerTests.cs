using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ApiContactos.Tests.IntegrationTests;

// Prueba de estrés y concurrencia: Aquí sometemos al sistema a una carga real de 100 peticiones simultáneas.
// Si nuestra arquitectura no fuera "thread-safe", aquí los IDs chocarían y el test fallaría.
// Es la prueba definitiva de que nuestro sistema puede escalar sin errores en entornos de alto tráfico.

public class ContactosControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ContactosControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_WhenMultipleConcurrentRequests_ShouldBeThreadSafeAndAssignUniqueIds()
    {
        // Arrange
        var client = _factory.CreateClient();
        int requestCount = 100; // Peticiones concurrentes a enviar
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act: Enviar peticiones POST en paralelo sin esperar (awaits se hacen después)
        for (int i = 0; i < requestCount; i++)
        {
            var request = new { Nombre = $"Usuario {i}", Telefono = $"555-000-{i}" };
            tasks.Add(client.PostAsJsonAsync("/api/contactos", request));
        }

        // Ejecutar todas las llamadas de red concurrentemente
        await Task.WhenAll(tasks);

        // Assert: Validar que todas retornaron código 201 Created y IDs no colisionan
        int successfulCreates = 0;
        var generatedIds = new HashSet<int>();

        foreach (var task in tasks)
        {
            var response = await task;
            if (response.StatusCode == HttpStatusCode.Created)
            {
                successfulCreates++;
                var responseContent = await response.Content.ReadFromJsonAsync<ContactoResponse>();
                Assert.NotNull(responseContent);
                Assert.True(responseContent.Id > 0);
                
                // Asegurar que cada ID es único
                bool isUnique = generatedIds.Add(responseContent.Id);
                Assert.True(isUnique, $"Se ha generado un ID duplicado por colisión: {responseContent.Id}");
            }
            else
            {
                // Si falla por alguna razón que no sea concurrencia
                var error = await response.Content.ReadAsStringAsync();
                Assert.Fail($"Falló la creación con status {response.StatusCode}. Error: {error}");
            }
        }

        // Verificamos que se crearon exactamente 100 contactos exitosos con IDs únicos
        Assert.Equal(requestCount, successfulCreates);
        Assert.Equal(requestCount, generatedIds.Count);

        // Doble validación: Revisar con el endpoint GET que existan los 100
        var getAllResponse = await client.GetAsync("/api/contactos");
        var allContacts = await getAllResponse.Content.ReadFromJsonAsync<List<ContactoResponse>>();
        
        Assert.NotNull(allContacts);
        Assert.Equal(requestCount, allContacts.Count);
    }
    
    // DTO interno para leer la respuesta de la API
    private record ContactoResponse(int Id, string Nombre, string Telefono);
}
