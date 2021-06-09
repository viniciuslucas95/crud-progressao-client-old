using crud_progressao_library.Scripts;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace crud_progressao_library.Services {
    public static class ServerApi {
        public static bool IsProcessingAsyncOperation { get; private set; }

        private static readonly string _baseUrl = ConfigFileGetter.GetConfigFile();
        private static readonly HttpClient _client = new();

        public static void SetHeader(string name, string value) {
            _client.DefaultRequestHeaders.Add(name, value);
        }

        public static async Task<dynamic> GetAsync(string url, string query) {
            LogWritter.WriteLog("Trying to get data from the database...");
            IsProcessingAsyncOperation = true;

            try{
                using HttpResponseMessage res = await _client.GetAsync($"{_baseUrl}/{url}/?{query}");

                if (!res.IsSuccessStatusCode) {
                    LogWritter.WriteError("Trying to get data from the database");
                    IsProcessingAsyncOperation = false;
                    res.Dispose();
                    return null;
                }

                string jsonResult = await res.Content.ReadAsStringAsync();
                dynamic result = JsonConvert.DeserializeObject<dynamic>(jsonResult);
                LogWritter.WriteLog("Data gotten");
                IsProcessingAsyncOperation = false;
                res.Dispose();
                return result;
            } catch(Exception e) {
                LogWritter.WriteError(e.Message);
                IsProcessingAsyncOperation = false;
                return null;
            }
        }

        public static async Task<string> RegisterAsync<T>(string url, string param, T data) {
            LogWritter.WriteLog($"Trying to register the data in the database...");
            IsProcessingAsyncOperation = true;

            try {
                using HttpResponseMessage res = await _client.PostAsJsonAsync($"{_baseUrl}/{url}/{param}", data);

                if (!res.IsSuccessStatusCode) {
                    LogWritter.WriteError($"Trying to register the data in the database");
                    IsProcessingAsyncOperation = false;
                    res.Dispose();
                    return "";
                }

                string id = await res.Content.ReadAsStringAsync();
                LogWritter.WriteLog($"Data registered");
                IsProcessingAsyncOperation = false;
                res.Dispose();
                return id;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                IsProcessingAsyncOperation = false;
                return "";
            }
        }

        public static async Task<bool> UpdateAsync<T>(string url, string param, T data) {
            LogWritter.WriteLog($"Trying to update the data in the database...");
            IsProcessingAsyncOperation = true;

            try {
                using HttpResponseMessage res = await _client.PutAsJsonAsync($"{_baseUrl}/{url}/{param}", data);

                if (!res.IsSuccessStatusCode) {
                    LogWritter.WriteError($"Trying to update the data in the database");
                    IsProcessingAsyncOperation = false;
                    res.Dispose();
                    return false;
                }
                
                LogWritter.WriteLog($"Data updated");
                IsProcessingAsyncOperation = false;
                res.Dispose();
                return res.IsSuccessStatusCode;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                IsProcessingAsyncOperation = false;
                return false;
            }
        }

        public static async Task<bool> DeleteAsync(string url, string param, string query) {
            LogWritter.WriteLog("Trying to delete data from the database...");
            IsProcessingAsyncOperation = true;

            try {
                using HttpResponseMessage res = await _client.DeleteAsync($"{_baseUrl}/{url}/{param}?{query}");

                if (!res.IsSuccessStatusCode) {
                    LogWritter.WriteError("Trying to delete data from the database");
                    IsProcessingAsyncOperation = false;
                    res.Dispose();
                    return false;
                }

                LogWritter.WriteLog("Data deleted");
                IsProcessingAsyncOperation = false;
                res.Dispose();
                return res.IsSuccessStatusCode;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                IsProcessingAsyncOperation = false;
                return false;
            }
        }
    }
}
