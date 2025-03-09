using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using addressbook_app.Models;
using OfficeOpenXml;

namespace addressbook_app.Controllers
{
    [CheckAccess]
    public class StateController : Controller
    {
        private IConfiguration _configuration;


        public StateController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region StateList
        public IActionResult StateList()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_State_SelectAll";
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            return View(dataTable);
        }
        #endregion

        #region CountryDropDown
        public void CountryDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command2 = connection.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_Country_SelectForDropDown";
            command2.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<CountryDropDownModel> countryList = new List<CountryDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                CountryDropDownModel model = new CountryDropDownModel();
                model.CountryID = Convert.ToInt32(data["CountryID"]);
                model.CountryName = data["CountryName"].ToString();
                countryList.Add(model);
            }
            ViewBag.CountryList = countryList;
        }
        #endregion

        #region StateForm
        public IActionResult StateForm(int? stateID)
        {
            CountryDropDown();

            if (stateID == null)
            {
                var m = new StateModel
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
            command.CommandText = "PR_State_SelectByPK";
            command.Parameters.Add("@StateID", SqlDbType.Int).Value = stateID;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            StateModel model = new StateModel();

            foreach (DataRow dataRow in table.Rows)
            {
                model.StateName = dataRow["StateName"].ToString();
                model.StateCode = dataRow["StateCode"].ToString();
                model.StateCapital = dataRow["StateCapital"].ToString();
                model.CountryID = Convert.ToInt16(dataRow["CountryID"]);
            }
            return View(model);
        }
        #endregion

        #region StateAddEdit
        public IActionResult StateAddEdit(StateModel model)
        {
            if (model.CountryID <= 0)
            {
                ModelState.AddModelError("CountryID", "A valid country is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (model.StateID == 0)
                {
                    command.CommandText = "PR_State_Insert";
                }
                else
                {
                    command.CommandText = "PR_State_UpdateByPK";
                    command.Parameters.Add("@StateID", SqlDbType.Int).Value = model.StateID;
                }
                command.Parameters.Add("@StateName", SqlDbType.VarChar).Value = model.StateName;
                command.Parameters.Add("@StateCode", SqlDbType.VarChar).Value = model.StateCode;
                command.Parameters.Add("@StateCapital", SqlDbType.VarChar).Value = model.StateCapital;
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = model.CountryID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
                command.ExecuteNonQuery();
                TempData["SuccessMsg"] = "Task performed successfully";
                return RedirectToAction("StateList");
            }

            CountryDropDown();
            return View("StateForm", model);
        }
        #endregion

        #region StateDelete
        public IActionResult StateDelete(int stateID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "PR_State_DeleteByPK";
                command.Parameters.Add("@StateID", SqlDbType.Int).Value = stateID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }
            return RedirectToAction("StateList");
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
            sqlCommand.CommandText = "PR_State_SelectAll";
            sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
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
                worksheet.Cells[1, 4].Value = "StateCapital";
                worksheet.Cells[1, 5].Value = "CountryName";
                worksheet.Cells[1, 6].Value = "CreationDate";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["StateID"]; // Replace with your property
                    worksheet.Cells[row, 2].Value = item["StateName"]; // Replace with your property
                    worksheet.Cells[row, 3].Value = item["StateCode"];
                    worksheet.Cells[row, 4].Value = item["StateCapital"];
                    worksheet.Cells[row, 5].Value = item["CountryName"];
                    worksheet.Cells[row, 6].Value = item["CreationDate"];
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
