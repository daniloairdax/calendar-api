using Calendar.Api.Controllers.Filters;
using Calendar.Api.Controllers.Swagger;
using Calendar.Api.HealthCheck;
using Calendar.Application.Interfaces;
using Calendar.Application.Mappings;
using Calendar.Infrastructure.Middlewares;
using Calendar.Infrastructure.Persistence;
using Calendar.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Calendar.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Register services here
        public void ConfigureServices(IServiceCollection services)
        {
            // Core ASP.NET services
            services.AddEndpointsApiExplorer();
            services.AddControllers(options =>
                {
                    options.Filters.Add<ApiExceptionFilterAttribute>();
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // Swagger/OpenAPI
            ConfigureSwagger(services);

            // Validation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Health Checks
            services.AddHealthChecks()
                .AddCheck<SqlDbConfigurationCheck>("Sql Db Configuration");

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.WithOrigins(
                                   "http://localhost:4200",
                                   "https://calendarapi.com",
                                   "https://calendarapiweb.com",
                                   "https://calendarapiweb-stg.com")
                           .AllowAnyMethod()
                           .AllowAnyHeader());
            });

            // Cookie Policy
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = HttpOnlyPolicy.Always;
                options.Secure = CookieSecurePolicy.Always;
            });

            // Security
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
            services.AddAuthorization();

            // Persistence
            services.AddDbContext<CalendarDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("SqlConnection")));

            // Third-party libraries
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationMapperProfile).Assembly));
            services.AddAutoMapper(typeof(ApplicationMapperProfile).Assembly);

            // Register dependency injection services
            services.AddSingleton<IEmailService, EmailService>();
            services.AddScoped<IAnimalRepository, AnimalRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<ICalendarDbContext>(provider => provider.GetRequiredService<CalendarDbContext>());         
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Calendar API");
            });

            app.UseCors("CorsPolicy");
            app.UseRouting();
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.Always
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<ApiKeyMiddleware>();

            // Init and seed the database
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CalendarDbContext>();
                DbSeeder.Seed(dbContext);
            }

            app.MapControllers();
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Title = "Calendar API",
                    Description = "Calendar API is an appointment system for a veterinary clinic",
                    Version = "v1"
                });

                var fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
                options.IncludeXmlComments(filePath);
                options.CustomSchemaIds(i => i.FullName);

                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API Key needed to access the endpoints. X-Api-Key: {key}",
                    Name = "X-Api-Key",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                       }
                   });
                options.OperationFilter<SwaggerApiKeyOperationFilter>();
            });
        }
    }
}