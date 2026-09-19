# CartAPI

API REST en ASP.NET Core (.NET 10) + EF Core 10 + SQL Server.

## Ejecutar con Docker (recomendado)

Solo necesitas Docker.

```bash
cp .env.example .env
# edita .env: SA_PASSWORD (8+ caracteres con mayúsculas, minúsculas, números y símbolos)
#             JWT_KEY     (32+ caracteres:  openssl rand -base64 48)
docker compose up --build
```

- API: `http://localhost:8080` · Swagger: `http://localhost:8080/swagger`
- SQL Server: `localhost,1433` (usuario `sa`, contraseña la de `.env`)

Al arrancar, la API aplica las migraciones y siembra los datos iniciales. Sembrar dos veces no duplica nada.

## Ejecutar en local

Requisitos: SDK de .NET 10 y un SQL Server accesible.

```bash
dotnet tool restore                        # dotnet-ef del manifiesto dotnet-tools.json
cd CartAPI
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost,1433;Database=CartAPI;User ID=sa;Password=<la tuya>;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 48)"
cd ..
dotnet run --project CartAPI               # Development, http://localhost:5141


# -------
dotnet watch --project CartAPI run \
  --urls http://0.0.0.0:5141 \
  --property:StaticWebAssetsEnabled=false
```

- `TrustServerCertificate=True` hace falta con un SQL Server de desarrollo (certificado autofirmado).
- Dentro de un dev container, para llegar desde el host: `dotnet run --project CartAPI --urls http://0.0.0.0:5141`.
- Script SQL de la base: `dotnet ef migrations script --idempotent --project CartAPI -o schema.sql`.

### Dev Container

Abre el repo en VS Code con la extensión **Dev Containers** y elige **Reopen in Container**: trae .NET 10,
`dotnet-ef` y las extensiones del proyecto.

## Configuración

Se carga en capas; cada una pisa a la anterior:

1. `appsettings.json` (base)
2. `appsettings.{Ambiente}.json` (`Development`, `Staging`, `Production`)
3. user-secrets (solo en `Development`)
4. Variables de entorno (`Seccion__Clave`)

El ambiente lo decide `ASPNETCORE_ENVIRONMENT`; sin ella, `Production`. Los perfiles de `launchSettings.json`
(`http`, `staging`, `production`) solo se usan en local: `dotnet run --project CartAPI --launch-profile staging`.

| Clave | Variable de entorno | Obligatoria | Por defecto |
|---|---|---|---|
| `ConnectionStrings:Default` | `ConnectionStrings__Default` | sí | — |
| `Jwt:Key` (32+ caracteres) | `Jwt__Key` | sí | — |
| `Jwt:Issuer` / `Jwt:Audience` | `Jwt__Issuer` / `Jwt__Audience` | no | `CartAPI` / `CartAPI.Clients` |
| `Jwt:ExpiresMinutes` | `Jwt__ExpiresMinutes` | no | `60` |
| `Cors:AllowedOrigins` | `Cors__AllowedOrigins__0` | no | `http://localhost:4200` en Development |
| `Database:MigrateOnStartup` | `Database__MigrateOnStartup` | no | `true` |

Sin cadena de conexión o con una clave JWT de menos de 32 caracteres, **la API no arranca** y dice qué falta.
Ningún secreto está en el repositorio.

## Usuarios de prueba

| Rol | Email | Contraseña |
|---|---|---|
| Admin | `admin@cartapi.local` | `Admin123!` |
| Customer | `cliente@cartapi.local` | `Cliente123!` |

Credenciales de demostración (sección `Seed` de `appsettings.json`).

## Swagger

1. Abre `/swagger`.
2. `POST /api/auth/login` con un usuario de prueba y copia el `accessToken`.
3. **Authorize** → pega el token (sin la palabra `Bearer`).

## Endpoints

| Método | Ruta | Descripción | Respuestas |
|---|---|---|---|
| POST | `/api/auth/login` | Login, devuelve el JWT | 200, 400, 401 |
| GET | `/api/auth/me` | Usuario autenticado | 200, 401 |

Todo endpoint exige token salvo que se declare `AllowAnonymous()`. Con token sin el rol pedido: `403`.

## Errores

