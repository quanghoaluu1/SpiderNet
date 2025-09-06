# SpiderNet Backend Deployment Guide

This guide covers how to deploy the SpiderNet backend using Docker.

## Prerequisites

- Docker and Docker Compose installed
- .NET 8.0 SDK (for local development)
- PostgreSQL database (if not using Docker Compose)

## Quick Start with Docker Compose

The easiest way to run the entire stack locally:

```bash
# Navigate to the backend directory
cd backend

# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f spidernet-api

# Stop all services
docker-compose down
```

This will start:
- SpiderNet API on port 8080
- PostgreSQL database on port 5432

## Docker Build Only

To build and run just the API container:

```bash
# Build the Docker image
docker build -t spidernet-backend .

# Run the container
docker run -d \
  --name spidernet-api \
  -p 8080:8080 \
  -e ConnectionStrings__PostgresConnection="Your_Connection_String_Here" \
  -e JWT__Key="your_jwt_secret_key" \
  -e JWT__Issuer="SpiderNetAPI" \
  -e JWT__Audience="SpiderNetClient" \
  -e Cloudinary__CloudName="your_cloudinary_name" \
  -e Cloudinary__ApiKey="your_api_key" \
  -e Cloudinary__ApiSecret="your_api_secret" \
  spidernet-backend
```

## Production Deployment

### Environment Variables

For production deployment, set these environment variables:

```bash
# Database
ConnectionStrings__PostgresConnection="Server=your-db-host;Database=spidernetdb;Port=5432;User Id=your-user;Password=your-password;Ssl Mode=Require;"

# JWT Configuration
JWT__Key="your-secure-jwt-secret-key-at-least-32-characters"
JWT__Issuer="SpiderNetAPI"
JWT__Audience="SpiderNetClient"

# Cloudinary (for image uploads)
Cloudinary__CloudName="your-cloudinary-cloud-name"
Cloudinary__ApiKey="your-cloudinary-api-key"
Cloudinary__ApiSecret="your-cloudinary-api-secret"
```

### Health Checks

The container includes a health check endpoint. You can test it:

```bash
curl http://localhost:8080/health
```

### Database Migrations

If you need to run database migrations in production:

```bash
# Run migrations using the container
docker exec -it spidernet-api dotnet ef database update
```

### Scaling

To run multiple instances behind a load balancer:

```bash
docker-compose up -d --scale spidernet-api=3
```

## Security Considerations

- The container runs as a non-root user for security
- Sensitive configuration is handled via environment variables
- SSL/TLS should be terminated at the load balancer or reverse proxy
- Use strong, unique passwords for all services
- Keep your JWT secret key secure and rotate it regularly

## Monitoring

### Logs

View application logs:

```bash
# Docker Compose
docker-compose logs -f spidernet-api

# Single container
docker logs -f spidernet-api
```

### Metrics

The application exposes standard ASP.NET Core metrics. Consider integrating with:
- Prometheus + Grafana
- Application Insights
- Datadog
- New Relic

## Troubleshooting

### Common Issues

1. **Database Connection Issues**
   - Verify connection string format
   - Ensure database is accessible from the container
   - Check firewall rules

2. **Port Conflicts**
   - Change the host port in docker-compose.yml if 8080 is in use
   - Update your reverse proxy configuration accordingly

3. **Memory Issues**
   - Add memory limits to docker-compose.yml if needed:
   ```yaml
   deploy:
     resources:
       limits:
         memory: 512M
   ```

### Useful Commands

```bash
# Check container status
docker-compose ps

# Restart a specific service
docker-compose restart spidernet-api

# View resource usage
docker stats

# Access container shell
docker exec -it spidernet-api /bin/bash

# Clean up unused resources
docker system prune
```

## Cloud Deployment

### Azure Container Instances

```bash
az container create \
  --resource-group myResourceGroup \
  --name spidernet-backend \
  --image spidernet-backend:latest \
  --ports 8080 \
  --environment-variables \
    ConnectionStrings__PostgresConnection="your-connection-string" \
    JWT__Key="your-jwt-key"
```

### AWS ECS/Fargate

Create a task definition with the image and environment variables, then deploy to ECS.

### Google Cloud Run

```bash
gcloud run deploy spidernet-backend \
  --image spidernet-backend:latest \
  --port 8080 \
  --set-env-vars ConnectionStrings__PostgresConnection="your-connection-string"
```

## Development

For local development without Docker:

```bash
# Restore dependencies
dotnet restore

# Run the application
dotnet run --project SpiderNet.WebApi

# Run with hot reload
dotnet watch run --project SpiderNet.WebApi
```
