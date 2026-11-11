# LABOFMUTANTAPI / SQL Server

API de la PWA LAB OF MUTANT, integrada en el proyecto existente de Visual Studio. Guarda en SQL Server el perfil, preferencias, progreso y diario de cada dispositivo.

## Ejecutar

1. Abre la solución existente LABOFMUNTANTAPI.slnx en Visual Studio.
2. Selecciona el perfil http o https y pulsa F5. La API mantiene http://localhost:5000 y Swagger.
3. Al arrancar, Entity Framework aplica las migraciones pendientes y crea LABOFMUTANT_PWA si todavía no existe. No usa ni modifica la base de WebApiProMajoOrigina.
4. En la PWA ejecuta npm run pwa. Vite reenvía /api a http://localhost:5000.

La conexión de desarrollo está en appsettings.Development.json. Se tomó de la referencia cambiando únicamente el nombre de la base de datos a LABOFMUTANT_PWA. Usa localhost\\SQLEXPRESS y autenticación de Windows: la cuenta que ejecuta la API debe tener acceso al servidor y permiso de crear la base en el primer arranque. En producción configura ConnectionStrings__DefaultConnection para ese entorno.

Para crear o actualizar únicamente la base sin dejar la API escuchando:

```powershell
dotnet run --project LABOFMUNTANTAPI.csproj -- --init-db
```

## Capas y tablas

- Controllers/SyncController.cs: POST y GET /api/sync.
- Dto: operaciones recibidas y confirmación de secuencias.
- Services/Interfaces y Services/Services: validación y sincronización idempotente.
- Context/AplicacionBdContext.cs: DbContext de Entity Framework Core y relaciones.
- Context/AplicacionDataContext.cs: lectura y guardado transaccional del estado.
- Migrations: esquema inicial de SQL Server.

Tablas de la base nueva:

- Dispositivos: identificador anónimo, última secuencia y fecha de sincronización.
- Perfiles: nombre del explorador.
- Preferencias: linterna y pestaña de la guía.
- Progresos: visitas, fechas, récord de sigilo, detecciones y controles probados.
- NotasDiario: texto y fecha de cada nota; su clave combina dispositivo e identificador local.

Las operaciones se guardan dentro de una transacción. Un bloqueo de SQL Server por dispositivo evita conflictos entre solicitudes concurrentes; la última secuencia impide duplicar reintentos o recuperar datos borrados mediante un envío antiguo. La API solo confirma cuando la transacción se ha confirmado en SQL Server.

El contrato de la PWA sigue siendo POST /api/sync con { operations: [...] } y Authorization: Bearer <UUID secreto del dispositivo>. La respuesta incluye accepted, lastSequence y syncedAt. GET /api/sync permite consultar el mismo dispositivo. El UUID no es una cuenta de usuario ni debe compartirse; en las tablas solo se almacena su hash.

## Funcionamiento sin conexión

IndexedDB conserva los cambios y la cola de envío en la laptop. Si falta red, SQL Server o la API, los cambios permanecen pendientes. Se reintenta al abrir la app, recuperar conexión o volver a enfocarla, además de cada 30 segundos mientras está visible. Background Sync también permite reintentos desde el Service Worker cuando el navegador lo admite y decide ejecutarlos. Si el navegador está completamente cerrado, vuelve a abrir la app.

Los borrados también se sincronizan. Borrar todos los datos envía reset y conserva únicamente el identificador técnico necesario para completar ese borrado. La instalación y desinstalación siguen siendo las nativas del navegador/sistema.

Para consultar la base en SQL Server Management Studio, conecta a localhost\\SQLEXPRESS y actualiza Bases de datos; allí aparecerá LABOFMUTANT_PWA. No abras la base WebApi102 para buscar estos registros.

Antes de distribuir a otros equipos publica tanto la PWA como la API mediante HTTPS y configura el proxy /api o VITE_SYNC_API_URL y CORS. La dirección localhost solo funciona en la laptop donde corre la API.
