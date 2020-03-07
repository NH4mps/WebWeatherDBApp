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
                List<Tuple<string, string>> readHeaders = new List<Tuple<string, string>>();

                IWorkbook excelBook;
                FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                if (ext == "xls")
                {
                    excelBook = new HSSFWorkbook(file);
                }
                else
                {
                    excelBook = new XSSFWorkbook(file);
                }

                IRow r3 = excelBook.GetSheetAt(0).GetRow(2);
                IRow r4 = excelBook.GetSheetAt(0).GetRow(3);

                bool flag = true;
                for (int i = 0; i < r3.LastCellNum; i++)
                {
                    readHeaders.Add(new Tuple<string, string>(r3.GetCell(i).ToString(), r4.GetCell(i).ToString()));
                    if(readHeaders.ElementAt(i).Item1 != headers.ElementAt(i).Item1 &&
                       readHeaders.ElementAt(i).Item2 != headers.ElementAt(i).Item2)
                    {
                        flag = false;
                        break;
                    }
                }
                if (readHeaders.Count != headers.Count)
                    return false;

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
            List<WeatherRow> res = new List<WeatherRow>();
            if (!IsFormatted)
                return res;

            IWorkbook excelBook;
            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            if (ext == "xls")
            {
                excelBook = new HSSFWorkbook(file);
            }
            else
            {
                excelBook = new XSSFWorkbook(file);
            }

            ISheet sheet = excelBook.GetSheetAt(0);

            for (int i = 4; i < sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
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
                

            return res;
        }
    }
}
