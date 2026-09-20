# CartAPI — atajos.
#
#   Docker   no necesitas .NET instalado. Todo corre en contenedores.
#   Local    necesitas .NET 10 y un SQL Server accesible.
#
# Ejecuta `make` para ver la lista.

SHELL   := /bin/bash
COMPOSE := docker compose
SDK     := mcr.microsoft.com/dotnet/sdk:10.0
API     := http://localhost:8080

.DEFAULT_GOAL := help
.PHONY: help env up down reset logs sh db test run local-test migrate secrets

help:
	@echo ""
	@echo "  DOCKER  (no necesitas .NET instalado)"
	@grep -hE '^[a-z-]+:.*?## D ' $(MAKEFILE_LIST) \
	  | awk -F':.*?## D ' '{printf "    \033[36m%-12s\033[0m %s\n", $$1, $$2}'
	@echo ""
	@echo "  LOCAL   (necesitas .NET 10)"
	@grep -hE '^[a-z-]+:.*?## L ' $(MAKEFILE_LIST) \
	  | awk -F':.*?## L ' '{printf "    \033[36m%-12s\033[0m %s\n", $$1, $$2}'
	@echo ""

# DOCKER ------

env:
	@test -f .env && echo ".env ya existe, no lo toco" \
	  || { cp .env.example .env; echo "Creado .env — edítalo: SA_PASSWORD y JWT_KEY"; }

up: 
	@test -f .env || { echo "Falta .env. Ejecuta: make env"; exit 1; }
	$(COMPOSE) up -d --build
	@echo "Arrancando. Sigue el log con: make logs   →   $(API)/swagger"

down:
	$(COMPOSE) down

reset:
	$(COMPOSE) down -v
	$(MAKE) up

logs:
	$(COMPOSE) logs -f api

sh:
	$(COMPOSE) exec api sh

db:
	@test -n "$(Q)" || { echo 'Uso: make db Q="SELECT ..."'; exit 1; }
	@set -a; . ./.env; set +a; \
	$(COMPOSE) exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd \
	  -S localhost -U sa -P "$$SA_PASSWORD" -C -Q "$(Q)"

test:
	docker run --rm -v "$(CURDIR)":/src -w /src \
	  -v cartapi-nuget:/root/.nuget/packages \
	  -e DOTNET_CLI_TELEMETRY_OPTOUT=1 \
	  $(SDK) dotnet test


#  LOCAL ------

secrets:
	dotnet user-secrets set "Jwt:Key" "$$(openssl rand -base64 48)" --project CartAPI
	@echo 'Falta la cadena. Ejecuta:'
	@echo '  dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost,1433;Database=CartAPI;User ID=sa;Password=TuPassword;TrustServerCertificate=True" --project CartAPI'

run:
	dotnet run --project CartAPI

local-test:
	dotnet test

migrate:
	dotnet build
	dotnet ef database update --project CartAPI
