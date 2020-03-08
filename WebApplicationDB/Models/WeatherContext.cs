using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationDB.Models
{
    public class WeatherContext : DbContext
    {
        public DbSet<WeatherRow> WeatherRows { get; set; }

        public WeatherContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WeatherDB;Trusted_Connection=True;");
        }
    }
}
