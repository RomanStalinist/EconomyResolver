using CsvHelper;
using EconomyResolver.BusinessLogic.Enums;
using Microsoft.Win32;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace EconomyResolver.BusinessLogic
{
    public static class МенеджерИмпорта
    {
        public static IList<Показатель> Импортировать()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv|XML файлы (*.xml)|*.xml|JSON файлы (*.json)|*.json|Excel файлы (*.xlsx)|*.xlsx"
            };

            if (dialog.ShowDialog() == true)
            {
                switch (Path.GetExtension(dialog.FileName).ToLower())
                {
                    case ".csv":
                        return ImportCsv(dialog.FileName);
                    case ".xml":
                        return ImportXml(dialog.FileName);
                    case ".json":
                        return ImportJson(dialog.FileName);
                    case ".xlsx":
                        return ImportXlsx(dialog.FileName);
                }
            }

            return null;
        }

        private static IList<Показатель> ImportCsv(string filePath)
        {
            var dataCollection = new List<Показатель>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<CsvПоказатель>().ToList();
                foreach (var item in records)
                {
                    var unit = ParseUnit(item.ЕдиницаИзмерения);
                    dataCollection.Add(Показатель.Новый(item.Наименование, item.Значение, unit));
                }
            }

            return dataCollection;
        }

        private static IList<Показатель> ImportXml(string filePath)
        {
            var dataCollection = new List<Показатель>();

            var xmlData = XDocument.Load(filePath);
            var data = xmlData.Root.Elements("magnitude")
                .Select(x => new Показатель
                {
                    Наименование = (string)x.Element("name"),
                    Значение = (double)x.Element("value"),
                    ЕдиницаИзмерения = ParseUnit((string)x.Element("unit"))
                })
                .ToList();

            dataCollection.AddRange(data);

            return dataCollection;
        }

        private static IList<Показатель> ImportJson(string filePath)
        {
            var dataCollection = new List<Показатель>();

            var jsonData = File.ReadAllText(filePath);
            var data = JsonConvert.DeserializeObject<List<JsonПоказатель>>(jsonData)
                .Select(item => new Показатель(item.Name, item.Value, ParseUnit(item.Unit)))
                .ToList();

            dataCollection.AddRange(data);

            return dataCollection;
        }

        private static IList<Показатель> ImportXlsx(string filePath)
        {
            var dataCollection = new List<Показатель>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    var name = worksheet.Cells[row, 1].Text;
                    if (double.TryParse(worksheet.Cells[row, 2].Text, out double value))
                    {
                        var unit = ParseUnit(worksheet.Cells[row, 3].Text);
                        dataCollection.Add(Показатель.Новый(name, value, unit));
                    }
                }
            }

            return dataCollection;
        }

        private static ЕдиницаИзмерения ParseUnit(string unitDescription)
        {
            return Enum.GetValues(typeof(ЕдиницаИзмерения))
                       .Cast<ЕдиницаИзмерения>()
                       .FirstOrDefault(u => u.ПолучитьОписание() == unitDescription);
        }
    }
}