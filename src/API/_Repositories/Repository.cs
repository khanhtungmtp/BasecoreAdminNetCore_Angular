using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace API._Repositories;

public class Repository<T, DBContext>(DBContext context) : IRepository<T> where T : class where DBContext : DbContext
{
    private readonly DBContext _context = context;

    public void Add(T entity)
    {
        _context.Add(entity);
    }

    public void AddMultiple(List<T> entities)
    {
        _context.AddRange(entities);
    }

    public IQueryable<T> FindAll(bool? noTracking = false)
    {
        return noTracking == true ? _context.Set<T>().AsNoTracking() : _context.Set<T>();
    }

    public IQueryable<T> FindAll(Expression<Func<T, bool>> predicate, bool? noTracking = false)
    {
        return noTracking == true ? _context.Set<T>().Where(predicate).AsNoTracking() : _context.Set<T>().Where(predicate);
    }

    public async Task<T> FindByIdAsync(object id)
    {
        return await _context.Set<T>().FindAsync(id) ?? null!;
    }

    public async Task<T> FindAsync(params object[] keyValues)
    {
        return await _context.Set<T>().FindAsync(keyValues) ?? null!;
    }


    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public void Remove(object id)
    {
        Remove(FindByIdAsync(id));
    }

    public void RemoveMultiple(List<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }

    public void UpdateMultiple(List<T> entities)
    {
        _context.Set<T>().UpdateRange(entities);
    }

    public bool All(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().All(predicate);
    }

