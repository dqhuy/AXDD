#!/bin/bash

# AXDD Docker Configuration Validation Script
# This script validates that all Docker services are configured correctly

set -e

echo "=========================================="
echo "AXDD Configuration Validation"
echo "=========================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Check if docker compose is available
if ! command -v docker &> /dev/null; then
    echo -e "${RED}✗ Docker is not installed${NC}"
    exit 1
fi

if ! docker compose version &> /dev/null; then
    echo -e "${RED}✗ Docker Compose is not available${NC}"
    exit 1
fi

echo -e "${GREEN}✓ Docker and Docker Compose are available${NC}"
echo ""

# Function to check service health
check_service() {
    local service=$1
    local status=$(docker compose ps --format json $service 2>/dev/null | jq -r '.[0].Health // .[0].State' 2>/dev/null || echo "unknown")
    
    if [ "$status" == "healthy" ] || [ "$status" == "running" ]; then
        echo -e "${GREEN}✓${NC} $service: $status"
        return 0
    elif [ "$status" == "unknown" ]; then
        echo -e "${YELLOW}?${NC} $service: not found"
        return 1
    else
        echo -e "${RED}✗${NC} $service: $status"
        return 1
    fi
}

# Function to check HTTP endpoint
check_endpoint() {
    local name=$1
    local url=$2
    local timeout=5
    
    if curl -f -s -m $timeout "$url" > /dev/null 2>&1; then
        echo -e "${GREEN}✓${NC} $name endpoint: $url"
        return 0
    else
        echo -e "${RED}✗${NC} $name endpoint: $url (not accessible)"
        return 1
    fi
}

echo "1. Checking Infrastructure Services..."
echo "--------------------------------------"
check_service "sqlserver"
check_service "postgres-gis"
check_service "redis"
check_service "rabbitmq"
check_service "minio"
check_service "elasticsearch"
echo ""

echo "2. Checking Microservices..."
echo "--------------------------------------"
check_service "auth-api"
check_service "masterdata-api"
check_service "enterprise-api"
check_service "investment-api"
check_service "filemanager-api"
check_service "report-api"
check_service "notification-api"
check_service "logging-api"
check_service "search-api"
check_service "gis-api"
echo ""

echo "3. Checking API Gateway and Web Apps..."
echo "--------------------------------------"
check_service "api-gateway"
check_service "admin-webapp"
check_service "enterprise-portal"
echo ""

echo "4. Testing HTTP Endpoints..."
echo "--------------------------------------"
check_endpoint "API Gateway" "http://localhost:5000"
check_endpoint "Admin Web App" "http://localhost:8080"
check_endpoint "Enterprise Portal" "http://localhost:4200"
echo ""

echo "5. Testing API Gateway Routes..."
echo "--------------------------------------"
# Note: These will fail if services don't have /health endpoints
# They're just to verify the gateway is routing correctly
for service in auth masterdata enterprise investment filemanager report notification logging search gis; do
    url="http://localhost:5000/api/$service/health"
    if curl -f -s -m 2 "$url" > /dev/null 2>&1; then
        echo -e "${GREEN}✓${NC} Gateway route: /api/$service"
    else
        echo -e "${YELLOW}?${NC} Gateway route: /api/$service (may not have /health endpoint)"
    fi
done
echo ""

echo "6. Checking Database Connections..."
echo "--------------------------------------"

# Check SQL Server
if docker compose exec -T sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "SELECT 1" -C > /dev/null 2>&1; then
    echo -e "${GREEN}✓${NC} SQL Server connection"
else
    echo -e "${RED}✗${NC} SQL Server connection"
fi

# Check PostgreSQL
if docker compose exec -T postgres-gis psql -U postgres -d gisdb -c "SELECT 1" > /dev/null 2>&1; then
    echo -e "${GREEN}✓${NC} PostgreSQL connection"
else
    echo -e "${RED}✗${NC} PostgreSQL connection"
fi

# Check Redis
if docker compose exec -T redis redis-cli ping > /dev/null 2>&1; then
    echo -e "${GREEN}✓${NC} Redis connection"
else
    echo -e "${RED}✗${NC} Redis connection"
fi

# Check RabbitMQ
if docker compose exec -T rabbitmq rabbitmq-diagnostics -q ping > /dev/null 2>&1; then
    echo -e "${GREEN}✓${NC} RabbitMQ connection"
else
    echo -e "${RED}✗${NC} RabbitMQ connection"
fi

echo ""
echo "=========================================="
echo "Validation Complete!"
echo "=========================================="
echo ""
echo "Access Points:"
echo "  Admin Portal:      http://localhost:8080"
echo "  Enterprise Portal: http://localhost:4200"
echo "  API Gateway:       http://localhost:5000"
echo ""
echo "Management Consoles:"
echo "  RabbitMQ:          http://localhost:15672 (admin/admin)"
echo "  MinIO:             http://localhost:9001 (minioadmin/minioadmin)"
echo ""
echo "To view logs:"
echo "  docker compose logs -f [service-name]"
echo ""
