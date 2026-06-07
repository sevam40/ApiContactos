using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de Inyección de Dependencias
// Singleton es VITAL aquí para que el ConcurrentDictionary mantenga el estado en memoria para todas las peticiones
builder.Services.AddSingleton<IContactoRepository, InMemoryContactoRepository>();
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

// Necesario para los Integration Tests futuros
public partial class Program { }