Toda respuesta de error es `ProblemDetails` (RFC 9457) con un `code` estable y el `traceId` que aparece en el log:

```json
{ "title": "Unauthorized", "status": 401, "detail": "Invalid email or password.", "code": "invalid_credentials", "traceId": "00-..." }
```

El cliente decide por `code`, nunca por el texto. Los `400` de validación traen `errors` por campo (camelCase).
Un error no previsto devuelve `500 internal_error` sin detalles internos; el detalle queda solo en el log.

| Excepción de dominio | Status |
|---|---|
| `ValidationException` | 400 |
| `UnauthorizedException` | 401 |
| `NotFoundException` | 404 |
| `ConflictException` (y clave única duplicada en SQL Server) | 409 |
| cualquier otra | 500 |

## Arquitectura

**Vertical slicing + Clean Architecture por slice + DDD táctico ligero.** El código se agrupa por área de
negocio y, dentro de cada una, por capas.

```
CartAPI/
  Features/<Contexto>/
    <Contexto>Module.cs        registra servicios, modelo EF y endpoints del contexto
    <Slice>/
      Domain/                  entidades con reglas, value objects, XxxErrors, IXxxRepository
      Application/             Commands/ y Queries/ (un archivo = command + handler), DTOs, Abstractions/
      Infrastructure/          Http/ (endpoints + XxxBody), Persistence/, Seeding/, Security/...
    Shared/                    lo que usan 2+ slices del contexto
    Contracts/                 lo único que ven OTROS contextos (tipos primitivos)
  Shared/                      mecanismo sin negocio: errores, auth, transacciones, paginación, OpenAPI
  Persistence/                 AppDbContext, migraciones, FKs entre contextos
  Host/                        composition root
```

```
Escritura:  Endpoint → CommandHandler → Agregado → Repositorio → AppDbContext
Lectura:    Endpoint → QueryHandler → AppDbContext (AsNoTracking + proyección a DTO)
```

| Equivale a | Aquí |
|---|---|
| Controllers | `Infrastructure/Http/*Endpoints.cs` (Minimal APIs) |
| Services | `Application/Commands` y `Application/Queries`: un handler por caso de uso |
| Entities / Models | `Domain/`: agregados sin setters públicos, con sus reglas |
| Repositories | `Domain/I*Repository` + `Infrastructure/Persistence/*Repository` |
| DTOs | `Application/*Dto` (salida) e `Infrastructure/Http/*Body` (entrada) |

`CartAPI.Tests/Architecture` comprueba las fronteras: `dotnet test` falla si el dominio usa EF, si un command
handler usa el `DbContext`, si un contexto usa otro por dentro o si aparece una carpeta fuera de la forma.

### Añadir un slice

1. `Features/<Ctx>/<Slice>/Domain`: el agregado (`Create` + métodos con intención), `XxxErrors`, `IXxxRepository`.
2. `Application/Commands/XxxCommandHandler.cs` y `Application/Queries/XxxQueryHandler.cs`.
3. `Infrastructure/Persistence`: `IEntityTypeConfiguration<T>` (con `HasMaxLength` en lo indexado y
   `HasPrecision(18, 2)` en el dinero) y el repositorio `internal sealed`.
4. `Infrastructure/Http`: `XxxBody` públicos con DataAnnotations y `XxxEndpoints` con `WithSummary` y `ProducesProblem`.
5. `<Ctx>Module.cs`: registrar handlers y repositorio, `ConfigureModel` y `Map<Ctx>Endpoints`. Un contexto nuevo
   se llama desde `Host/` y `Persistence/AppDbContext.cs`.
6. `CartAPI.Tests/Architecture/ArchitectureFixture.cs`: declarar el contexto y el slice.
7. `dotnet ef migrations add <Nombre> --project CartAPI --output-dir Persistence/Migrations`, leerla y `dotnet build`.

## Tests

```bash
dotnet test
```

No necesitan SQL Server: los de integración levantan la API en memoria con SQLite.

| Carpeta | Qué prueba |
|---|---|
| `Unit/` | Reglas de dominio y handlers con fakes |
| `Integration/` | La API por HTTP (`WebApplicationFactory` + SQLite en memoria, una base por test) |
| `Architecture/` | Las fronteras entre capas, contextos y slices (ArchUnitNET) |
