using System;
using System.Diagnostics;
using System.IO;

namespace crud_progressao_library.Scripts {
    public static class LogWritter {
        private static readonly StreamWriter _writer =
            new(Directory.GetCurrentDirectory() + "/log.txt", append: true) { AutoFlush = true };

        public static void WriteLog(string text, bool logInConsole=true) {
            string dateTime = DateTime.Now.ToString();
            string content = $"[{dateTime}] {text}";
            _writer.WriteLine(content);

            if(logInConsole) Debug.WriteLine(content);
        }

        public static void WriteError(string text, bool logInConsole = true) {
            string dateTime = DateTime.Now.ToString();
            string content = $"[{dateTime}] ERROR: {text}";
            _writer.WriteLine(content);

            if (logInConsole) Debug.WriteLine(content);
        }
    }
}
