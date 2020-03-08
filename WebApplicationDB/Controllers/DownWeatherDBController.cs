using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplicationDB.Models;
using WebApplicationDB.ViewModels;

namespace WebApplicationDB.Controllers
{
    public class DownWeatherDBController : Controller
    {
        private List<WeatherRow> DataFromDB;
        private List<YearWithMonths> YearsFromDB;

        public DownWeatherDBController()
        {
            WeatherContext dbContext = new WeatherContext();
            DataFromDB = dbContext.WeatherRows.ToList();
        }

        [HttpGet]
        public IActionResult ShowDBTable(List<WeatherRow> selectedRows, int? currentYear, int? currentMonth)
        {
            if (selectedRows == null)
                return View(new WRowsAndYears {
                    WeatherRows = DataFromDB,
                    YearsWithMonths = YearsFromDB
                });
            else
                // Here's 
                return View(new WRowsAndYears {
                    WeatherRows = selectedRows, 
                    YearsWithMonths = YearsFromDB,
                    CurrentYear = currentYear,
                    CurrentMonth = currentMonth
                });
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