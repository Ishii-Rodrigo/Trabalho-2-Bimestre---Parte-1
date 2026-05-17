using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using umfgcloud.loja.dominio.service.Classes;

namespace umfgcloud.loja.webapi.Extensions
{
    internal static class AutenticacaoExtensions
    {
        internal static void AddAutenticacao(this IServiceCollection services,
            IConfiguration configuration)
        {
            var confirurationSectionJwtOptions =
                configuration.GetSection(nameof(JwtOptions)).GetChildren();

            var issuer = confirurationSectionJwtOptions
                .FirstOrDefault(x => x.Key == nameof(JwtOptions.Issuer))?.Value ?? string.Empty;
            var audiance = confirurationSectionJwtOptions
                .FirstOrDefault(x => x.Key == nameof(JwtOptions.Audience))?.Value ?? string.Empty;
            var securityKey = confirurationSectionJwtOptions
                .FirstOrDefault(x => x.Key == nameof(JwtOptions.SecurityKey))?.Value ?? string.Empty;

            //ASCII: a letra K = 107
            var symmetricSecurityKey = 
                new SymmetricSecurityKey(Convert.FromBase64String(securityKey));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = issuer,

                ValidateAudience = false,
                ValidAudience = audiance,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricSecurityKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ClockSkew = TimeSpan.Zero,
            };


            services.Configure<JwtOptions>(options =>
            {
                options.Issuer = issuer;
                options.Audience = audiance;

                options.AcessTokenExpiration = int.Parse(
                    confirurationSectionJwtOptions
                    .FirstOrDefault(x => x.Key == nameof(JwtOptions.AcessTokenExpiration))?.Value ?? string.Empty);

                options.RefreshTokenExpiration = int.Parse(
                    confirurationSectionJwtOptions
                    .FirstOrDefault(x => x.Key == nameof(JwtOptions.RefreshTokenExpiration))?.Value ?? string.Empty);

                options.SigningCredentials =
                    new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            });

            //define quais as caracteristicas de uma senha
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
            });

            //definir a autenticacao via JWT
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => 
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = tokenValidationParameters;
                });
        }
    }
}