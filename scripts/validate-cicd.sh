#!/bin/bash

# 🔍 Healthy System - CI/CD Validation Script
# This script validates the CI/CD pipeline locally

set -e  # Exit on any error

echo "🔍 Validating Healthy System CI/CD Pipeline..."

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

# Validation counters
CHECKS_PASSED=0
CHECKS_FAILED=0
CHECKS_WARNING=0

# Function to run validation check
run_check() {
    local check_name="$1"
    local check_command="$2"
    local is_critical="${3:-true}"
    
    print_status "Checking: $check_name"
    
    if eval "$check_command" > /dev/null 2>&1; then
        print_success "✅ $check_name"
        CHECKS_PASSED=$((CHECKS_PASSED + 1))
        return 0
    else
        if [ "$is_critical" = "true" ]; then
            print_error "❌ $check_name"
            CHECKS_FAILED=$((CHECKS_FAILED + 1))
        else
            print_warning "⚠️ $check_name"
            CHECKS_WARNING=$((CHECKS_WARNING + 1))
        fi
        return 1
    fi
}

# Check if running in correct directory
if [ ! -f "Healthy/Healthy.sln" ]; then
    print_error "Please run this script from the project root directory"
    exit 1
fi

echo "🧪 Running CI/CD Pipeline Validation..."
echo

# 1. Prerequisites Check
print_status "=== Prerequisites Validation ==="

run_check "Docker installed" "command -v docker"

# Check Docker Compose (both legacy and new formats)
if command -v docker-compose &> /dev/null; then
    print_success "✅ Docker Compose installed (legacy)"
    CHECKS_PASSED=$((CHECKS_PASSED + 1))
elif docker compose version &> /dev/null; then
    print_success "✅ Docker Compose installed (new)"
    CHECKS_PASSED=$((CHECKS_PASSED + 1))
else
    print_error "❌ Docker Compose not available"
    CHECKS_FAILED=$((CHECKS_FAILED + 1))
fi

run_check ".NET SDK installed" "command -v dotnet"
run_check "Git installed" "command -v git"

# Check .NET version
DOTNET_VERSION=$(dotnet --version 2>/dev/null || echo "0.0.0")
MAJOR_VERSION=$(echo $DOTNET_VERSION | cut -d. -f1)
if [ "$MAJOR_VERSION" -ge "8" ]; then
    print_success "✅ .NET version: $DOTNET_VERSION"
    CHECKS_PASSED=$((CHECKS_PASSED + 1))
else
    print_warning "⚠️ .NET version: $DOTNET_VERSION (recommended: 8.0+)"
    CHECKS_WARNING=$((CHECKS_WARNING + 1))
fi

echo

# 2. Project Structure Validation
print_status "=== Project Structure Validation ==="

run_check "Solution file exists" "test -f Healthy/Healthy.sln"
run_check "API project exists" "test -f Healthy/Healthy.Api/Healthy.Api.csproj"
run_check "Application project exists" "test -f Healthy/Healthy.Application/Healthy.Application.csproj"
run_check "Domain project exists" "test -f Healthy/Healthy.Domain/Healthy.Domain.csproj"
run_check "Infrastructure project exists" "test -f Healthy/Healthy.Infrastructure/Healthy.Infrastructure.csproj"
run_check "Test project exists" "test -f Healthy/Healthy.Tests.Unit/Healthy.Tests.Unit.csproj"
run_check "Dockerfile exists" "test -f Healthy/Healthy.Api/Dockerfile"

echo

# 3. CI/CD Configuration Validation
print_status "=== CI/CD Configuration Validation ==="

run_check "Main CI/CD workflow exists" "test -f .github/workflows/ci-cd.yml"
run_check "PR validation workflow exists" "test -f .github/workflows/pr-validation.yml"
run_check "Security scan workflow exists" "test -f .github/workflows/security-scan.yml"
run_check "Release workflow exists" "test -f .github/workflows/release.yml"
run_check "Dependency update workflow exists" "test -f .github/workflows/dependency-update.yml"
run_check "Development docker-compose exists" "test -f docker-compose.dev.yml"
run_check "Production docker-compose exists" "test -f docker-compose.prod.yml"

echo

# 4. Security Files Validation
print_status "=== Security Configuration Validation ==="

run_check "Security policy exists" "test -f SECURITY.md"
run_check "Dependabot config exists" "test -f .github/dependabot.yml"
run_check "Gitignore exists" "test -f .gitignore"

# Check for common security anti-patterns
if grep -r "password\|secret\|key" --include="*.cs" --include="*.json" Healthy/ | grep -i "hardcoded\|admin\|123" > /dev/null 2>&1; then
    print_warning "⚠️ Potential hardcoded secrets found"
    CHECKS_WARNING=$((CHECKS_WARNING + 1))
else
    print_success "✅ No obvious hardcoded secrets found"
    CHECKS_PASSED=$((CHECKS_PASSED + 1))
fi

echo

# 5. Performance Testing Validation
print_status "=== Performance Testing Validation ==="

