using ExtensionMethods;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PulsarReaded
{

    public class ParsekParcer
    {
        List<Datchick> AvailableDatchiks = new List<Datchick>();
        List<KP> AvailableKPs = new List<KP>();
        public ParsekParcer(string DatchikInfoFileName, string KPinfoFileName)
        {
            InitializeDatchiks(DatchikInfoFileName);
            InitializeKPs(KPinfoFileName);
        }
        private void InitializeKPs(string fileName)
        {
            Regex regex = new Regex(@"№([0-9]+?):\s(.*)");
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName;

            StreamReader file =
                new StreamReader(path, Encoding.GetEncoding(1251));
            for (int count = 0; count < File.ReadAllLines(path).Count(); count++)
            {
                KP kp = new KP();
                string line= file.ReadLine();
                string test = regex.Match(line).Groups[1].Value;
                kp.number = Convert.ToByte(regex.Match(line).Groups[1].Value);
                int ooo = kp.number;
                kp.Name = regex.Match(line).Groups[2].Value;
                AvailableKPs.Add(kp);
            }
        }
        private void InitializeDatchiks(string fileName)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName;

            StreamReader file =
                new StreamReader(path, Encoding.GetEncoding(1251));
            for (int count = 0; count < File.ReadAllLines(path).Count() / 7; count++)
            {
                Datchick datchick = new Datchick();
                datchick.Name = file.ReadLine();
                datchick.Unit = file.ReadLine();
                datchick.Number = int.Parse(file.ReadLine());
                datchick.MinValue = float.Parse(file.ReadLine().Replace(".", ","));
                datchick.MaxValue = float.Parse(file.ReadLine().Replace(".", ","));
                datchick.MinCode = byte.Parse(file.ReadLine());
                datchick.MaxCode = byte.Parse(file.ReadLine());
                AvailableDatchiks.Add(datchick);
            }
        }

        public List<Record> ParseFile(string filePath)
        {
            List<Record> Records = new List<Record>();
            byte[] fileBytes = File.ReadAllBytes(filePath);
            byte[] dataBytes = fileBytes.SubArray(4, fileBytes.Length - 4);

            Regex regex = new Regex(@"[0-9]{4}[0-9ABCDEF]{4}");
            string fileName = regex.Match(filePath).Value;

            while (dataBytes.Length > 0)
            {
                for (int i = 0; i < dataBytes[4]; i++)
                {
                    Record r = new Record();
                    r.Date = Year(fileName) + "-" +Month(fileName)+"-"+Day(dataBytes[0])+" "                         
                            + Hour(dataBytes[1]) + ":" + Minute(dataBytes[2]) + ":00";
                    r.Parameter = AvailableDatchiks[dataBytes[i * 3 + 6]].Name;
                    r.StationNumber = Convert.ToByte(fileName.Substring(4, 2), 16);
                    r.StationName = StationName(r.StationNumber);
                    r.Value = DatchikValue(dataBytes[i * 3 + 5], dataBytes[i * 3 + 6]);
                    r.Unit = AvailableDatchiks[dataBytes[i * 3 + 6]].Unit;
                    Records.Add(r);
                }
                dataBytes = dataBytes.SubArray(5 + dataBytes[4] * 3, dataBytes.Length - (5 + dataBytes[4] * 3));
            }
            return Records;
        }
        private string StationName(byte Number)
        {
            foreach(KP kp in AvailableKPs)
            {
                if (kp.number == Number) return kp.Name;
            }
            return "";
        }
        private string Year(string name)
        {
            return (int.Parse(name.Substring(2, 2)) > 50 ? int.Parse(name.Substring(2, 2)) + 1900 : int.Parse(name.Substring(2, 2)) + 2000).ToString();
        }
        private string Month(string name)
        {
            return name.Substring(0, 2);
        }
        private string Day(byte data)
        {
            return (data < 10 ? ("0" + data.ToString()) : (data.ToString()));
        }
        private string Hour(byte data)
        {
            return (data < 10 ? ("0" + data.ToString()) : (data.ToString()));
        }
        private string Minute(byte data)
        {
            return (data < 10 ? ("0" + data.ToString()) : (data.ToString()));
        }
        private string getYearMonth(string name)
        {
            int year = int.Parse(name.Substring(2, 2)) > 50 ? int.Parse(name.Substring(2, 2)) + 1900 : int.Parse(name.Substring(2, 2)) + 2000;
            return year + "-" + name.Substring(0, 2);
        }
        private float DatchikValue(byte value, byte type)
        {
            Datchick d = AvailableDatchiks[type];

            float sh = ((float)(value - d.MinCode) / (float)(d.MaxCode - d.MinCode));
            return d.MinValue + sh * (d.MaxValue - d.MinValue);
        }
    }
}
