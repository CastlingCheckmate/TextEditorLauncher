using System;
using System.IO;

namespace TextEditorLauncher.UI.Log
{

    internal sealed class Logger : IDisposable
    {

        private static Lazy<Logger> _instance;

        static Logger()
        {
            _instance = new Lazy<Logger>(() => new Logger(Path.Combine(Environment.CurrentDirectory, LogFileName)));
        }

        public static Logger Instance =>
            _instance.Value;

        public static string LogFileName =>
            "TextEditorLauncher.Log.txt";

        private Logger(string logFilePath)
        {
            var isLogFileExists = File.Exists(logFilePath);
            var fileStream = new FileStream(logFilePath, isLogFileExists ? FileMode.Append : FileMode.Create, FileAccess.Write);
            _logFileWriter = new StreamWriter(fileStream)
            {
                AutoFlush = true
            };
        }

        private object _syncObject = new object();
        private StreamWriter _logFileWriter;

        public Logger Log(Severity severity, string message)
        {
            lock (_syncObject)
            {
                _logFileWriter.WriteLine($"{severity.ToFriendlyString()} [{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}]: {message}");
            }
            return this;
        }

        public Logger Flush()
        {
            _logFileWriter.Flush();
            return this;
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

        public static string ToFriendlyString(this Severity severity)
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