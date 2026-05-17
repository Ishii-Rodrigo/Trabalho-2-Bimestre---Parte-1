using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using umfgcloud.loja.dominio.service.DTO;
using umfgcloud.loja.dominio.service.Interfaces.Servicos;

namespace umfgcloud.loja.webapi.Controllers
{
    [ApiVersion("1.0")]
    public sealed class ProdutosController : AbstractController
    {
        private readonly IProdutoServico _servico;

        public ProdutosController(IProdutoServico servico)
        {
            _servico = servico ?? throw new ArgumentNullException(nameof(servico));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AdicionarAsync(ProdutoDTO.ProdutoRequest dto)
        {
            try
            {
                await _servico.AdicionarAsync(dto);
                return Created(); //retorna status 201 se conseguir incluir no banco de dados
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ObterTodosAsync()
        {
            try
            {
                //retorna um status 200 e uma lista de registros
                return Ok(await _servico.ObterTodosAsync()); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorIdAsync([FromRoute] string id)
            => await InvokeMethodAsync(_servico.ObterPorIdAsync, new Guid(id));

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarAsync([FromRoute] string id, [FromBody] ProdutoDTO.ProdutoRequest dto)
        {
            try
            {
                var atualizarDTO = dto.Adapt<ProdutoDTO.ProdutoRequestWithId>();
                atualizarDTO.Id = new Guid(id);

                await _servico.AtualizarAsync(atualizarDTO);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverAsync([FromRoute] string id)
        {
            try
            {
                await _servico.RemoverAsync(new Guid(id));
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
