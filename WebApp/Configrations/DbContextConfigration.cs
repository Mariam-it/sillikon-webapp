using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Configrations;

public static class DbContextConfigration
{
    public static void RegisterDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WebAppContext>(x => x.UseSqlServer(configuration.GetConnectionString("WebApp_Database")));
    }
}
