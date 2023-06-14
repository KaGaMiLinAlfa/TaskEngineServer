using DAO.DQDbContext.Imp;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DAO.DQDbContext.Methods
{
    public class EfRepository<T> : IEfRepository<T> where T : BaseEntity

    {
        protected readonly IBaseDbContext _context;

        public EfRepository(IBaseDbContext context)
        {
            this._context = context;
        }

        public void DbContext(Action action)
        {
            try
            {
                _context.GetDatabase().BeginTransaction();

                action();

                _context.GetDatabase().CommitTransaction();
            }
            catch (Exception ex)
            {
                //throw new Exception($"执行事务出现异常：{ex.Message}");
                throw ex;
            }

        }


        protected DbSet<T> DbSet => _context.GetDbSet<T>();

        public int Add(T entity) => _context.Add(entity);
        public int Add(List<T> entitys) => _context.Add(entitys);
        public int Update(Expression<Func<T, T>> updateParameters, Expression<Func<T, bool>> @where) => _context.Update(updateParameters, @where);
        public int Delete(Expression<Func<T, bool>> where) => _context.Delete(where);

        public IQueryable<T> GetQueryable() => DbSet.AsQueryable();


    }

}
