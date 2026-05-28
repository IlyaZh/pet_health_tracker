# Список фиктивных целей (чтобы не путать с файлами)
.PHONY: build restore test run run-debug logs stop clean-db migration db-update reset-all

# Основные команды
restore:
	dotnet restore ArchieHealthTracker.sln

build: restore
	dotnet build ArchieHealthTracker.sln --no-restore -c Release

docs-build:
	docfx metadata docs/docfx.json
	docfx build docs/docfx.json

docs-serve:
	docfx docs/docfx.json --serve

test:
	@echo "No tests yet."
	# dotnet test ArchieHealthTracker.sln --no-build -c Release

run:
	docker compose up -d --build 

run-debug:
	ASPNETCORE_ENVIRONMENT=Development docker compose up -d --build

logs:
	docker compose logs -f archie_app

stop:
	docker compose down

# Полная очистка: удаляет контейнеры и VOLUME (базу данных)
clean-db:
	docker compose down -v

# Создание новой миграции (использование: make migration name=InitialCreate)
migration:
	dotnet ef migrations add $(name) --project src/ArchieHealthTracker.Infrastructure/ArchieHealthTracker.Infrastructure.csproj --startup-project src/ArchieHealthTracker.WebService/ArchieHealthTracker.WebService.csproj

# Накатить миграции локально (нужен запущенный контейнер с БД)
db-update:
	dotnet ef database update --project src/ArchieHealthTracker.Infrastructure/ArchieHealthTracker.Infrastructure.csproj --startup-project src/ArchieHealthTracker.WebService/ArchieHealthTracker.WebService.csproj

# HARD RESET: Удалить базу, удалить миграции и создать всё с нуля
reset-all: clean-db
	rm -rf src/ArchieHealthTracker.Infrastructure/Migrations/
	dotnet ef migrations add InitialCreate --project src/ArchieHealthTracker.Infrastructure/ArchieHealthTracker.Infrastructure.csproj --startup-project src/ArchieHealthTracker.WebService/ArchieHealthTracker.WebService.csproj
	docker compose up -d db
	@echo "Waiting for DB to start..."
	sleep 10
	make db-update
	docker compose up -d --build archi_health_tracker

db-update-local:
	@export $$(grep -v '^#' src/ArchieHealthTracker.WebService/.env | xargs) && \
	dotnet ef database update --connection "Server=localhost;Port=3306;Database=$${MYSQL_DATABASE};Uid=root;Pwd=$${MYSQL_ROOT_PASSWORD};" --project src/ArchieHealthTracker.Infrastructure/ArchieHealthTracker.Infrastructure.csproj --startup-project src/ArchieHealthTracker.WebService/ArchieHealthTracker.WebService.csproj
	
db-shell:
	docker exec -it archie_mysql mysql -u root -p --default-character-set=utf8mb4 archie
