using Org.BouncyCastle.Asn1.Esf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umfgcloud.loja.dominio.service.DTO;

namespace umfgcloud.aplicacao.service.testes.Classes
{
    [TestClass]
    public sealed class ProdutoServicoTestes : AbstractServicoTestes
    {
        private const string C_OWNER = "Juliano Maciel";
        private const string C_CATEGORY = "produto";
        private const decimal C_VALOR_NEGATIVO = -89.90m;

        [TestMethod]
        [Owner(C_OWNER)]
        [TestCategory(C_CATEGORY)]
        public async Task ProdutoServico_AdicionarAsync_Sucesso()
        {
            try
            {
                //o objetivo do using é o desenvolvedor ter controlle sobre o 
                //dispose do objeto
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
                //o objetivo do using é o desenvolvedor ter controlle sobre o 
                //dispose do objeto
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
                //o objetivo do using é o desenvolvedor ter controlle sobre o 
                //dispose do objeto
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
                //o objetivo do using é o desenvolvedor ter controlle sobre o 
                //dispose do objeto
                using var context = GetSqlServerDatabaseContext(Guid.NewGuid().ToString());

                Assert.ThrowsException<InvalidDataException>(() => GetProdutoServicoInvalidJWT(context));
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
