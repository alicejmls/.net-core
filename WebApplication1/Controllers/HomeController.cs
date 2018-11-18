using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using WebApplication1.Codes;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            string DefaultConnection = configuration.GetSection("ConnectionStrings")["DefaultConnection"];

            string message = "";
            try
            {
                var redisClient = RedisClientSingleton.GetInstance(configuration);
                var db = redisClient.GetDatabase("Redis_Default");

                if (db.KeyExists("Message"))
                    message = "Redis:" + db.StringGet("Message");
                else
                {
                    message = "本周更新下一篇 最近还在写其他的";
                    db.StringSet("Message", message);
                    db.KeyExpire("Message", DateTime.Now.AddSeconds(30));
                }


            }
            catch (Exception ex)
            {
                message = ex.Message + "\r\n" + ex.StackTrace;
            }

            List<TestTableVIewModel> lists = new List<TestTableVIewModel>();
            TestTableVIewModel entity = null;

            using (IDbConnection conn = new MySqlConnection(DefaultConnection))
            {
                conn.Open();
                entity = conn.Query<TestTableVIewModel>("SELECT * FROM TestTable where id=@Id", new { @Id = 1 }).FirstOrDefault();
             
                lists = conn.Query<TestTableVIewModel>("SELECT * FROM TestTable ").AsList();

            }

            ViewBag.Test = entity;
            ViewBag.Ulist = lists;

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
