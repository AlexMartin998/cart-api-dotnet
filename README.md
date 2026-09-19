# CartAPI

API base en ASP.NET Core (.NET 10).

## Ejecutar

```bash
dotnet restore
dotnet build

dotnet run                                # Development
dotnet run --launch-profile staging       # Staging
dotnet run --launch-profile production    # Production
```

Dentro de un contenedor, para acceder desde el host:

```bash
dotnet run --launch-profile staging -- --urls http://0.0.0.0:5141
dotnet watch --launch-profile staging -- --urls http://0.0.0.0:5141

dotnet watch --project CartAPI run \
  --urls http://0.0.0.0:5141 \
  --property:StaticWebAssetsEnabled=false
```

`0.0.0.0` escucha en todas las interfaces. Con `localhost`, la app solo acepta conexiones desde dentro del contenedor.

URL local: `http://localhost:5141`. Las requests de prueba están en `CartAPI/CartAPI.http`.

## Dev Container

Solo necesitas Docker y VS Code con la extensión **Dev Containers**; no hace falta .NET en el host. Abre el repo y elige **Reopen in Container**. El contenedor trae .NET 10, `dotnet-ef` y las extensiones del proyecto.

## Ambientes

El ambiente se define con la variable `ASPNETCORE_ENVIRONMENT`. Si no está definida, se usa **Production**.

La configuración se carga en capas, y cada capa pisa a la anterior:

1. `appsettings.json` (base)
2. `appsettings.{Ambiente}.json`
3. Variables de entorno

| Archivo | Ambiente |
|---|---|
| `appsettings.Development.json` | Development |
| `appsettings.Staging.json` | Staging |
| `appsettings.Production.json` | Production |

`launchSettings.json` solo se usa en local. En el servidor:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Staging"
dotnet CartAPI.dll
```
