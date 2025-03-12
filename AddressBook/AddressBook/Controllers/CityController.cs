using System.Data;
using System.Data.SqlClient;
using AddressBook.Models;
using AddressBook.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace AddressBook.Controllers
{
    [CheckAccess]
    public class CityController : Controller
    {
        private readonly DropDownService _dropdownService;
        private readonly IConfiguration _configuration;

        public CityController(IConfiguration configuration, DropDownService dropdownService)
        {
            _configuration = configuration;
            _dropdownService = dropdownService;
        }

        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_City_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "CityID";
                worksheet.Cells[1, 2].Value = "CityName";
                worksheet.Cells[1, 3].Value = "STDCode";
                worksheet.Cells[1, 4].Value = "PinCode";
                worksheet.Cells[1, 5].Value = "StateName";
                worksheet.Cells[1, 6].Value = "CountryName";
                worksheet.Cells[1, 7].Value = "UserName";
                worksheet.Cells[1, 8].Value = "CreationDate";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["CityID"];
                    worksheet.Cells[row, 2].Value = item["CityName"];
                    worksheet.Cells[row, 3].Value = item["STDCode"];
                    worksheet.Cells[row, 4].Value = item["PinCode"];
                    worksheet.Cells[row, 5].Value = item["StateName"];
                    worksheet.Cells[row, 6].Value = item["CountryName"];
                    worksheet.Cells[row, 7].Value = item["UserName"];
                    worksheet.Cells[row, 8].Value = item["CreationDate"];
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Data-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        public IActionResult CityList()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_City_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult AddEdit_City(int cityID)
        {
            ViewBag.StateList = _dropdownService.GetSatetDropDown();
            ViewBag.CountryList = _dropdownService.GetCountryDropDown();
            ViewBag.UserList = _dropdownService.GetUserDropDown();


            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_City_SelectByPK";
            command.Parameters.AddWithValue("@CityID", cityID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            CityModel cityModel = new CityModel();

            foreach (DataRow dataRow in table.Rows)
            {
                cityModel.CityName = @dataRow["CityName"].ToString();
                cityModel.STDCode = @dataRow["STDCode"].ToString();
                cityModel.PinCode = @dataRow["PinCode"].ToString();
                cityModel.CountryID = Convert.ToInt32(@dataRow["CountryID"]);
                cityModel.StateID = Convert.ToInt32(@dataRow["StateID"]);
                cityModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }

            return View("AddEdit_City", cityModel);
        }
        public IActionResult CitySave(CityModel cityModel)
        {
            if (cityModel.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }
            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (cityModel.CityID== null || cityModel.CityID == 0)
                {
                    command.CommandText = "PR_City_Insert";
                }
                else
                {
                    command.CommandText = "PR_City_Update";
                    command.Parameters.Add("@CityID", SqlDbType.Int).Value = cityModel.CityID;
                }
                command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = cityModel.CityName;
                command.Parameters.Add("@STDCode", SqlDbType.VarChar).Value = cityModel.STDCode;
                command.Parameters.Add("PinCode",SqlDbType.VarChar).Value = cityModel.PinCode;
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = cityModel.CountryID;
                command.Parameters.Add("@StateID", SqlDbType.Int).Value = cityModel.StateID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = cityModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("CityList");
            }
            return View("AddEdit_City", cityModel);
        }
        public IActionResult CityDelete(int cityID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_City_Delete";
                command.Parameters.Add("@CityID", SqlDbType.Int).Value = cityID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("CityList");
        }
        
    }
}
