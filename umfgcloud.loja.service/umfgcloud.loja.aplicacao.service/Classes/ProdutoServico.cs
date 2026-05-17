using Mapster;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umfgcloud.loja.dominio.service.DTO;
using umfgcloud.loja.dominio.service.Entidades;
using umfgcloud.loja.dominio.service.Interfaces.Repositorios;
using umfgcloud.loja.dominio.service.Interfaces.Servicos;

namespace umfgcloud.loja.aplicacao.service.Classes
{
    public sealed class ProdutoServico : AbstractServico, IProdutoServico
    {
        private readonly IProdutoRepositorio _repositorio;

        public ProdutoServico(IHttpContextAccessor httpContextAccessor, 
            IProdutoRepositorio repositorio)
            : base(httpContextAccessor)
        {
            _repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }

        public async Task AdicionarAsync(ProdutoDTO.ProdutoRequest dto)
        {
            //os dados inerentes ao usuário são enviados via JWT
            var produto = new ProdutoEntity(UserId, UserEmail);

            //dto transita os dados inerentes a tabela
            produto.SetDescricao(dto.Descricao);
            produto.SetEAN(dto.EAN);
            produto.SetValorCompra(dto.ValorCompra);
            produto.SetValorVenda(dto.ValorVenda);

            await _repositorio.AdicionarAsync(produto);
        }

        public async Task AtualizarAsync(ProdutoDTO.AbstractProdutoWithIdDTO dto)
        {
            var produto = await _repositorio.ObterPorIdAsync(dto.Id);

            produto.SetDescricao(dto.Descricao);
            produto.SetEAN(dto.EAN);
            produto.SetValorCompra(dto.ValorCompra);
            produto.SetValorVenda(dto.ValorVenda);

            produto.Update(UserId, UserEmail);

            await _repositorio.AtualizarAsync(produto);
        }

        //so consegue fazer a conversa, de atributos publicos na classe destino e ICollection/List
        public async Task<ProdutoDTO.ProdutoResponse> ObterPorIdAsync(Guid id)
            => (await _repositorio.ObterPorIdAsync(id)).Adapt<ProdutoDTO.ProdutoResponse>();

        public async Task<IEnumerable<ProdutoDTO.ProdutoResponse>> ObterTodosAsync()
            => (await _repositorio.ObterTodosAsync()).Adapt<IEnumerable<ProdutoDTO.ProdutoResponse>>();

        public async Task RemoverAsync(Guid id)
            => await _repositorio.RemoverAsync(await _repositorio.ObterPorIdAsync(id));
    }
}
