using Microsoft.AspNetCore.Mvc;
using umfgcloud.loja.dominio.service.DTO;
using umfgcloud.loja.dominio.service.Interfaces.Servicos;

namespace umfgcloud.loja.webapi.Controllers
{
    [ApiVersion("1.0")]    
    public sealed class SigninController : AbstractController
    {
        private readonly IUsuarioServico _servico;

        public SigninController(IUsuarioServico servico)
        {
            _servico = servico ?? throw new ArgumentNullException(nameof(servico));
        }

        /// <summary>
        /// Efetua o login do usuário
        /// Aula 27/04/2026 conceito de refatoração:
        ///     Alterar os atributos internos do código sem alterar o seu comportamento
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AutenticarAsync(UsuarioDTO.SingInRequest dto)
            => await InvokeMethodAsync(_servico.AutenticarAsync, dto);
    }
}
