﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using WebApplicationDB.lib;
using WebApplicationDB.Models;
using WebApplicationDB.ViewModels;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplicationDB.Controllers
{
    public class UpWeatherDBController : Controller
    {
        // GET: /<controller>/
        IWebHostEnvironment appEnvironment;

        public UpWeatherDBController(IWebHostEnvironment _appEnvironment) => appEnvironment = _appEnvironment;

        [HttpGet]
        public IActionResult AddExcelTable(List<string> statusStrings, List<string> colors)
        {
            if (statusStrings.Count() == 0)
            {
                return View(new AddTableViewModel
                {
                    statusStringList = new List<string> { "Chose one ore more Excel files" },
                    colorList = new List<string> { "" }
                });
            }
            else
            {
                return View(new AddTableViewModel
                {
                    statusStringList = statusStrings,
                    colorList = colors
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> uploadedFiles, [FromServices] WeatherContext db)
        {
            List<string> statusStringList = new List<string>();
            List<string> colorList = new List<string>();
            if (uploadedFiles != null)
            {
                foreach (IFormFile file in uploadedFiles)
                {
                    // Is file excel spreadsheet
                    string ext = System.IO.Path.GetExtension(file.FileName);
                    if (ext != ".xls" && ext != ".xlsx")
                    {
                        statusStringList.Add(file.FileName + " extension is not .xls or .xlsx. File wan't uploaded.");
                        colorList.Add("text-danger");
                        continue;
                    }

                    // Gets file stream
                    MemoryStream fileStream = new MemoryStream();
                    await file.CopyToAsync(fileStream);

                    string formatFilePath = appEnvironment.WebRootPath + "/excelTables/formatedList.xlsx";

                    // Is file formatted
                    ExcelReader parser = new ExcelReader(fileStream, formatFilePath);
                    if (!parser.IsFormatted)
                    {
                        statusStringList.Add(file.FileName + " file's format is incorrect. File wan't uploaded.");
                        colorList.Add("text-danger");
                        continue;
                    }

                    // Parsing
                    bool operationStatus = true;
                    List<WeatherRow> excelRows = parser.GetEntities(ref operationStatus);
                    if (!operationStatus)
                    {
                        statusStringList.Add(
                            "In " + file.FileName + " something wrong with " + (excelRows.Count + 1) +
                            " data row (may be some not nullable cells are null)");
                        colorList.Add("text-danger");
                        continue;
                    }

                    // Adds rows to DB table
                    foreach (WeatherRow row in excelRows)
                        db.WeatherRows.Add(row);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch
                    {
                        statusStringList.Add("In file" + file.FileName + " one or more from uploading rows already exists in DataBase. Files wasn't uploaded");
                        colorList.Add("text-danger");
                        continue;
                    }

                    // Sends operation result
                    statusStringList.Add( file.FileName + " file was uploaded sucsessfully");
                    colorList.Add("text-success");
                }
            }
            else
            {
                statusStringList.Add("Files were not uploaded");
                colorList.Add("text-danger");
            }

            return RedirectToActionPermanent("AddExcelTable", "UpWeatherDB", new
            {
                statusStrings = statusStringList,
                colors = colorList
            });
        }
    }
}
