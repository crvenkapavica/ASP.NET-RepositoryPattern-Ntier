using System.Linq.Expressions;
using BookStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BookStore.DataAccess.Repository;

public abstract class Repository<T> : IRepository<T>
    where T : class
{
    private readonly DbSet<T> _dbSet;
    private readonly char[] _separator = { ',' };

    protected Repository(DbContext db)
    {
        _dbSet = db.Set<T>();
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public T? Get(Expression<Func<T, bool>> filter, string? includeProperties = null)
    {
        var query = _dbSet.Where(filter);
        if (!string.IsNullOrEmpty(includeProperties))
        {
            query = includeProperties.Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, p) => current.Include(p));
        }
        return query.FirstOrDefault();
    }

    public IEnumerable<T> GetAll(string? includeProperties = null)
    {
        IQueryable<T> query = _dbSet;
        if (!string.IsNullOrEmpty(includeProperties))
        {
            query = includeProperties.Split(_separator, StringSplitOptions.RemoveEmptyEntries)
                .Aggregate(query, (current, p) => current.Include(p));
        }
        return query.ToList();
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}