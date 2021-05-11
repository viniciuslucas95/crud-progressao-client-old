using Newtonsoft.Json;
using System;
using System.IO;

namespace crud_progressao.Scripts {
    public static class ConfigFileGetter {
        public static string GetConfigFile() {
            LogWritter.WriteLog("Trying to get the config file...");

            try {
                using StreamReader streamReader = new("config.json");
                string json = streamReader.ReadToEnd();
                dynamic config = JsonConvert.DeserializeObject(json);
                streamReader.Dispose();
                LogWritter.WriteLog("Config file gotten");
                return config.serverUri;
            } catch (Exception e) {
                LogWritter.WriteError(e.Message);
                return null;
            }
        }
    }
}
