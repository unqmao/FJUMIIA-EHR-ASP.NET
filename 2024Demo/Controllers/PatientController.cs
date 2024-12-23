using _2024Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace _2024Demo.Controllers
{
    public class PatientController : Controller
    {
        private readonly string ConnStr = "Data Source=比電\\SQLEXPRESS02;Integrated Security=True;Persist Security Info=False;Pooling=False;";

        public IActionResult Index()
        {
            var patientViewModel = new PatientViewModel();
            var genderCodeList = new List<SelectListItem>
            {
                new SelectListItem { Text = "男", Value = "M" },
                new SelectListItem { Text = "女", Value = "F" }
            };
            var bloodTypeList = new List<SelectListItem>
            { 
                new() { Text = "A", Value = "A" },
                new() { Text = "B", Value = "B" },
                new() { Text = "AB", Value = "AB" },
                new() { Text = "O", Value = "O" }
            };

            ViewBag.PatientViewModel = patientViewModel;
            ViewBag.GenderCodeList = genderCodeList;
            ViewBag.BloodTypeList = bloodTypeList;

            return View();
        }


        //新增
        //<param name="patientViewModel"> 病人資訊 ViewModel </param>
        [HttpPost]
        public IActionResult Save([FromForm] PatientViewModel patientViewModel)
        {
            try
            {
                var patientDBModel = ConvertPatientViewModeltoDBModel(patientViewModel);
                var dbResult = false;

                if (patientDBModel.PatientId == 0)
                {
                    dbResult = InsertPatient(patientDBModel).Result > 0;
                }
                else
                {
                    dbResult = UpdatePatient(patientDBModel).Result;
                }

                if (dbResult)
                {
                    return Ok(dbResult);
                }

                return BadRequest(dbResult);
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Search(long? patientId, string? idNo, string? familyName, string? givenName)
        {
            try
            {
                var resultList = new List<PatientViewModel>();

                var dbResult = QueryPatientList(patientId, idNo, familyName, givenName);

                if (dbResult.Result.Count() > 0)
                {
                    resultList = dbResult.Result.Select(ConvertPatientDBModeltoViewModel).ToList();
                    return Ok(dbResult.Result);
                }
                else if (dbResult.Result.Count() == 0)
                {
                    return Ok("查無資料");
                }

                return BadRequest(dbResult);
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Error = ex.Message });
            }
        }
        [HttpPost]
        public IActionResult Delete(long patientId)
        {
            try
            {
                var patientDBModel = new PatientDBModel();

                var patientDBModelList = QueryPatientList(patientId).Result;


                if (patientDBModelList.Count() > 0)
                {
                    patientDBModel = patientDBModelList.First();
                }

                patientDBModel.Active = false;

                var dbResult = UpdatePatient(patientDBModel);

                if (dbResult.Result)
                {
                    return Ok(dbResult.Result);
                }

                return BadRequest(dbResult);
            }
            catch (Exception ex)
            {
                return Json(new { Status = "Error", Error = ex.Message });
            }
        }


        #region sql

        public Task<long> InsertPatient(PatientDBModel patient)
        {
            long insertId = 0;

            SqlConnection connection = new SqlConnection(ConnStr);

            var insertStr = @"INSERT INTO DB1.dbo.Patient
                      VALUES (@IdNo, @Active, @FamilyName, @GivenName, @Telecom, @Gender, @Birthday, @Address, 
                              @BloodType, @EmergencyContact, @EmergencyContactPhone, @MedicalHistory, @AllergyInfo)
                      SELECT @InsertId = SCOPE_IDENTITY()";

            SqlCommand command = new SqlCommand(insertStr, connection);

            SqlParameter outputValue = new SqlParameter("@InsertId", SqlDbType.BigInt);
            outputValue.Direction = ParameterDirection.Output;

            command.Parameters.Add(outputValue);
            command.Parameters.Add(new SqlParameter("@IdNo", patient.IdNo));
            command.Parameters.Add(new SqlParameter("@Active", patient.Active));
            command.Parameters.Add(new SqlParameter("@FamilyName", patient.FamilyName));
            command.Parameters.Add(new SqlParameter("@GivenName", patient.GivenName));
            command.Parameters.Add(new SqlParameter("@Telecom", patient.Telecom));
            command.Parameters.Add(new SqlParameter("@Gender", patient.Gender));
            command.Parameters.Add(new SqlParameter("@Birthday", patient.Birthday.ToString("yyyy/MM/dd")));
            command.Parameters.Add(new SqlParameter("@Address", patient.Address));
            command.Parameters.Add(new SqlParameter("@BloodType", patient.BloodType));
            command.Parameters.Add(new SqlParameter("@EmergencyContact", patient.EmergencyContact));
            command.Parameters.Add(new SqlParameter("@EmergencyContactPhone", patient.EmergencyContactPhone));  // 新增
            command.Parameters.Add(new SqlParameter("@MedicalHistory", patient.MedicalHistory));
            command.Parameters.Add(new SqlParameter("@AllergyInfo", patient.AllergyInfo));

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

            if (outputValue.Value != DBNull.Value)
            {
                insertId = Convert.ToInt64(outputValue.Value);
            }

            return Task.FromResult(insertId);
        }


        public Task<List<PatientDBModel>> QueryPatientList(long? patientId = null, string? idNo = null, string? familyName = null, string? givenName = null)
        {
            var result = new List<PatientDBModel>();

            try
            {              
                SqlConnection connection = new(ConnStr);

                var param = new List<SqlParameter>();

                var queryStr = @"SELECT * FROM [DB1].[dbo].[Patient] WHERE 1=1";
                queryStr += " AND Active = @Active";
                param.Add(new SqlParameter("@Active", true));

                if (patientId != null)
                {
                    queryStr += " AND PatientId= @PatientId";
                    param.Add(new SqlParameter("@PatientId", patientId));
                }

                if (idNo != null)
                {
                    queryStr += " AND IdNo= @IdNo ";
                    param.Add(new SqlParameter("@IdNo", idNo));
                }

                if (familyName != null)
                {
                    queryStr += " AND FamilyName LIKE '%' + @FamilyName + '%'";
                    param.Add(new SqlParameter("@FamilyName", familyName));
                }

                if (givenName != null)
                {
                    queryStr += " AND GivenName LIKE '%' + @GivenName + '%'";
                    param.Add(new SqlParameter("@GivenName", givenName));
                }

                SqlCommand command = new SqlCommand(queryStr, connection);

                foreach (var p in param)
                {
                    command.Parameters.Add(p);
                }

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var patient = new PatientDBModel
                        {
                            PatientId = reader.GetInt64(reader.GetOrdinal("PatientId")),
                            IdNo = reader.GetString(reader.GetOrdinal("IdNo")),
                            Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                            FamilyName = reader.GetString(reader.GetOrdinal("FamilyName")),
                            GivenName = reader.GetString(reader.GetOrdinal("GivenName")),
                            Telecom = reader.GetString(reader.GetOrdinal("Telecom")),
                            Gender = reader.GetString(reader.GetOrdinal("Gender")),
                            Birthday = reader.GetDateTime(reader.GetOrdinal("Birthday")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            BloodType = reader.GetString(reader.GetOrdinal("BloodType")),
                            EmergencyContact = reader.GetString(reader.GetOrdinal("EmergencyContact")),
                            EmergencyContactPhone = reader.GetString(reader.GetOrdinal("EmergencyContactPhone")),  // 新增
                            MedicalHistory = reader.GetString(reader.GetOrdinal("MedicalHistory")),
                            AllergyInfo = reader.GetString(reader.GetOrdinal("AllergyInfo"))
                        };

                        result.Add(patient);
                    }
                }
                else
                {
                    Console.WriteLine("查無資料");
                }

                connection.Close();

                return Task.FromResult(result);
            }
            catch (SqlException ex)
            {
                throw new Exception("查詢失敗", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("未知錯誤", ex);    
            }
        }


        public Task<bool> UpdatePatient(PatientDBModel patient)
        {
            bool result = false;
            SqlConnection connection = new SqlConnection(ConnStr);
            var updateStr = @"UPDATE DB1.dbo.Patient
                      SET IdNo = @IdNo, Active = @Active, FamilyName = @FamilyName, 
                          GivenName = @GivenName, Telecom = @Telecom, Gender = @Gender, 
                          Birthday = @Birthday, Address = @Address, 
                          BloodType = @BloodType, EmergencyContact = @EmergencyContact, 
                          EmergencyContactPhone = @EmergencyContactPhone,
                          MedicalHistory = @MedicalHistory, AllergyInfo = @AllergyInfo
                      WHERE PatientId = @PatientId";
            SqlCommand command = new SqlCommand(updateStr, connection);

            command.Parameters.Add(new SqlParameter("@PatientId", patient.PatientId));
            command.Parameters.Add(new SqlParameter("@IdNo", patient.IdNo));
            command.Parameters.Add(new SqlParameter("@Active", patient.Active));
            command.Parameters.Add(new SqlParameter("@FamilyName", patient.FamilyName));
            command.Parameters.Add(new SqlParameter("@GivenName", patient.GivenName));
            command.Parameters.Add(new SqlParameter("@Telecom", patient.Telecom));
            command.Parameters.Add(new SqlParameter("@Gender", patient.Gender));
            command.Parameters.Add(new SqlParameter("@Birthday", patient.Birthday.ToString("yyyy/MM/dd")));
            command.Parameters.Add(new SqlParameter("@Address", patient.Address));
            command.Parameters.Add(new SqlParameter("@BloodType", patient.BloodType));
            command.Parameters.Add(new SqlParameter("@EmergencyContact", patient.EmergencyContact));
            command.Parameters.Add(new SqlParameter("@EmergencyContactPhone", patient.EmergencyContactPhone));  // 新增
            command.Parameters.Add(new SqlParameter("@MedicalHistory", patient.MedicalHistory));
            command.Parameters.Add(new SqlParameter("@AllergyInfo", patient.AllergyInfo));

            connection.Open();
            var updateResult = command.ExecuteNonQuery();
            connection.Close();

            if (updateResult > 0)
            {
                result = true;
            }
            return Task.FromResult(result);
        }

        #endregion sql

        #region private

        //<param name="viewModel"> Patient ViewModel </param>
        //資料可以在這裡做轉換

        /* 
         * 你的資料要透過一定的驗證程序，或是邏輯處理，才能把資料存到資料庫中。
         * 舉例來說，你在前端 POST 註冊資料的時候，裡面會有一組密碼，但你不會希望密碼是被明碼儲存
         * 例如你的密碼是設定 1234 那你就不會希望資料庫裡面是直接呈現 1234 的密碼
         * 所以密碼從前端透過 ViewModel 傳來之後，就要先經過 雜湊 (Hash) 變成例如 mn36!@bvcftyuk02f516w2 的字串
         * 而後才放到資料庫
         */
        private PatientDBModel ConvertPatientViewModeltoDBModel(PatientViewModel viewModel)
        {
            return new PatientDBModel()
            {
                PatientId = viewModel.PatientId,
                IdNo = viewModel.IdNo,
                Active = viewModel.Active,
                FamilyName = viewModel.FamilyName,
                GivenName = viewModel.GivenName,
                Telecom = viewModel.Telecom,
                Gender = viewModel.Gender,
                Birthday = viewModel.Birthday,
                Address = viewModel.Address,
                BloodType = viewModel.BloodType,
                EmergencyContact = viewModel.EmergencyContact,
                MedicalHistory = viewModel.MedicalHistory,
                AllergyInfo = viewModel.AllergyInfo,
                EmergencyContactPhone = viewModel.EmergencyContactPhone,

            };
        }

        // 假設你從資料庫要抓取個人資料的時候，可以選擇忽略存取密碼，或是一些敏感個資，僅傳遞前端或後端邏輯處理時，所需要的充分必要資訊。
        private PatientViewModel ConvertPatientDBModeltoViewModel(PatientDBModel dbModel)
        {
            return new PatientViewModel()
            {
                PatientId = dbModel.PatientId,
                IdNo = dbModel.IdNo,
                Active = dbModel.Active,
                FamilyName = dbModel.FamilyName,
                GivenName = dbModel.GivenName,
                Telecom = dbModel.Telecom,
                Gender = dbModel.Gender,
                Birthday = dbModel.Birthday,
                Address = dbModel.Address,
                BloodType = dbModel.BloodType,            
                EmergencyContact = dbModel.EmergencyContact,  
                MedicalHistory = dbModel.MedicalHistory,  
                AllergyInfo = dbModel.AllergyInfo,
                EmergencyContactPhone = dbModel.EmergencyContactPhone,
            };
        }
        #endregion Private
    }
}