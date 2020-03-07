using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using WebApplicationDB.Models;

namespace WebApplicationDB.lib
{

    public class ExcelReader
    {
        private string filePath;
        private string ext;
        private readonly List<Tuple<string, string>> headers;

        public ExcelReader(string _filePath, IWebHostEnvironment _appEnvironment)
        {
            // Fields initialization
            filePath = _filePath;
            ext = Path.GetExtension(filePath);

            // Intitalizes format headers
            string formatFilePath = _appEnvironment.WebRootPath + "/excelTables/formatedList.xlsx";
            FileStream formatFile = new FileStream(formatFilePath, FileMode.Open, FileAccess.Read);
            IWorkbook excelFormatFile = new XSSFWorkbook(formatFile);

            // Reads 3rd and 4th rows from sample 
            headers = new List<Tuple<string, string>>();
            for (int i = 0; i < excelFormatFile.GetSheetAt(0).GetRow(2).LastCellNum; i++)
                headers.Add(new Tuple<string, string>(
                    excelFormatFile.GetSheetAt(0).GetRow(2).GetCell(i).ToString(),
                    excelFormatFile.GetSheetAt(0).GetRow(3).GetCell(i).ToString()));
        }

        public bool IsFormatted
        { 
            get
            {
                // Inintializes buffer for headers
                List<Tuple<string, string>> readHeaders = new List<Tuple<string, string>>();

                // Initializes Excel workbook
                IWorkbook excelBook;
                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                if (ext == "xls")
                    excelBook = new HSSFWorkbook(file);
                else
                    excelBook = new XSSFWorkbook(file);

                // For each sheet in Excel file checks if 3rd and 4th rows are foramtted
                bool flag = true;
                for (int i = 0; i < excelBook.NumberOfSheets; i++)
                {
                    IRow r3 = excelBook.GetSheetAt(i).GetRow(2);
                    IRow r4 = excelBook.GetSheetAt(i).GetRow(3);
                    if(r3.LastCellNum != headers.Count ||
                       r4.LastCellNum != headers.Count)
                    {
                        flag = false;
                        return flag;
                    }
                    for (int j = 0; j < r3.LastCellNum; j++)
                    {
                        readHeaders.Add(new Tuple<string, string>(r3.GetCell(j).ToString(), r4.GetCell(j).ToString()));
                        if (readHeaders.ElementAt(j).Item1 != headers.ElementAt(j).Item1 &&
                           readHeaders.ElementAt(j).Item2 != headers.ElementAt(j).Item2)
                        {
                            flag = false;
                            return flag;
                        }
                    }
                    readHeaders.Clear();
                }

                return flag;
            }
        }

        public static int? nullableIntParse(string number)
        {
            int res;
            if (int.TryParse(number, out res))
                return res;
            else
                return null;
        }

        public List<WeatherRow> GetEntities()
        {
            // Initializes result list
            List<WeatherRow> res = new List<WeatherRow>();

            // Is file formated
            if (!IsFormatted)
                return res;

            // Initializes Excel workbook
            IWorkbook excelBook;
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            if (ext == "xls")
                excelBook = new HSSFWorkbook(file);
            else
                excelBook = new XSSFWorkbook(file);

            //Gets list of weather etnities
            for(int i = 0; i < excelBook.NumberOfSheets; i++)
            {
                ISheet sheet = excelBook.GetSheetAt(i);
                for (int j = 4; j < sheet.LastRowNum; j++)
                {
                    IRow row = sheet.GetRow(j);
                    res.Add(new WeatherRow
                    {
                        Id = DateTime.Parse(row.GetCell(0).ToString() + " " + row.GetCell(1).ToString()),
                        T = float.Parse(row.GetCell(2).ToString()),
                        Humidity = float.Parse(row.GetCell(3).ToString()),
                        Td = float.Parse(row.GetCell(4).ToString()),
                        Pressure = int.Parse(row.GetCell(5).ToString()),
                        WindDir = row.GetCell(6)?.ToString(),
                        WindSpeed = nullableIntParse(row.GetCell(7).ToString()),
                        Cloudy = nullableIntParse(row.GetCell(8).ToString()),
                        H = nullableIntParse(row.GetCell(9).ToString()),
                        VV = nullableIntParse(row.GetCell(10).ToString()),
                        WeatherConds = row.GetCell(11)?.ToString()
                    });
                }
            }

            return res;
        }
    }
}
