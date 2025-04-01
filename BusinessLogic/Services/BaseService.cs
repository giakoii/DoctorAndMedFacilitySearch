using DataAccessObject;
using System.Linq.Expressions;

namespace BusinessLogic.Services;

public class BaseService<T, TU, TView> : IBaseService<T, TU, TView> where T : class where TView : class
{
    protected readonly IBaseRepository<T, TU, TView> _repository;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="repository"></param>
    public BaseService(IBaseRepository<T, TU, TView> repository)
    {
        _repository = repository;
    }

    public async Task<IQueryable<T?>> FindAsync(Expression<Func<T, bool>> predicate = null, bool isTracking = true, params Expression<Func<T, object>>[] includes)
    {
        return await _repository.FindAsync(predicate, isTracking, includes);
    }

    /// <summary>
    /// Find multiple or single record - Use for get information. Do not use Delete, Update
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="isTracking"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    public IQueryable<TView?> FindView(Expression<Func<TView, bool>> predicate = null!,
        bool isTracking = false,
        params Expression<Func<TView, object>>[] includes)
    {
        return _repository.FindView(predicate, isTracking, includes);
    }

    /// <summary>
    /// Find multiple or single record - User for Delete, Update
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="isTracking"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    public IQueryable<T?> Find(Expression<Func<T, bool>> predicate = null, bool isTracking = true, params Expression<Func<T, object>>[] includes)
    {
        return _repository.Find(predicate, isTracking, includes);
    }

    /// <summary>
    /// Get single record by ID (asynchronous)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<T?> GetByIdAsync(TU id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public T? GetById(TU id)
    {
        return _repository.GetById(id);
    }

    public bool Add(T entity)
    {
        return _repository.Add(entity);
    }

    public void Update(T entity)
    {
        _repository.Update(entity);
    }

    public async Task AddAsync(T entity)
    {
        await _repository.AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        await _repository.UpdateAsync(entity);
    }

    /// <summary>
    /// Delete entity by ID (asynchronous)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<bool> DeleteByIdAsync(TU id)
    {
        return await _repository.DeleteByIdAsync(id);
    }

    /// <summary>
    /// Save change for all - Should be used once
    /// If need Delete (set IsActive = false), use needLogicalDelete = true
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="needLogicalDelete"></param>
    /// <returns></returns>
    public int SaveChanges(string userName, bool needLogicalDelete = false)
    {
        return _repository.SaveChanges(userName, needLogicalDelete);
    }

    /// <summary>
    /// Save change for all - Should be used once
    /// If need Delete (set IsActive = false), use needLogicalDelete = true
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="needLogicalDelete"></param>
    /// <returns></returns>
    public async Task<int> SaveChangesAsync(string userName, bool needLogicalDelete = false)
    {
        return await _repository.SaveChangesAsync(userName, needLogicalDelete);
    }
}
