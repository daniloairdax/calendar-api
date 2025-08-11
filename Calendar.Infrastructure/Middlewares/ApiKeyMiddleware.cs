using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Calendar.Infrastructure.Middlewares
{
    public class ApiKeyMiddleware
    {
        private const string ApiKeyHeaderName = "X-Api-Key";
        private const string ApiKey = "danilotest"; // Hardcoded key which will be usualy in some Vault as secret

        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey) ||
                extractedApiKey != ApiKey)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("API Key is missing or invalid.");
                return;
            }

            await _next(context);
        }
    }
}
