using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System.Data.SqlClient;
using System.Data;
using addressbook_app.Models;
using System.Diagnostics.Metrics;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace addressbook_app.Controllers
{
    [CheckAccess]
    public class ContactController : Controller
    {
        private readonly IConfiguration _configuration;

        public ContactController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult ContactList()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Contact_SelectAll";
            sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            return View(data);
        }


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


        #region CityDropDown
        public JsonResult CityDropDown(int? stateID)
        {
            if (stateID != null)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command2 = connection.CreateCommand();
                command2.CommandType = System.Data.CommandType.StoredProcedure;
                command2.CommandText = "PR_City_SelectForDropDown";
                command2.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
                command2.Parameters.Add("@StateID", SqlDbType.Int).Value = stateID;

                SqlDataReader reader2 = command2.ExecuteReader();
                DataTable dataTable2 = new DataTable();
                dataTable2.Load(reader2);
                List<CityDropDownModel> cityList = new List<CityDropDownModel>();
                foreach (DataRow data in dataTable2.Rows)
                {
                    CityDropDownModel model = new CityDropDownModel();
                    model.CityID = Convert.ToInt32(data["CityID"]);
                    model.CityName = data["CityName"].ToString();
                    cityList.Add(model);
                }
                ViewBag.CityList = cityList;

                return Json(cityList);

            }
            else
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command2 = connection.CreateCommand();
                command2.CommandType = System.Data.CommandType.StoredProcedure;
                command2.CommandText = "PR_City_SelectForDropDown";
                command2.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();

                SqlDataReader reader2 = command2.ExecuteReader();
                DataTable dataTable2 = new DataTable();
                dataTable2.Load(reader2);
                List<CityDropDownModel> cityList = new List<CityDropDownModel>();
                foreach (DataRow data in dataTable2.Rows)
                {
                    CityDropDownModel model = new CityDropDownModel();
                    model.CityID = Convert.ToInt32(data["CityID"]);
                    model.CityName = data["CityName"].ToString();
                    cityList.Add(model);
                }
                ViewBag.CityList = cityList;

                return Json(cityList);
            }
        }
        #endregion

        #region ContactDelete
        public IActionResult ContactDelete(int ContactID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "PR_Contact_DeleteByPK";
                command.Parameters.Add("@ContactID", SqlDbType.Int).Value = ContactID;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }
            return RedirectToAction("ContactList");
        }
        #endregion

        #region ContactForm
        public IActionResult ContactForm(int? contactID,int? countryID,int? stateID)
        {
            CountryDropDown();


            if (contactID == null)
            {
                return View(new ContactModel());
            }

            StateDropDown(countryID);
            CityDropDown(stateID);

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Contact_SelectByPK";
            command.Parameters.Add("@ContactID", SqlDbType.Int).Value = contactID;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);

            ContactModel model = new ContactModel();

            foreach (DataRow dataRow in table.Rows)
            {
                model.Name = dataRow["Name"].ToString();
                model.Address = dataRow["Address"].ToString();
                model.ContactID = Convert.ToInt32(dataRow["ContactID"]);
                model.CountryID = Convert.ToInt32(dataRow["CountryID"]);
                model.StateID = Convert.ToInt32(dataRow["StateID"]);
                model.CityID = Convert.ToInt32(dataRow["CityID"]);
                model.Gender = dataRow["Gender"].ToString();
                model.MobileNo = dataRow["MobileNo"].ToString();
                model.WhatsAppNo = dataRow["WhatsAppNo"].ToString();
                model.EmailID = dataRow["EmailID"].ToString();
                model.DoB = Convert.ToDateTime(dataRow["DoB"]);
                model.FaceBookID = dataRow["FaceBookID"].ToString();
                model.InstagramID = dataRow["InstagramID"].ToString();
                model.BloodGroup = dataRow["BloodGroup"].ToString();

            }

            return View(model);
        }
        #endregion

        #region ContactAddEdit
        public IActionResult ContactAddEdit(ContactModel model)
        {

            //if (ModelState.IsValid)
            //{
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (model.ContactID == 0)
                {
                    command.CommandText = "PR_Contact_Insert";
                }
                else
                {
                    command.CommandText = "PR_Contact_UpdateByPK";
                    command.Parameters.Add("@ContactID", SqlDbType.Int).Value = model.ContactID;
                }
                command.Parameters.Add("@Name", SqlDbType.VarChar).Value = model.Name;
                command.Parameters.Add("@CountryID", SqlDbType.Int).Value = model.CountryID;
                command.Parameters.Add("@StateID", SqlDbType.Int).Value = model.StateID;
                command.Parameters.Add("@CityID", SqlDbType.Int).Value = model.CityID;
                command.Parameters.Add("@Gender", SqlDbType.VarChar).Value = model.Gender;
                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = model.MobileNo;
                command.Parameters.Add("@WhatsappNo", SqlDbType.VarChar).Value = model.WhatsAppNo;
                command.Parameters.Add("@EmailID", SqlDbType.VarChar).Value = model.EmailID;
                command.Parameters.Add("@Address", SqlDbType.VarChar).Value = model.Address;
                command.Parameters.Add("@DoB", SqlDbType.DateTime).Value = model.DoB;
                command.Parameters.Add("@FaceBookID", SqlDbType.VarChar).Value = model.FaceBookID;
                command.Parameters.Add("@InstagramID", SqlDbType.VarChar).Value = model.InstagramID;
                command.Parameters.Add("@BloodGroup", SqlDbType.VarChar).Value = model.BloodGroup;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
                command.ExecuteNonQuery();
                TempData["SuccessMsg"] = "Task performed successfully";
                return RedirectToAction("ContactList");
            //}
            //else
            //{
            //    TempData["error"] = "aasdfasdfasd";
            //}

            //CountryDropDown();
            //StateDropDown(null);
            //CityDropDown(null);
            //return View("ContactForm", model);
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
            sqlCommand.CommandText = "PR_Contact_SelectAll";
            sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 2].Value = "Name";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["Name"];
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


