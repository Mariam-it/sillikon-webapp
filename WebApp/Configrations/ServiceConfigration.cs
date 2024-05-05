using Infrastructure.Services;

namespace WebApp.Configrations;

public static class ServiceConfigration
{
    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<CategoryService>();
        services.AddScoped<CourseService>();
    }
}
