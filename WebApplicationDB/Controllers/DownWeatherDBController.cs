using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplicationDB.Models;
using WebApplicationDB.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationDB.Controllers
{
    public class DownWeatherDBController : Controller
    {
        private WeatherContext db;
        private List<YearWithMonths> yearsFromDB;

        public DownWeatherDBController(WeatherContext _db) 
        {
            db = _db;
            yearsFromDB = new List<YearWithMonths>();

            IQueryable <WeatherRow> source = db.WeatherRows;
            var years = source.Select(wr => wr.Id.Year).Distinct().ToList();
            foreach (int year in years)
            {
                List<int> months = source.Where(wr => wr.Id.Year == year).Select(wr => wr.Id.Month).Distinct().ToList();
                yearsFromDB.Add(new YearWithMonths
                {
                    Year = year,
                    Months = months
                });
            }
        }

        public async Task<IActionResult> ShowDBTable(int? currentYear, int? currentMonth, int pageNum = 1)
        {
            int pageSize = 15 ;
            IQueryable<WeatherRow> queryItems = db.WeatherRows;
            
            if (currentYear == null && currentMonth != null)
                ViewBag.Message = "Chose year!";
            else
            {
                if (currentYear != null)
                    queryItems = queryItems.Where(wr => wr.Id.Year == currentYear);
                if (currentMonth != null)
                    queryItems = queryItems.Where(wr => wr.Id.Month == currentMonth);
            }

            var count = await queryItems.CountAsync();
            var pageItems = await queryItems.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();

            WRowsAndYears data = new WRowsAndYears
            {
                WeatherRows = pageItems,
                YearsWithMonths = yearsFromDB,
                CurrentYear = currentYear,
                CurrentMonth = currentMonth
            };

            PageViewModel pages = new PageViewModel(count, pageNum, pageSize);

            return View(new ShowDBViewModel
            {
                Data = data,
                Pages = pages
            });
        }
    }
}