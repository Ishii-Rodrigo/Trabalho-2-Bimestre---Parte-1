using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umfgcloud.loja.dominio.service.Entidades;

namespace umfgcloud.loja.dominio.service.Interfaces.Repositorios
{
    public interface IAbstractRepositorio<T> where T : AbstractEntity
    {
        Task<ICollection<T>> ObterTodosAsync();
        Task<T> ObterPorIdAsync(Guid id);
        Task AdicionarAsync(T entidade);
        Task AtualizarAsync(T entidade);
        Task RemoverAsync(T entidade);
    }
}
