using System;
using System.IO;

namespace TextEditorLauncher.UI.Log
{

    // логгер реализован как одиночка (Singleton)
    // зачем: если делать как static class, то нельзя будет реализовать интерфейс IDisposable
    internal sealed class Logger : IDisposable
    {

        // Lazy - по сути, враппер над экземпляром объекта, служащий для отложенной инициализации
        // иными словами, при первом обращении к свойству Value будет выполнен делегат Func<T>, после чего ссылка на объект, который будет возвращён из функции, будет подцеплена в Lazy
        // и далее при запросе Value, будет отдаваться эта самая ссылка
        private static Lazy<Logger> _instance;

        // вызывается при первом обращении к типу Logger
        static Logger()
        {
            // создаём враппер, прокидываем туда лямбду, создающую экземпляр логгера, и указываем, что этот логгер одновременно может использоваться лишь одним потоком (false вторым параметром)
            // singlethreading для того, чтобы не было одновременного доступа к критическому ресурсу (StreamWriter'у)
            _instance = new Lazy<Logger>(() => new Logger(LogFilePath), false);
        }

        // ссылка на единственный экземпляр класса логгера
        public static Logger Instance =>
            _instance.Value;

        // имя лог-файла
        private static string LogFileName =>
            "TextEditorLauncher.Log.txt";

        // полный путь к лог-файлу
        public static string LogFilePath =>
            Path.Combine(Environment.CurrentDirectory, LogFileName);

        // конструктор логгера
        // private для того, чтобы нельзя было дать юзеру возможность создать объект
        // проверяем, существует ли уже лог-файл: если да, то открываем его на дозапись, если нет, то создаём и открываем на запись
        // заворачиваем в StreamWriter, чтобы можно было записывать стандартные типы данных без преобразования их руками в массив байтиков
        // AutoFlush: при каждой записи в StreamWriter, всё что в нём есть, сбрасывается в файл
        private Logger(string logFilePath)
        {
            var isLogFileExists = File.Exists(logFilePath);
            var fileStream = new FileStream(logFilePath, isLogFileExists ? FileMode.Append : FileMode.Create, FileAccess.Write);
            _logFileWriter = new StreamWriter(fileStream)
            {
                AutoFlush = true
            };
        }

        // ссылка на StreamWriter, который пишет лог-файл
        private StreamWriter _logFileWriter;

        // метод, пишущий в лог-файл
        // так как объект логгера теперь может использоваться одновременно лишь одним потоком (указано в конструкторе Lazy-враппера над текущим объектом), lock больше не нужен
        public Logger Log(Severity severity, string message)
        {
            _logFileWriter.WriteLine($"{severity.ToFriendlyString()} [{DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}]: {message}");
            return this;
        }   

        // эта штука - легаси, ибо уже установлен AutoFlush в StreamWriter'e, не знаю, почему я это не выпилил сразу (=
        [Obsolete]
        public Logger Flush()
        {
            _logFileWriter.Flush();
            return this;
        }

        // освобождаем StreamWriter
        public void Dispose()
        {
            _logFileWriter.Dispose();
        }

    }

    // типы логов: информация, предупреждение, ошибка
    internal enum Severity
    {
        Notification,
        Warning,
        Error
    }

    // для определения метода, преобразующего экземпляр перечисления в кастомную строку
    internal static class SeverityExtensions
    {

        // просто синтаксический сахар. Пишем: severityInstance.ToFriendlyString(), на самом деле происходит это: SeverityExtensions.TOFriendlyString(severityInstance)
        // в принципе, на этом и работают все Linq-методы
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