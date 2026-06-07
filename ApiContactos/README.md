# API de Contactos - Arquitectura Limpia

Esta es una API de Contactos desarrollada en .NET 8, diseñada desde cero para cumplir con principios de **Clean Architecture**, manejo de concurrencia y diseño funcional de errores.

## Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)

## Cómo Ejecutar el Proyecto

Sitúate en la raíz del repositorio (`c:\mio\Prueba\ApiContactos`) y ejecuta:

```bash
dotnet run --project ApiContactos/ApiContactos.csproj
```

Abre tu navegador en `http://localhost:5000/swagger` o `https://localhost:5001/swagger` (según tu configuración local del host) para probar los endpoints interactivamente.

## Endpoints

- **GET /api/contactos**: Obtiene todos los contactos registrados.
- **GET /api/contactos/{id}**: Obtiene un contacto por su ID.
- **POST /api/contactos**: Crea un nuevo contacto. Requiere JSON en el body con `Nombre` y `Telefono`.

## Cómo Ejecutar los Tests

Para ejecutar las pruebas y validar el comportamiento thread-safe:

```bash
dotnet test ApiContactos.Tests/ApiContactos.Tests.csproj
```

Las pruebas de integración enviarán ráfagas concurrentes masivas de peticiones HTTP en memoria para validar la solidez de la solución frente a hilos concurrentes.
