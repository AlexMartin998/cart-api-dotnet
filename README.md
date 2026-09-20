# CartAPI

API REST para un carrito de compras: catálogo con búsqueda y filtros, carrito persistido,
descuento automático por importe y checkout transaccional.

.NET 10 · Minimal APIs · EF Core · SQL Server · JWT

## Arquitectura

**Vertical slices + Clean Architecture.** En vez de agrupar por tipo técnico (`Controllers/`,
`Services/`, `Repositories/`), el código se agrupa por funcionalidad, y cada una lleva sus tres
capas dentro:

```
CartAPI/
  Features/
    Accounts/  Catalog/  Ordering/          contextos
      <Slice>/
        Domain/           entidades con sus reglas + interfaces de repositorio
        Application/      casos de uso (Commands / Queries) + DTOs
        Infrastructure/   endpoints HTTP + EF Core
      Contracts/          lo único que un contexto expone a otro
  Shared/  Persistence/  Host/
CartAPI.Tests/            Tests
```

Las reglas de negocio viven en las entidades (`Product`, `Cart`, `Order`), no repartidas en
servicios.


## Levantar

Único requisito: **Docker**. No necesitas .NET, ni SQL Server, ni `make`.

Los comandos de abajo son los de siempre y funcionan en Linux, macOS, Git Bash y PowerShell.
Si tienes `make`, a la derecha está el atajo equivalente.

### 1. Crear el fichero de variables

```bash
cp .env.example .env                    # con make:  make env
```

### 2. Rellenarlo

Edita `.env`. Los dos primeros son obligatorios y la API no arranca sin ellos:

```
SA_PASSWORD=Cambiar.Esta1              # 8+ caracteres: mayúsculas, minúsculas, números y símbolos
JWT_KEY=pon-aqui-32-caracteres-o-mas   # genera una con:  openssl rand -base64 48
FRONTEND_ORIGIN=http://localhost:4200  # opcional, para el CORS del front
```

### 3. Arrancar

```bash
docker compose up -d --build            # con make:  make up
```

La primera vez tarda varios minutos: descarga la imagen de SQL Server y compila la API.

### 4. Esperar a que esté lista

```bash
docker compose logs -f api              # con make:  make logs
```

**La base se crea, se migra y se siembra sola.** No hay que ejecutar ningún seeder a mano ni entrar
al contenedor. En el log verás, en este orden:

```
Applying migration '20260919170748_InitialAccounts'.
Applying migration '20260919210617_AddCatalog'.
Applying migration '20260920054244_AddCarts'.
Applying migration '20260920180527_AddOrders'.
Now listening on: http://*:8080          ← ya está lista
```

Si el arranque se queda esperando, es que SQL Server aún no acepta conexiones: el `depends_on` del
compose espera a su healthcheck. Dale hasta un minuto la primera vez.

Sal del log con `Ctrl+C` (no para la API, solo deja de seguirla).

### 5. Comprobar

```bash
curl -i http://localhost:8080/api/categories
```

Debe responder **`401 Unauthorized`**. Eso ya es buena señal: la API está viva y los endpoints
están protegidos.

```
http://localhost:8080            API (a través de nginx)
http://localhost:8080/swagger    documentación interactiva
```

### Parar

```bash
docker compose down                     # con make:  make down     conserva los datos
docker compose down -v                  # con make:  make reset    borra la base
```

## Usuarios de prueba

| Rol | Email | Contraseña |
|---|---|---|
| Admin | `admin@cartapi.local` | `Admin123!` |
| Cliente | `cliente@cartapi.local` | `Cliente123!` |

## Probar

### Opción A — Swagger

1. Abre `http://localhost:8080/swagger`.
2. `POST /api/auth/login` → **Try it out** → pega el cuerpo y ejecuta:
   ```json
   { "email": "cliente@cartapi.local", "password": "Cliente123!" }
   ```
