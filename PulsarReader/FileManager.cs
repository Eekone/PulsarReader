using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PulsarReaded
{
    public static class FileManager
    {
        public static bool Make(string FileName, List<Record> records, string type)
        {           
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"Export\");
                   
            switch (type)
            {
                case ".csv": return makeCSV(FileName, records);
                case ".xlsx": return makeXLSX(FileName, records);
                default: return false;
            }                
        }

        private static bool makeCSV(string FileName, List<Record> records)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"Export\" + FileName + ".csv";
            if (File.Exists(path))            
                File.Delete(path);
           
            File.Create(path).Dispose();
            TextWriter tw = new StreamWriter(path, false, Encoding.GetEncoding(1251));
            for (int i = 0; i < records.Count; i++)
            {
                string line = string.Format("{0},{1},{2},{3},{4},{5},{6}\n",
                    i+1,
                    records[i].Date, 
                    records[i].StationNumber, 
                    records[i].StationName,
                    records[i].Parameter, 
                    records[i].Value.ToString().Replace(",","."), 
                    records[i].Unit);
                tw.WriteLine(line);
            }
            tw.Close();            
           
            return false;
        }

        private static bool makeXLSX(string FileName, List<Record> records)
        {           
            FileInfo newFile = new FileInfo(AppDomain.CurrentDomain.BaseDirectory+@"Export\" + FileName + ".xlsx");
            try
            { 
                if (newFile.Exists) newFile.Delete(); 

                using (var package = new ExcelPackage(newFile))
                {               
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("data");
                    for(int i=0; i < records.Count; i++)
                    {
                        worksheet.Cells[i + 1, 1].Value = i+1;
                        worksheet.Cells[i+1, 2].Value = records[i].Date;
                        worksheet.Cells[i+1, 3].Value = records[i].StationNumber;
                        worksheet.Cells[i + 1, 4].Value = records[i].StationName;
                        worksheet.Cells[i+1, 5].Value = records[i].Parameter;
                        worksheet.Cells[i+1, 6].Value = records[i].Value;
                        worksheet.Cells[i+1, 7].Value = records[i].Unit;
                    }            
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    package.Save();              
                }
                return true;
            }
            catch (Exception ex)  { return false; }       
        }
    }
}
