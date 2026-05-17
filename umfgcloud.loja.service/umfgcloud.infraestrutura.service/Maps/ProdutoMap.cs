using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umfgcloud.loja.dominio.service.Entidades;

namespace umfgcloud.infraestrutura.service.Maps
{
    internal sealed class ProdutoMap : AbstractEntityMap<ProdutoEntity>
    {
        public override void Configure(EntityTypeBuilder<ProdutoEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("PRODUTO");

            builder.Property(x => x.Descricao).HasColumnName("DS_PRODUTO").HasMaxLength(1000).IsRequired();
            builder.Property(x => x.EAN).HasColumnName("CD_BARRA_EAN").HasMaxLength(13).IsRequired();
            builder.Property(x => x.ValorCompra).HasColumnName("VL_COMPRA").HasPrecision(14, 2).IsRequired();
            builder.Property(x => x.ValorVenda).HasColumnName("VL_VENDA").HasPrecision(14, 2).IsRequired();
        }
    }
}
