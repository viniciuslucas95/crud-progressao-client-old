using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace crud_progressao {
    static class ApiDatabaseManager {
        private readonly static HttpClient _client = new HttpClient();
        private static string _url;

        public static void InitializeAuthentication(string username, string password) {
            _client.DefaultRequestHeaders.Add("username", username);
            _client.DefaultRequestHeaders.Add("password", password);
        }

        public static async Task<bool> LoginAsync(string username, string password) {
            if (string.IsNullOrEmpty(_url)) if (!GetConfigFile()) return false;

            object data = new { username, password };

            LogManager.Write("Trying to log in...");

            try {
                using (HttpResponseMessage res = await _client.PostAsJsonAsync($"{_url}/auth", data)) {
                    if (!res.IsSuccessStatusCode) {
                        LogManager.Write("ERROR trying to log in");
                        res.Dispose();
                        return false;
                    }

                    LogManager.Write("Logged in");
                    res.Dispose();
                    return true;
                }
            } catch (Exception e) {
                LogManager.Write(e.Message);
                return false;
            }
        }

        public static async Task<bool> GetDatabaseAsync(string firstName, string lastName, string className, string responsible, string address, string discount) {
            if (string.IsNullOrEmpty(_url)) if (!GetConfigFile()) return false;

            LogManager.Write("Trying to get students from the database...");

            try {
                using (HttpResponseMessage res = await _client.GetAsync($"{_url}/students/?firstName={firstName}&lastName={lastName}&className={className}&responsible={responsible}&address={address}&discount={discount}")) {
                    if (!res.IsSuccessStatusCode) {
                        LogManager.Write("ERROR trying to get students from the database");
                        res.Dispose();
                        return false;
                    }

                    string json = await res.Content.ReadAsStringAsync();
                    dynamic database = JsonConvert.DeserializeObject(json);

                    Student.Database.Clear();

                    for (int i = 0; i < database.Count; i++)
                        Student.Database.Add(ConvertJsonDataToStudent(database[i]));

                    LogManager.Write("Students gotten");
                    res.Dispose();
                    return true;
                }
            } catch (Exception e) {
                LogManager.Write(e.Message);
                MainWindow.Singleton.SetFeedbackDgText($"Não foi possível acessar o banco de dados!", true);
                return false;
            }
        }

        /*public static async Task<bool> GetDatabaseAsync() {
            if (string.IsNullOrEmpty(_url)) if(!GetConfigFile()) return false;

            LogManager.Write("Trying to get the database...");

            try {
                using (HttpResponseMessage res = await _client.GetAsync($"{_url}/students")) {
                    if (!res.IsSuccessStatusCode) {
                        LogManager.Write("ERROR trying to get the database");
                        res.Dispose();
                        return false;
                    }

                    string json = await res.Content.ReadAsStringAsync();
                    dynamic database = JsonConvert.DeserializeObject(json);

                    if (database.Count == 0) {
                        LogManager.Write("Database gotten, but it is empty");
                        return true;
                    }

                    Student.Database.Clear();

                    for (int i = 0; i < database.Count; i++)
                        Student.Database.Add(ConvertJsonDataToStudent(database[i]));

                    LogManager.Write("Database gotten");
                    res.Dispose();
                    return true;
                }
            } catch (Exception e) {
                LogManager.Write(e.Message);
                MainWindow.Singleton.SetFeedbackDgText($"Não foi possível acessar o banco de dados!", true);
                return false;
            }
        }*/

        /// <returns>Returns the new student id if the register is successful, or empty if isn't</returns>
        public static async Task<string> RegisterStudentAsync(Student student) {
            if (string.IsNullOrEmpty(_url)) if (!GetConfigFile()) return "";

            LogManager.Write("Trying to register the student in the database...");

            JsonData data = ConvertStudentToJsonData(student);

            try {
                using (HttpResponseMessage res = await _client.PostAsJsonAsync($"{_url}/students", data)) {
                    string newStudentId = await res.Content.ReadAsStringAsync();

                    if (res.IsSuccessStatusCode) {
                        LogManager.Write("Student registered");
                    } else
                        LogManager.Write("ERROR trying to register the student in the database");

                    res.Dispose();
                    return newStudentId;
                }
            } catch (Exception e) {
                LogManager.Write(e.Message);
                return "";
            }
        }

        public static async Task<bool> UpdateStudentAsync(Student student) {
            if (string.IsNullOrEmpty(_url)) if (!GetConfigFile()) return false;

            LogManager.Write("Trying to update the student informations in the database...");

            JsonData data = ConvertStudentToJsonData(student);

            try {
                using (HttpResponseMessage res = await _client.PutAsJsonAsync($"{_url}/students", data)) {
                    if (res.IsSuccessStatusCode) {
                        LogManager.Write("Student updated");
                    } else
                        LogManager.Write("ERROR trying to update the student informations in the database");

                    res.Dispose();
                    return res.IsSuccessStatusCode;
                }
            } catch (Exception e) {
                LogManager.Write(e.Message);
                return false;
            }
        }

        public static async Task<bool> DeleteStudentAsync(string id) {
            if (string.IsNullOrEmpty(_url)) if (!GetConfigFile()) return false;

            LogManager.Write("Trying to delete the student from the database...");

            try {
                using (HttpResponseMessage res = await _client.DeleteAsync($"{ _url}/students/{id}")) {
                    if (res.IsSuccessStatusCode) {
                        LogManager.Write("Student deleted");
                    } else
                        LogManager.Write("ERROR trying to delete the student from the database");

                    res.Dispose();
                    return res.IsSuccessStatusCode;
                }
            } catch (Exception e) {
                LogManager.Write(e.Message);
                return false;
            }
        }

        private static bool GetConfigFile() {
            LogManager.Write("Trying to get the config files...");

            try {
                using (StreamReader streamReader = new StreamReader("config.json")) {
                    string json = streamReader.ReadToEnd();
                    dynamic config = JsonConvert.DeserializeObject(json);
                    _url = config.serverUri;
                    LogManager.Write("Config file gotten");
                    streamReader.Dispose();
                    return true;
                }
            } catch (Exception e) {
                LogManager.Write(e.Message);
                return false;
            }
        }

        private static BitmapImage StringToBitmapImage(string value) {
            if (string.IsNullOrEmpty(value)) return null;

            LogManager.Write("Trying to convert a string into a bitmap image");

            try {
                BitmapImage img = new BitmapImage();
                byte[] data = Convert.FromBase64String(value);

                using (MemoryStream ms = new MemoryStream(data)) {
                    img.BeginInit();
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.StreamSource = ms;
                    img.EndInit();
                    ms.Dispose();
                }

                LogManager.Write("String converted");
                return img;
            }catch(Exception e) {
                LogManager.Write(e.Message);
                return null;
            }
        }

        private static string BitmapImageToString(BitmapImage img) {
            if (img == null) return "";

            LogManager.Write("Trying to convert a bitmap image into a string");

            try {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                byte[] data;

                using (MemoryStream ms = new MemoryStream()) {
                    encoder.Save(ms);
                    data = ms.ToArray();
                    ms.Dispose();
                }

                LogManager.Write("Bitmap image converted");
                return Convert.ToBase64String(data);
            }catch(Exception e) {
                LogManager.Write(e.Message);
                return "";
            }
        }

        private static JsonData ConvertStudentToJsonData(Student student) {
            return new JsonData()
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                ClassName = student.ClassName,
                Responsible = student.Responsible,
                Address = student.Address,
                Installment = student.Installment,
                Discount = student.Discount,
                DiscountType = (int)student.DiscountType,
                Note = student.Note,
                Picture = BitmapImageToString(student.Picture)
            };
        }

        private static Student ConvertJsonDataToStudent(dynamic studentData) {
            Student test = new Student()
            {
                Id = studentData._id,
                FirstName = studentData.firstName,
                LastName = studentData.lastName,
                ClassName = studentData.className,
                Responsible = studentData.responsible,
                Address = studentData.address,
                Installment = studentData.installment,
                Discount = studentData.discount,
                DiscountType = (Student.DiscountTypeOptions)studentData.discountType,
                Note = studentData.note,
                Picture = StringToBitmapImage((string)studentData.picture)
            };

            return test;
        }

        public struct JsonData {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string ClassName { get; set; }
            public string Responsible { get; set; }
            public string Address { get; set; }
            public double Installment { get; set; }
            public double Discount { get; set; }
            public int DiscountType { get; set; }
            public string Note { get; set; }
            public string Picture { get; set; }
        }
    }
}
