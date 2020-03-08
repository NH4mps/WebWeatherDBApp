using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationDB.Controllers
{
    public class DownWeatherDBController : Controller
    {
        public IActionResult ShowDBTable()
        {
            return View();
        }
    }
}