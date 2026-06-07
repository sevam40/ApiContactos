using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// "Swagger habilitado".
// Esto genera una documentación interactiva automática, permitiendo a cualquier desarrollador 
// o evaluador probar la API fácilmente desde su navegador.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
 
// Registramos el almacenamiento en memoria como "Singleton". 
// Esto garantiza que todas las peticiones web compartan el mismo "diccionario" de datos 
// y que la información no se borre entre un clic y otro.
builder.Services.AddSingleton<IContactoRepository, InMemoryContactoRepository>();

// El servicio, en cambio, se crea y destruye en cada petición (Scoped) para ahorrar recursos, 
// ya que no necesita guardar estado propio.
builder.Services.AddScoped<ContactoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Este pequeño detalle es clave para la calidad del software:
// Exponemos la clase Program para que nuestro proyecto de Testing (WebApplicationFactory) 
// pueda arrancar la API virtualmente y ejecutar pruebas de integración y concurrencia.
public partial class Program { }
