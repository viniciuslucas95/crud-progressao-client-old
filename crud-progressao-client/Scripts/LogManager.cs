using System;
using System.IO;

namespace crud_progressao {
    public static class LogManager {
        private static readonly StreamWriter _writer =
            new StreamWriter(Directory.GetCurrentDirectory() + "/log.txt", append: true) { AutoFlush = true };

        public static void Write(string text, bool logInConsole=true) {
            string dateTime = DateTime.Now.ToString();
            _writer.WriteLine($"[{dateTime}] {text}");

            if(logInConsole) Console.WriteLine(text);
        }
    }
}
