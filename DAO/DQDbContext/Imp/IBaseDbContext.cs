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
        int Add<T>(List<T> entitys) where T : BaseEntity;

        int Update<T>(Expression<Func<T, T>> updateParameters, Expression<Func<T, bool>> @where) where T : BaseEntity;

        int Delete<T>(Expression<Func<T, bool>> @where) where T : BaseEntity;
    }

}
