using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceCore.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity GetById(object id);
        Task<IEnumerable<TEntity>> GetAll();
        bool Insert(TEntity entity);
        bool Update(TEntity entity);
        bool Delete(object id);
        Task<int> GetAllCount(Expression<Func<TEntity, bool>> predicate);
        void Save();
        Task<IEnumerable<TEntity>> GetAllWithPredicate(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetWithPredicate(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetWithInclude(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<IEnumerable<TEntity>> GetWithIncludeNoTrackingAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetByIdAsync(object id);
        Task<TEntity> GetWithIncludeAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties);
        IQueryable<TEntity> GetBaseQuery();
        Task<TEntity> GetLastWithPredicate(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, object>> orderBy);
        Task<IEnumerable<TEntity>> GetAllWithQuery(IQueryable<TEntity> query);
        Task ExecuteScriptAsync(string connectionString, string sqlScript, object[] sqlParameters);
        IQueryable<TEntity> GetBaseQuerywithInclude(params Expression<Func<TEntity, object>>[] includeProperties);
    }
}
