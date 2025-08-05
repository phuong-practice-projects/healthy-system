# 🚀 GitHub Workflows Documentation

This directory contains GitHub Actions workflows for the Healthy System project, providing comprehensive CI/CD automation.

## 📋 Available Workflows

### 1. 🏗️ Main CI/CD Pipeline (`ci-cd.yml`)

**Triggers:**
- Push to `main` or `develop` branches
- Pull requests to `main` or `develop` branches
- Manual workflow dispatch

**Features:**
- 🔍 Smart change detection (API, tests, Docker, docs)
- 🏗️ Build and test with SQL Server
- 🐳 Docker image building and pushing
- 🚀 Automated deployment to development/production
- 🛡️ Security scanning with Trivy
- ⚡ Performance testing with Artillery
- 📊 Test coverage reporting
- 🧹 Automatic cleanup

**Environments:**
- **Development**: Auto-deployed from `develop` branch
- **Production**: Auto-deployed from `main` branch

### 2. 🔍 Pull Request Validation (`pr-validation.yml`)

**Triggers:**
- Pull request opened, synchronized, or reopened
- Pull request ready for review

**Features:**
- 🧹 Code quality checks and formatting validation
- 🧪 Comprehensive test validation with coverage
- 🗄️ Database migration validation
- 🐳 Docker build validation
- 🛡️ Security vulnerability scanning
- 📋 Automated PR summary generation

### 3. 🚀 Release Management (`release.yml`)

**Triggers:**
- Git tags matching `v*.*.*` pattern
- Manual workflow dispatch with version input

**Features:**
- 🔍 Release validation and testing
- 🏗️ Multi-platform build artifacts (Linux x64, Windows x64)
- 🐳 Docker image publishing with proper tagging
- 📋 Automated GitHub release creation with changelog
- 🌟 Production deployment for stable releases
- 🛡️ Container security scanning

### 4. 📦 Dependency Updates (`dependency-update.yml`)

**Triggers:**
- Weekly schedule (Mondays at 9 AM UTC)
- Manual workflow dispatch with update type selection

**Features:**
- 🔍 Automated outdated package detection
- 🔄 Smart dependency updates (patch/minor/major)
- 🏗️ Build and test validation
- 🛡️ Security vulnerability auditing
- 🚀 Automated pull request creation
- 📊 Update summary and reporting

### 5. 🛡️ Security Scanning (`security-scan.yml`)

**Triggers:**
- Weekly schedule (Mondays at 2 AM UTC)
- Push to main branches
- Manual workflow dispatch

**Features:**
- 🔍 Static Application Security Testing (SAST) with CodeQL
- 📦 Dependency vulnerability scanning
- 🐳 Container image security analysis
- 📋 SARIF report generation for GitHub Security tab
- 🚨 Security issue tracking and notifications

## 🔧 Setup Instructions

### 1. Repository Settings

#### Environments
Create the following environments in your repository settings:

- **development**
  - `DEVELOPMENT_URL`: http://localhost:5001
  - Auto-deployment: Enabled for `develop` branch

- **production**
  - `PRODUCTION_URL`: https://api.healthy-system.com
  - Protection rules: Require reviewers
  - Auto-deployment: Enabled for `main` branch

#### Secrets
Add these secrets in your repository settings:

```bash
# Required Secrets
PRODUCTION_DB_PASSWORD          # Production database password
STAGING_DB_PASSWORD            # Staging database password (if used)

# Optional Secrets
CODECOV_TOKEN                  # For code coverage reporting
NOTIFICATION_WEBHOOK_URL       # Slack/Teams webhook for notifications
DOCKER_REGISTRY_TOKEN          # If using external registry
```

#### Variables
Add these variables in your repository settings:

```bash
# Environment URLs
DEVELOPMENT_URL=http://localhost:5001
PRODUCTION_URL=https://api.healthy-system.com

# Configuration
BUILD_CONFIGURATION=Release
TEST_RESULTS_RETENTION_DAYS=30
ARTIFACT_RETENTION_DAYS=90
```

### 2. Branch Protection Rules

Configure branch protection for `main` and `develop`:

```yaml
# main branch
- Require status checks to pass before merging
- Require branches to be up to date before merging
- Required status checks:
  - Build and Test
  - Code Quality
  - Security Check
- Require pull request reviews before merging
- Dismiss stale PR approvals when new commits are pushed
- Require review from code owners
- Restrict pushes to matching branches

# develop branch  
- Require status checks to pass before merging
- Required status checks:
  - Build and Test
  - Code Quality
```

## 🎯 Workflow Usage

### Development Workflow

1. **Feature Development**
   ```bash
   # Create feature branch from develop
   git checkout develop
   git pull origin develop
   git checkout -b feature/your-feature-name
   
   # Make changes and commit
   git add .
   git commit -m "feat: add new feature"
   git push origin feature/your-feature-name
   ```

