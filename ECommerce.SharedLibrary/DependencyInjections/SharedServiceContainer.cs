using ECommerce.SharedLibrary.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ECommerce.SharedLibrary.DependencyInjections
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddSharedServices<TContext>(this IServiceCollection services, IConfiguration config, string fileName)
        where TContext : DbContext
        {


            services.AddDbContext<TContext>(option => option.UseSqlServer(config.GetConnectionString("ECommerceConnectionString"),
             sqlServerOption => sqlServerOption.EnableRetryOnFailure()));

            Log.Logger = new LoggerConfiguration().
            MinimumLevel.Information().
            WriteTo.Debug().
            WriteTo.Console().
            WriteTo.File(path: $"{fileName}-.text",
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz }[{Level:u3}] {message:lj} {NewLine} {Exception}",
            rollingInterval: RollingInterval.Day
            ).CreateLogger();

            JwtAuthenticationScheme.AddJwtAuthenticationScheme(services, config);








            return services;
        }

        public static IApplicationBuilder UseSharedPolicies(this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();
            app.UseMiddleware<ListenToOnlyApiGateway>();

            return app;
            
        }
    }
}