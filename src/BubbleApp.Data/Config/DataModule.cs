using BubbleApp.Data.IRepository;
using BubbleApp.Data.Mongo;
using BubbleApp.Data.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BubbleApp.Data.Config
{
    public static class DataModule
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration config)
        {
            // ✅ Bind via Action<T> to avoid overload issues
            services.Configure<MongoSettings>(opts =>
            {
                config.GetSection("Mongo").Bind(opts);
            });

            services.AddSingleton<MongoContext>();

            services.AddSingleton<IAdminRepository, AdminRepository>();
            services.AddSingleton<IWorkspaceRepository, WorkspaceRepository>();
            services.AddSingleton<INoteRepository, NoteRepository>();

            return services;
        }
    }
}