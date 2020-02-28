using System;
using System.IO;

namespace TextEditorLauncher.UI.Log
{

    internal sealed class Logger : IDisposable
    {

        private static Lazy<Logger> _instance;
        private object _syncObject = new object();

        static Logger()
        {
            _instance = new Lazy<Logger>(() => new Logger(Path.Combine(Environment.CurrentDirectory, "TextEditorLauncher.Log.txt")));
        }

        public static Logger Instance =>
            _instance.Value;

        private Logger(string logFilePath)
        {
            var isLogFileExists = File.Exists(logFilePath);
            var fileStream = new FileStream(logFilePath, isLogFileExists ? FileMode.Append : FileMode.Create, FileAccess.Write);
            _logFileWriter = new StreamWriter(fileStream)
            {
                AutoFlush = true
            };
        }

        private StreamWriter _logFileWriter;

        public Logger Log(Severity severity, string message)
        {
            lock (_syncObject)
            {
                _logFileWriter.WriteLine($"{severity.ToString()} [{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}]: {message}");
                _logFileWriter.Flush();
            }
            return this;
        }

        public void Flush()
        {
            _logFileWriter.Flush();
        }

        public void Dispose()
        {
            _logFileWriter.Dispose();
        }

    }

    internal enum Severity
    {
        Notification,
        Warning,
        Error
    }

    internal static class SeverityExtensions
    {

        public static string ToString(this Severity severity)
        {
            switch (severity)
            {
                case Severity.Notification:
                    return "[Information]";
                case Severity.Warning:
                    return "[Warning]";
                case Severity.Error:
                    return "[Error]";
                default:
                    return "[Undefined]";
            }
        }

    }

}