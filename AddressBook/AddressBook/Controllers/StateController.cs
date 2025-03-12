using System.Data;
using System.Data.SqlClient;
using AddressBook.Models;
using AddressBook.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace AddressBook.Controllers
{
    [CheckAccess]
    public class StateController : Controller
    {
        private readonly DropDownService _dropdownService;
        private readonly IConfiguration _configuration;

        public StateController(IConfiguration configuration, DropDownService dropdownService)
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
            sqlCommand.CommandText = "PR_State_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "StateID";
                worksheet.Cells[1, 2].Value = "StateName";
                worksheet.Cells[1, 3].Value = "StateCode";
                worksheet.Cells[1, 4].Value = "CountryName";
                worksheet.Cells[1, 5].Value = "UserName";
                worksheet.Cells[1, 6].Value = "CreationDate";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["StateID"];
                    worksheet.Cells[row, 2].Value = item["StateName"];
                    worksheet.Cells[row, 3].Value = item["StateCode"];
                    worksheet.Cells[row, 4].Value = item["CountryName"];
                    worksheet.Cells[row, 5].Value = item["UserName"];
                    worksheet.Cells[row, 6].Value = item["CreationDate"];
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Data-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        public IActionResult StateList()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_State_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult AddEdit_State(int stateID)
        {
            ViewBag.CountryList = _dropdownService.GetCountryDropDown();
            ViewBag.UserList = _dropdownService.GetUserDropDown();

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_State_SelectByPK";
            command.Parameters.AddWithValue("@StateID", stateID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            StateModel stateModel = new StateModel();

            foreach(DataRow dataRow in table.Rows)
            {
                stateModel.StateID = Convert.ToInt32(@dataRow["StateID"]);
                stateModel.StateName = @dataRow["StateName"].ToString();
                stateModel.StateCode = @dataRow["StateCode"].ToString();
                stateModel.CountryID = Convert.ToInt32(@dataRow["CountryID"]);
                stateModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }

            return View("AddEdit_State",stateModel);
        }
        public IActionResult StateSave(StateModel stateModel)
        {
            if(stateModel.UserID <= 0)
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
                if (stateModel.StateID == null || stateModel.StateID == 0)
                {
                    command.CommandText = "PR_State_Insert";
                }
                else
                {
                    command.CommandText = "PR_State_Update";
                    command.Parameters.Add("@StateID", SqlDbType.Int).Value = stateModel.StateID;
                }
                command.Parameters.Add("@StateName", SqlDbType.VarChar).Value = stateModel.StateName;
                command.Parameters.Add("@StateCode", SqlDbType.VarChar).Value = stateModel.StateCode;
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = stateModel.CountryID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = stateModel.UserID;
                command.ExecuteNonQuery();
                return RedirectToAction("StateList");
            }
            return View("AddEdit_State", stateModel);
        }
        public IActionResult StateDelete(int stateID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_State_Delete";
                command.Parameters.Add("@StateID", SqlDbType.Int).Value = stateID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("StateList");
        }
    }
}
