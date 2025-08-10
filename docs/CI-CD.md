# 🚀 CI/CD Pipeline Documentation

## 📋 Overview

The Healthy System employs a comprehensive CI/CD pipeline using GitHub Actions that includes:

- **Continuous Integration**: Automated building, testing, and validation
- **Continuous Deployment**: Automated deployment to development and production environments
- **Security Scanning**: Automated vulnerability detection and dependency checks
- **Performance Testing**: Load testing and performance monitoring
- **Quality Assurance**: Code quality checks and coverage reporting

## 🏗️ Pipeline Architecture

### Workflow Files

| Workflow | Purpose | Trigger |
|----------|---------|---------|
| `ci-cd.yml` | Main CI/CD pipeline | Push to main/develop, Manual dispatch |
| `pr-validation.yml` | Pull request validation | PR opened/updated |
| `security-scan.yml` | Security scanning | Schedule (weekly), Push to main/develop |
| `release.yml` | Release management | Tags (v*.*.*), Manual dispatch |
| `dependency-update.yml` | Dependency updates | Schedule (weekly), Manual dispatch |

### Pipeline Stages

```
Code Changes → Change Detection → Build & Test → Docker Build → Deploy → Test → Monitor
                                ↓               ↓
                         Code Quality    Security Scan
```

## 🔄 Continuous Integration

### Build Process
1. **Environment Setup**: .NET 9.0 SDK installation
2. **Dependency Caching**: NuGet package caching for faster builds
3. **Restore**: Package dependency restoration
4. **Build**: Solution compilation in Release mode
5. **Test Database**: SQL Server container setup
6. **Database Migration**: EF Core migration application
7. **Unit Tests**: Comprehensive test execution with coverage
8. **Coverage Reporting**: Test coverage analysis and reporting

### Quality Gates
- Build must succeed
- All unit tests must pass
- Code coverage thresholds (configurable)
- Security scans must pass
- No critical vulnerabilities in dependencies

## 🐳 Containerization

### Docker Strategy
- **Multi-stage build** for optimized image size
- **Non-root user** for security
- **Health checks** for monitoring
- **Multi-platform** support (AMD64, ARM64)

### Image Lifecycle
1. **Build**: Create optimized production image
2. **Scan**: Trivy vulnerability scanning
3. **Push**: GitHub Container Registry (ghcr.io)
4. **Deploy**: Container orchestration
5. **Monitor**: Health check validation

## 🚀 Deployment Strategy

### Environments

#### Development Environment
- **Trigger**: Push to `develop` branch
- **Target**: Development infrastructure
- **Features**: 
  - Hot reload enabled
  - Debug logging
  - Test data seeding
  - Performance testing

#### Production Environment
- **Trigger**: Push to `main` branch
- **Target**: Production infrastructure
- **Features**:
  - Optimized performance
  - Security hardening
  - Database migrations
  - Smoke testing

### Deployment Process
1. **Pre-deployment**: Environment validation
2. **Database Migration**: Schema updates
3. **Application Deployment**: Rolling deployment
4. **Health Checks**: Service validation
5. **Post-deployment**: Smoke tests

## 🔒 Security Integration

### Security Scanning
- **SAST**: Static Application Security Testing (CodeQL)
- **Dependency Scanning**: Vulnerability detection (Snyk)
- **Container Scanning**: Image vulnerability scanning (Trivy)
- **Secret Scanning**: Credential leak detection (TruffleHog)
- **License Compliance**: Open source license validation

### Security Gates
- No high/critical vulnerabilities
- All secrets properly configured
- License compliance validated
- Security policy adherence

## ⚡ Performance Testing

### Load Testing Strategy
- **Framework**: Artillery.js
- **Scenarios**: Realistic user workflows
- **Metrics**: Response time, throughput, error rates
- **Thresholds**: Performance regression detection

### Test Scenarios
1. **Health Checks**: Basic availability testing
2. **Authentication**: Login/logout flows
3. **CRUD Operations**: Data manipulation testing
4. **Dashboard Access**: Read-heavy operations

