using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAO.DQDbContext.Imp
{
    public interface IEfRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetQueryable();

        void DbContext(Action action);

        int Add(T entity);


    }
}
