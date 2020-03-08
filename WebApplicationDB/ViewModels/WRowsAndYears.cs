using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationDB.Models;

namespace WebApplicationDB.ViewModels
{
    public class WRowsAndYears
    {
        public List<WeatherRow> WeatherRows { get; set; }
        public List<YearWithMonths> YearsWithMonths { get; set; }
        public int? CurrentYear { get; set; }
        public int? CurrentMonth { get; set; }
    }
}
