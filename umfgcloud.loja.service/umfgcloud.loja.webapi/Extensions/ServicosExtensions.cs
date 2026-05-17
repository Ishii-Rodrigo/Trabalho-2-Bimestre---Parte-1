using Microsoft.AspNetCore.Identity;
using umfgcloud.infraestrutura.service.Classes;
using umfgcloud.infraestrutura.service.Context;
using umfgcloud.loja.aplicacao.service.Classes;
using umfgcloud.loja.dominio.service.Interfaces.Repositorios;
using umfgcloud.loja.dominio.service.Interfaces.Servicos;

namespace umfgcloud.loja.webapi.Extensions
{
    /// <summary>
    /// Define quais as implementações para as interfaces
    /// criadas ou importadas na solução
    /// </summary>
    internal static class ServicosExtensions
    {        
        internal static void AddServicos(this IServiceCollection services)
        {
            services
                .AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<MySqlDataBaseContext>()
                .AddDefaultTokenProviders();

            //AddScoped -> cada requisição do front-end cria um objeto na memória e ao final remove
            //AddSingleton -> compartilha um unico objeto com todo a aplicação
            //AddTransient -> cada requisição do front-end cria um objeto na memória e o mantem,
            //                precisa gerenciar para não estourar a capacidade do servidor                                
            services.AddScoped<IUsuarioServico, UsuarioServico>();

            //respeitar a criação de hierarquia das instancias
            //produto servico depende de produto repositorio
            //repositorio deve ser definido antes
            services.AddScoped<IProdutoRepositorio, ProdutoRepositorio>();
            services.AddScoped<IProdutoServico, ProdutoServico>();
        }
    }
}
