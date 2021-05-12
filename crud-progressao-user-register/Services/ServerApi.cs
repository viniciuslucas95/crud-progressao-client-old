using crud_progressao.Scripts;
using crud_progressao_user_register.Models;
using crud_progressao_user_register.Scripts;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace crud_progressao_user_register.Services {
    public static class ServerApi {
        private static bool _isProcessingAsyncOperation;

        private static string Url {
            get {
                return ConfigFileGetter.GetConfigFile();
            }
        }

        private readonly static HttpClient _client = new();

        public static async Task<User[]> GetUsersAsync() {
            _isProcessingAsyncOperation = true;

            try {
                using HttpResponseMessage res = await _client.GetAsync($"{Url}/users");
                if (!res.IsSuccessStatusCode) {
                    _isProcessingAsyncOperation = false;
                    res.Dispose();
                    return Array.Empty<User>();
                }

                string json = await res.Content.ReadAsStringAsync();
                dynamic database = JsonConvert.DeserializeObject(json);

                if (database.Count == 0) {
                    _isProcessingAsyncOperation = false;
                    res.Dispose();
                    return Array.Empty<User>();
                }

                User[] users = new User[database.Count];

                for (int i = 0; i < database.Count; i++)
                    users[i] = new User() {
                        Id = database[i]._id,
                        Username = database[i].username,
                        Password = database[i].password,
                        Privilege = database[i].privilege
                    };

                _isProcessingAsyncOperation = false;
                res.Dispose();
                return users;
            } catch (Exception) {
                _isProcessingAsyncOperation = false;
                return null;
            }
        }

        /// <returns>Returns the new user id if the register is successful, or empty if isn't</returns>
        public static async Task<string> RegisterUserAsync(User user) {
            _isProcessingAsyncOperation = true;

            try {
                using HttpResponseMessage res = await _client.PostAsJsonAsync($"{Url}/users", user);
                string newUserId = await res.Content.ReadAsStringAsync();

                _isProcessingAsyncOperation = false;
                res.Dispose();
                return newUserId;
            } catch (Exception) {
                _isProcessingAsyncOperation = false;
                return "";
            }
        }

        public static async Task<bool> UpdateUserAsync(User user) {
            _isProcessingAsyncOperation = true;

            try {
                using HttpResponseMessage res = await _client.PutAsJsonAsync($"{Url}/users/{user.Id}", user);
                _isProcessingAsyncOperation = false;
                res.Dispose();
                return res.IsSuccessStatusCode;
            } catch (Exception) {
                _isProcessingAsyncOperation = false;
                return false;
            }
        }

        public static async Task<bool> DeleteUserAsync(string id) {
            _isProcessingAsyncOperation = true;

            try {
                using HttpResponseMessage res = await _client.DeleteAsync($"{Url}/users/{id}");
                _isProcessingAsyncOperation = false;
                res.Dispose();
                return res.IsSuccessStatusCode;
            } catch (Exception) {
                _isProcessingAsyncOperation = false;
                return false;
            }
        }

        public static bool IsProcessingAsyncOperation(Label label = null) {
            if (_isProcessingAsyncOperation) {
                if (label != null)
                    LabelTextSetter.SetText(label, $"Ainda processando operação!", true);

                return true;
            }

            return false;
        }
    }
}
