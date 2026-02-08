#!/bin/bash

# AXDD System - Quick Start Script
# This script helps you quickly start the entire AXDD system with Docker Compose

set -e

echo "=========================================="
echo "AXDD System - Docker Quick Start"
echo "=========================================="
echo ""

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "❌ Docker is not installed. Please install Docker first."
    exit 1
fi

echo "✓ Docker is installed"

# Check if Docker Compose is available
if ! docker compose version &> /dev/null; then
    echo "❌ Docker Compose V2 is not available. Please update Docker."
    exit 1
fi

echo "✓ Docker Compose is available"
echo ""

# Function to wait for a service to be healthy
wait_for_health() {
    local service=$1
    local max_wait=300
    local elapsed=0
    
    echo "⏳ Waiting for $service to be healthy..."
    
    while [ $elapsed -lt $max_wait ]; do
        if docker compose ps | grep "$service" | grep "healthy" &> /dev/null; then
            echo "✓ $service is healthy"
            return 0
        fi
        sleep 5
        elapsed=$((elapsed + 5))
    done
    
    echo "⚠️  $service did not become healthy in time"
    return 1
}

# Parse command line arguments
ACTION="${1:-up}"

case "$ACTION" in
    up|start)
        echo "🚀 Starting AXDD system..."
        echo ""
        
        # Start infrastructure services first
        echo "📦 Starting infrastructure services..."
        docker compose up -d sqlserver postgres-gis redis rabbitmq minio elasticsearch
        
        echo ""
        echo "⏳ Waiting for infrastructure services to be healthy..."
        echo "This may take 1-2 minutes..."
        echo ""
        
        # Wait for critical infrastructure
        wait_for_health "sqlserver"
        wait_for_health "postgres-gis"
        wait_for_health "redis"
        wait_for_health "rabbitmq"
        wait_for_health "minio"
        wait_for_health "elasticsearch"
        
        echo ""
        echo "🔧 Starting microservices..."
        docker compose up -d auth-api masterdata-api enterprise-api investment-api \
                          filemanager-api report-api notification-api logging-api \
                          search-api gis-api
        
        echo ""
        echo "⏳ Waiting for services to start..."
        sleep 10
        
        echo ""
        echo "🌐 Starting API Gateway and Web Applications..."
        docker compose up -d api-gateway admin-webapp enterprise-portal
        
        echo ""
        echo "=========================================="
        echo "✅ AXDD System Started Successfully!"
        echo "=========================================="
        echo ""
        echo "Access Points:"
        echo "  🌐 Admin Portal:       http://localhost:8080"
        echo "  🌐 Enterprise Portal:  http://localhost:4200"
        echo "  🔌 API Gateway:        http://localhost:5000"
        echo ""
        echo "Infrastructure Management:"
        echo "  📊 RabbitMQ Console:   http://localhost:15672 (admin/admin)"
        echo "  💾 MinIO Console:      http://localhost:9001 (minioadmin/minioadmin)"
        echo ""
        echo "To view logs:"
        echo "  docker compose logs -f"
        echo ""
        echo "To stop the system:"
        echo "  ./docker-start.sh stop"
        echo ""
        ;;
        
    down|stop)
        echo "🛑 Stopping AXDD system..."
        docker compose down
        echo "✅ System stopped"
        ;;
        
    restart)
        echo "🔄 Restarting AXDD system..."
        docker compose down
        sleep 2
        "$0" up
        ;;
        
    logs)
        docker compose logs -f
        ;;
        
    ps|status)
        docker compose ps
        ;;
        
    clean)
        echo "⚠️  This will stop all services and remove all volumes (data will be lost!)"
        read -p "Are you sure? (yes/no): " confirm
        if [ "$confirm" == "yes" ]; then
            docker compose down -v
            echo "✅ System stopped and volumes removed"
        else
            echo "Cancelled"
        fi
        ;;
        
    *)
        echo "Usage: $0 {up|down|restart|logs|ps|clean}"
        echo ""
        echo "Commands:"
        echo "  up       - Start the AXDD system"
        echo "  down     - Stop the AXDD system"
        echo "  restart  - Restart the AXDD system"
        echo "  logs     - View logs from all services"
        echo "  ps       - Show status of all services"
        echo "  clean    - Stop and remove all data (use with caution!)"
        exit 1
        ;;
esac
