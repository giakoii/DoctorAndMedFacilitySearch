using System.Linq.Expressions;
using DataAccessObject.Models.Helper;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject.Repositories;

/// <summary>
/// BaseRepository - Base class for all repositories
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="Type"></typeparam>
public class BaseRepository<TEntity, Type, TView> : IBaseRepository<TEntity, Type, TView> where TEntity : class where TView : class
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="context"></param>
    public BaseRepository(AppDbContext context)
    {
        _context = context;
    }

    private DbSet<TEntity> DbSet => _context.Set<TEntity>();
    
    private DbSet<TView> DbSetSelect => _context.Set<TView>();


    /// <summary>
    /// Find multiple records - Use for select
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="isTracking"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    public IQueryable<TEntity?> Find(Expression<Func<TEntity?, bool>> predicate = null, bool isTracking = false,
        params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = DbSet;

        if (predicate != null) query = query.Where(predicate);

        if (includes != null) query = includes.Aggregate(query, (current, inc) => current.Include(inc));

        if (!isTracking) query = query.AsNoTracking();

        return query;
    }

    /// <summary>
    /// Find multiple records (View) - Use for select 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="isTracking"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    public IQueryable<TView?> FindView(Expression<Func<TView, bool>> predicate = null, bool isTracking = false,
        params Expression<Func<TView, object>>[] includes)
    {
        IQueryable<TView> query = DbSetSelect;

        // Predicate
        if (predicate != null) query = query.Where(predicate);

        // Include
        if (includes != null) query = includes.Aggregate(query, (current, inc) => current.Include(inc));

        // No tracking
        if (!isTracking) query = query.AsNoTracking();

        return query;
    }

    /// <summary>
    /// Find multiple records asynchronously - Use for select
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="isTracking"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    public async Task<IQueryable<TEntity?>> FindAsync(Expression<Func<TEntity, bool>> predicate = null,
        bool isTracking = false, params Expression<Func<TEntity, object>>[] includes)
    {
        IQueryable<TEntity> query = DbSet;
        
        if (predicate != null) query = query.Where(predicate);

        if (includes != null) query = includes.Aggregate(query, (current, inc) => current.Include(inc));

        if (!isTracking) query = query.AsNoTracking();

        return query;
    }

    /// <summary>
    /// Get entity by Id - Use for Insert, Update, Delete
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public TEntity? GetById(Type id)
    {
        return DbSet.Find(id);
    }

    /// <summary>
    /// Get entity by Id asynchronously - Use for Insert, Update, Delete
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<TEntity?> GetByIdAsync(Type id)
    {
        return await DbSet.FindAsync(id);
    }

    /// <summary>
    /// Add entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool Add(TEntity entity)
    {
        _context.Add(entity);
        return true;
    }

    /// <summary>
    /// Add entity asynchronously
    /// </summary>
    public async Task AddAsync(TEntity entity)
    {
         _context.AddAsync(entity);
    }

    /// <summary>
    /// Update entity
    /// </summary>
    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }

    /// <summary>
    /// Update entity asynchronously
    /// </summary>
    public Task UpdateAsync(TEntity entity)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Delete entity by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool DeleteById(Type id)
    {
        var entity = GetById(id);
        if (entity == null) return false;

        DbSet.Update(entity);
        return true;
    }

    /// <summary>
    /// Delete entity by ID asynchronously
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteByIdAsync(Type id)
    {
        var entity = await GetByIdAsync(id);

        // Check if entity is null
        if (entity == null) return false;

        DbSet.Update(entity);
        return true;
    }

    /// <summary>
    /// Save changes
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="needLogicalDelete"></param>
    /// <exception cref="NotImplementedException"></exception>
    public int SaveChanges(string userName, bool needLogicalDelete = false)
    {
        return _context.SaveChanges(userName, needLogicalDelete);
    }

    /// <summary>
    /// Save changes asynchronously
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="needLogicalDelete"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<int> SaveChangesAsync(string userName, bool needLogicalDelete = false)
    {
        return await _context.SaveChangesAsync(userName, needLogicalDelete);
    }

    /// <summary>
    /// Execute multiple operations within a transaction.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public void ExecuteInTransaction(Action action)
    {
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                action();
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }
    }

    /// <summary>
    /// Execute multiple operations within a transaction asynchronously.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                await action();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }
    }
}