using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using addressbook_app.Models;
using OfficeOpenXml;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace addressbook_app.Controllers
{
    public class ContactCategoryController : Controller
    {
        private IConfiguration _configuration;

        public ContactCategoryController(IConfiguration conf)
        {
            _configuration = conf;
        }

        #region CountryList
        public IActionResult ContactCategoryList()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "PR_ContactCategory_SelectAll";
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader reader = command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(reader);
            return View(dataTable);
        }
        #endregion

        #region URL Encryption-Decryption
        public static class UrlEncryptor
        {
            private static readonly string EncryptionKey = "pjsGLNYrMqU6wny4"; // Change this key

            public static string Encrypt(string text)
            {
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                    aesAlg.IV = new byte[16];

                    var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }

            public static string Decrypt(string encryptedText)
            {
                using (var aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                    aesAlg.IV = new byte[16];

                    var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText)))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        #endregion

        #region ContactCategoryDelete
        public IActionResult ContactCategoryDelete(string ContactCategoryID)
        {
            try
            {
                //if (string.IsNullOrEmpty(encryptedContactCategoryID))
                //{
                //    TempData["ErrorMsg"] = "Invalid contact category ID.";
                //    return RedirectToAction("ContactCategoryList");
                //}

                //encryptedContactCategoryID = HttpUtility.UrlDecode(encryptedContactCategoryID);
                //int decryptedContactCategoryID = Convert.ToInt32(UrlEncryptor.Decrypt(encryptedContactCategoryID));

                string connectionString = this._configuration.GetConnectionString("ConnectionString");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_ContactCategory_DeleteByPK";

                        command.Parameters.Add("@ContactCategoryID", SqlDbType.Int).Value = ContactCategoryID;
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();

                        command.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMsg"] = "Contact category deleted successfully.";
            }
            catch (FormatException)
            {
                TempData["ErrorMsg"] = "The contact category ID format is invalid.";
            }
            catch (CryptographicException)
            {
                TempData["ErrorMsg"] = "Decryption failed. The contact category ID might be corrupted.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }

            return RedirectToAction("ContactCategoryList");
        }

        #endregion

        #region MultipleDelete
        [HttpPost]
        public IActionResult DeleteMultiple(int[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length == 0)
            {
                TempData["ErrorMsg"] = "No items selected for deletion.";
                return RedirectToAction("ContactCategoryList");
            }

            try
            {
                string connectionString = _configuration.GetConnectionString("ConnectionString");
                int userId = (int)CommonVariable.UserID(); // Replace with the actual user ID (e.g., from session or authentication context)

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var id in selectedIds)
                    {
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "PR_ContactCategory_DeleteByPK";
                            command.Parameters.AddWithValue("@ContactCategoryID", id);
                            command.Parameters.AddWithValue("@UserID", userId);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                TempData["SuccessMsg"] = "Selected contact categories deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMsg"] = ex.Message;
            }

            return RedirectToAction("ContactCategoryList");
        }



        #endregion

        #region CountryForm
        public IActionResult ContactCategoryForm(int? ContactCategoryID)
        {
            ContactCategoryModel model = new ContactCategoryModel();

            if (ContactCategoryID != null)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("PR_ContactCategory_SelectByPK", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add("@ContactCategoryID", SqlDbType.Int).Value = ContactCategoryID;
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                model.ContactCategoryID = Convert.ToInt32(reader["ContactCategoryID"]);
                                model.ContactCategoryName = Convert.ToString(reader["ContactCategoryName"]);
                            }
                        }
                    }
                }
            }

            return View(model);
        }

        #endregion

        #region CountryAddEdit
        public IActionResult ContactCategoryAddEdit(ContactCategoryModel model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        if (model.ContactCategoryID == 0)
                        {
                            command.CommandText = "PR_ContactCategory_Insert";
                        }
                        else
                        {
                            command.CommandText = "PR_ContactCategory_UpdateByPK";
                            command.Parameters.Add("@ContactCategoryID", SqlDbType.Int).Value = model.ContactCategoryID;
                        }

                        command.Parameters.Add("@ContactCategoryName", SqlDbType.VarChar).Value = model.ContactCategoryName;
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();

                        command.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMsg"] = "Task performed successfully";
                return RedirectToAction("ContactCategoryList");
            }

            return View("ContactCategoryForm", model);
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
            sqlCommand.CommandText = "PR_ContactCategory_SelectAll";
            sqlCommand.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers

                worksheet.Cells[1, 1].Value = "ContactCategoryName";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["ContactCategoryName"];

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