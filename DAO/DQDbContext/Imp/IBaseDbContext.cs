using Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DAO.DQDbContext.Imp
{
    public interface IBaseDbContext : IDisposable
    {
        DatabaseFacade GetDatabase();

        DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class;

        int SaveChanges();

        int Add<T>(T entity) where T : IBaseEntity;

        int Update<T>(T model, Expression<Func<T, bool>> @where, Expression<Func<T, object>> updateParameters) where T : IBaseEntity;

        int Delete<T>(Expression<Func<T, bool>> @where) where T : IBaseEntity;
    }

}