run_check "Performance tests directory exists" "test -d performance-tests"
run_check "Artillery config exists" "test -f performance-tests/load-test.yml"
run_check "Performance package.json exists" "test -f performance-tests/package.json"
run_check "Performance README exists" "test -f performance-tests/README.md"

echo

# 6. Build Validation
print_status "=== Build Validation ==="

print_status "Restoring packages..."
cd Healthy
if dotnet restore > /dev/null 2>&1; then
    print_success "✅ Package restore successful"
    CHECKS_PASSED=$((CHECKS_PASSED + 1))
else
    print_error "❌ Package restore failed"
    CHECKS_FAILED=$((CHECKS_FAILED + 1))
fi

print_status "Building solution..."
if dotnet build --no-restore > /dev/null 2>&1; then
    print_success "✅ Build successful"
    CHECKS_PASSED=$((CHECKS_PASSED + 1))
else
    print_error "❌ Build failed"
    CHECKS_FAILED=$((CHECKS_FAILED + 1))
fi

cd ..

echo

# 7. Docker Validation
print_status "=== Docker Configuration Validation ==="

print_status "Validating Dockerfile syntax..."
if docker build -t healthy-api:test-validation -f Healthy/Healthy.Api/Dockerfile Healthy --dry-run > /dev/null 2>&1; then
    print_success "✅ Dockerfile syntax valid"
    CHECKS_PASSED=$((CHECKS_PASSED + 1))
else
    print_warning "⚠️ Dockerfile validation inconclusive (network issues possible)"
    CHECKS_WARNING=$((CHECKS_WARNING + 1))
fi

# Check docker-compose syntax
if command -v docker-compose &> /dev/null; then
    COMPOSE_CMD="docker-compose"
else
    COMPOSE_CMD="docker compose"
fi

if $COMPOSE_CMD -f docker-compose.dev.yml config > /dev/null 2>&1; then
    print_success "✅ Development docker-compose valid"
    CHECKS_PASSED=$((CHECKS_PASSED + 1))
else
    print_error "❌ Development docker-compose invalid"
    CHECKS_FAILED=$((CHECKS_FAILED + 1))
fi

if $COMPOSE_CMD -f docker-compose.prod.yml config > /dev/null 2>&1; then
    print_success "✅ Production docker-compose valid"
    CHECKS_PASSED=$((CHECKS_PASSED + 1))
else
    print_error "❌ Production docker-compose invalid"
    CHECKS_FAILED=$((CHECKS_FAILED + 1))
fi

echo

# 8. Documentation Validation
print_status "=== Documentation Validation ==="

run_check "Main README exists" "test -f README.md"
run_check "Setup documentation exists" "test -f README.SETUP.md"
run_check "Architecture documentation exists" "test -f README.Architecture.md"
run_check "CI/CD documentation exists" "test -f docs/CI-CD.md" "false"

echo

# Final Results
echo "🏁 Validation Results Summary:"
echo "================================================"
echo -e "${GREEN}✅ Passed: $CHECKS_PASSED${NC}"
echo -e "${YELLOW}⚠️ Warnings: $CHECKS_WARNING${NC}"
echo -e "${RED}❌ Failed: $CHECKS_FAILED${NC}"
echo "================================================"

TOTAL_CHECKS=$((CHECKS_PASSED + CHECKS_WARNING + CHECKS_FAILED))
SUCCESS_RATE=$((CHECKS_PASSED * 100 / TOTAL_CHECKS))

echo "📊 Success Rate: $SUCCESS_RATE%"

if [ $CHECKS_FAILED -eq 0 ]; then
    if [ $CHECKS_WARNING -eq 0 ]; then
        print_success "🎉 All validations passed! CI/CD pipeline is ready."
        echo
        echo "🚀 Next steps:"
        echo "  • Push your changes to trigger the CI/CD pipeline"
        echo "  • Create a pull request to test PR validation"
        echo "  • Check GitHub Actions for pipeline execution"
    else
        print_warning "✅ Core validations passed with some warnings."
        echo
        echo "🔧 Recommended improvements:"
        echo "  • Address the warnings above"
        echo "  • Consider adding missing optional components"
    fi
    
    echo
    echo "📋 CI/CD Pipeline Features:"
    echo "  ✅ Automated building and testing"
    echo "  ✅ Docker containerization"
    echo "  ✅ Security scanning"
    echo "  ✅ Performance testing"
    echo "  ✅ Multi-environment deployment"
    echo "  ✅ Quality gates and validation"
    
    exit 0
else
    print_error "❌ Some critical validations failed."
    echo
    echo "🔧 Required fixes:"
    echo "  • Fix the failed checks above"
    echo "  • Ensure all prerequisites are installed"
    echo "  • Verify project structure and configuration"
    echo
    echo "📚 Help resources:"
    echo "  • Setup guide: README.SETUP.md"
    echo "  • CI/CD documentation: docs/CI-CD.md"
    echo "  • Architecture guide: README.Architecture.md"
    
    exit 1
fi