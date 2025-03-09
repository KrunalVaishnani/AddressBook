using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using addressbook_app.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;

namespace addressbook_app.Controllers
{
    [CheckAccess]
    public class CityController : Controller
    {
        private IConfiguration _configuration;

        public CityController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region CityFilter
        public IActionResult CityFilter(IFormCollection fc)
        {
            StateDropDown(null);
            CountryDropDown();

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_City_SelectAll";
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = fc["CityName"].ToString();
            command.Parameters.Add("@CityCode", SqlDbType.VarChar).Value = fc["CityCode"].ToString();
            command.Parameters.Add("@StateID", SqlDbType.Int).Value = (string.IsNullOrEmpty(fc["StateID"])) ? null : Convert.ToInt32(fc["StateID"]);
            command.Parameters.Add("@CountryID", SqlDbType.Int).Value = (string.IsNullOrEmpty(fc["CountryID"])) ? null : Convert.ToInt32(fc["CountryID"]);

            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            return View("CityList", dataTable);
        }
        #endregion

        #region CityList
        public IActionResult CityList()
        {
            CountryDropDown();
            StateDropDown(null);

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_City_SelectAll";
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            /*command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = fc["CityName"].ToString();
            command.Parameters.Add("@CityCode", SqlDbType.VarChar).Value = fc["CityCode"].ToString();
            command.Parameters.Add("@StateID", SqlDbType.Int).Value = Convert.ToInt32(fc["StateID"]);
            command.Parameters.Add("@CountryID", SqlDbType.Int).Value = Convert.ToInt32(fc["CountryID"]);*/

            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            return View(dataTable);
        }
        #endregion

        #region StateDropDown
        public void StateDropDown(int? countryID)
        {
            if (countryID.HasValue)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "PR_State_SelectByCountryID";
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
                    command.Parameters.Add("@CountryID", SqlDbType.Int).Value = countryID;
                    SqlDataReader reader = command.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    List<StateDropDownModel> stateList = new List<StateDropDownModel>();
                    foreach (DataRow data in dataTable.Rows)
                    {
                        StateDropDownModel model = new StateDropDownModel();
                        model.StateID = Convert.ToInt32(data["StateID"]);
                        model.StateName = data["StateName"].ToString();
                        stateList.Add(model);
                    }

                    ViewBag.StateList = stateList;
                }
            }
            else
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = "PR_State_DropDown";
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
                    SqlDataReader reader = command.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    List<StateDropDownModel> stateList = new List<StateDropDownModel>();
                    foreach (DataRow data in dataTable.Rows)
                    {
                        StateDropDownModel model = new StateDropDownModel();
                        model.StateID = Convert.ToInt32(data["StateID"]);
                        model.StateName = data["StateName"].ToString();
                        stateList.Add(model);
                    }

                    ViewBag.StateList = stateList;
                }
            }
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

        #region GetStatesByCountry
        [HttpGet]
        public JsonResult GetStatesByCountry(int countryId)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                Console.WriteLine("CountryID ==============" + countryId);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_State_SelectByCountryID";
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = countryId;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();

                SqlDataReader reader = command.ExecuteReader();
                DataTable dataTable2 = new DataTable();
                dataTable2.Load(reader);

                List<StateDropDownModel> stateList = new List<StateDropDownModel>();
                foreach (DataRow data in dataTable2.Rows)
                {
                    StateDropDownModel model = new StateDropDownModel();
                    model.StateID = Convert.ToInt32(data["StateID"]);
                    model.StateName = data["StateName"].ToString();
                    Console.WriteLine("ID =================" + model.StateID);
                    Console.WriteLine("Name =================" + model.StateName);
                    stateList.Add(model);
                }
                return Json(stateList);
            }
        }
        #endregion

        #region CityForm
        public IActionResult CityForm(int? cityID,int? countryID)
        {
            StateDropDown(countryID);
            CountryDropDown();

            if (cityID == null)
            {
                var m = new CityModel
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
            command.CommandText = "PR_City_SelectByID";
            command.Parameters.Add("@CityID", SqlDbType.Int).Value = cityID;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            CityModel model = new CityModel();

            foreach (DataRow dataRow in table.Rows)
            {
                model.CityName = dataRow["CityName"].ToString();
                model.CityCode = dataRow["CityCode"].ToString();
                model.StateID = Convert.ToInt32(dataRow["StateID"]);
                model.CountryID = Convert.ToInt32(dataRow["CountryID"]);
            }
            return View(model);
        }
        #endregion

        #region CityAddEdit
        public IActionResult CityAddEdit(CityModel model)
        {

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (model.CityID == 0)
                {
                    command.CommandText = "PR_City_Insert";
                }
                else
                {
                    command.CommandText = "PR_City_UpdateByPK";
                    command.Parameters.Add("@CityID", SqlDbType.Int).Value = model.CityID;
                }
                command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = model.CityName;
                command.Parameters.Add("@CityCode", SqlDbType.VarChar).Value = model.CityCode;
                command.Parameters.Add("@StateID", SqlDbType.Int).Value = model.StateID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
                command.ExecuteNonQuery();
                TempData["SuccessMsg"] = "Task performed successfully";
                return RedirectToAction("CityList");
            }

            CountryDropDown();
            return View("CityForm", model);
        }
        #endregion

        #region CityDelete
        public IActionResult CityDelete(int cityID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "PR_City_DeleteByPK";
                command.Parameters.Add("@CityID", SqlDbType.Int).Value = cityID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }
            return RedirectToAction("CityList");
        }
        #endregion

        #region Export to excel
        public IActionResult ExportToExcel()
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_City_SelectAll";
            sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "CityID";
                worksheet.Cells[1, 2].Value = "CityName";
                worksheet.Cells[1, 3].Value = "CityCode";
                worksheet.Cells[1, 4].Value = "StateName";
                worksheet.Cells[1, 5].Value = "CreationDate";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["CityID"]; // Replace with your property
                    worksheet.Cells[row, 2].Value = item["CityName"]; // Replace with your property
                    worksheet.Cells[row, 3].Value = item["CityCode"];
                    worksheet.Cells[row, 4].Value = item["StateName"];
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