using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DinkToPdf;
using InsuranceCore.DTO;
using InsuranceCore.Enums;
using InsuranceCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceManagement.Controllers
{
    public class FileController : Controller
    {
        private readonly IInsuranceService _service;
        public FileController(IInsuranceService service)
        {
            _service = service;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ExportData(InsuranceReportQuery filters,DataExportFormat format)
        {
            var filter = await _service.BuildInsuranceReportQuery(filters);

            var data = await _service.MapInsuranceRequestsToInfo(filter);// my data source
             return ExportData(data, format);
        }
        public static FileContentResult ExportData<T>(IEnumerable<T> data, DataExportFormat format)
        {
            switch (format)
            {
                case DataExportFormat.Csv:
                    return ExportToCsv(data);
                case DataExportFormat.Pdf:
                    return ExportToPdf(data);
                case DataExportFormat.Excel:
                    return ExportToExcel(data, $"InsuranceInformation {DateTime.Now}");
                default:
                    throw new ArgumentException("Invalid export format.");
            }
        }

        private static FileContentResult ExportToCsv<T>(IEnumerable<T> data)
        {
            var csvContents = ToCsv(data);
            var fileName = $"export_{DateTime.Now:yyyyMMddHHmmss}.csv";

            return new FileContentResult(Encoding.UTF8.GetBytes(csvContents), "text/csv")
            {
                FileDownloadName = fileName
            };
        }

        private static FileContentResult ExportToPdf<T>(IEnumerable<T> data)
        {
            var converter = new BasicConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
            PaperSize = PaperKind.A4,
            Orientation = Orientation.Landscape,
                },
                Objects = {
                    new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = ToHtmlTable(data), // Convert data to HTML table
                    }
                }
            };

            byte[] pdfBytes = converter.Convert(doc);

            var fileName = $"export_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            return new FileContentResult(pdfBytes, "application/pdf")
            {
                FileDownloadName = fileName
            };
        }

        public static FileContentResult ExportToExcel<T>(IEnumerable<T> data, string fileName)
        {
            // Create a new Excel workbook and worksheet
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Data");

            // Extract the properties from the generic type T
            var properties = typeof(T).GetProperties();

            // Create a list to store the header row
            var headerRow = new List<string>();

            // Populate the header row with property names
            foreach (var property in properties)
            {
                headerRow.Add(property.Name);
            }

            // Write the header row to the Excel worksheet
            for (int i = 0; i < headerRow.Count; i++)
            {
                worksheet.Cell(1, i + 1).Value = headerRow[i];
            }

            // Create a list to store data rows
            var dataRows = new List<string[]>();

            // Populate data rows with property values for each item in the data collection
            foreach (var item in data)
            {
                var rowData = properties.Select(p => p.GetValue(item)?.ToString()).ToArray();
                dataRows.Add(rowData);
            }

            // Write the data rows to the Excel worksheet
            for (int rowIndex = 0; rowIndex < dataRows.Count; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < dataRows[rowIndex].Length; columnIndex++)
                {
                    worksheet.Cell(rowIndex + 2, columnIndex + 1).Value = dataRows[rowIndex][columnIndex];
                }
            }

            // Create a memory stream to save the Excel workbook
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);

                // Return the Excel file as a FileContentResult with the specified file name
                return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = fileName
                };
            }
        }



        private static string ToCsv<T>(IEnumerable<T> data)
        {
            var csv = new StringBuilder();
            var properties = typeof(T).GetProperties();

            // Write header row
            var headerRow = string.Join(",", properties.Select(p => p.Name));
            csv.AppendLine(headerRow);

            // Write data rows
            foreach (var item in data)
            {
                var dataRow = string.Join(",", properties.Select(p => p.GetValue(item)?.ToString()));
                csv.AppendLine(dataRow);
            }

            return csv.ToString();
        }
        private static string ToHtmlTable<T>(IEnumerable<T> data)
        {
            var sb = new StringBuilder();
            sb.Append("<table>");

            // Add table headers
            sb.Append("<tr>");
            foreach (var prop in typeof(T).GetProperties())
            {
                sb.AppendFormat("<th>{0}</th>", prop.Name);
            }
            sb.Append("</tr>");

            // Add data rows
            foreach (var item in data)
            {
                sb.Append("<tr>");
                foreach (var prop in typeof(T).GetProperties())
                {
                    var value = prop.GetValue(item);
                    sb.AppendFormat("<td>{0}</td>", value);
                }
                sb.Append("</tr>");
            }

            sb.Append("</table>");
            return sb.ToString();
        }

    }


}