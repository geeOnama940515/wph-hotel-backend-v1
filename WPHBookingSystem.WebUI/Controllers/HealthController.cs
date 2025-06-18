using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WPHBookingSystem.WebUI.Extensions;

namespace WPHBookingSystem.WebUI.Controllers
{
    /// <summary>
    /// Health check controller for monitoring application status.
    /// 
    /// This controller provides endpoints for health monitoring and Docker health checks.
    /// It includes basic application health status and can be extended to include
    /// database connectivity, external service dependencies, and other health metrics.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<HealthController> _logger;

        public HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }

        /// <summary>
        /// Basic health check endpoint for Docker health checks.
        /// 
        /// Returns a simple 200 OK response to indicate the application is running.
        /// This endpoint is used by Docker health checks and load balancers.
        /// </summary>
        /// <returns>200 OK if the application is healthy</returns>
        /// <response code="200">Application is healthy</response>
        /// <response code="503">Application is unhealthy</response>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Health check requested");
            return this.CreateResponse(200, "Application is healthy", new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                version = "1.0.0"
            });
        }

        /// <summary>
        /// Detailed health check endpoint with dependency status.
        /// 
        /// Performs comprehensive health checks including database connectivity,
        /// external service dependencies, and application-specific health metrics.
        /// </summary>
        /// <returns>Detailed health status report</returns>
        /// <response code="200">All health checks passed</response>
        /// <response code="503">One or more health checks failed</response>
        [HttpGet("detailed")]
        public async Task<IActionResult> GetDetailed()
        {
            _logger.LogInformation("Detailed health check requested");
            
            try
            {
                var healthReport = await _healthCheckService.CheckHealthAsync();
                
                var isHealthy = healthReport.Status == HealthStatus.Healthy;
                var statusCode = isHealthy ? 200 : 503;
                
                var result = new
                {
                    status = healthReport.Status.ToString().ToLower(),
                    timestamp = DateTime.UtcNow,
                    version = "1.0.0",
                    checks = healthReport.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                        duration = entry.Value.Duration.TotalMilliseconds
                    }).ToList()
                };

                return this.CreateResponse(statusCode, 
                    isHealthy ? "All health checks passed" : "One or more health checks failed", 
                    result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during health check");
                return this.CreateResponse(503, "Health check failed", new { 
                    status = "unhealthy", 
                    error = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Readiness check endpoint for Kubernetes readiness probes.
        /// 
        /// Indicates whether the application is ready to receive traffic.
        /// This is typically used in Kubernetes deployments.
        /// </summary>
        /// <returns>Readiness status</returns>
        /// <response code="200">Application is ready</response>
        /// <response code="503">Application is not ready</response>
        [HttpGet("ready")]
        public IActionResult GetReady()
        {
            _logger.LogInformation("Readiness check requested");
            return this.CreateResponse(200, "Application is ready", new { 
                status = "ready", 
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Liveness check endpoint for Kubernetes liveness probes.
        /// 
        /// Indicates whether the application is alive and should not be restarted.
        /// This is typically used in Kubernetes deployments.
        /// </summary>
        /// <returns>Liveness status</returns>
        /// <response code="200">Application is alive</response>
        /// <response code="503">Application is not responding</response>
        [HttpGet("live")]
        public IActionResult GetLive()
        {
            _logger.LogInformation("Liveness check requested");
            return this.CreateResponse(200, "Application is alive", new { 
                status = "alive", 
                timestamp = DateTime.UtcNow
            });
        }
    }
} 