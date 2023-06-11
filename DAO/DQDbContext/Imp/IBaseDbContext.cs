using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

        int Add<T>(T entity) where T : BaseEntity;

        int Update<T>(T model, Expression<Func<T, bool>> @where, Expression<Func<T, object>> updateParameters) where T : BaseEntity;

        int Delete<T>(Expression<Func<T, bool>> @where) where T : BaseEntity;
    }

}
