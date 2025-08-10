using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Calendar.Api.Controllers.Swagger
{
    /// <summary>
    /// An <see cref="IOperationFilter"/> implementation that adds an API key security requirement to Swagger operations.
    /// </summary>
    public class SwaggerApiKeyOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Applies the API key security requirement to the given Swagger operation.
        /// </summary>
        /// <param name="operation">The Swagger operation to modify.</param>
        /// <param name="context">The operation filter context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            }
                        },
                        new List<string>()
                    }
                }
            };
        }
    }
}
