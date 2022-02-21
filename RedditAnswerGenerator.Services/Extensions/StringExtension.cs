using System.Text;
using System.Text.RegularExpressions;

namespace RedditAnswerGenerator.Services.Extensions
{
    public static class StringExtension
    {
        private static string patternUrl = @"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)";
        private static char[] separatorsAfter = { '.', '!', '?', '\n' };

        public static string RemoveColor(this string message) =>
             Regex.Replace(message, @"\[\d\d?\d?;\d\d?\d?;\d\d?\d?;\d\d?\d?;\d\d?\d?m|\[0m|", string.Empty);

        public static int GetNumbers(this string obj)
        {
            int.TryParse(new string(obj.Where(char.IsDigit).ToArray()), out var some);
            return some;
        }

        public static int GetNumbersWithSettings(this string obj, params KeyValuePair<char, int>[] pairs)
        {
            int.TryParse(new string(obj.Where(char.IsDigit).ToArray()), out var some);
            foreach (var item in pairs)
            {
                if (obj.Contains(item.Key)) some *= item.Value;
            }
            return some;
        }

        public static string RemoveURLs(this string obj)
        {
            return Regex.Replace(obj, patternUrl, string.Empty); ;
        }

        public static bool IsHasUrl(this string obj)
        {
            return new Regex(patternUrl).Match(obj).Success;
        }

        public static string AllTrim(this string obj)
        {
            return obj.Replace("  ", " ").Replace("   ", " ").Trim();
        }

        public static string ToSentenceView(this string obj) //not finished yet
        {
            var sb = new StringBuilder(obj.AllTrim());
            sb[0] = char.ToUpper(sb[0]);
            var nextInUpper = false;

            for (int i = 1; i < sb.Length - 1; i++)
            {
                if (sb[i] != ' ' && nextInUpper)
                {
                    sb[i] = char.ToUpper(sb[i]);
                    nextInUpper = false;
                }

                if (separatorsAfter.Contains(sb[i]))
                {
                    sb[i + 1] = char.ToUpper(sb[i + 1]);
                    nextInUpper = true;
                }
                else
                {
                    sb[i + 1] = char.ToLower(sb[i + 1]);
                }
            }

            return sb.ToString();
        }

        public static bool IsUnicode(this string input)
        {
            return (Encoding.UTF8.GetByteCount(input) == input.Length);
        }
    }
}
