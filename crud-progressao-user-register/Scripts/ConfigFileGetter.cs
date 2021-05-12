using Newtonsoft.Json;
using System;
using System.IO;

namespace crud_progressao_user_register.Scripts {
    public class ConfigFileGetter {
        public static string GetConfigFile() {
            try {
                using StreamReader streamReader = new("config.json");
                string json = streamReader.ReadToEnd();
                dynamic config = JsonConvert.DeserializeObject(json);
                streamReader.Dispose();
                return config.serverUri;
            } catch (Exception) {
                return null;
            }
        }
    }
}
