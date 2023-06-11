using DAO.DQDbContext.Imp;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Z.EntityFramework.Plus;

namespace DAO.DQDbContext.Methods
{
    public class BaseDbContext : DbContext, IBaseDbContext
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
        {
        }

        public DatabaseFacade GetDatabase() => Database;

        public DbSet<TEntity> GetDbSet<TEntity>() where TEntity : class
        {
            if (Model.FindEntityType(typeof(TEntity)) != null)
                return Set<TEntity>();
            throw new Exception($"类型{typeof(TEntity).Name}未在数据库上下文中注册，请先在DbContextOption设置ModelAssemblyName以将所有实体类型注册到数据库上下文中。");
        }



        //public DbSet<TaskInfoEntity> TaskInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            MappingEntityTypes(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }


        private static string[] NotAssemblies = new string[]
        {
            "Model",
            "model"
        };


        private void MappingEntityTypes(ModelBuilder modelBuilder)
        {
            var assembsssly = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == "");

            var assemblysss = AppDomain.CurrentDomain.GetAssemblies().Where(x => NotAssemblies.Any(a => x.FullName.StartsWith(a)));
            var allType = assemblysss.SelectMany(s => s.GetTypes());

            var count = allType.Count();

            var types = allType.Where(x => x.IsSubclassOf(typeof(BaseEntity)) && x != typeof(BaseEntity)).ToList();

            var zqweqe = assembly?.GetTypes();

            if (types.Any())
            {
                types.ForEach(x =>
                {
                    var entityType = modelBuilder.Model.FindEntityType(x);
                    if (entityType == null)
                        modelBuilder.Model.AddEntityType(x);

                    //其他表处理,表名变更可以在这里反射处理名称
                    //modelBuilder.Model.FindEntityType(x).SetTableName($"TabelNewName");
                });
            }

        }

        public new virtual int Add<T>(T entity) where T : BaseEntity
        {

            //this.BulkInsert();
            base.Add(entity);
            return SaveChanges();
        }

        public int Update<T>(T model, Expression<Func<T, bool>> where, Expression<Func<T, object>> updateParameters) where T : BaseEntity
        {
            //性能最优推荐原生拼接，需要测试是否能跟随上下文事务

            //this.BulkUpdate
            GetDbSet<T>().Where(where).UpdateFromQuery(updateParameters);
            return 0;
        }

        public int Delete<T>(Expression<Func<T, bool>> where) where T : BaseEntity
        {
            //性能最优推荐原生拼接，需要测试是否能跟随上下文事务

            GetDbSet<T>().Where(where).Delete();
            return 0;
        }
    }
    public static class ObjectExtension
    {
        public static bool IsImplement(this Type entityType, Type interfaceType)
        {
            return entityType.GetTypeInfo().GetInterfaces().Any(x => x == interfaceType);
        }
    }
}
