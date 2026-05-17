using Microsoft.EntityFrameworkCore;
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
    public abstract class AbstractRepositorio<T> : IAbstractRepositorio<T> where T : AbstractEntity
    {
        private readonly MySqlDataBaseContext _context;

        //basicamente ele aponta qual tabela voce quer carregar do database x classe
        public DbSet<T> Entity => _context.Set<T>();

        //injeção de dependencia
        protected AbstractRepositorio(MySqlDataBaseContext context)
            => _context = context ?? throw new ArgumentNullException(nameof(context));

        //retorna todos os dados da tabela, se nao possuir retorna
        //lista vazia
        public virtual async Task<ICollection<T>> ObterTodosAsync()
            => await Entity.Where(x => x.IsActive).ToListAsync() ?? [];

        //procura um registro pelo id e se está ativo
        //se nao encontrar gera exception
        public virtual async Task<T> ObterPorIdAsync(Guid id)
            => await Entity.FirstOrDefaultAsync(x => x.Id == id && x.IsActive) 
            ?? throw new ApplicationException($"Registro nao encontrado | {this.GetType()}");

        public async Task AdicionarAsync(T entity)
        {
            await Entity.AddAsync(entity);
            await _context.SaveChangesAsync(); //commita os dados no database
        }

        public async Task RemoverAsync(T entity)
        {
            Entity.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(T entity)
        {
            Entity.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
