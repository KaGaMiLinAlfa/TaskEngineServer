using DAO.DQDbContext.Imp;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public IQueryable<T> GetQueryable() => DbSet.AsQueryable();


    }

}
