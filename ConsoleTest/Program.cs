﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Config;
using VIC.DataAccess.MSSql;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace MSSqlExample
{
    public class Program
    {
        private static IServiceProvider _Provider;
        private static IDbManager _DB;

        public static void Main(string[] args)
        {
            var a = Assembly.GetAssembly(typeof(SqlDataAdapter))
                .GetType("System.Data.SqlClient.SqlCommandSet");
            a.ToString();
            //ExecuteTimer("Batch", () =>
            //{
            //    DataTable t = new DataTable();
            //    t.Columns.Add("@Name", typeof(string));
            //    t.Columns.Add("@Id", typeof(int));
            //    for (int i = 0; i < 500; i++)
            //    {
            //        var row = t.Rows.Add(i.ToString() + "T", i + 1);
            //        row.AcceptChanges();
            //        row.SetModified();
            //    }
            //    using (var c = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            //    {
            //        c.Open();
            //        var com = c.CreateCommand();
            //        com.CommandText = "update top(1) [dbo].[Students]  set Name = @Name where Id = @Id";
            //        var p = com.Parameters.Add("@Name", SqlDbType.VarChar);
            //        p.SourceColumn = "@Name";
            //        p = com.Parameters.Add("@Id", SqlDbType.Int);
            //        p.SourceColumn = "@Id";
            //        com.UpdatedRowSource = UpdateRowSource.None;
            //        using (SqlDataAdapter a = new SqlDataAdapter())
            //        {
            //            a.UpdateBatchSize = 500;
            //            a.UpdateCommand = com;
            //            a.Update(t);
            //        }
            //    }
            //});
            //Init();
            //var command = _DB.GetCommand("SelectByName");
            //var num = command.ExecuteEntityList<Student>(new { Id = new List<int>() { 1, 3, 5, 7, 9 } });
            //Test().Wait();
            //Test2().Wait();
        }

        public async static Task Test2()
        {
            var count = 500;
            var students = GenerateStudents(count);
            await ExecuteTimer("First clear", async () =>
            {
                var command = _DB.GetCommand("Clear");
                var s = command.ExecuteNonQuery();
                Console.WriteLine($"clear count : {s}");
            });
            await ExecuteTimer("Insert", async () =>
            {
                var command = _DB.GetCommand("Insert");
                var s = command.ExecuteNonQuerys(students, 400);
                Console.WriteLine($"Insert count : {s}");
            });
            await ExecuteTimer("Update", async () =>
            {
                var command = _DB.GetCommand("Update");
                var s = command.ExecuteNonQuerys(students, 400);
                Console.WriteLine($"Update count : {s}");
            });
            await ExecuteTimer("Update2", async () =>
            {
                foreach (var item in students.Take(100))
                {
                    var command = _DB.GetCommand("Update");
                    var s = command.ExecuteNonQuery(item);
                }
            });
            //Console.ReadKey();
        }

        private static async Task Test()
        {
            using (var a = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var count = 500;
                var students = GenerateStudents(count);
                await ExecuteTimer("First clear", async () =>
                {
                    var command = _DB.GetCommand("Clear");
                    var s = command.ExecuteNonQuery();
                    Console.WriteLine($"clear count : {s}");
                });

                await ExecuteTimer("First ExecuteBulkCopyAsync", async () =>
                {
                    var command = _DB.GetCommand("BulkCopy");
                    await command.ExecuteBulkCopyAsync(students);
                });
                await ExecuteTimer("Second clear", async () =>
                {
                    var command = _DB.GetCommand("Clear");
                    var s = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"clear count : {s}");
                });

                await ExecuteTimer("Second ExecuteBulkCopyAsync", async () =>
                {
                    var command = _DB.GetCommand("BulkCopy");
                    await command.ExecuteBulkCopyAsync(students);
                });

                await ExecuteTimer("First ExecuteEntityListAsync", async () =>
                {
                    var command = _DB.GetCommand("SelectAll");
                    var ss = await command.ExecuteEntityListAsync<Student>();
                    Console.WriteLine($"student count {ss.Count}");
                });

                await ExecuteTimer("Second ExecuteEntityListAsync", async () =>
                {
                    var command = _DB.GetCommand("SelectAll");
                    var ss = await command.ExecuteEntityListAsync<Student>();
                    Console.WriteLine($"student count {ss.Count}");
                });

                await ExecuteTimer($"do {count} ExecuteEntityListAsync", async () =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        var command = _DB.GetCommand("SelectAll");
                        await command.ExecuteEntityListAsync<Student>();
                    }
                });

                await ExecuteTimer("First ExecuteEntityAsync", async () =>
                {
                    var command = _DB.GetCommand("SelectByName");
                    var s = await command.ExecuteEntityAsync<Student>(new { Name = "3" });
                    Console.WriteLine($"student name {s.Name}, id {s.Id}");
                });

                await ExecuteTimer("Second ExecuteEntityAsync", async () =>
                {
                    var command = _DB.GetCommand("SelectByName");
                    var s = await command.ExecuteEntityAsync<Student>(new { Name = "3" });
                    Console.WriteLine($"student name {s.Name}, id {s.Id}");
                });

                await ExecuteTimer($"do {count} ExecuteEntityAsync", async () =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        var command = _DB.GetCommand("SelectByName");
                        await command.ExecuteEntityAsync<Student>(new { Name = "3" });
                    }
                });

                await ExecuteTimer("First ExecuteScalarAsync", async () =>
                {
                    var command = _DB.GetCommand("SelectAllAge");
                    var num = await command.ExecuteScalarAsync<int?>();
                    Console.WriteLine($"All Age {num}");
                });

                await ExecuteTimer("Second ExecuteScalarAsync", async () =>
                {
                    var command = _DB.GetCommand("SelectAllAge");
                    var num = await command.ExecuteScalarAsync<int?>();
                    Console.WriteLine($"All Age {num}");
                });

                await ExecuteTimer($"do {count} ExecuteScalarAsync", async () =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        var command = _DB.GetCommand("SelectAllAge");
                        await command.ExecuteScalarAsync<int?>();
                    }
                });

                await ExecuteTimer("Last clear", async () =>
                {
                    var command = _DB.GetCommand("Clear");
                    var s = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"clear count : {s}");
                    a.Complete();
                });
            }
        }

        private static List<Student> GenerateStudents(int count)
        {
            var result = new List<Student>();
            for (int i = 0; i < count; i++)
            {
                result.Add(new Student()
                {
                    Age = i,
                    Id = i + 1,
                    JoinDate = DateTime.UtcNow,
                    Name = i.ToString(),
                    Money = i
                });
            }
            return result;
        }

        private static async Task ExecuteTimer(string info, Func<Task> action)
        {
            var sw = Stopwatch.StartNew();
            await action();
            sw.Stop();
            Console.WriteLine($"{info} : {sw.ElapsedMilliseconds} ms");
        }

        private static void ExecuteTimer(string info, Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            Console.WriteLine($"{info} : {sw.ElapsedMilliseconds} ms");
        }

        private static void Init()
        {
            ExecuteTimer("Init Config", () =>
             {
                 _Provider = new ServiceCollection()
                     .UseDataAccess()
                     .UseDataAccessConfig(Directory.GetCurrentDirectory(), false,null, "db.xml")
                     .BuildServiceProvider();

                 _DB = _Provider.GetService<IDbManager>();
             });
        }
    }
}