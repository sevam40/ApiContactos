# API de Contactos - Arquitectura Limpia

Este proyecto es una API diseñada para gestionar contactos de manera eficiente y segura. 

## 🚀 Cómo ponerlo en marcha

### Desde Visual Studio (La forma más fácil)
1. Abre el archivo `ApiContactos.sln` con Visual Studio.
2. Asegúrate de que el proyecto **ApiContactos** esté seleccionado como "Proyecto de inicio" (haz clic derecho sobre él y selecciona "Establecer como proyecto de inicio").
3. Presiona el botón verde de **Play** (o la tecla `F5`).
4. Tu navegador se abrirá automáticamente mostrando una guía interactiva (Swagger) donde puedes probar los contactos directamente.

### Desde la Terminal (Para usuarios avanzados)
Si prefieres usar la línea de comandos, sitúate en la carpeta raíz y escribe:

dotnet run --project ApiContactos/ApiContactos.csproj

### Pruebas y Calidad
Este proyecto incluye una sección de pruebas automáticas (unitarias y de integración) que validan el correcto funcionamiento de la lógica de negocio y la consistencia de datos bajo carga.

Estado de las pruebas
La sección de pruebas ya ha sido ejecutada exitosamente, confirmando la solidez de la arquitectura y el manejo concurrente. Los informes detallados de cobertura se encuentran generados en la carpeta /coveragereport.

Cómo ejecutar las pruebas tú mismo
Desde Visual Studio: Ve al menú Prueba > Explorador de pruebas y haz clic en "Ejecutar todas las pruebas".

Desde la Terminal: Ejecuta en la raíz del proyecto:

Bash
dotnet test