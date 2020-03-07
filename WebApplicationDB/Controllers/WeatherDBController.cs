using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using WebApplicationDB.lib;
using WebApplicationDB.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplicationDB.Controllers
{
    public class WeatherDBController : Controller
    {
        // GET: /<controller>/
        IWebHostEnvironment _appEnvironment;

        public WeatherDBController(IWebHostEnvironment appEnvironment) => _appEnvironment = appEnvironment;

        [HttpGet]
        public IActionResult AddExcelTable(string statusstring, bool status)
        {
            if (statusstring == null)
            {
                ViewBag.StatusString = "Chose file, then click submit button";
                ViewBag.Color = "";
            }
            else
            {
                ViewBag.StatusString = statusstring;
                if (status == true)
                    ViewBag.Color = "text-success";
                else
                    ViewBag.Color = "text-danger";
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                // Is file excel spreadsheet
                string ext = System.IO.Path.GetExtension(uploadedFile.FileName);
                if (ext != ".xls" && ext != ".xlsx")
                    return RedirectToActionPermanent("AddExcelTable", "WeatherDB", new
                    {
                        statusstring = "File extension is not .xls or .xlsx",
                        status = false
                    });

                // Uploads to rhe root and gets file stream
                string filePath = _appEnvironment.WebRootPath + "/excelTables/" + uploadedFile.FileName;

                using (var stream = System.IO.File.Create(filePath))
                {
                    await uploadedFile.CopyToAsync(stream);
                }

                // Is file formatted
                ExcelReader parser = new ExcelReader(filePath, _appEnvironment);
                if (!parser.IsFormatted)
                    return RedirectToActionPermanent("AddExcelTable", "WeatherDB", new
                    {
                        statusstring = "File's format is incorrect",
                        status = false
                    });

                // Parsing
                List<WeatherRow> excelRows = parser.GetEntities();

                // Is not empty
                if (excelRows.Count == 0)
                    return RedirectToActionPermanent("AddExcelTable", "WeatherDB", new
                    {
                        statusstring = "File has no rows that could be read (may be some not nullable params are null)",
                        status = false
                    });

                // Sends operation result
                return RedirectToActionPermanent("AddExcelTable", "WeatherDB", new
                {
                    statusstring = uploadedFile.FileName + " was uploaded sucsessfully",
                    status = true
                });
            }
            else
                return RedirectToActionPermanent("AddExcelTable", "WeatherDB", new
                {
                    statusstring =  "Data wasn't uploaded",
                    status = false
                });
        }

        public IActionResult ShowDBTable()
        {
            return View();
        }
    }
}
