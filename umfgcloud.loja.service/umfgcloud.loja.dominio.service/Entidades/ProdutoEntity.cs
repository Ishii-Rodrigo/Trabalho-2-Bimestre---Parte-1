using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace umfgcloud.loja.dominio.service.Entidades
{
    public sealed class ProdutoEntity : AbstractEntity
    {
        public string Descricao { get; private set; } = string.Empty;
        public string EAN { get; private set; } = string.Empty;
        public decimal ValorCompra { get; private set; } = decimal.Zero;
        public decimal ValorVenda { get; private set; } = decimal.Zero;

        private ProdutoEntity() : base() { }

        public ProdutoEntity(string userId, string userEmail) 
            : base(userId, userEmail) { }

        public void SetDescricao(string descricao) 
            => Descricao = descricao?.ToUpper() ?? throw new ArgumentNullException(nameof(descricao));

        public void SetEAN(string ean)
            => EAN = ean ?? throw new ArgumentNullException(nameof(ean));

        public void SetValorCompra(decimal valorCompra)
        {
            if (valorCompra <= decimal.Zero)
                throw new InvalidDataException($"Valor compra informado {valorCompra} inválido!");

            ValorCompra = Math.Round(valorCompra, 2);
        }

        public void SetValorVenda(decimal valorVenda)
        {
            if (valorVenda <= decimal.Zero)
                throw new InvalidDataException($"Valor venda informado {valorVenda} inválido!");

            ValorVenda = Math.Round(valorVenda, 2);
        }
    }
}
