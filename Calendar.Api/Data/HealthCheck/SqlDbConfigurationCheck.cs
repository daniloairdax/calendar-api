using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Calendar.Api.HealthCheck
{
    public class SqlDbConfigurationCheck : IHealthCheck
    {
        private readonly IConfiguration _configuration;

        public SqlDbConfigurationCheck(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("SqlConnection");

                if (!string.IsNullOrEmpty(connectionString))
                    return HealthCheckResult.Healthy();

                return HealthCheckResult.Unhealthy("Failed to get SQL connection string.");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Failed to get SQL connection string.", ex);
            }
        }
    }
}