    public async Task<bool> AllAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().AllAsync(predicate);
    }

    public bool Any()
    {
        return _context.Set<T>().Any();
    }

    public bool Any(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().Any(predicate);
    }

    public async Task<bool> AnyAsync()
    {
        return await _context.Set<T>().AnyAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().AnyAsync(predicate);
    }

    public T FirstOrDefault(bool? noTracking = false)
    {
        return noTracking == true ? _context.Set<T>().AsNoTracking().FirstOrDefault() ?? default! : _context.Set<T>().FirstOrDefault() ?? default!;
    }

    public T FirstOrDefault(Expression<Func<T, bool>> predicate, bool? noTracking = false)
    {
        return noTracking == true ? _context.Set<T>().AsNoTracking().FirstOrDefault(predicate) ?? default! : _context.Set<T>().FirstOrDefault(predicate) ?? default!;
    }

    public async Task<T> FirstOrDefaultAsync(bool? noTracking = false)
    {
        return noTracking == true ? await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync() ?? default! : await _context.Set<T>().FirstOrDefaultAsync() ?? default!;
    }

    public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool? noTracking = false)
    {
        return noTracking == true ? await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate) ?? default! : await _context.Set<T>().FirstOrDefaultAsync(predicate) ?? default!;
    }

    public int Count()
    {
        return _context.Set<T>().Count();
    }

    public int Count(Expression<Func<T, bool>> predicate)
    {
        return _context.Set<T>().Count(predicate);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Set<T>().CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _context.Set<T>().CountAsync(predicate);
    }

    public T LastOrDefault(bool? noTracking = false)
    {
        return noTracking == true ? _context.Set<T>().AsNoTracking().LastOrDefault() ?? default! : _context.Set<T>().LastOrDefault() ?? default!; ;
    }

    public T LastOrDefault(Expression<Func<T, bool>> predicate, bool? noTracking = false)
    {
        return noTracking == true ? _context.Set<T>().AsNoTracking().LastOrDefault(predicate) ?? default! : _context.Set<T>().LastOrDefault(predicate) ?? default!;
    }

    public async Task<T> LastOrDefaultAsync(bool? noTracking = false)
    {
        return noTracking == true ? await _context.Set<T>().AsNoTracking().LastOrDefaultAsync() ?? default! : await _context.Set<T>().LastOrDefaultAsync() ?? default!;
    }

    public async Task<T> LastOrDefaultAsync(Expression<Func<T, bool>> predicate, bool? noTracking = false)
    {
        return noTracking == true ? await _context.Set<T>().AsNoTracking().LastOrDefaultAsync(predicate) ?? default! : await _context.Set<T>().LastOrDefaultAsync(predicate) ?? default!;
    }

    public decimal Sum(Expression<Func<T, decimal>> selector)
    {
        return _context.Set<T>().Sum(selector);
    }

    public decimal? Sum(Expression<Func<T, decimal?>> selector)
    {
        return _context.Set<T>().Sum(selector);
    }

    public decimal Sum(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> selector)
    {
        return _context.Set<T>().Where(predicate).Sum(selector);
    }

    public decimal? Sum(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal?>> selector)
    {
        return _context.Set<T>().Where(predicate).Sum(selector);
    }

    public async Task<decimal> SumAsync(Expression<Func<T, decimal>> selector)
    {
        return await _context.Set<T>().SumAsync(selector);
    }

    public async Task<decimal?> SumAsync(Expression<Func<T, decimal?>> selector)
    {
        return await _context.Set<T>().SumAsync(selector);
    }

    public async Task<decimal> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal>> selector)
    {
        return await _context.Set<T>().Where(predicate).SumAsync(selector);
    }

    public async Task<decimal?> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, decimal?>> selector)
    {
        return await _context.Set<T>().Where(predicate).SumAsync(selector);
    }

    public int Sum(Expression<Func<T, int>> selector)
    {
        return _context.Set<T>().Sum(selector);
    }

    public int? Sum(Expression<Func<T, int?>> selector)
    {
        return _context.Set<T>().Sum(selector);
    }

    public int Sum(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> selector)
    {
        return _context.Set<T>().Where(predicate).Sum(selector);
    }

    public int? Sum(Expression<Func<T, bool>> predicate, Expression<Func<T, int?>> selector)
    {
        return _context.Set<T>().Where(predicate).Sum(selector);
    }

    public async Task<int> SumAsync(Expression<Func<T, int>> selector)
    {
        return await _context.Set<T>().SumAsync(selector);
    }

    public async Task<int?> SumAsync(Expression<Func<T, int?>> selector)
    {
        return await _context.Set<T>().SumAsync(selector);
    }

    public async Task<int> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> selector)
    {
        return await _context.Set<T>().Where(predicate).SumAsync(selector);
    }

    public async Task<int?> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, int?>> selector)
    {
        return await _context.Set<T>().Where(predicate).SumAsync(selector);
    }

    public long Sum(Expression<Func<T, long>> selector)
    {
        return _context.Set<T>().Sum(selector);
    }

    public long? Sum(Expression<Func<T, long?>> selector)
    {
        return _context.Set<T>().Sum(selector);
    }

    public long Sum(Expression<Func<T, bool>> predicate, Expression<Func<T, long>> selector)
    {
        return _context.Set<T>().Where(predicate).Sum(selector);
    }

    public long? Sum(Expression<Func<T, bool>> predicate, Expression<Func<T, long?>> selector)
    {
        return _context.Set<T>().Where(predicate).Sum(selector);
    }

    public async Task<long> SumAsync(Expression<Func<T, long>> selector)
    {
        return await _context.Set<T>().SumAsync(selector);
    }

    public async Task<long?> SumAsync(Expression<Func<T, long?>> selector)
    {
        return await _context.Set<T>().SumAsync(selector);
    }

    public async Task<long> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, long>> selector)
    {
        return await _context.Set<T>().Where(predicate).SumAsync(selector);
    }

    public async Task<long?> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, long?>> selector)
    {
        return await _context.Set<T>().Where(predicate).SumAsync(selector);
    }

    public float Sum(Expression<Func<T, float>> selector)
    {
        return _context.Set<T>().Sum(selector);
    }

    public float? Sum(Expression<Func<T, float?>> selector)
    {
        return _context.Set<T>().Sum(selector);
    }

    public float Sum(Expression<Func<T, bool>> predicate, Expression<Func<T, float>> selector)
    {
        return _context.Set<T>().Where(predicate).Sum(selector);
    }

    public float? Sum(Expression<Func<T, bool>> predicate, Expression<Func<T, float?>> selector)
    {
        return _context.Set<T>().Where(predicate).Sum(selector);
    }

    public async Task<float> SumAsync(Expression<Func<T, float>> selector)
    {
        return await _context.Set<T>().SumAsync(selector);
    }

    public async Task<float?> SumAsync(Expression<Func<T, float?>> selector)
    {
        return await _context.Set<T>().SumAsync(selector);
    }

    public async Task<float> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, float>> selector)
    {
        return await _context.Set<T>().Where(predicate).SumAsync(selector);
    }

    public async Task<float?> SumAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, float?>> selector)
    {
        return await _context.Set<T>().Where(predicate).SumAsync(selector);
    }
}
