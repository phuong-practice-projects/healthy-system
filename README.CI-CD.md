# CI/CD Pipeline Documentation

This document describes the Continuous Integration and Continuous Deployment (CI/CD) setup for the Healthy System project.

## Overview

The CI/CD pipeline is built using GitHub Actions and provides automated testing, building, security scanning, and deployment capabilities for the .NET 9.0 API application.

## Workflows

### 1. Main CI/CD Pipeline (`ci-cd.yml`)

**Triggers**: Push to `main`/`develop` branches, Pull Requests

**Jobs**:
- **Build and Test**: Compiles the application, runs tests, and generates coverage reports
- **Docker Build**: Builds and pushes Docker images to GitHub Container Registry
- **Deploy Development**: Deploys to development environment (develop branch)
- **Deploy Production**: Deploys to production environment (main branch)
- **Security Checks**: Runs OWASP ZAP scans and dependency checks
- **Performance Testing**: Runs load tests against development environment

### 2. Release Workflow (`release.yml`)

**Triggers**: Push of version tags (e.g., `v1.0.0`)

**Features**:
- Automated version extraction
- Build and test verification
- Docker image building and pushing
- Changelog generation
- GitHub release creation

### 3. Security Scanning (`security-scan.yml`)

**Triggers**: Weekly schedule, manual dispatch, push to main/develop

**Jobs**:
- **SAST**: Static Application Security Testing with CodeQL
- **Dependency Scan**: Vulnerability scanning with Snyk
- **Container Scan**: Docker image security with Trivy
- **Secrets Scan**: Secret detection with TruffleHog
- **License Check**: Package license compliance
- **Security Policy**: Security policy validation

### 4. Dependency Updates (`dependency-update.yml`)

**Triggers**: Weekly schedule, manual dispatch

**Features**:
- Automated dependency updates
- Test verification after updates
- Pull request creation for updates

## Environment Setup

### Required Secrets

Configure these secrets in your GitHub repository settings:

```bash
# Security Scanning
SNYK_TOKEN=your_snyk_token

# Deployment (if using external services)
DEPLOYMENT_TOKEN=your_deployment_token
PRODUCTION_URL=your_production_url
DEVELOPMENT_URL=your_development_url

# Notifications (optional)
SLACK_WEBHOOK_URL=your_slack_webhook
TEAMS_WEBHOOK_URL=your_teams_webhook
```

### Environment Protection

Set up environment protection rules for:
- **development**: Requires PR review for deployment
- **production**: Requires PR review and approval from maintainers

## Docker Configuration

### Image Tags

The pipeline automatically tags Docker images with:
- `latest`: Latest successful build on main branch
- `develop`: Latest build on develop branch
- `v*`: Version tags for releases
- `sha-*`: Commit SHA for traceability

### Registry

Images are pushed to GitHub Container Registry:
```
ghcr.io/your-username/your-repo:tag
```

## Security Features

### Automated Scanning

1. **CodeQL Analysis**: Static code analysis for security vulnerabilities
2. **Snyk Dependency Scanning**: Checks for known vulnerabilities in dependencies
3. **Trivy Container Scanning**: Scans Docker images for vulnerabilities
4. **TruffleHog Secret Detection**: Prevents accidental secret commits
5. **OWASP ZAP**: Dynamic application security testing

### Security Policies

- All dependencies are scanned for vulnerabilities
- Container images are scanned before deployment
- Secrets are automatically detected and blocked
- Security reports are uploaded as artifacts

## Performance Monitoring

### Load Testing

The pipeline includes Artillery-based load testing:
- Runs against development environment
- Generates performance reports
- Uploads results as artifacts

### Performance Budgets

Set performance budgets in your deployment configuration:
- Response time limits
- Throughput requirements
- Resource utilization targets

## Deployment Strategy

### Development Environment

- **Trigger**: Push to `develop` branch
- **Strategy**: Blue-green deployment
- **Rollback**: Automatic on failure
- **Health Checks**: Automated verification

### Production Environment

- **Trigger**: Push to `main` branch
- **Strategy**: Rolling deployment
- **Rollback**: Manual with automated assistance
- **Health Checks**: Comprehensive verification
- **Approval**: Required from maintainers

## Monitoring and Alerting

### Health Checks

The pipeline includes automated health checks:
- API endpoint availability
- Database connectivity
- Service dependencies
- Performance metrics

### Notifications

Configure notifications for:
- Deployment success/failure
- Security vulnerabilities
- Performance issues
- Build failures

## Troubleshooting

### Common Issues

1. **Build Failures**
   - Check .NET version compatibility
   - Verify all dependencies are available
   - Review build logs for specific errors

2. **Test Failures**
   - Ensure all tests are properly configured
   - Check for environment-specific issues
   - Verify test data availability

3. **Deployment Failures**
   - Check environment secrets
   - Verify target environment availability
   - Review deployment logs

4. **Security Scan Failures**
   - Address high-severity vulnerabilities
   - Update dependencies if needed
   - Review false positives

### Debugging

1. **Enable Debug Logging**
   ```yaml
   env:
     ACTIONS_STEP_DEBUG: true
   ```

2. **Check Artifacts**
   - Download and review uploaded artifacts
   - Examine test reports and coverage
   - Review security scan results

3. **Local Reproduction**
   - Run the same commands locally
   - Use the same environment variables
   - Check for local vs. CI differences

## Best Practices

### Code Quality

1. **Follow Standards**
   - Use consistent naming conventions
   - Follow .NET coding standards
   - Maintain code documentation

2. **Testing**
   - Write comprehensive unit tests
   - Include integration tests
   - Maintain good test coverage

3. **Security**
   - Regular dependency updates
   - Security scanning integration
   - Secret management

### Deployment

1. **Environment Management**
   - Use environment-specific configurations
   - Implement proper secret management
   - Maintain environment parity

2. **Monitoring**
   - Implement comprehensive logging
   - Set up health checks
   - Monitor performance metrics

3. **Rollback Strategy**
   - Maintain deployment history
   - Implement quick rollback procedures
   - Test rollback procedures regularly

## Maintenance

### Regular Tasks

1. **Weekly**
   - Review security scan results
   - Update dependencies
   - Monitor performance metrics

2. **Monthly**
   - Review and update workflows
   - Update security policies
   - Performance optimization

3. **Quarterly**
   - Major dependency updates
   - Security audit
   - Infrastructure review

### Updates

1. **Workflow Updates**
   - Test changes in development
   - Update documentation
   - Communicate changes to team

2. **Security Updates**
   - Apply security patches promptly
   - Test thoroughly before deployment
   - Monitor for issues

## Support

For issues with the CI/CD pipeline:

1. Check the workflow logs for specific errors
2. Review the troubleshooting section
3. Create an issue with detailed information
4. Contact the DevOps team for assistance

## Contributing

To contribute to the CI/CD setup:

1. Create a feature branch
2. Make your changes
3. Test thoroughly
4. Submit a pull request
5. Include documentation updates

---

**Last Updated**: [Date]
**Version**: 1.0.0 