#!/bin/bash

# 🚀 Healthy System - Local Development Setup Script
# This script sets up the local development environment

set -e  # Exit on any error

echo "🚀 Setting up Healthy System Development Environment..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if running in correct directory
if [ ! -f "Healthy/Healthy.sln" ]; then
    print_error "Please run this script from the project root directory"
    exit 1
fi

# Check prerequisites
print_status "Checking prerequisites..."

# Check Docker
if ! command -v docker &> /dev/null; then
    print_error "Docker is not installed. Please install Docker first."
    exit 1
fi

# Check Docker Compose
if ! command -v docker-compose &> /dev/null; then
    print_error "Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

# Check .NET
if ! command -v dotnet &> /dev/null; then
    print_error ".NET SDK is not installed. Please install .NET 9.0 SDK first."
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version | cut -d. -f1)
if [ "$DOTNET_VERSION" -lt "8" ]; then
    print_warning "Detected .NET version $(dotnet --version). Recommended: .NET 8.0 or higher."
fi

print_success "All prerequisites are installed"

# Setup environment file
print_status "Setting up environment configuration..."

if [ ! -f ".env" ]; then
    if [ -f ".env.example" ]; then
        cp .env.example .env
        print_success "Created .env file from .env.example"
    else
        print_warning ".env.example not found, creating basic .env file"
        cat > .env << EOF
# Development Environment Configuration
ASPNETCORE_ENVIRONMENT=Development
DB_PASSWORD=Dev@Passw0rd123!
JWT_SECRET_KEY=your-super-secret-jwt-key-here-minimum-32-characters
JWT_ISSUER=HealthySystem
JWT_AUDIENCE=HealthySystemUsers
JWT_EXPIRY_MINUTES=60
EOF
        print_success "Created basic .env file"
    fi
else
    print_success "Environment file already exists"
fi

# Start database container
print_status "Starting database container..."
docker-compose -f docker-compose.dev.yml up -d healthy-db

# Wait for database to be ready
print_status "Waiting for database to be ready..."
sleep 10

# Test database connection
MAX_ATTEMPTS=30
ATTEMPT=1
while [ $ATTEMPT -le $MAX_ATTEMPTS ]; do
    if docker-compose -f docker-compose.dev.yml exec -T healthy-db /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Dev@Passw0rd123" -C -Q "SELECT 1" > /dev/null 2>&1; then
        print_success "Database is ready!"
        break
    else
        print_status "Waiting for database... (attempt $ATTEMPT/$MAX_ATTEMPTS)"
        sleep 3
        ATTEMPT=$((ATTEMPT + 1))
    fi
done

if [ $ATTEMPT -gt $MAX_ATTEMPTS ]; then
    print_error "Database failed to start after $MAX_ATTEMPTS attempts"
    exit 1
fi

# Restore .NET packages
print_status "Restoring .NET packages..."
cd Healthy
dotnet restore

if [ $? -eq 0 ]; then
    print_success "Packages restored successfully"
else
    print_error "Failed to restore packages"
    exit 1
fi

# Build the solution
print_status "Building the solution..."
dotnet build

if [ $? -eq 0 ]; then
    print_success "Solution built successfully"
else
    print_error "Failed to build solution"
    exit 1
fi

# Run database migrations
print_status "Running database migrations..."
dotnet ef database update --project Healthy.Infrastructure --startup-project Healthy.Api

if [ $? -eq 0 ]; then
    print_success "Database migrations completed"
else
    print_warning "Database migrations failed, but continuing..."
fi

cd ..

# Setup performance testing
print_status "Setting up performance testing..."
if [ -d "performance-tests" ]; then
    cd performance-tests
    if command -v npm &> /dev/null; then
        npm install
        print_success "Performance testing dependencies installed"
    else
        print_warning "npm not found, performance testing setup skipped"
    fi
    cd ..
fi

# Final status
print_success "🎉 Development environment setup completed!"
echo
echo "📋 Next steps:"
echo "  1. Start the API: cd Healthy && dotnet run --project Healthy.Api"
echo "  2. Open browser: http://localhost:5001/swagger"
echo "  3. Check health: http://localhost:5001/health"
echo
echo "🐳 Container commands:"
echo "  • Start all services: docker-compose -f docker-compose.dev.yml up -d"
echo "  • Stop all services: docker-compose -f docker-compose.dev.yml down"
echo "  • View logs: docker-compose -f docker-compose.dev.yml logs -f"
echo
echo "🧪 Testing commands:"
echo "  • Run tests: cd Healthy && dotnet test"
echo "  • Performance tests: cd performance-tests && npm test"
echo
echo "📚 Documentation:"
echo "  • Setup guide: README.SETUP.md"
echo "  • CI/CD guide: docs/CI-CD.md"
echo "  • Architecture: README.Architecture.md"
echo
print_success "Happy coding! 🚀"