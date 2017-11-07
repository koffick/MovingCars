using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MovingCars.Controllers
{
    public class ReportController : Controller
    {
        protected StorageContext db;

        public ReportController()
        {
            this.db = new StorageContext();
        }

        // GET: Report
        public ActionResult Index()
        {
            var entyties = this.db.Drivers.Select(s => new { Id = s.Id, Name = s.LastName + " " + s.FirstName + " " + s.Patronymic });
            ViewBag.Drivers = new SelectList(entyties, "Id", "Name");
            return View();
        }

        public FileResult Export()
        {
            MemoryStream stream = CreateMemoryStreamFromFile();
            OpenAndAddToSpreadsheetStream(stream);
            string file_type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string file_name = "Result.xlsx";
            return File(stream, file_type, file_name);
        }

        private static void OpenAndAddToSpreadsheetStream(MemoryStream stream)
        {
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(stream, true);
            // Add a new worksheet.
            WorksheetPart newWorksheetPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = spreadsheetDocument.WorkbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new worksheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Count() > 0)
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            // Give the new worksheet a name.
            string sheetName = "Sheet" + sheetId;

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            spreadsheetDocument.WorkbookPart.Workbook.Save();

            // Close the document handle.
            spreadsheetDocument.Close();

            stream.Seek(0, SeekOrigin.Begin);
        }

        private MemoryStream CreateMemoryStreamFromFile()
        {
            string path = Server.MapPath("~/Content/Documents/Шаблон учета времени.xlsx");
            FileStream fileStream = new FileStream(path, FileMode.Open);
            MemoryStream stream = new MemoryStream();
            fileStream.CopyTo(stream);
            fileStream.Close();
            return stream;
        }

    }
}