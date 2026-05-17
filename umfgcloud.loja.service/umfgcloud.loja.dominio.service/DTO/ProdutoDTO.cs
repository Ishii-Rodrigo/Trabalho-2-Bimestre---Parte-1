using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace umfgcloud.loja.dominio.service.DTO
{
    public sealed class ProdutoDTO
    {
        public abstract class AbstractProdutoDTO
        {
            [JsonPropertyName("descricao")]
            [Required(ErrorMessage = "O atributo descricao é obrigatório.")]
            public string Descricao { get; set; } = string.Empty;

            [JsonPropertyName("ean")]
            [Required(ErrorMessage = "O atributo ean é obrigatório.")]
            public string EAN { get; set; } = string.Empty;

            [JsonPropertyName("valorCompra")]
            [Required(ErrorMessage = "O atributo valorCompra é obrigatório.")]
            public decimal ValorCompra { get; set; } = decimal.Zero;

            [JsonPropertyName("valorVenda")]
            [Required(ErrorMessage = "O atributo valorVenda é obrigatório.")]
            public decimal ValorVenda { get; set; } = decimal.Zero;
        }

        public abstract class AbstractProdutoWithIdDTO : AbstractProdutoDTO
        {
            [JsonPropertyName("id")]
            [Required(ErrorMessage = "O atributo id é obrigatório.")]
            public Guid Id { get; set; } = Guid.Empty;
        }

        public sealed class ProdutoRequest : AbstractProdutoDTO { }
        public sealed class ProdutoRequestWithId : AbstractProdutoWithIdDTO { }
        public sealed class ProdutoResponse : AbstractProdutoWithIdDTO { }
    }
}