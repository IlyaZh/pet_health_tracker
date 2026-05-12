# Archie Health Tracker 🐾

Archie Health Tracker is a Telegram bot designed to help pet owners track their dogs' health, including weight,
symptoms, hygiene events, and medical history. It provides an easy-to-use interface for logging data and generates
reports (Text/PDF) to monitor your pet's well-being over time.

## Features

- **Weight Tracking**: Log and monitor weight changes.
- **Symptom Logging**: Record symptoms with notes.
- **Hygiene Tracking**: Track grooming, baths, and other hygiene events.
- **Medical Events**: Log vaccinations, vet visits, and medications.
- **Reports**: Generate Telegram-based or PDF health reports.
- **Authorization**: Built-in whitelist to restrict access to specific users.

---

## 🚀 Deployment (Coolify / VPS)

This project is optimized for deployment via **Coolify** or any Docker-based CI/CD.

### 1. Database Setup

Since you are using an existing MySQL instance, ensure you have created a new database (e.g., `archie_health_tracker`).

### 2. Coolify Configuration

1. **Source**: Connect your GitHub repository.
2. **Build Pack**: Select **Dockerfile**.
3. **Environment Variables**: Add the following variables in the Coolify dashboard:
    - `ConnectionStrings__Database`:
      `Server=your_db_host;Database=archie_health_tracker;Uid=your_user;Pwd=your_password;CharSet=utf8mb4;`
    - `BotConfiguration__Token`: Your Telegram Bot Token.
    - `BotConfiguration__UpdateMode`: `Webhook` (recommended for production).
    - `BotConfiguration__WebhookUrl`: `https://your-domain.com/bots/dogs-health-tracker`
    - `BotConfiguration__SecretToken`: A long random string for webhook security.
    - `BotConfiguration__AllowedUsers`: A comma-separated list of Telegram User IDs (e.g., `12345678,87654321`).
    - `ASPNETCORE_ENVIRONMENT`: `Production`

### 3. Webhook Setup

Once deployed, the bot will automatically attempt to set the webhook URL provided in `BotConfiguration__WebhookUrl` on
startup.

---

## 💻 Local Development

### Prerequisites

- .NET 10 SDK
- Docker & Docker Compose

### Setup

1. Clone the repository.
2. Copy `.env.example` to `.env` and fill in your local MySQL credentials and Bot Token.
3. Copy `appsettings.example.json` to `appsettings.json`.
4. Run the application using Docker Compose:
   ```bash
   docker compose up --build
   ```

The local database will be automatically created and migrations applied on startup.

---

## 🛡️ Security Note

- Never commit your `appsettings.json` or `.env` files. They are excluded via `.gitignore`.
- In production, always use `Webhook` mode with a `SecretToken` to ensure updates only come from Telegram.
- The `Dockerfile` is configured to run the application as a non-privileged user.

---

## License

MIT
