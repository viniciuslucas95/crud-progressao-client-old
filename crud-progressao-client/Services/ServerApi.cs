﻿using crud_progressao.Models;
using crud_progressao.Scripts;
using crud_progressao.Views.Windows;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace crud_progressao.Services {
    static class ServerApi {
        public static bool HasPrivilege { get { return _client.DefaultRequestHeaders.Contains("privilege"); } }

        private static string Url {
            get {
                return ConfigFileGetter.GetConfigFile();
            }
        }

        private readonly static HttpClient _client = new();

        public static async Task<bool> LoginAsync(string username, string password) {
            LogWritter.WriteLog("Trying to log in...");

            object data = new { username, password };

            try {
                using HttpResponseMessage res = await _client.PostAsJsonAsync($"{Url}/login", data);
                if (!res.IsSuccessStatusCode) {
                    LogWritter.WriteError("Trying to log in");
                    res.Dispose();
                    return false;
                }

                _client.DefaultRequestHeaders.Add("username", username);
                _client.DefaultRequestHeaders.Add("password", password);

                if (await res.Content.ReadAsAsync<bool>()) _client.DefaultRequestHeaders.Add("privilege", "true");

                res.Dispose();
                LogWritter.WriteLog("Logged in");
                return true;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return false;
            }
        }

        public static async Task<bool> GetStudentsAsync(string firstName, string lastName, string className, string responsible, string address, string discount) {
            LogWritter.WriteLog("Trying to get students from the database...");

            try {
                using HttpResponseMessage res = await _client.GetAsync($"{Url}/students/?firstName={firstName}&lastName={lastName}&className={className}&responsible={responsible}&address={address}&discount={discount}");
                if (!res.IsSuccessStatusCode) {
                    LogWritter.WriteError("Trying to get students from the database");
                    res.Dispose();
                    return false;
                }

                string json = await res.Content.ReadAsStringAsync();
                dynamic database = JsonConvert.DeserializeObject(json);

                Student.Database.Clear();

                for (int i = 0; i < database.Count; i++)
                    Student.Database.Add(ConvertJsonDataToStudent(database[i]));

                LogWritter.WriteLog("Students gotten");
                res.Dispose();
                return true;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return false;
            }
        }

        /// <returns>Returns the new student id if the register is successful, or empty if isn't</returns>
        public static async Task<string> RegisterStudentAsync(Student student) {
            LogWritter.WriteLog("Trying to register the student in the database...");

            JsonData data = ConvertStudentToJsonData(student);

            try {
                using HttpResponseMessage res = await _client.PostAsJsonAsync($"{Url}/students", data);
                string newStudentId = await res.Content.ReadAsStringAsync();

                if (res.IsSuccessStatusCode) {
                    LogWritter.WriteLog("Student registered");
                } else
                    LogWritter.WriteError("Trying to register the student in the database");

                res.Dispose();
                return newStudentId;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return "";
            }
        }

        /// <returns>Returns the new payment id if the register is successful, or empty if isn't</returns>
        public static async Task<string> RegisterPaymentAsync(string studentId, Student.Payment payment) {
            LogWritter.WriteLog("Trying to register the student payment in the database...");

            try {
                using HttpResponseMessage res = await _client.PostAsJsonAsync($"{Url}/students/payments/{studentId}", payment);
                string newPaymentId = await res.Content.ReadAsStringAsync();

                if (res.IsSuccessStatusCode) {
                    LogWritter.WriteLog("Payment registered");
                } else
                    LogWritter.WriteError("Trying to register the payment in the database");

                res.Dispose();
                return newPaymentId;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return "";
            }
        }

        public static async Task<bool> UpdateStudentAsync(Student student) {
            LogWritter.WriteLog("Trying to update the student informations in the database...");

            JsonData data = ConvertStudentToJsonData(student);

            try {
                using HttpResponseMessage res = await _client.PutAsJsonAsync($"{Url}/students/{student.Id}", data);
                if (res.IsSuccessStatusCode) {
                    LogWritter.WriteLog("Student updated");
                } else
                    LogWritter.WriteError("Trying to update the student informations in the database");

                res.Dispose();
                return res.IsSuccessStatusCode;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return false;
            }
        }

        public static async Task<bool> UpdatePaymentAsync(string studentId, Student.Payment payment) {
            LogWritter.WriteLog("Trying to update the student payment in the database...");

            try {
                using HttpResponseMessage res = await _client.PutAsJsonAsync($"{Url}/students/payments/{studentId}", payment);
                if (res.IsSuccessStatusCode) {
                    LogWritter.WriteLog("Payment updated");
                } else
                    LogWritter.WriteError("Trying to update the student payment in the database");

                res.Dispose();
                return res.IsSuccessStatusCode;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return false;
            }
        }

        public static async Task<bool> DeleteStudentAsync(string id) {
            LogWritter.WriteLog("Trying to delete the student from the database...");

            try {
                using HttpResponseMessage res = await _client.DeleteAsync($"{Url}/students/{id}");
                if (res.IsSuccessStatusCode) {
                    LogWritter.WriteLog("Student deleted");
                } else
                    LogWritter.WriteError("Trying to delete the student from the database");

                res.Dispose();
                return res.IsSuccessStatusCode;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return false;
            }
        }

        public static async Task<bool> DeletePaymentAsync(string studentId, string paymentId) {
            LogWritter.WriteLog("Trying to delete the payment from the database...");

            try {
                using HttpResponseMessage res = await _client.DeleteAsync($"{Url}/students/payments/{studentId}?paymentId={paymentId}");
                if (res.IsSuccessStatusCode) {
                    LogWritter.WriteLog("Payment deleted");
                } else
                    LogWritter.WriteError("Trying to delete the payment from the database");

                res.Dispose();
                return res.IsSuccessStatusCode;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return false;
            }
        }

        private static BitmapImage StringToBitmapImage(string value) {
            if (string.IsNullOrEmpty(value)) return null;

            LogWritter.WriteLog("Trying to convert a string into a bitmap image");

            try {
                BitmapImage img = new();
                byte[] data = Convert.FromBase64String(value);

                using MemoryStream ms = new(data);
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.StreamSource = ms;
                img.EndInit();
                ms.Dispose();

                LogWritter.WriteLog("String converted");
                return img;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return null;
            }
        }

        private static string BitmapImageToString(BitmapImage img) {
            if (img == null) return "";

            LogWritter.WriteLog("Trying to convert a bitmap image into a string");

            try {
                JpegBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create(img));
                byte[] data;

                using (MemoryStream ms = new()) {
                    encoder.Save(ms);
                    data = ms.ToArray();
                    ms.Dispose();
                }

                LogWritter.WriteLog("Bitmap image converted");
                return Convert.ToBase64String(data);
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
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
                DueDate = student.DueDate,
                Note = student.Note,
                Picture = BitmapImageToString(student.Picture),
                Payments = student.Payments
            };
        }

        private static Student ConvertJsonDataToStudent(dynamic studentData) {
            Student student = new()
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
                DueDate = studentData.dueDate,
                Note = studentData.note,
                Picture = StringToBitmapImage((string)studentData.picture),
            };

            student.Payments = new Student.Payment[studentData.payments.Count];

            for (int i = 0; i < studentData.payments.Count; i++) {
                dynamic paymentData = studentData.payments[i];

                Student.Payment payment = new()
                {
                    Id = paymentData._id,
                    DueDate = paymentData.dueDate.ToObject<int[]>(),
                    PaymentDate = paymentData.paymentDate.ToObject<int[]>(),
                    Installment = paymentData.installment,
                    Discount = paymentData.discount,
                    DiscountType = (Student.DiscountTypeOptions)paymentData.discountType,
                    PaidValue = paymentData.paidValue,
                    Note = paymentData.note
                };

                student.Payments[i] = payment;
            }

            return student;
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
            public int DueDate { get; set; }
            public string Note { get; set; }
            public string Picture { get; set; }
            public Student.Payment[] Payments { get; set; }
        }
    }
}
