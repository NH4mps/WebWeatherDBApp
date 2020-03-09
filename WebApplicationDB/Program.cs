using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplicationDB.Models;

namespace WebApplicationDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (WeatherContext db = new WeatherContext())
            {
                var allRows = db.WeatherRows;
                db.WeatherRows.RemoveRange(allRows);
                db.SaveChanges();
                Console.WriteLine("Объекты успешно удалены");
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
