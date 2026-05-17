using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using umfgcloud.loja.dominio.service.Classes;
using umfgcloud.loja.dominio.service.DTO;
using umfgcloud.loja.dominio.service.Interfaces.Servicos;

namespace umfgcloud.loja.aplicacao.service.Classes
{
    public sealed class UsuarioServico : IUsuarioServico
    {
        private readonly string[] _roles = [Role.Desenvolvedor, Role.Admin, Role.Padrao];

        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JwtOptions _jwtOptions;

        public UsuarioServico(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        }

        public async Task<UsuarioDTO.SingInResponse> AutenticarAsync(UsuarioDTO.SingInRequest dto)
        {
            var resultado = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, false, true);

            if (!resultado.Succeeded)
            {
                if (resultado.IsLockedOut)
                    throw new InvalidOperationException("Conta bloqueada!");

                if (resultado.IsNotAllowed)
                    throw new InvalidOperationException("Conta sem permissão para login!");

                if (resultado.RequiresTwoFactor)
                    throw new InvalidOperationException("Confirme seu login com o seu segundo fator de autenticação!");

                throw new InvalidOperationException("Email ou Password inválidos!");
            }

            var identityUser = (await _userManager.FindByEmailAsync(dto.Email))
                ?? throw new ArgumentException("Usuário não encontrado!");
            var claims = new List<Claim>();

            //convenção do JWT
            claims.Add(new Claim(ClaimTypes.NameIdentifier, identityUser.Id));
            claims.Add(new Claim(ClaimTypes.Name, identityUser.UserName ?? string.Empty));            
            claims.Add(new Claim(ClaimTypes.Email, identityUser.Email ?? string.Empty));            

            var token = new JwtSecurityToken
                (
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.AddMinutes(_jwtOptions.AcessTokenExpiration),
                    signingCredentials: _jwtOptions.SigningCredentials,
                    claims: claims
                );

            return new UsuarioDTO.SingInResponse()
            {
                AcessToken = new JwtSecurityTokenHandler().WriteToken(token),                
            };
        }

        public async Task CadastrarAsync(UsuarioDTO.SingUpRequest dto)
        {
            // classe concreta é toda classe que eu utilizo o new para criar
            // uma instancia em memoria
            // classes estaticas e abstratas não tem essa possibilidade
            var identityUser = new IdentityUser()
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true,
            };

            var resultado = await _userManager.CreateAsync(identityUser, dto.Password);

            if (!resultado.Succeeded && resultado.Errors.Any())
                throw new InvalidOperationException(string.Join("\n", resultado.Errors.Select(x => x.Description)));

            foreach (var role in _roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if ((await _roleManager.FindByNameAsync(Role.Padrao)) is null)
                throw new ArgumentException("Configuração padrão não cadastrada!");

            await _userManager.AddToRoleAsync(identityUser, Role.Padrao);
            await _userManager.SetLockoutEnabledAsync(identityUser, true);
        }
    }
}
