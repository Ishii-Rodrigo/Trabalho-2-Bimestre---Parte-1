using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using umfgcloud.infraestrutura.service.Classes;
using umfgcloud.infraestrutura.service.Context;
using umfgcloud.loja.aplicacao.service.Classes;
using umfgcloud.loja.dominio.service.Classes;
using umfgcloud.loja.dominio.service.DTO;

namespace umfgcloud.aplicacao.service.testes.Classes
{
    public abstract class AbstractServicoTestes
    {
        private const string C_ROLE_DEFAULT = "default-role";
        private const string C_AUTHENTICATION_AUDIENCE = "altas.mars.net.br";
        private const string C_AUTHENTICATION_ISSUER = "marscloud.atlas.service";

        private const string C_AUTHORIZATION_INVALID = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9." +
            "eyJtb2R1bGVzIjoiVG9kb3MiLCJzdWIiOiI0NTUzZmMwNi1mYWM2LTQwMmMtOWVmYy05MDA0ZDhjMjE3" +
            "ZmIiLCJuYW1laWQiOiI2ZTQ3NWFiMS05ODY5LTQ0NWItODJiNy1kMWNkMmZjMmQzMTQiLCJuYW1lIjoi" +
            "TUFSUyIsImVtYWlsIjoiTUFSUy5URUMuQlJAR01BSUwuQ09NIiwianRpIjoiYTA0NmQ3MzQtZjUxOC00" +
            "MTFiLTg1MjAtZjhjYzlmMGYwMThkIiwibmJmIjoxNzczNDA5OTA1LCJpYXQiOjE3NzM0MDk5MDUsInJv" +
            "bGUiOiJBZG1pbmlzdHJhZG9yIiwiZXhwIjoxNzczNDEzNTA1LCJpc3MiOiJtYXJzY2xvdWQuYXRsYXMu" +
            "c2VydmljZSIsImF1ZCI6ImF0bGFzLm1hcnMubmV0LmJyIn0.Yb1CXbTIl93aeOqCP9ioWrLk09hP0Q65" +
            "2l2A4HzDpGw";

        private const string C_AUTHORIZATION_VALID = "Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImYxNjBmMTVhLTNmOTItNDIxOC05OTU0LWY2ODk1YTFhODhhMyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJ1c2VyQGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoidXNlckBleGFtcGxlLmNvbSIsIm5iZiI6MTc3NzUwMTAxOCwiZXhwIjoxNzc3NTAxMDc4LCJpc3MiOiJ1bWZnY2xvdWQuYXV0ZW50aWNhY2FvIiwiYXVkIjoidW1mZ2Nsb3VkLmFwcHMifQ.fzcyEOAuUL-xAqrhMMqHaN-el-G8snuHzLEg3XGysa4";

        protected const string C_USER_PASSWORD = "123Mudar@";

        private static string Token => GetHttpContextAccessorInvalidJWT()?.HttpContext?.Request.Headers[HeaderNames.Authorization].ToString().Split(" ").LastOrDefault() ?? string.Empty;
        protected static string UserId => new JwtSecurityToken(Token).Payload["nameid"].ToString() ?? string.Empty;
        protected static string UserName => new JwtSecurityToken(Token).Payload["name"].ToString() ?? string.Empty;
        protected static string UserEmail => new JwtSecurityToken(Token).Payload["email"].ToString() ?? string.Empty;

        #region context

        protected static MySqlDataBaseContext GetSqlServerDatabaseContext() => GetSqlServerDatabaseContext("umfg_api");

        protected static MySqlDataBaseContext GetSqlServerDatabaseContext(string nomeBancoDeDados)
        {
            var options = new DbContextOptionsBuilder<MySqlDataBaseContext>()
                .UseInMemoryDatabase(databaseName: nomeBancoDeDados)
                .Options;

            return new MySqlDataBaseContext(options);
        }

        #endregion context

        protected static ProdutoServico GetProdutoServicoInvalidJWT(MySqlDataBaseContext context)
            => new(GetHttpContextAccessorInvalidJWT(), GetProdutoRepositorio(context));

        protected static ProdutoServico GetProdutoServicoValidJWT(MySqlDataBaseContext context)
            => new(GetHttpContextAccessorValidJWT(), GetProdutoRepositorio(context));

        private static ProdutoRepositorio GetProdutoRepositorio(MySqlDataBaseContext context)
            => new(context);

        protected static UsuarioServico GetUsuarioServico(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager)
           => new(userManager, roleManager, signInManager, GetJwtOptions());

