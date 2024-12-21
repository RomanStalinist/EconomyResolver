using CsvHelper;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using EconomyResolver.BusinessLogic.Enums;
using Microsoft.Win32;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;

namespace EconomyResolver.BusinessLogic
{
    public class МенеджерЭкспорта
    {
        private readonly IEnumerable<Показатель> СписокПоказателей;

        public МенеджерЭкспорта(IEnumerable<Показатель> списокПоказателей)
            => СписокПоказателей = списокПоказателей;

        public bool Экспортировать()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSV файл с разделителем (*.csv)|*.csv|XML файл (*.xml)|*.xml|JSON файл (*.json)|*.json|Excel файл (*.xlsx)|*.xlsx|Word файл (*.docx)|*.docx"
            };

            if (dialog.ShowDialog() == true)
            {
                switch (Path.GetExtension(dialog.FileName).ToLower())
                {
                    case ".csv":
                        ExportCsv(dialog.FileName);
                        break;
                    case ".xml":
                        ExportXml(dialog.FileName);
                        break;
                    case ".json":
                        ExportJson(dialog.FileName);
                        break;
                    case ".xlsx":
                        ExportXlsx(dialog.FileName);
                        break;
                    case ".docx":
                        ExportDocx(dialog.FileName);
                        break;
                }

                return true;
            }

            return false;
        }

        private void ExportCsv(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(СписокПоказателей.Select(item => new
                {
                    item.Наименование,
                    Значение = item.ЕдиницаИзмерения is ЕдиницаИзмерения.Проценты ? item.Значение * 100 : item.Значение,
                    ЕдиницаИзмерения = item.ЕдиницаИзмерения.ПолучитьОписание()
                }));
            }
        }

        private void ExportXml(string filePath)
        {
            var xmlData = new XDocument(
                new XElement("collection",
                    СписокПоказателей.Select(x => new XElement("magnitude",
                        new XElement("name", x.Наименование),
                        new XElement("value", x.ЕдиницаИзмерения is ЕдиницаИзмерения.Проценты ? x.Значение * 100 : x.Значение),
                        new XElement("unit", x.ЕдиницаИзмерения.ПолучитьОписание())
                    ))
                )
            );

            xmlData.Save(filePath);
        }

        private void ExportJson(string filePath)
        {
            var jsonData = JsonConvert.SerializeObject(СписокПоказателей.Select(item => new
            {
                name = item.Наименование,
                value = item.ЕдиницаИзмерения is ЕдиницаИзмерения.Проценты ? item.Значение * 100 : item.Значение,
                unit = item.ЕдиницаИзмерения.ПолучитьОписание()
            }), Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }

        private void ExportXlsx(string filePath)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Экономический расчёт");
                worksheet.Cells[1, 1].Value = "Наименование";
                worksheet.Cells[1, 2].Value = "Значение";
                worksheet.Cells[1, 3].Value = "Единица измерения";

                int row = 2;
                foreach (var item in СписокПоказателей)
                {
                    worksheet.Cells[row, 1].Value = item.Наименование;
                    worksheet.Cells[row, 2].Value = item.ЕдиницаИзмерения is ЕдиницаИзмерения.Проценты ? item.Значение * 100 : item.Значение;
                    worksheet.Cells[row, 3].Value = item.ЕдиницаИзмерения.ПолучитьОписание();
                    row++;
                }

                package.SaveAs(new FileInfo(filePath));
            }
        }

        private void ExportDocx(string filePath)
        {
            using (var document = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
            {
                var body = new Body();
                var mainPart = document.AddMainDocumentPart();

                mainPart.Document = new Document();
                mainPart.Document.Append(body);

                // Создаем таблицу
                var table = new Table();

                // Устанавливаем ширину столбцов (по желанию)
                table.Append(new TableProperties(
                    new TableWidth { Width = "5000", Type = TableWidthUnitValues.Pct }));

                // Заголовок таблицы
                var headerRow = new TableRow();
                headerRow.Append(CreateDocxTableCell("Наименование"));
                headerRow.Append(CreateDocxTableCell("Значение"));
                headerRow.Append(CreateDocxTableCell("Единица измерения"));

                table.Append(headerRow);

                // Добавляем данные в таблицу
                foreach (var item in СписокПоказателей)
                {
                    var dataRow = new TableRow();
                    dataRow.Append(CreateDocxTableCell(item.Наименование));
                    dataRow.Append(CreateDocxTableCell((item.ЕдиницаИзмерения is ЕдиницаИзмерения.Проценты ? item.Значение * 100 : item.Значение).ToString()));
                    dataRow.Append(CreateDocxTableCell(item.ЕдиницаИзмерения.ПолучитьОписание()));
                    table.Append(dataRow);
                }

                body.Append(table);
                mainPart.Document.Save();
            }
        }

        private TableCell CreateDocxTableCell(string text)
        {
            var cell = new TableCell();

            // Создание Run с заданным стилем
            var run = new Run(new Text(text))
            {
                RunProperties = new RunProperties(
                    new Justification { Val = JustificationValues.Center },
                    new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                    new FontSize { Val = "24" }) // Размер шрифта 12pt (24 half-points)
            };

            var paragraph = new Paragraph(run);
            cell.Append(paragraph);

            cell.Append(new TableCellProperties(
                new TableCellBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 })));

            return cell;
        }
    }
}