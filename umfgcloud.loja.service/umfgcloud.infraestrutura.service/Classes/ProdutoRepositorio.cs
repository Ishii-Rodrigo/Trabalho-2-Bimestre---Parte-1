using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umfgcloud.infraestrutura.service.Context;
using umfgcloud.loja.dominio.service.Entidades;
using umfgcloud.loja.dominio.service.Interfaces.Repositorios;

namespace umfgcloud.infraestrutura.service.Classes
{
    public sealed class ProdutoRepositorio : AbstractRepositorio<ProdutoEntity>,
        IProdutoRepositorio
    {
        public ProdutoRepositorio(MySqlDataBaseContext context) : base(context)
        {
        }
    }
}
