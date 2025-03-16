using System.Linq.Expressions;

namespace BusinessLogic.Services;

public interface IBaseService<T, U, TView> where T : class where TView : class
{
    /// <summary>
    /// Find method - Use for get data from database, user for Update, Delete
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="isTracking"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    IQueryable<T?> Find(Expression<Func<T, bool>> predicate = null!, 
        bool isTracking = true,  
        params Expression<Func<T, object>>[] includes);
    
    /// <summary>
    /// FindAsync method - Use for get data from database, user for Update, Delete
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="isTracking"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    Task<IQueryable<T?>> FindAsync(Expression<Func<T, bool>> predicate = null!,
        bool isTracking = true, 
        params Expression<Func<T, object>>[] includes);
    
    /// <summary>
    /// Find method - Use for get data from database
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="isTracking"></param>
    /// <param name="includes"></param>
    /// <returns></returns>
    IQueryable<TView?> FindView(Expression<Func<TView, bool>> predicate = null!, 
        bool isTracking = false,  
        params Expression<Func<TView, object>>[] includes);
    
    // Get single record by ID (synchronous)
    Task<T?> GetByIdAsync(U id);
    
    // Get single record by ID (asynchronous)
    T? GetById(U id);
    
    // Add entity (synchronous)
    bool Add(T entity);

    // Update entity (synchronous)
    void Update(T entity);

    // Add entity (asynchronous)
    Task AddAsync(T entity);

    // Update entity (asynchronous)
    Task UpdateAsync(T entity);
    
    // Save changes
    int SaveChanges(string userName, bool needLogicalDelete = false);
    
    // Save changes asynchronously
    Task<int> SaveChangesAsync(string userName, bool needLogicalDelete = false);
}