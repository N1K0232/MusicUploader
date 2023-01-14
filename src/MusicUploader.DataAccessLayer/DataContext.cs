using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MusicUploader.DataAccessLayer.Entities.Common;

namespace MusicUploader.DataAccessLayer;

public class DataContext : DbContext, IDataContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }


    public void Create<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        var set = Set<TEntity>();
        set.Add(entity);
    }

    public void Delete<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        var set = Set<TEntity>();
        set.Remove(entity);
    }

    public void Edit<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        var set = Set<TEntity>();
        set.Update(entity);
    }

    public Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : BaseEntity
    {
        var set = GetData<TEntity>();
        return set.AnyAsync(expression);
    }

    public ValueTask<TEntity> GetAsync<TEntity>(params object[] keyValues) where TEntity : BaseEntity
    {
        var set = Set<TEntity>();
        return set.FindAsync(keyValues);
    }

    public IQueryable<TEntity> GetData<TEntity>(bool ignoreQueryFilters = false, bool trackingChanges = false) where TEntity : BaseEntity
    {
        var set = Set<TEntity>().AsQueryable();

        if (ignoreQueryFilters)
        {
            set = set.IgnoreQueryFilters();
        }

        return trackingChanges ?
            set.AsTracking() :
            set.AsNoTrackingWithIdentityResolution();
    }

    public Task SaveAsync() => SaveChangesAsync();

    public Task ExecuteTransactionAsync(Func<Task> action)
    {
        var strategy = Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            using var transaction = await Database.BeginTransactionAsync().ConfigureAwait(false);
            await action.Invoke().ConfigureAwait(false);
            await transaction.CommitAsync().ConfigureAwait(false);
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}