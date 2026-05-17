using Microsoft.EntityFrameworkCore;
using umfgcloud.infraestrutura.service.Context;

namespace umfgcloud.loja.webapi.Extensions
{
    internal static class DataContextExtensions
    {
        internal static void AddDataContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("MySQL");

            services.AddDbContext<MySqlDataBaseContext>(options
                => options.UseMySQL(connection ?? string.Empty));
        }
    }
}
