using System.Data;
using System.Data.SqlClient;
using AddressBook.Models;
using AddressBook.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.IO;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.Kernel.Font;


namespace AddressBook.Controllers
{
    [CheckAccess]
    public class CountryController : Controller
    {
        private readonly DropDownService _dropdownService;
        private readonly IConfiguration _configuration;

        public CountryController(IConfiguration configuration, DropDownService dropdownService)
        {
            _configuration = configuration;
            _dropdownService = dropdownService;
        }

        public IActionResult ExportToPDF()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(stream);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Create a bold font object
                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // Title with bold font
                Paragraph title = new Paragraph()
                    .Add(new Text("Country Data").SetFont(boldFont)) // ✅ Correct way to set bold font
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER);

                document.Add(title);

                // Create Table with 3 columns
                Table table = new Table(4)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER) // Center the whole table
                    .SetWidth(UnitValue.CreatePercentValue(80)); // Set width to 80% of the page

                // Add Table Header (Make headers bold)
                table.AddCell(new Cell().Add(new Paragraph("Sr.No").SetFont(boldFont)));
                table.AddCell(new Cell().Add(new Paragraph("CountryName").SetFont(boldFont)));
                table.AddCell(new Cell().Add(new Paragraph("CountryCode").SetFont(boldFont)));
                table.AddCell(new Cell().Add(new Paragraph("UserName").SetFont(boldFont)));


                string connectionString = _configuration.GetConnectionString("ConnectionString");
                SqlConnection sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                sqlCommand.CommandText = "PR_Country_SelectAll";
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(sqlDataReader);
                int i = 1;
                foreach(DataRow row in data.Rows)
                {
                    table.AddCell(new Cell().Add(new Paragraph(i.ToString()).SetFont(regularFont)));
                    table.AddCell(new Cell().Add(new Paragraph(row["CountryName"].ToString()).SetFont(regularFont)));
                    table.AddCell(new Cell().Add(new Paragraph(row["CountryCode"].ToString()).SetFont(regularFont)));
                    table.AddCell(new Cell().Add(new Paragraph(row["UserName"].ToString()).SetFont(regularFont)));
                    i++;
                }

                document.Add(table);
                document.Close();

                return File(stream.ToArray(), "application/pdf", "Table.pdf");
            }
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Country_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "CountryID";
                worksheet.Cells[1, 2].Value = "CountryName";
                worksheet.Cells[1, 3].Value = "CountryCode";
                worksheet.Cells[1, 4].Value = "UserName";
                worksheet.Cells[1, 5].Value = "CreationDate";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["CountryID"];
                    worksheet.Cells[row, 2].Value = item["CountryName"];
                    worksheet.Cells[row, 3].Value = item["CountryCode"];
                    worksheet.Cells[row, 4].Value = item["UserName"];
                    worksheet.Cells[row, 5].Value = item["CreationDate"];
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Data-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        public IActionResult CountryList()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Country_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult AddEdit_Country(int CountryID)
        {
            ViewBag.UserList = _dropdownService.GetUserDropDown();

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Country_SelectByPK";
            command.Parameters.AddWithValue("@CountryID", CountryID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            CountryModel countryModel = new CountryModel();

            foreach(DataRow dataRow in table.Rows)
            {
                countryModel.CountryID = Convert.ToInt32(@dataRow["CountryID"]);
                countryModel.CountryName = @dataRow["CountryName"].ToString();
                countryModel.CountryCode = @dataRow["CountryCode"].ToString();
                countryModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }
            return View("AddEdit_Country",countryModel);
        }
        public IActionResult CountrySave(CountryModel countryModel)
        {
            if (countryModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }
            if(ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (countryModel.CountryID== null || countryModel.CountryID == 0)
                {
                    command.CommandText = "PR_Country_Insert";
                }
                else
                {
                    command.CommandText = "PR_Country_Update";
                    command.Parameters.Add("@CountryID", SqlDbType.Int).Value = countryModel.CountryID;
                }
                command.Parameters.Add("@CountryName",SqlDbType.VarChar).Value = countryModel.CountryName;
                command.Parameters.Add("@CountryCode",SqlDbType.VarChar).Value=countryModel.CountryCode;
                command.Parameters.Add("@UserID",SqlDbType.Int).Value = countryModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("CountryList");
            }
            return View("AddEdit_Country", countryModel);
        }
        public IActionResult CountryDelete(int countryID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Country_Delete";
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = countryID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("CountryList");
        }
    }
}
