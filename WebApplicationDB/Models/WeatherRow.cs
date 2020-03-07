using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplicationDB.Models
{
    public class WeatherRow
    {
        public DateTime Id { get; set; }
        public float T { get; set; }
        public float Humidity { get; set; }
        public float Td { get; set; }
        public int Pressure { get; set; }
        public string WindDir { get; set; }
        public int? WindSpeed { get; set; }
        public int? Cloudy { get; set; }
        public int? H { get; set; }
        public int? VV { get; set; }
        public string WeatherConds { get; set; }
    }
}
