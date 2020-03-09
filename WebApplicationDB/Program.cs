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
            //using (WeatherContext db = new WeatherContext())
            //{
            //    // создаем два объекта User
            //    WeatherRow ent1 = new WeatherRow
            //    {
            //        Id = DateTime.Parse("12.01.2001 18:00"),
            //        T = (float)-5.5,
            //        Humidity = 90,
            //        Td = (float)-11.2,
            //        Pressure = 753,
            //        WindDir ="C"
            //    };
            //    WeatherRow ent2 = new WeatherRow
            //    {
            //        Id = DateTime.Parse("13.01.2001 18:00"),
            //        T = (float)-5.5,
            //        Humidity = 90,
            //        Td = (float)-11.2,
            //        Pressure = 753,
            //        WindDir = "C"
            //    };

            //    // добавляем их в бд
            //    db.WeatherRows.Add(ent1);
            //    db.WeatherRows.Add(ent2);
            //    db.SaveChanges();
            //    Console.WriteLine("Объекты успешно сохранены");

            //    // получаем объекты из бд и выводим на консоль
            //    var rows = db.WeatherRows.ToList();
            //    Console.WriteLine("Список объектов:");
            //    foreach (WeatherRow w in rows)
            //    {
            //        Console.WriteLine($"{w.Id}.{w.T} - {w.Humidity}");
            //    }
            //}
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