        protected static SignInManager<IdentityUser> GetSignInManagerSuccess()
        {
            var signInManager = GetMockSignInManager();

            signInManager
                .Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.Run(() => SignInResult.Success));

            return signInManager.Object;
        }

        protected static SignInManager<IdentityUser> GetSignInManagerLockedOut()
        {
            var signInManager = GetMockSignInManager();

            signInManager
                .Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.Run(() => SignInResult.LockedOut));

            return signInManager.Object;
        }

        protected static SignInManager<IdentityUser> GetSignInManagerNotAllowed()
        {
            var signInManager = GetMockSignInManager();

            signInManager
                .Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.Run(() => SignInResult.NotAllowed));

            return signInManager.Object;
        }

        protected static SignInManager<IdentityUser> GetSignInManagerTwoFactorRequired()
        {
            var signInManager = GetMockSignInManager();

            signInManager
                .Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.Run(() => SignInResult.TwoFactorRequired));

            return signInManager.Object;
        }

        protected static SignInManager<IdentityUser> GetSignInManagerFailed()
        {
            var signInManager = GetMockSignInManager();

            signInManager
                .Setup(x => x.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.Run(() => SignInResult.Failed));

            return signInManager.Object;
        }

        protected static UserManager<IdentityUser> GetUserManagerSuccess() => GetUserManagerSuccess(GetDefaultClaims(), GetDefaultRoles(), GetIdentityUser());
        protected static UserManager<IdentityUser> GetUserManagerSuccess(IdentityUser identityUser) => GetUserManagerSuccess(GetDefaultClaims(), GetDefaultRoles(), identityUser);

        protected static UserManager<IdentityUser> GetUserManagerSuccess(IList<Claim> claims, IList<string> roles, IdentityUser identityUser)
        {
            var userStore = GetUserStore();
            var serviceProvider = GetServiceProvider();
            var userManager = new Mock<UserManager<IdentityUser>>(userStore, null, null, null, null, null, null, serviceProvider, null);

            userManager
                .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsNotNull<string>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            userManager
                .Setup(x => x.SetLockoutEnabledAsync(It.IsAny<IdentityUser>(), It.IsNotNull<bool>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            userManager
                .Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.Run(() => claims));

            userManager
                .Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.Run(() => roles));

            userManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            userManager
                .Setup(x => x.AddClaimAsync(It.IsAny<IdentityUser>(), It.IsAny<Claim>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            userManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => identityUser));

            return userManager.Object;
        }

        protected static UserManager<IdentityUser> GetUserManagerWithoutCreateAsync() => GetUserManagerWithoutCreateAsync(GetDefaultClaims(), GetDefaultRoles());

        protected static UserManager<IdentityUser> GetUserManagerWithoutCreateAsync(IList<Claim> claims, IList<string> roles)
        {
            var userStore = GetUserStore();
            var serviceProvider = GetServiceProvider();
            var userManager = new Mock<UserManager<IdentityUser>>(userStore, null, null, null, null, null, null, serviceProvider, null);

            userManager
                .Setup(x => x.SetLockoutEnabledAsync(It.IsAny<IdentityUser>(), It.IsNotNull<bool>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            userManager
                .Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.Run(() => claims));

            userManager
                .Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.Run(() => roles));

            userManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            userManager
                .Setup(x => x.AddClaimAsync(It.IsAny<IdentityUser>(), It.IsAny<Claim>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            userManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => GetIdentityUser()));

            return userManager.Object;
        }

        protected static UserManager<IdentityUser> GetUserManagerWithoutFindByEmailAsync() => GetUserManagerWithoutFindByEmailAsync(GetDefaultClaims(), GetDefaultRoles());

        protected static UserManager<IdentityUser> GetUserManagerWithoutFindByEmailAsync(IList<Claim> claims, IList<string> roles)
        {
            var userStore = GetUserStore();
            var serviceProvider = GetServiceProvider();
            var userManager = new Mock<UserManager<IdentityUser>>(userStore, null, null, null, null, null, null, serviceProvider, null);

            userManager
                .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsNotNull<string>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            userManager
                .Setup(x => x.SetLockoutEnabledAsync(It.IsAny<IdentityUser>(), It.IsNotNull<bool>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            userManager
                .Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.Run(() => claims));

            userManager
                .Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.Run(() => roles));

            userManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            userManager
                .Setup(x => x.AddClaimAsync(It.IsAny<IdentityUser>(), It.IsAny<Claim>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            return userManager.Object;
        }

        protected static UserManager<IdentityUser> GetUserManagerFailed() => GetUserManagerFailed(GetDefaultClaims(), GetDefaultRoles(), GetIdentityUser());
        protected static UserManager<IdentityUser> GetUserManagerFailed(IdentityUser identityUser) => GetUserManagerFailed(GetDefaultClaims(), GetDefaultRoles(), identityUser);

        protected static UserManager<IdentityUser> GetUserManagerFailed(IList<Claim> claims, IList<string> roles, IdentityUser identityUser)
        {
            var userStore = GetUserStore();
            var serviceProvider = GetServiceProvider();
            var userManager = new Mock<UserManager<IdentityUser>>(userStore, null, null, null, null, null, null, serviceProvider, null);

            userManager
                .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsNotNull<string>()))
                .Returns(Task.Run(() => IdentityResult.Failed([new() { Code = "MOCK", Description = "MOCK FALHA", }])));

            userManager
                .Setup(x => x.SetLockoutEnabledAsync(It.IsAny<IdentityUser>(), It.IsNotNull<bool>()))
                .Returns(Task.Run(() => IdentityResult.Failed([new() { Code = "MOCK", Description = "MOCK FALHA", }])));

            userManager
                .Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.Run(() => claims));

            userManager
                .Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.Run(() => roles));

            userManager
                .Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns(Task.Run(() => IdentityResult.Failed([new() { Code = "MOCK", Description = "MOCK FALHA", }])));

            userManager
                .Setup(x => x.AddClaimAsync(It.IsAny<IdentityUser>(), It.IsAny<Claim>()))
                .Returns(Task.Run(() => IdentityResult.Failed([new() { Code = "MOCK", Description = "MOCK FALHA", }])));

            userManager
                .Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => identityUser));

            return userManager.Object;
        }

        protected static RoleManager<IdentityRole> GetRoleManagerExistsTrue() => GetRoleManagerExistisTrue(GetDefaultIdentityRole());

        protected static RoleManager<IdentityRole> GetRoleManagerExistisTrue(IdentityRole identityRole)
        {

            var roleStore = GetRoleStore();
            var roleManager = new Mock<RoleManager<IdentityRole>>(roleStore, null, null, null, null);

            roleManager
                .Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => true));

            roleManager
                .Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            roleManager
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => identityRole));

            return roleManager.Object;
        }

        protected static RoleManager<IdentityRole> GetRoleManagerWithoutFindByNameAsync() => GetRoleManagerWithoutFindByNameAsync(GetDefaultIdentityRole());

        protected static RoleManager<IdentityRole> GetRoleManagerWithoutFindByNameAsync(IdentityRole identityRole)
        {

            var roleStore = GetRoleStore();
            var roleManager = new Mock<RoleManager<IdentityRole>>(roleStore, null, null, null, null);

            roleManager
                .Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => true));

            roleManager
                .Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            return roleManager.Object;
        }

        protected static RoleManager<IdentityRole> GetRoleManagerExistisFalse() => GetRoleManagerExistisFalse(GetDefaultIdentityRole());

        protected static RoleManager<IdentityRole> GetRoleManagerExistisFalse(IdentityRole identityRole)
        {
            var roleManager = new Mock<RoleManager<IdentityRole>>();

            roleManager
                .Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => false));

            roleManager
                .Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            roleManager
                .Setup(x => x.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.Run(() => identityRole));

            return roleManager.Object;
        }

        protected static IList<Claim> GetDefaultClaims() => [];
        protected static IList<string> GetDefaultRoles() => [];
        protected static IdentityRole GetDefaultIdentityRole() => new(C_ROLE_DEFAULT);

        protected static UsuarioDTO.SingInRequest GetSingInRequestDTO() => GetSingInRequestDTO(UserEmail, C_USER_PASSWORD);

        protected static UsuarioDTO.SingInRequest GetSingInRequestDTO(string email, string password)
            => new()
            {
                Email = email,
                Password = password,
            };

        protected static UsuarioDTO.SingUpRequest GetSingUpRequestDTO() => new()
        {
            Email = UserEmail,
            Password = C_USER_PASSWORD,
            ConfirmedPassword = C_USER_PASSWORD,
        };

        protected static IdentityUser GetIdentityUser() => new(UserEmail);

        private static Mock<SignInManager<IdentityUser>> GetMockSignInManager()
        {
            var userManager = GetUserManagerSuccess();
            var contextAccessor = GetHttpContextAccessorInvalidJWT();
            var claimsFactory = GetClaimsFactory();

            return new Mock<SignInManager<IdentityUser>>(userManager, contextAccessor, claimsFactory, null, null, null, null);
        }

        private static IUserClaimsPrincipalFactory<IdentityUser> GetClaimsFactory()
        {
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();

            claimsFactory
                .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.Run(() => new ClaimsPrincipal()));

            return claimsFactory.Object;
        }

        private static IUserStore<IdentityUser> GetUserStore()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();

            userStore
                .Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            userStore
                .Setup(x => x.DeleteAsync(It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            userStore
                .Setup(x => x.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityUser()));

            userStore
                .Setup(x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityUser()));

            userStore
                .Setup(x => x.GetNormalizedUserNameAsync(It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityUser().NormalizedUserName));

            userStore
                .Setup(x => x.GetUserIdAsync(It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityUser().Id));

            userStore
                .Setup(x => x.GetUserNameAsync(It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityUser().UserName));

            userStore
                .Setup(x => x.SetNormalizedUserNameAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityUser()));

            userStore
                .Setup(x => x.SetUserNameAsync(It.IsAny<IdentityUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityUser()));

            userStore
                .Setup(x => x.UpdateAsync(It.IsAny<IdentityUser>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            return userStore.Object;
        }

        private static IRoleStore<IdentityRole> GetRoleStore()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            roleStore
                .Setup(x => x.CreateAsync(It.IsAny<IdentityRole>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            roleStore
                .Setup(x => x.DeleteAsync(It.IsAny<IdentityRole>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            roleStore
                .Setup(x => x.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityRole()));

            roleStore
                .Setup(x => x.FindByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityRole()));

            roleStore
                .Setup(x => x.GetNormalizedRoleNameAsync(It.IsAny<IdentityRole>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityRole().Name));

            roleStore
                .Setup(x => x.GetRoleIdAsync(It.IsAny<IdentityRole>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityRole().Id));

            roleStore
                .Setup(x => x.GetRoleNameAsync(It.IsAny<IdentityRole>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityRole().NormalizedName));

            roleStore
                .Setup(x => x.SetNormalizedRoleNameAsync(It.IsAny<IdentityRole>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityRole()));

            roleStore
                .Setup(x => x.SetRoleNameAsync(It.IsAny<IdentityRole>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => GetIdentityRole()));

            roleStore
                .Setup(x => x.UpdateAsync(It.IsAny<IdentityRole>(), It.IsAny<CancellationToken>()))
                .Returns(Task.Run(() => IdentityResult.Success));

            return roleStore.Object;
        }

        private static IServiceProvider GetServiceProvider()
        {
            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(ILookupProtector)))
                .Returns(GetLookupProtector());

            return serviceProvider.Object;
        }

        private static ILookupProtector GetLookupProtector()
        {
            var lookupProtector = new Mock<ILookupProtector>();

            lookupProtector
                .Setup(x => x.Protect(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(string.Empty);

            lookupProtector
                .Setup(x => x.Unprotect(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(string.Empty);

            return lookupProtector.Object;
        }

        private static IOptions<JwtOptions> GetJwtOptions()
            => Options.Create(new JwtOptions()
            {
                SecurityKey = "key-unit-test",
                Issuer = C_AUTHENTICATION_ISSUER,
                Audience = C_AUTHENTICATION_AUDIENCE,
                AcessTokenExpiration = 1,
                RefreshTokenExpiration = 1,
            });

        private static IdentityRole GetIdentityRole() => new(C_ROLE_DEFAULT);

        private static IHttpContextAccessor GetHttpContextAccessorInvalidJWT()
        {
            var headerDictionary = new HeaderDictionary();
            var mock = new Mock<IHttpContextAccessor>();

            headerDictionary
                .Add(HeaderNames.Authorization, C_AUTHORIZATION_INVALID);

            mock
                .Setup(x => x.HttpContext.Request.Headers)
                .Returns(headerDictionary);

            return mock.Object;
        }

        private static IHttpContextAccessor GetHttpContextAccessorValidJWT()
        {
            var headerDictionary = new HeaderDictionary();
            var mock = new Mock<IHttpContextAccessor>();

            headerDictionary
                .Add(HeaderNames.Authorization, C_AUTHORIZATION_VALID);

            mock
                .Setup(x => x.HttpContext.Request.Headers)
                .Returns(headerDictionary);

            return mock.Object;
        }
    }
}
