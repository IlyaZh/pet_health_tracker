# Основные команды
run:
	docker compose up -d --build 

logs:
	docker compose logs -f archi_health_tracker

stop:
	docker compose down

# Полная очистка: удаляет контейнеры и VOLUME (базу данных)
clean-db:
	docker compose down -v

# Создание новой миграции (использование: make migration name=InitialCreate)
migration:
	dotnet ef migrations add $(name)

# Накатить миграции локально (нужен запущенный контейнер с БД)
db-update:
	dotnet ef database update

# HARD RESET: Удалить базу, удалить миграции и создать всё с нуля
reset-all: clean-db
	rm -rf Migrations/
	dotnet ef migrations add InitialCreate
	docker compose up -d db
	@echo "Waiting for DB to start..."
	sleep 10
	dotnet ef database update
	docker compose up -d --build archi_health_tracker