3. Copia el valor de `accessToken` de la respuesta.
4. Botón **Authorize** (arriba a la derecha) → pega el token → **Authorize** → **Close**.
5. A partir de aquí todos los endpoints funcionan desde la misma página.

### Opción B — curl, el recorrido completo

También tienes todo esto en `CartAPI/CartAPI.http`, para VS Code.

**1. Login y token**

```bash
BASE=http://localhost:8080
TOKEN=$(curl -s -X POST $BASE/api/auth/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"cliente@cartapi.local","password":"Cliente123!"}' \
  | sed -n 's/.*"accessToken":"\([^"]*\)".*/\1/p')
H="Authorization: Bearer $TOKEN"
J="Content-Type: application/json"

echo $TOKEN      # si sale vacío, la API no está lista o las credenciales no son esas
```

**2. Catálogo**

```bash
curl -s "$BASE/api/products?pageSize=3" -H "$H"        # paginado
curl -s "$BASE/api/products?search=cafetera" -H "$H"   # búsqueda
curl -s "$BASE/api/products/8" -H "$H"                 # detalle
curl -s "$BASE/api/categories" -H "$H"                 # categorías
```

**3. El borde del descuento** — con 100,00 exactos NO hay descuento; a partir de ahí, 10 %

```bash
curl -s -X DELETE $BASE/api/cart -H "$H"                                                # empezar limpio

curl -s -X POST $BASE/api/cart/items -H "$H" -H "$J" -d '{"productId":8,"quantity":4}'   # subtotal  60,00
curl -s -X POST $BASE/api/cart/items -H "$H" -H "$J" -d '{"productId":14,"quantity":1}'  # subtotal  78,60
curl -s -X POST $BASE/api/cart/items -H "$H" -H "$J" -d '{"productId":12,"quantity":1}'  # subtotal 100,00 → discount 0
curl -s -X POST $BASE/api/cart/items -H "$H" -H "$J" -d '{"productId":8,"quantity":1}'   # subtotal 115,00 → discount 11,50, total 103,50
```

**4. Comprar** — sin cuerpo: las líneas salen del carrito guardado

```bash
curl -i -X POST $BASE/api/orders -H "$H"    # 201 + cabecera Location: /api/orders/1

curl -s $BASE/api/cart   -H "$H"            # el carrito quedó vacío
curl -s $BASE/api/orders -H "$H"            # aparece en el historial
curl -s $BASE/api/orders/1 -H "$H"          # detalle con sus líneas
```

**5. Stock insuficiente** — el producto 4 tiene 2 unidades

```bash
curl -s -X POST $BASE/api/cart/items -H "$H" -H "$J" -d '{"productId":4,"quantity":3}'
# 409  {"code":"insufficient_stock","detail":"Only 2 units of SKU-004 are left."}
```

**6. Roles** — crear un producto exige ser admin

```bash
curl -s -o /dev/null -w "cliente: %{http_code}\n" -X POST $BASE/api/products -H "$H" -H "$J" \
  -d '{"code":"SKU-900","name":"Prueba","price":10.00,"stock":1,"categoryId":1}'    # 403

ADMIN=$(curl -s -X POST $BASE/api/auth/login -H "$J" \
  -d '{"email":"admin@cartapi.local","password":"Admin123!"}' \
  | sed -n 's/.*"accessToken":"\([^"]*\)".*/\1/p')

curl -s -o /dev/null -w "admin:   %{http_code}\n" -X POST $BASE/api/products \
  -H "Authorization: Bearer $ADMIN" -H "$J" \
  -d '{"code":"SKU-900","name":"Prueba","price":10.00,"stock":1,"categoryId":1}'    # 201
```

**7. Empezar de cero** (opcional)

```bash
docker compose down -v && docker compose up -d --build     # con make:  make reset
```

## Tests

Los tests usan SQLite en memoria: no necesitan SQL Server ni la API levantada.

