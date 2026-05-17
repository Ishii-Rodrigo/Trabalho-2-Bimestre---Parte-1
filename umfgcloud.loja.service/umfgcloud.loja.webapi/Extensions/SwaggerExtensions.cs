using Microsoft.OpenApi.Models;

namespace umfgcloud.loja.webapi.Extensions
{
    internal static class SwaggerExtensions
    {
        private const string C_FILENAME_XML = "umfgcloud-loja-service.xml";
        private const string C_DESCRIPTION = @"JWT Authorization Header - utilizado com Bearer Authentication.\r\n\
                                 Digite 'Bearer' [espaço] e então seu token no campo abaixo.\r\n\
                                 Exemplo (informar sem as aspas): 'Bearer 12345abcdef'";

        internal static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = C_DESCRIPTION,
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                options.ResolveConflictingActions(x => x.FirstOrDefault());
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, C_FILENAME_XML));
            });
        }
    }
}