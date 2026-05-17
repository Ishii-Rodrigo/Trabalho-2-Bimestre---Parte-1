using Org.BouncyCastle.Asn1.Esf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using umfgcloud.loja.dominio.service.DTO;

namespace umfgcloud.aplicacao.service.testes.Classes
{
    [TestClass]
    public sealed class ProdutoServicoTestes : AbstractServicoTestes
    {
        private const string C_OWNER = "Juliano Maciel";
        private const string C_CATEGORY = "produto";
        private const decimal C_VALOR_NEGATIVO = -89.90m;

        #region Testes Originais (Adicionar e Instanciar)

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_AdicionarAsync_Sucesso()
        {
            try
            {
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());

                var servico = GetProdutoServicoValidJWT(context);
                var dto = new ProdutoDTO.ProdutoRequest()
                {
                    Descricao = "TESTE",
                    EAN = "123456789",
                    ValorCompra = 39.90m,
                    ValorVenda = 89.90m,
                };

                await servico.AdicionarAsync(dto);

                var produto = (await servico.ObterTodosAsync()).FirstOrDefault();

                Assert.IsNotNull(produto);
                Assert.AreNotEqual(Guid.Empty, produto.Id);
                Assert.AreEqual("TESTE", produto.Descricao);
                Assert.AreEqual("123456789", produto.EAN);
                Assert.AreEqual(39.90m, produto.ValorCompra);
                Assert.AreEqual(89.90m, produto.ValorVenda);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_AdicionarAsync_FalhaValorCompraNegativo()
        {
            try
            {
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());

                var servico = GetProdutoServicoValidJWT(context);
                var dto = new ProdutoDTO.ProdutoRequest()
                {
                    Descricao = "TESTE",
                    EAN = "123456789",
                    ValorCompra = -39.90m,
                    ValorVenda = 89.90m,
                };

                await Assert.ThrowsExceptionAsync<InvalidDataException>(() => servico.AdicionarAsync(dto));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_AdicionarAsync_FalhaValorVendaNegativo()
        {
            try
            {
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());

                var servico = GetProdutoServicoValidJWT(context);
                var dto = new ProdutoDTO.ProdutoRequest()
                {
                    Descricao = "TESTE",
                    EAN = "123456789",
                    ValorCompra = 39.90m,
                    ValorVenda = -89.90m,
                };

                await Assert.ThrowsExceptionAsync<InvalidDataException>(() => servico.AdicionarAsync(dto));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public void ProdutoServico_Instanciar_Falha()
        {
            try
            {
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());

                Assert.ThrowsException<InvalidDataException>(() => GetProdutoServicoInvalidJWT(context));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion

        #region Novos Testes (Obter, Atualizar e Remover)

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_ObterTodosAsync_Sucesso()
        {
            try
            {
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                await servico.AdicionarAsync(new ProdutoDTO.ProdutoRequest { Descricao = "P1", EAN = "1", ValorCompra = 10, ValorVenda = 20 });
                await servico.AdicionarAsync(new ProdutoDTO.ProdutoRequest { Descricao = "P2", EAN = "2", ValorCompra = 15, ValorVenda = 30 });

                var lista = await servico.ObterTodosAsync();

                Assert.IsNotNull(lista);
                Assert.AreEqual(2, lista.Count());
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_ObterPorIdAsync_Sucesso()
        {
            try
            {
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                var dtoInsert = new ProdutoDTO.ProdutoRequest { Descricao = "Original", EAN = "123", ValorCompra = 10, ValorVenda = 20 };
                await servico.AdicionarAsync(dtoInsert);

                var produtoInserido = (await servico.ObterTodosAsync()).First();

                var produtoBuscado = await servico.ObterPorIdAsync(produtoInserido.Id);

                Assert.IsNotNull(produtoBuscado);
                Assert.AreEqual(produtoInserido.Id, produtoBuscado.Id);
                Assert.AreEqual("Original", produtoBuscado.Descricao);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_AtualizarAsync_Sucesso()
        {
            try
            {
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                await servico.AdicionarAsync(new ProdutoDTO.ProdutoRequest { Descricao = "Antes", EAN = "111", ValorCompra = 5, ValorVenda = 10 });
                var produtoInserido = (await servico.ObterTodosAsync()).First();

                var dtoUpdate = new ProdutoDTO.ProdutoResponse
                {
                    Id = produtoInserido.Id,
                    Descricao = "Depois",
                    EAN = "222",
                    ValorCompra = 8,
                    ValorVenda = 16
                };

                await servico.AtualizarAsync(dtoUpdate);

                var produtoAtualizado = await servico.ObterPorIdAsync(produtoInserido.Id);
                Assert.AreEqual("Depois", produtoAtualizado.Descricao);
                Assert.AreEqual("222", produtoAtualizado.EAN);
                Assert.AreEqual(8, produtoAtualizado.ValorCompra);
                Assert.AreEqual(16, produtoAtualizado.ValorVenda);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_RemoverAsync_Sucesso()
        {
            try
            {
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());
                var servico = GetProdutoServicoValidJWT(context);

                await servico.AdicionarAsync(new ProdutoDTO.ProdutoRequest { Descricao = "Deletar", EAN = "999", ValorCompra = 1, ValorVenda = 2 });
                var produtoInserido = (await servico.ObterTodosAsync()).First();

                await servico.RemoverAsync(produtoInserido.Id);

                var lista = await servico.ObterTodosAsync();
                Assert.AreEqual(0, lista.Count());
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        #endregion
    }
}