```bash
# en un contenedor del SDK, sin instalar .NET            (con make:  make test)
docker run --rm -v "$PWD":/src -w /src mcr.microsoft.com/dotnet/sdk:10.0 dotnet test

# con el .NET de tu máquina                               (con make:  make local-test)
dotnet test
```

## Comandos

`make` a secas lista todo. Están separados en dos grupos:

**Docker** — no necesitas .NET instalado, todo corre en contenedores:

| Comando | Qué hace |
|---|---|
| `make env` | crea `.env` desde `.env.example` |
| `make up` | construye y arranca todo |
| `make down` | para (conserva los datos) |
| `make reset` | **borra la base** y rearranca, sembrada de cero |
| `make logs` | logs de la API |
| `make sh` | shell dentro del contenedor de la API |
| `make db Q="SELECT ..."` | consulta SQL contra la base |
| `make test` | tests en un contenedor del SDK |

**Local** — necesitas .NET 10:

| Comando | Qué hace |
|---|---|
| `make secrets` | genera `Jwt:Key` en user-secrets |
| `make run` | arranca la API (`http://localhost:5141`) |
| `make local-test` | tests con el .NET de tu máquina |
| `make migrate` | aplica las migraciones pendientes |

## Ejecutar sin Docker

Requiere .NET 10 y un SQL Server accesible.

```bash
dotnet user-secrets set "ConnectionStrings:Default" \
  "Server=localhost,1433;Database=CartAPI;User ID=sa;Password=TuPassword;TrustServerCertificate=True" \
  --project CartAPI
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 48)" --project CartAPI   # make secrets
dotnet run --project CartAPI      # http://localhost:5141                         # make run
```

## Endpoints

Todos exigen token salvo el login. JSON en camelCase. Sin token: `401`; con token de cliente en
un endpoint de admin: `403`.

| Método | Ruta | Rol |
|---|---|---|
| POST | `/api/auth/login` | — |
| GET | `/api/auth/me` | autenticado |
| GET | `/api/categories` | autenticado |
| GET | `/api/products` · `/api/products/{id}` | autenticado |
| POST · PUT · DELETE | `/api/products` · `/api/products/{id}` | **Admin** |
| GET | `/api/cart` | cliente |
| POST | `/api/cart/items` | cliente |
| PUT · DELETE | `/api/cart/items/{productId}` | cliente |
| DELETE | `/api/cart` | cliente |
| POST | `/api/orders` | cliente |
| GET | `/api/orders` · `/api/orders/{id}` | cliente |

`GET /api/products` acepta `search`, `categoryId`, `minPrice`, `maxPrice`, `inStock`, `page` y
`pageSize` (máx. 100).

Los errores son `ProblemDetails` (RFC 9457) con un `code` estable y un `traceId`. El cliente
decide por `code`, nunca por el texto:

```json
{ "status": 409, "detail": "Only 1 unit of SKU-011 is left.",
  "code": "insufficient_stock", "traceId": "00-..." }
```

`validation_error` · `invalid_credentials` · `forbidden` · `product_not_found` ·
`cart_item_not_found` · `order_not_found` · `insufficient_stock` · `cart_empty` ·
`product_code_taken` · `internal_error`

## Consideraciones

- **El stock baja con un `UPDATE` condicional atómico**, nunca leyendo, restando y guardando:
  dos compras simultáneas de la última unidad no pueden pasar las dos.
- **El checkout es transaccional**: descuento de stock, creación de la orden y vaciado del
  carrito van juntos. Si una línea no tiene stock, no queda rastro de las anteriores.
- **El descuento (10 % por encima de $100, estrictamente) se calcula en un solo sitio**,
  compartido por el carrito y la orden: el total cobrado es siempre el que se mostró.
- **La orden congela** código, nombre y precio del producto: el histórico no cambia aunque el
  catálogo sí.
- **`DELETE /api/products/{id}` retira, no borra**: hay órdenes que lo referencian.
- **Umbral y porcentaje del descuento son configuración** (`Ordering__Pricing__*`), validada al
  arrancar: cambiar la promoción no exige recompilar.
