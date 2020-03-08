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

        public DownWeatherDBController() 
        {
            db = new WeatherContext();
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

        [HttpGet]
        public async Task<IActionResult> ShowDBTable(int? currentYear, int? currentMonth, int pageNum = 1)
        {
            int pageSize = 15;
            Console.WriteLine("JOPA BLYAT");
            if (currentYear == null && currentMonth == null)
            {
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
            else if (currentMonth == null)
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
            else
            {
                IQueryable<WeatherRow> source = db.WeatherRows;
                var items = source.Where(wr => wr.Id.Year == currentYear).Where(wr => wr.Id.Month == currentMonth);
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

        [HttpPost]
        public IActionResult SelectRows(int _Year, int? _Month)
        {
            WeatherContext dbContext = new WeatherContext();
            List<WeatherRow> rowsByYearNMonth = dbContext.WeatherRows.ToList();
            // Here's row selection
            //
            //
            //
            return RedirectToActionPermanent("ShowDBTable", "DownWeatherDB", new { selectedRows = rowsByYearNMonth });
        }
    }
}