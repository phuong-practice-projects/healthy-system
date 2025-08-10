# 🚀 Performance Testing

This directory contains performance testing infrastructure for the Healthy System API using [Artillery](https://artillery.io/).

## 📋 Overview

The performance testing suite includes:
- **Load testing** with realistic user scenarios
- **Stress testing** to find breaking points
- **API endpoint validation** during load
- **Response time monitoring**
- **Automated reporting**

## 🛠️ Setup

### Prerequisites
- Node.js 18+ installed
- Healthy System API running (default: http://localhost:5001)
- Valid test user accounts (see test data)

### Installation
```bash
cd performance-tests
npm install
```

Or install Artillery globally:
```bash
npm install -g artillery@latest
```

## 🎯 Test Scenarios

### Current Test Scenarios
1. **Health Check** (20% weight)
   - Basic health endpoint validation
   - Ensures API is responsive

2. **User Authentication & Data Retrieval** (30% weight)
   - Login with admin credentials
   - Fetch users list
   - Tests authentication flow

3. **Body Record Creation** (25% weight)
   - User login
   - Create new body records
   - Tests write operations

4. **Dashboard Data** (25% weight)
   - User login
   - Fetch dashboard data
   - Tests read-heavy operations

## 🚀 Running Tests

### Quick Health Check
```bash
npm run test:quick
```

### Full Load Test
```bash
npm run test
```

### Generate Report
```bash
npm run test:report
```

### Manual Artillery Commands
```bash
# Basic load test
artillery run load-test.yml

# With output for reporting
artillery run load-test.yml --output results.json

# Generate HTML report
artillery report results.json --output report.html
```

## 📊 Test Configuration

### Load Phases
1. **Warm up**: 60s @ 10 requests/second
2. **Load test**: 120s @ 50 requests/second  
3. **Stress test**: 60s @ 100 requests/second

### Expectations
- Health endpoint: 200 status code
- Authenticated endpoints: 200 status code
- Create operations: 201 status code
- Response times: Monitored and logged

## 🔧 Customization

### Updating Target URL
Edit `load-test.yml`:
```yaml
config:
  target: 'https://your-api-domain.com'
```

### Adding New Scenarios
Add to the `scenarios` section in `load-test.yml`:
```yaml
- name: "Your New Scenario"
  weight: 10
  flow:
    - get:
        url: "/your-endpoint"
        expect:
          - statusCode: 200
```

### Test Data
Update test credentials in `load-test.yml`:
- Admin: `admin@healthysystem.com` / `Admin@123`
- User: `user@healthysystem.com` / `User@123`

## 📈 Results Analysis

### Key Metrics
- **Request rate**: Requests per second
- **Response time**: p50, p95, p99 percentiles
- **Error rate**: Failed requests percentage
- **Concurrent users**: Simulated user load

### Typical Thresholds
- Response time p95: < 2000ms
- Error rate: < 1%
- Successful requests: > 99%

## 🔄 CI/CD Integration

Performance tests run automatically in the CI/CD pipeline:
- Triggered after successful development deployment
- Results uploaded as artifacts
- Failure notifications for performance degradation

### CI/CD Environment Variables
- `PERFORMANCE_TARGET_URL`: Override default target URL
- `PERFORMANCE_DURATION`: Override test duration
- `PERFORMANCE_RATE`: Override request rate

## 🛡️ Best Practices

### Test Environment
- Use dedicated test database
- Ensure consistent test data
- Monitor resource usage during tests
- Clean up test data after runs

### Performance Baselines
- Establish baseline metrics for comparison
- Track performance trends over time
- Set alerts for regression detection
- Document acceptable performance criteria

## 🆘 Troubleshooting

### Common Issues

#### Connection Refused
```
ECONNREFUSED localhost:5001
```
**Solution**: Ensure the API is running on the correct port

#### Authentication Failures
```
401 Unauthorized
```
**Solution**: Verify test user credentials and ensure users exist in test database

#### High Error Rates
```
Error rate > 5%
```
**Solution**: Check API logs, database connections, and resource constraints

### Debug Mode
Run with verbose logging:
```bash
artillery run load-test.yml --verbose
```

## 📚 Additional Resources

- [Artillery Documentation](https://artillery.io/docs/)
- [Performance Testing Best Practices](https://artillery.io/docs/guides/overview/getting-started-with-performance-testing.html)
- [Healthy System API Documentation](../README.md)