2. **Create Pull Request**
   - Open PR to `develop` branch
   - PR Validation workflow runs automatically
   - Review feedback and make changes if needed

3. **Merge to Develop**
   - After PR approval, merge to `develop`
   - CI/CD pipeline deploys to development environment
   - Run integration and performance tests

### Release Workflow

1. **Prepare Release**
   ```bash
   # Create release branch from develop
   git checkout develop
   git pull origin develop
   git checkout -b release/v1.0.0
   
   # Update version numbers, changelog, etc.
   git commit -m "chore: prepare release v1.0.0"
   ```

2. **Create Release PR**
   - Open PR from `release/v1.0.0` to `main`
   - All validation workflows run
   - Get required approvals

3. **Create Release**
   ```bash
   # After merging to main, create and push tag
   git checkout main
   git pull origin main
   git tag v1.0.0
   git push origin v1.0.0
   ```

4. **Automated Release Process**
   - Release workflow builds artifacts
   - Creates GitHub release with changelog
   - Deploys to production environment

### Manual Operations

#### Run Dependency Updates
```bash
# Navigate to Actions tab in GitHub
# Select "Dependency Updates" workflow
# Click "Run workflow"
# Choose update type: patch/minor/major/all
```

#### Trigger Security Scan
```bash
# Navigate to Actions tab in GitHub
# Select "Security Scan" workflow  
# Click "Run workflow"
```

#### Manual Deployment
```bash
# Navigate to Actions tab in GitHub
# Select "CI/CD Pipeline" workflow
# Click "Run workflow"
# Choose environment: development/staging/production
```

## 📊 Monitoring and Notifications

### GitHub Checks
- All workflows integrate with GitHub's status checks
- PR requirements ensure quality gates are met
- Security alerts appear in GitHub Security tab

### Artifacts and Reports
- Test results and coverage reports
- Performance test results
- Security scan reports
- Build artifacts for releases

### Workflow Summaries
Each workflow generates detailed summaries showing:
- ✅ Successful steps
- ❌ Failed steps
- 📊 Test coverage metrics
- 🔍 Change detection results
- 📋 Next steps and recommendations

## 🔧 Customization

### Adding New Environments

1. **Create Environment**
   ```yaml
   # In repository settings > Environments
   staging:
     url: https://staging-api.healthy-system.com
     protection_rules:
       - reviewers: ["team-leads"]
       - wait_timer: 5
   ```

2. **Update Workflows**
   ```yaml
   # Add to ci-cd.yml
   deploy-staging:
     name: 🎭 Deploy to Staging
     environment: staging
     if: github.ref == 'refs/heads/release/*'
   ```

### Custom Notifications

Add notification steps to workflows:

```yaml
- name: 📢 Notify Team
  if: always()
  uses: 8398a7/action-slack@v3
  with:
    status: ${{ job.status }}
    webhook_url: ${{ secrets.SLACK_WEBHOOK }}
```

### Performance Thresholds

Customize performance testing thresholds:

```yaml
# In performance test step
- name: ⚡ Performance Tests
  run: |
    artillery run load-test.yml \
      --threshold response_time_p95 2000 \
      --threshold request_rate 100
```

## 🛠️ Troubleshooting

### Common Issues

1. **Database Connection Failures**
   - Check SQL Server container health
   - Verify connection strings
   - Ensure proper startup timing

2. **Docker Build Failures**
   - Check Dockerfile syntax
   - Verify base image availability
   - Review build context

3. **Test Failures**
   - Review test logs in artifacts
   - Check for environment-specific issues
   - Verify test database setup

4. **Deployment Issues**
   - Check environment variables
   - Verify secrets configuration
   - Review deployment logs

### Debug Steps

1. **Enable Debug Logging**
   ```yaml
   - name: Enable Debug
     run: echo "ACTIONS_STEP_DEBUG=true" >> $GITHUB_ENV
   ```

2. **Check Workflow Logs**
   - Navigate to Actions tab
   - Select failed workflow run
   - Review detailed logs for each step

3. **Validate Locally**
   ```bash
   # Test Docker build locally
   docker build -f Healthy/Healthy.Api/Dockerfile ./Healthy
   
   # Test with docker-compose
   docker-compose -f docker-compose.dev.yml up --build
   ```

## 📚 Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [.NET CI/CD Guide](https://docs.microsoft.com/en-us/dotnet/devops/)
- [Security Scanning with CodeQL](https://docs.github.com/en/code-security/code-scanning)

## 🆘 Support

For issues with workflows:

1. Check this documentation
2. Review workflow logs in GitHub Actions
3. Check repository issues for known problems
4. Create a new issue with detailed error information

---

**Happy CI/CD! 🚀**
