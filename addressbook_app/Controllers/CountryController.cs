using addressbook_app.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using OfficeOpenXml;

namespace addressbook_app.Controllers
{
    [CheckAccess]
    public class CountryController : Controller
    {
        private IConfiguration _configuration;

        public CountryController(IConfiguration conf)
        {
            _configuration = conf;
        }

        #region CountryList
        public IActionResult CountryList()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_Country_SelectAll";
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            return View(dataTable);
        }
        #endregion

        #region CountryDelete
        public IActionResult CountryDelete(int countryID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "PR_Country_DeleteByPK";
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = countryID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }
            return RedirectToAction("CountryList");
        }
        #endregion

        #region CountryForm
        public IActionResult CountryForm(int? countryID)
        {
            if (countryID == null)
            {
                var m = new CountryModel
                {
                    CreationDate = DateTime.Now
                };
                return View(m);
            }

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Country_SelectByPK";
            command.Parameters.AddWithValue("@CountryID", countryID);
            command.Parameters.AddWithValue("@UserID", CommonVariable.UserID());
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            CountryModel model = new CountryModel();

            foreach (DataRow dataRow in table.Rows)
            {
                model.CountryName = dataRow["CountryName"].ToString();
                model.CountryCode = dataRow["CountryCode"].ToString();
                model.CountryCapital = dataRow["CountryCapital"].ToString();
            }
            return View(model);
        }
        #endregion

        #region CountryAddEdit
        public IActionResult CountryAddEdit(CountryModel model)
        {

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (model.CountryID == 0)
                {
                    command.CommandText = "PR_Country_Insert";
                }
                else
                {
                    command.CommandText = "PR_Country_UpdateByPK";
                    command.Parameters.Add("@CountryID", SqlDbType.Int).Value = model.CountryID;
                }
                command.Parameters.Add("@CountryName", SqlDbType.VarChar).Value = model.CountryName;
                command.Parameters.Add("@CountryCode", SqlDbType.VarChar).Value = model.CountryCode;
                command.Parameters.Add("@CountryCapital", SqlDbType.VarChar).Value = model.CountryCapital;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
                command.ExecuteNonQuery();
                TempData["SuccessMsg"] = "Task performed successfully";
                return RedirectToAction("CountryList");
            }

            return View("CountryForm", model);
        }
        #endregion

        #region Export to excel
        public IActionResult ExportToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Country_SelectAll";
            sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
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
                worksheet.Cells[1, 4].Value = "CountryCapital";
                worksheet.Cells[1, 5].Value = "CreationDate";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["CountryID"]; 
                    worksheet.Cells[row, 2].Value = item["CountryName"]; 
                    worksheet.Cells[row, 3].Value = item["CountryCode"];
                    worksheet.Cells[row, 4].Value = item["CountryCapital"];
                    worksheet.Cells[row, 5].Value = item["CreationDate"];
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Data-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
            #endregion
        }
    }
}
