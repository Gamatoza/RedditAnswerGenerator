using Pastel;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RedditAnswerGenerator.Services.Utils
{
    public class Logger
    {
        public Logger(string Name)
        {
            name = Name;
            nameDirectory = Path.Combine(Path.GetFullPath(UserLogPath), Name);

            if (!Directory.Exists(nameDirectory))
            {
                Directory.CreateDirectory(nameDirectory);
            }
        }

        private string name;
        private string nameDirectory;
        private const string UserLogPath = "UsersLog";
        private void Append(string log)
        {
            try
            {
                var logPath = Path.Combine(nameDirectory, $"{name}.txt");
                using var sw = new StreamWriter(logPath, true, Encoding.Default);
                sw.WriteLine(RemoveColor(log));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{"WARNING".Pastel(Color.LightGoldenrodYellow)}[{DateTime.Now}]: {ex.Message}");
            }
        }
        public string GetName() => name;
        public void Action(string message)
        {
            var log = $"ACTION[{DateTime.Now}]: [{name}] {message}".Pastel(Color.Cyan);
            Console.WriteLine(log);
            Append(log);
        }

        public void Info(string message)
        {

            var log = $"{$"INFO[{DateTime.Now}]".Pastel(Color.LightGreen)}: [{name}] {message}";
            Console.WriteLine(log);
            Append(log);
        }

        public void Warning(string message)
        {

            var log = $"{"WARNING".Pastel(Color.LightGoldenrodYellow)}[{DateTime.Now}]: [{name}] {message}";
            Console.WriteLine(log);
            Append(log);
        }

        public void Error(string message)
        {

            var log = $"{"ERROR".Pastel(Color.White).PastelBg(Color.Red)}[{DateTime.Now}]: [{name}] {message}";
            Console.WriteLine(log);
            Append(log);
        }
        public void Error(Exception ex)
        {

            var log = $"{"ERROR".Pastel(Color.White).PastelBg(Color.Red)}[{DateTime.Now}]: [{name}] {ex.Message} => {ex.StackTrace}";
            Console.WriteLine(log);
            Append(log);
        }

        private string RemoveColor(string message) =>
             Regex.Replace(message, @"\[\d\d?\d?;\d\d?\d?;\d\d?\d?;\d\d?\d?;\d\d?\d?m|\[0m|", string.Empty);
    }
}
