using Pastel;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace RedditAnswerGenerator.Services.Utils
{
    public static class Log
    {
        private const string LogPath = "logs.txt";

        public static void Action(string message)
        {
            var log = $"ACTION[{DateTime.Now}]: {message}".Pastel(Color.Cyan);
            Console.WriteLine(log);
            AppendToLog(log);
        }

        public static void Info(string message)
        {
            var log = $"{$"INFO[{DateTime.Now}]".Pastel(Color.LightGreen)}: {message}";
            Console.WriteLine(log);
            AppendToLog(log);
        }
        public static void Warning(string message)
        {
            var log = $"{"WARNING".Pastel(Color.LightGoldenrodYellow)}[{DateTime.Now}]: {message}";
            Console.WriteLine(log);
            AppendToLog(log);
        }
        public static void Error(string message)
        {
            var log = $"{"ERROR".Pastel(Color.White).PastelBg(Color.Red)}[{DateTime.Now}]: {message}";
            Console.WriteLine(log);
            AppendToLog(log);
        }
        public static void Error(Exception ex)
        {
            var log = $"{"ERROR".Pastel(Color.White).PastelBg(Color.Red)}[{DateTime.Now}]: {ex.Message} => {ex.StackTrace}";
            Console.WriteLine(log);
            AppendToLog(log);
        }
        private static string RemoveColor(this string message) =>
             Regex.Replace(message, @"\[\d\d?\d?;\d\d?\d?;\d\d?\d?;\d\d?\d?;\d\d?\d?m|\[0m|", string.Empty);
        private static void AppendToLog(string log)
        {
            try
            {
                using var sw = new StreamWriter(LogPath, true, Encoding.Default);
                sw.WriteLine(log.RemoveColor());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{"WARNING".Pastel(Color.LightGoldenrodYellow)}[{DateTime.Now}]: {ex.Message}");
            }
        }
    }
}
