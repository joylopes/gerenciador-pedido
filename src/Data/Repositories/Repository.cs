using GerenciadorPedido.Data.Context;
using GerenciadorPedido.Domain.Interfaces.Repository;
using GerenciadorPedido.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GerenciadorPedido.Data.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        protected readonly AppDbContext Db;
        protected readonly DbSet<TEntity> DbSet;

        protected Repository(AppDbContext db)
        {
            Db = db;
            DbSet = db.Set<TEntity>();
        }

        public virtual async Task<TEntity> Adicionar(TEntity entity)
        {
            DbSet.Add(entity);
            await SaveChanges();

            return entity;
        }
        public async Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
        }
        public virtual async Task<TEntity?> ObterPorId(int id)
        {
            return await DbSet.FindAsync(id);
        }
        public virtual async Task<IEnumerable<TEntity>> ObterTodos()
        {
            return await DbSet.ToListAsync();
        }
        public async Task<int> SaveChanges()
        {
            return await Db.SaveChangesAsync();
        }
        public virtual async Task Atualizar(TEntity entity)
        {
            DbSet.Update(entity);
            await SaveChanges();
        }
        public virtual async Task Remover(int id)
        {
            DbSet.Remove(new TEntity { Id = id });
            await SaveChanges();
        }
        public void Dispose()
        {
            Db.Dispose();
        }
    }
}
