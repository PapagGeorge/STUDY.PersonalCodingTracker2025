using Application.Interfaces;
using Infrastructure.Repoitories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure
{
    public static class InfraModule
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.Configure<DatabaseOptions>(configuration.GetSection("ConnectionStrings"));
            services.AddDbContext<AuthDbContext>((sp, options) =>
            {
                var dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;

                options.UseSqlServer(dbOptions.DefaultConnection,
                    x => x.MigrationsAssembly("Infrastructure"));
            });
            return services;
        }
    }
}