// Performance test helper functions
module.exports = {
  generateRandomUser,
  logRequest,
  checkResponseTime
};

function generateRandomUser(userContext, events, done) {
  userContext.vars.randomEmail = `test${Math.floor(Math.random() * 10000)}@example.com`;
  userContext.vars.randomPassword = `Test@${Math.floor(Math.random() * 10000)}`;
  return done();
}

function logRequest(requestParams, response, context, ee, next) {
  console.log(`Request to ${requestParams.url} - Status: ${response.statusCode}`);
  return next();
}

function checkResponseTime(requestParams, response, context, ee, next) {
  if (response.timings && response.timings.response > 2000) {
    console.warn(`Slow response detected: ${response.timings.response}ms for ${requestParams.url}`);
  }
  return next();
}