## 📊 Monitoring & Observability

### Key Metrics
- **Build Success Rate**: Pipeline reliability
- **Test Coverage**: Code quality indicator
- **Deployment Frequency**: Development velocity
- **Lead Time**: Change deployment speed
- **Mean Time to Recovery**: Incident response

### Alerts & Notifications
- Build failures
- Security vulnerabilities
- Performance degradation
- Deployment failures

## 🛠️ Development Workflow

### Feature Development
1. Create feature branch from `develop`
2. Implement changes with tests
3. Create pull request
4. Automated validation runs
5. Code review process
6. Merge to `develop`
7. Automated deployment to development

### Release Process
1. Create release branch from `develop`
2. Version bump and changelog
3. Create release tag
4. Automated production deployment
5. Release notes generation

## 🔧 Configuration

### Environment Variables

#### Required Variables
```bash
# Database
DB_PASSWORD=<strong-password>
ConnectionStrings__DefaultConnection=<connection-string>

# JWT
JWT_SECRET_KEY=<secret-key>
JWT_ISSUER=HealthySystem
JWT_AUDIENCE=HealthySystemUsers

# Application
ASPNETCORE_ENVIRONMENT=<environment>
```

#### CI/CD Variables
```bash
# Container Registry
REGISTRY=ghcr.io
IMAGE_NAME=phuong-practice-projects/healthy-system/healthy-api

# .NET Configuration
DOTNET_VERSION=9.0.x

# Performance Testing
PERFORMANCE_TARGET_URL=<target-url>
```

### GitHub Secrets
- `PRODUCTION_DB_PASSWORD`: Production database password
- `SNYK_TOKEN`: Snyk API token for security scanning
- `GITHUB_TOKEN`: Automatically provided by GitHub

### GitHub Variables
- `DEVELOPMENT_URL`: Development environment URL
- `PRODUCTION_URL`: Production environment URL

## 🚀 Getting Started

### Prerequisites
- GitHub repository with proper permissions
- Container registry access
- Target deployment infrastructure

### Setup Steps
1. **Fork/Clone**: Repository setup
2. **Secrets Configuration**: Add required secrets
3. **Environment Setup**: Configure variables
4. **Infrastructure**: Prepare deployment targets
5. **First Run**: Trigger initial pipeline

### Local Development
```bash
# Start development environment
docker-compose -f docker-compose.dev.yml up -d

# Run the application
cd Healthy
dotnet run --project Healthy.Api

# Run tests
dotnet test

# Performance testing
cd performance-tests
npm test
```

## 🐛 Troubleshooting

### Common Issues

#### Build Failures
- Check .NET version compatibility
- Verify package references
- Review dependency conflicts

#### Test Failures
- Database connection issues
- Missing test data
- Environment configuration

#### Deployment Issues
- Container registry permissions
- Target environment access
- Configuration mismatches

#### Performance Problems
- Resource constraints
- Database performance
- Network latency

### Debug Mode
Enable verbose logging in workflows:
```yaml
- name: Debug Step
  run: echo "ACTIONS_STEP_DEBUG=true" >> $GITHUB_ENV
```

## 📈 Optimization

### Build Performance
- Dependency caching
- Parallel job execution
- Incremental builds
- Resource optimization

### Deployment Efficiency
- Blue-green deployments
- Rolling updates
- Health check optimization
- Rollback strategies

## 📚 Best Practices

### Code Quality
- Write comprehensive tests
- Follow coding standards
- Implement proper logging
- Use dependency injection

### Security
- Regular security updates
- Principle of least privilege
- Secure secret management
- Regular vulnerability scanning

### Performance
- Monitor key metrics
- Set performance budgets
- Regular load testing
- Optimize bottlenecks

### Maintenance
- Regular dependency updates
- Pipeline optimization
- Documentation updates
- Monitoring review

## 📞 Support

- **Documentation**: See individual README files
- **Issues**: GitHub issue tracker
- **Security**: See [SECURITY.md](../SECURITY.md)
- **Performance**: See [performance-tests/README.md](../performance-tests/README.md)