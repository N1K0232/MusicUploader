using System.Linq.Expressions;
using MusicUploader.DataAccessLayer.Entities.Common;

namespace MusicUploader.DataAccessLayer;

public interface IDataContext
{
    Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : BaseEntity;

    IQueryable<TEntity> GetData<TEntity>(bool ignoreQueryFilters = false, bool trackingChanges = false) where TEntity : BaseEntity;

    ValueTask<TEntity> GetAsync<TEntity>(params object[] keyValues) where TEntity : BaseEntity;

    void Create<TEntity>(TEntity entity) where TEntity : BaseEntity;

    void Delete<TEntity>(TEntity entity) where TEntity : BaseEntity;

    void Edit<TEntity>(TEntity entity) where TEntity : BaseEntity;

    Task SaveAsync();

    Task ExecuteTransactionAsync(Func<Task> action);
}