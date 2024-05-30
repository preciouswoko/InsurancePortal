using InsuranceCore.Interfaces;
using InsuranceInfrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceInfrastructure.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly ILoggingService _log;
        public GenericRepository(ApplicationDbContext context, ILoggingService log)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<TEntity>();
            _log = log;
        }

        public TEntity GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            //return await _dbSet.ToListAsync();
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public bool Insert(TEntity entity)
        {
            try
            {
                _dbSet.Add(entity);
                Save();
                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToString(), $"Insert _{entity}");

                return false;
            }
        }

        public bool Update(TEntity entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                Save();
                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToString(), $"Update _{entity}");
                return false;
            }
        }

        public bool Delete(object id)
        {
            try
            {
                TEntity entityToDelete = _dbSet.Find(id);
                if (entityToDelete != null)
                {
                    _dbSet.Remove(entityToDelete);
                    Save();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToString(), $"Delete _{id}");

                return false;
            }
        }
        public async Task<IEnumerable<TEntity>> GetAllWithPredicate(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        public async Task<int> GetAllCount(Expression<Func<TEntity, bool>> predicate)
        {
            return  _dbSet.Where(predicate).Count();
        }
        public async Task<TEntity> GetWithPredicate(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).FirstOrDefaultAsync();
        }

        //public async Task<TEntity> GetLastWithPredicate(Expression<Func<TEntity, bool>> predicate)
        //{
        //    return await _dbSet
        //        .Where(predicate)
        //        .OrderByDescending(entity => entity.Id)
        //        .FirstOrDefaultAsync();
        //}
        public async Task<TEntity> GetLastWithPredicate(Expression<Func<TEntity, bool>> predicate,Expression<Func<TEntity, object>> orderBy)
        {
           return await _dbSet.Where(predicate)
                .OrderByDescending(orderBy)
                .FirstOrDefaultAsync();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
        public async Task<IEnumerable<TEntity>> GetWithInclude(Expression<Func<TEntity, bool>> predicate,
                                                            params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(predicate).ToListAsync();
        }
        public async Task<IEnumerable<TEntity>> GetWithIncludeOrderby( Expression<Func<TEntity, bool>> predicate,Expression<Func<TEntity, object>> orderBy,params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(predicate)
                              .OrderByDescending(orderBy)
                              .ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetWithIncludeNoTrackingAsync(Expression<Func<TEntity, bool>> predicate,
                                                                             params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.AsNoTracking().Where(predicate).ToListAsync();
        }


        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity> GetWithIncludeAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _dbSet;

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return await query.Where(predicate).FirstOrDefaultAsync();
        }
        public IQueryable<TEntity> GetBaseQuerywithInclude(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.AsQueryable();
        }
        public IQueryable<TEntity> GetBaseQuery()
        {
            return _context.Set<TEntity>().AsQueryable();
        }
        public async Task<IEnumerable<TEntity>> GetAllWithQuery(IQueryable<TEntity> query)
        {
            return await query.ToListAsync();
        }
        public async Task ExecuteScriptAsync(string connectionString, string sqlScript, object[] sqlParameters)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sqlScript;
                    command.Parameters.AddRange(sqlParameters);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

    }
}
