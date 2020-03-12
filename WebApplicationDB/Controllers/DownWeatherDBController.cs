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
            if (currentYear == null && currentMonth == null)
            {
                IQueryable<WeatherRow> source = db.WeatherRows;
                var count = await source.CountAsync();
                var items = await source.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();
                if (items.Count() == 0)
                {
                    ViewBag.Message = "No rows for forecast in DB";
                }
                WRowsAndYears data = new WRowsAndYears
                {
                    WeatherRows = items,
                    YearsWithMonths = yearsFromDB
                };
                PageViewModel pages = new PageViewModel(count, pageNum, pageSize);
                return View(new ShowDBViewModel
                {
                    Data = data,
                    Pages = pages
                });
            }
            else if (currentYear != null && currentMonth == null)
            {
                IQueryable<WeatherRow> source = db.WeatherRows;
                var items = source.Where(wr => wr.Id.Year == currentYear);
                var count = await items.CountAsync();
                var itemsPage = await items.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();
                WRowsAndYears data = new WRowsAndYears
                {
                    WeatherRows = itemsPage,
                    YearsWithMonths = yearsFromDB,
                    CurrentYear = currentYear
                };
                PageViewModel pages = new PageViewModel(count, pageNum, pageSize);
                return View(new ShowDBViewModel
                {
                    Data = data,
                    Pages = pages
                });
            }
            else if (currentYear == null && currentMonth != null)
            {
                ViewBag.Message = "Chose year!";
                IQueryable<WeatherRow> source = db.WeatherRows;
                var count = await source.CountAsync();
                var items = await source.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();
                WRowsAndYears data = new WRowsAndYears
                {
                    WeatherRows = items,
                    YearsWithMonths = yearsFromDB
                };
                PageViewModel pages = new PageViewModel(count, pageNum, pageSize);
                return View(new ShowDBViewModel
                {
                    Data = data,
                    Pages = pages
                });
            }
            else
            {
                IQueryable<WeatherRow> source = db.WeatherRows;
                var items = source.Where(wr => wr.Id.Year == currentYear).Where(wr => wr.Id.Month == currentMonth);
                if (items.Count() == 0)
                {
                    ViewBag.Message = "No rows for forecast in chosen month";
                }
                var count = await items.CountAsync();
                var itemsPage = await items.Skip((pageNum - 1) * pageSize).Take(pageSize).ToListAsync();
                WRowsAndYears data = new WRowsAndYears
                {
                    WeatherRows = itemsPage,
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
}