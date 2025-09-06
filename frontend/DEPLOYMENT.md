# SpiderNet Frontend Deployment Guide

This guide covers how to deploy the SpiderNet frontend using Docker.

## Prerequisites

- Docker and Docker Compose installed
- Node.js 20+ (for local development)
- Backend API running (see backend deployment guide)

## Quick Start with Docker

### Build and Run

```bash
# Navigate to the frontend directory
cd frontend

# Build the Docker image
docker build -t spidernet-frontend --build-arg NEXT_PUBLIC_API_URL=http://localhost:8080 .

# Run the container
docker run -d \
  --name spidernet-frontend \
  -p 3000:3000 \
  -e NEXT_PUBLIC_API_URL=http://localhost:8080 \
  spidernet-frontend
```

### Using Docker Compose

```bash
# Set environment variables (optional, defaults to localhost:8080)
export NEXT_PUBLIC_API_URL=http://localhost:8080

# Build and start the service
docker-compose up -d

# View logs
docker-compose logs -f spidernet-frontend

# Stop the service
docker-compose down
```

## Environment Configuration

### Required Environment Variables

Create a `.env.local` file for local development:

```bash
# Copy the example file
cp env.example .env.local

# Edit the file with your values
NEXT_PUBLIC_API_URL=http://localhost:8080
```

### Build-time vs Runtime Variables

- `NEXT_PUBLIC_API_URL`: **Build-time variable** - Must be set during Docker build
- This variable is embedded into the client-side bundle at build time

## Production Deployment

### Environment-Specific Builds

For different environments, build with appropriate API URLs:

```bash
# Development
docker build -t spidernet-frontend:dev --build-arg NEXT_PUBLIC_API_URL=http://localhost:8080 .

# Staging
docker build -t spidernet-frontend:staging --build-arg NEXT_PUBLIC_API_URL=https://api-staging.spidernet.com .

# Production
docker build -t spidernet-frontend:prod --build-arg NEXT_PUBLIC_API_URL=https://api.spidernet.com .
```

### Full Stack Deployment

Create a `docker-compose.full-stack.yml` for complete deployment:

```yaml
version: '3.8'

services:
  spidernet-backend:
    image: spidernet-backend:latest
    container_name: spidernet-backend
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__PostgresConnection=your-connection-string
    networks:
      - spidernet-network

  spidernet-frontend:
    image: spidernet-frontend:latest
    container_name: spidernet-frontend
    ports:
      - "3000:3000"
    environment:
      - NEXT_PUBLIC_API_URL=http://spidernet-backend:8080
    depends_on:
      - spidernet-backend
    networks:
      - spidernet-network

networks:
  spidernet-network:
    driver: bridge
```

### Reverse Proxy Setup (Nginx)

For production, use a reverse proxy:

```nginx
server {
    listen 80;
    server_name your-domain.com;

    # Frontend
    location / {
        proxy_pass http://localhost:3000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }

    # API
    location /api/ {
        proxy_pass http://localhost:8080/;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

## Performance Optimization

### Docker Multi-Stage Build

The Dockerfile uses a multi-stage build for optimization:

1. **deps**: Install only production dependencies
2. **builder**: Build the application with all dev dependencies
3. **runner**: Minimal runtime image with only necessary files

### Next.js Optimizations

The configuration includes:

- **Standalone output**: Minimal server bundle
- **Image optimization**: Disabled for containerized deployments
- **CSS optimization**: Experimental feature for better performance

### Build Optimizations

```bash
# Build with specific optimizations
docker build \
  --build-arg NEXT_PUBLIC_API_URL=https://api.spidernet.com \
  --target runner \
  -t spidernet-frontend:optimized .
```

## Health Checks and Monitoring

### Health Check Endpoint

The container includes a health check. Test it manually:

```bash
# Check container health
docker inspect --format='{{.State.Health.Status}}' spidernet-frontend

# Manual health check
curl http://localhost:3000
```

### Monitoring

Consider integrating with:

- **Vercel Analytics** (for Vercel deployments)
- **Google Analytics**
- **Sentry** for error tracking
- **LogRocket** for user session recording

## Cloud Deployment

### Vercel (Recommended for Next.js)

```bash
# Install Vercel CLI
npm i -g vercel

# Deploy
vercel --prod

# Set environment variables in Vercel dashboard
# NEXT_PUBLIC_API_URL=https://your-backend-url.com
```

### AWS ECS/Fargate

```bash
# Push to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin your-account.dkr.ecr.us-east-1.amazonaws.com

docker tag spidernet-frontend:latest your-account.dkr.ecr.us-east-1.amazonaws.com/spidernet-frontend:latest

docker push your-account.dkr.ecr.us-east-1.amazonaws.com/spidernet-frontend:latest
```

### Google Cloud Run

```bash
# Build and push to Container Registry
gcloud builds submit --tag gcr.io/your-project/spidernet-frontend

# Deploy to Cloud Run
gcloud run deploy spidernet-frontend \
  --image gcr.io/your-project/spidernet-frontend \
  --platform managed \
  --port 3000 \
  --set-env-vars NEXT_PUBLIC_API_URL=https://your-backend-url.com
```

### Azure Container Instances

```bash
az container create \
  --resource-group myResourceGroup \
  --name spidernet-frontend \
  --image spidernet-frontend:latest \
  --ports 3000 \
  --environment-variables NEXT_PUBLIC_API_URL=https://your-backend-url.com
```

## Development

### Local Development

```bash
# Install dependencies
npm install

# Run development server
npm run dev

# Build for production
npm run build

# Start production server
npm start

# Lint code
npm run lint
```

### Development with Docker

```bash
# Development with hot reload (requires volume mounting)
docker run -d \
  --name spidernet-frontend-dev \
  -p 3000:3000 \
  -v $(pwd):/app \
  -v /app/node_modules \
  spidernet-frontend:dev \
  npm run dev
```

## Troubleshooting

### Common Issues

1. **API Connection Issues**
   - Verify `NEXT_PUBLIC_API_URL` is correct
   - Check network connectivity between containers
   - Ensure backend is running and accessible

2. **Build Failures**
   - Check Node.js version compatibility
   - Clear build cache: `docker build --no-cache`
   - Verify all dependencies are available

3. **Runtime Errors**
   - Check container logs: `docker logs spidernet-frontend`
   - Verify environment variables are set correctly
   - Check file permissions in container

### Debug Commands

```bash
# Check running containers
docker ps

# View container logs
docker logs -f spidernet-frontend

# Access container shell
docker exec -it spidernet-frontend /bin/sh

# Check build output
docker build -t spidernet-frontend . --progress=plain

# Test health check manually
docker exec spidernet-frontend wget --spider http://localhost:3000
```

### Performance Issues

```bash
# Check resource usage
docker stats spidernet-frontend

# Analyze bundle size
npm run build && npx @next/bundle-analyzer

# Profile build time
docker build -t spidernet-frontend . --progress=plain --no-cache
```

## Security Considerations

- Container runs as non-root user (`nextjs:nodejs`)
- No sensitive data in client-side bundle
- Use HTTPS in production
- Implement proper CORS policies
- Regular security updates for base images

## CI/CD Pipeline Example

```yaml
# .github/workflows/deploy-frontend.yml
name: Deploy Frontend

on:
  push:
    branches: [main]
    paths: ['frontend/**']

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Build Docker image
        run: |
          cd frontend
          docker build \
            --build-arg NEXT_PUBLIC_API_URL=${{ secrets.API_URL }} \
            -t spidernet-frontend:${{ github.sha }} .
      
      - name: Deploy to production
        run: |
          # Your deployment commands here
```
