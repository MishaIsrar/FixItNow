# FixItNow

FixItNow is an ASP.NET Core 8 MVC service-booking platform for connecting customers with service providers. Customers can browse services, book work, save favorites, send messages, and leave reviews. Providers can manage their services and bookings, while admins can review platform activity and approve providers.

## Features

- Customer registration, sign-in, saved services, bookings, reviews, and messaging
- Provider registration, approval flow, service management, dashboard metrics, and inbox
- Admin panel for users, providers, bookings, reviews, revenue summary, and provider approvals
- Service browsing by category, sorting by price, and AJAX search
- Responsive Bootstrap UI with shared layouts and mobile-friendly tables/cards
- ASP.NET Core Identity integration with protected app session cookies
- Entity Framework Core data access with SQL Server for local development and SQLite support for lightweight production deployment
- Docker-based production deployment with GitHub Actions CI/CD support

## Tech Stack

| Area | Technology |
| --- | --- |
| Backend | ASP.NET Core 8 MVC |
| Authentication | ASP.NET Core Identity + protected app session cookie |
| Database | EF Core, SQL Server locally, SQLite option for EC2 |
| Frontend | Razor Views, Bootstrap, JavaScript, AJAX |
| Realtime | SignalR |
| Deployment | Docker, Docker Compose, GitHub Actions, EC2 |

## Project Structure

```text
FixItNow/
  FixItNow.sln
  docker-compose.yml
  FixItNow/
    Controllers/
    Data/
    Hubs/
    Models/
    Services/
    Views/
    wwwroot/
.github/workflows/
  deploy-ec2.yml
docker-compose.prod.yml
scripts/
  ec2-bootstrap.sh
```

## Local Development

### Requirements

- .NET SDK 8
- SQL Server LocalDB or SQL Server
- Visual Studio 2022, Rider, or VS Code

### Configure Database

The local connection string is in:

```text
FixItNow/FixItNow/appsettings.json
```

Default:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=fixitnow;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### Run Locally

```bash
dotnet restore FixItNow/FixItNow.sln
dotnet ef database update --project FixItNow/FixItNow/FixItNow.csproj
dotnet run --project FixItNow/FixItNow/FixItNow.csproj
```

## Docker

Build the application image:

```bash
docker build -f FixItNow/FixItNow/Dockerfile -t fixitnow ./FixItNow
```

Run with Docker Compose for production-style hosting:

```bash
docker compose -f docker-compose.prod.yml up -d
```

Production Compose uses SQLite by default and persists:

- database data
- uploaded files
- ASP.NET Data Protection keys

## CI/CD Deployment

The workflow at:

```text
.github/workflows/deploy-ec2.yml
```

builds the Docker image, pushes it to GitHub Container Registry, connects to EC2 over SSH, installs Docker if needed, and restarts the app with Docker Compose.

Required GitHub Actions secrets:

```text
EC2_HOST
EC2_USER
EC2_PORT
EC2_SSH_KEY
ADMIN_EMAIL
ADMIN_PASSWORD
```

For the current EC2 setup:

```text
EC2_HOST=13.217.83.232
EC2_USER=ubuntu
EC2_PORT=22
```

Use `ubuntu` for this EC2 instance.

## EC2 Notes

For a free-tier-friendly deployment, this project uses one containerized ASP.NET app with SQLite instead of running SQL Server on the EC2 instance.

Open these inbound security group ports:

- `22` from your own IP for SSH
- `80` from anywhere for HTTP
- `443` later when SSL is configured

The deployment script stores the app under:

```text
/opt/fixitnow
```

Useful server commands:

```bash
cd /opt/fixitnow
sudo docker compose ps
sudo docker compose logs -f
sudo docker compose restart
```

## Production Checklist

- Replace the development admin fallback with strong `ADMIN_EMAIL` and `ADMIN_PASSWORD` secrets
- Add a domain name
- Add HTTPS with Nginx, Caddy, or an AWS load balancer
- Configure backups for the SQLite volume or migrate to managed database storage
- Review upload limits and allowed file types for your real use case
- Add monitoring and log retention

## License

This project is currently private/internal unless a license is added.
