#!/bin/bash

# Load Sample Data into Search Service
# This script loads sample enterprises, documents, and projects into Elasticsearch

API_URL="${1:-http://localhost:5000}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "Loading sample data into Search Service at $API_URL"
echo "================================================"

# Initialize indexes
echo ""
echo "1. Initializing indexes..."
curl -X POST "$API_URL/api/v1/index/initialize" \
  -H "Content-Type: application/json" \
  -w "\nHTTP Status: %{http_code}\n\n"

sleep 2

# Load enterprises
echo "2. Loading enterprises..."
curl -X POST "$API_URL/api/v1/index/enterprises/bulk" \
  -H "Content-Type: application/json" \
  -d @"$SCRIPT_DIR/enterprises.json" \
  -w "\nHTTP Status: %{http_code}\n\n"

sleep 1

# Load documents
echo "3. Loading documents..."
curl -X POST "$API_URL/api/v1/index/documents/bulk" \
  -H "Content-Type: application/json" \
  -d @"$SCRIPT_DIR/documents.json" \
  -w "\nHTTP Status: %{http_code}\n\n"

sleep 1

# Load projects
echo "4. Loading projects..."
curl -X POST "$API_URL/api/v1/index/projects/bulk" \
  -H "Content-Type: application/json" \
  -d @"$SCRIPT_DIR/projects.json" \
  -w "\nHTTP Status: %{http_code}\n\n"

sleep 1

# Refresh indexes to make data searchable immediately
echo "5. Refreshing indexes..."
curl -X POST "$API_URL/api/v1/index/enterprises_idx/refresh" \
  -H "Content-Type: application/json" \
  -w "\nHTTP Status: %{http_code}\n"

curl -X POST "$API_URL/api/v1/index/documents_idx/refresh" \
  -H "Content-Type: application/json" \
  -w "\nHTTP Status: %{http_code}\n"

curl -X POST "$API_URL/api/v1/index/projects_idx/refresh" \
  -H "Content-Type: application/json" \
  -w "\nHTTP Status: %{http_code}\n"

echo ""
echo "================================================"
echo "Sample data loaded successfully!"
echo ""
echo "Try these example searches:"
echo "1. curl -X POST $API_URL/api/v1/search/enterprises -H 'Content-Type: application/json' -d '{\"query\":\"Samsung\"}'"
echo "2. curl -X POST $API_URL/api/v1/search/documents -H 'Content-Type: application/json' -d '{\"query\":\"hợp đồng\"}'"
echo "3. curl -X POST $API_URL/api/v1/search/all -H 'Content-Type: application/json' -d '{\"query\":\"Biên Hòa\"}'"
echo "4. curl '$API_URL/api/v1/search/suggestions?q=công&type=enterprise'"
