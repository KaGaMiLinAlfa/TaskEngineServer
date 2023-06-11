using DAO.DQDbContext.Methods;
using Microsoft.EntityFrameworkCore;
using Model;
using System;

namespace test
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();
            optionsBuilder.UseMySql("server=localhost;port=3306;database=TaskDB;uid=root;pwd=123123;",
                MySqlServerVersion.AutoDetect("server=localhost;port=3306;database=TaskDB;uid=root;pwd=123123;"));

            var options = optionsBuilder.Options;

            using var context = new BaseDbContext(options);
            var rep = new EfRepository<TaskInfo>(context);
            rep.Add(new TaskInfo
            {
                TaskName = "new 1",
            });



            Console.WriteLine("Hello World!");
        }
    }
}
