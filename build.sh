#!/bin/bash

echo "=========================================="
echo "AXDD - Build and Test Script"
echo "=========================================="

# Navigate to src directory
cd src

# Clean
echo ""
echo "Cleaning solution..."
dotnet clean

# Restore
echo ""
echo "Restoring packages..."
dotnet restore

# Build
echo ""
echo "Building solution..."
dotnet build

# Test
echo ""
echo "Running tests..."
dotnet test --no-build

# Success
echo ""
echo "=========================================="
echo "Build and test completed successfully!"
echo "=========================================="
echo ""
echo "To run services:"
echo "  - docker-compose up --build"
echo "  - Or run individual services with 'dotnet run'"
echo ""
